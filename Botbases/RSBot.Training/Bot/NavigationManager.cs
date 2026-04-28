using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Objects;

namespace RSBot.Training.Bot;

internal static class NavigationManager
{
    private const string LinkageUrl =
        "https://raw.githubusercontent.com/Silkroad-Developer-Community/Silkroad-NavLink/main/navigation_linkage.json";
    private static readonly string LinkagePath = Path.Combine(Kernel.BasePath, "Data", "navigation_linkage.json");
    private static NavigationLinkage _linkage;
    private static List<NodePathStep> _activePath;

    // Persisted metadata for logging after clearing heavy linkage data
    private static string _linkageVersion;
    private static string _linkageDate;
    private static string _linkageCopyright;
    private static string _linkageLicense;
    private static string _linkageMaintainer;
    private static string _linkageContributors;

    // A simple PriorityQueue implementation for environments where System.Collections.Generic.PriorityQueue is missing
    private class PriorityQueue<TElement, TPriority>
        where TPriority : IComparable<TPriority>
    {
        private List<(TElement Element, TPriority Priority)> _items = new();

        public int Count => _items.Count;

        public void Enqueue(TElement element, TPriority priority)
        {
            _items.Add((element, priority));
            _items.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public TElement Dequeue()
        {
            var item = _items[0].Element;
            _items.RemoveAt(0);
            return item;
        }
    }

    /// <summary>
    /// Loads the linkage data from the local file.
    /// </summary>
    public static bool LoadLinkageData()
    {
        if (_linkage != null)
            return true;

        try
        {
            if (!File.Exists(LinkagePath))
            {
                Log.Warn($"Navigation linkage file not found at {LinkagePath}");
                return false;
            }

            Log.Notify("Loading navigation linkage data...");
            var json = File.ReadAllText(LinkagePath);

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            _linkage = JsonSerializer.Deserialize<NavigationLinkage>(json, options);

            if (_linkage == null || _linkage.Nodes == null)
            {
                Log.Error("Failed to parse navigation linkage data.");
                return false;
            }

            // Persist metadata
            _linkageVersion = _linkage.Version;
            _linkageDate = _linkage.Date;
            _linkageCopyright = _linkage.Copyright;
            _linkageLicense = _linkage.License;
            _linkageMaintainer = _linkage.Maintainer;
            _linkageContributors = _linkage.Contributors;

            Log.Notify($"Loaded {_linkage.Nodes.Count} nodes and {_linkage.Edges.Count} edges.");
            LogLinkageMetadata();

            return true;
        }
        catch (Exception e)
        {
            Log.Error($"Error loading navigation data: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Logs the linkage metadata.
    /// </summary>
    private static void LogLinkageMetadata()
    {
        if (string.IsNullOrEmpty(_linkageVersion))
            return;

        Log.Notify($"[Navigation] Version: {_linkageVersion}");
        Log.Notify($"[Navigation] Date: {_linkageDate}");
        Log.Notify($"[Navigation] Copyright: {_linkageCopyright}");
        Log.Notify($"[Navigation] License: {_linkageLicense}");
        Log.Notify($"[Navigation] Maintainer: {_linkageMaintainer}");
        Log.Notify($"[Navigation] Contributors: {_linkageContributors}");
    }

    /// <summary>
    /// Fetches the navigation data from the remote URL.
    /// </summary>
    public static async Task<bool> FetchRemoteLinkageData()
    {
        try
        {
            Log.Notify("Fetching navigation linkage data from GitHub...");
            using var client = new HttpClient();
            var json = await client.GetStringAsync(LinkageUrl);

            if (string.IsNullOrWhiteSpace(json))
            {
                Log.Error("Fetched navigation data is empty.");
                return false;
            }

            var directory = Path.GetDirectoryName(LinkagePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(LinkagePath, json);
            Log.Notify("Successfully updated local navigation linkage data.");

            _linkage = null; // Force reload
            return LoadLinkageData();
        }
        catch (Exception e)
        {
            Log.Error($"Error fetching remote navigation data: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets the semantic name for a position.
    /// </summary>
    private static string GetSemanticName(float x, float y, ushort region)
    {
        return $"{(int)Math.Floor(x)}_{(int)Math.Floor(y)}_{region}";
    }

    /// <summary>
    /// Calculates a path to the target training area.
    /// </summary>
    public static bool CalculatePathToTrainingArea()
    {
        if (!LoadLinkageData())
        {
            Log.Notify("Local navigation data missing. Attempting to fetch from remote...");
            if (!FetchRemoteLinkageData().GetAwaiter().GetResult())
            {
                Log.Error("Could not find or download navigation linkage data.");
                return false;
            }
        }

        var targetPos = new Position(
            PlayerConfig.Get<ushort>("RSBot.Area.Region"),
            PlayerConfig.Get<float>("RSBot.Area.X"),
            PlayerConfig.Get<float>("RSBot.Area.Y"),
            PlayerConfig.Get<float>("RSBot.Area.Z")
        );

        var playerPos = Game.Player.Movement.Source;

        // 1. Semantic Mapping: Map every raw ID to its simplified "X_Y_R" semantic name.
        var idToSid = new Dictionary<string, string>();
        var sidToNids = new Dictionary<string, List<string>>();

        foreach (var kvp in _linkage.Nodes)
        {
            var sid = GetSemanticName(kvp.Value.X, kvp.Value.Y, kvp.Value.Region);
            idToSid[kvp.Key] = sid;
            if (!sidToNids.ContainsKey(sid))
                sidToNids[sid] = new List<string>();
            sidToNids[sid].Add(kvp.Key);
        }

        var startNodeId = GetNearestNodeId(playerPos);
        var endNodeId = GetNearestNodeId(targetPos);

        if (startNodeId == null || endNodeId == null)
        {
            Log.Warn("Could not find suitable entry/exit nodes for pathfinding.");
            return false;
        }

        if (!idToSid.TryGetValue(startNodeId, out var startSid) || !idToSid.TryGetValue(endNodeId, out var endSid))
        {
            Log.Warn("Start or end node semantic mapping failed.");
            return false;
        }

        Log.Notify($"Pathfinding from {startSid} to {endSid}...");

        // 2. Build Adjacency in Semantic Space
        var adjSid = new Dictionary<string, List<SemanticEdge>>();
        foreach (var edge in _linkage.Edges.Values)
        {
            if (!idToSid.TryGetValue(edge.From, out var uSid) || !idToSid.TryGetValue(edge.To, out var vSid))
                continue;

            if (!adjSid.ContainsKey(uSid))
                adjSid[uSid] = new List<SemanticEdge>();
            adjSid[uSid].Add(new SemanticEdge { ToSid = vSid, EdgeData = edge });

            if (edge.Type == "walk")
            {
                if (!adjSid.ContainsKey(vSid))
                    adjSid[vSid] = new List<SemanticEdge>();
                adjSid[vSid].Add(new SemanticEdge { ToSid = uSid, EdgeData = edge });
            }
        }

        var path = FindPath(startSid, endSid, adjSid, sidToNids);

        if (path == null)
        {
            Log.Warn("No path found to training area.");
            return false;
        }

        _activePath = path;

        // Add the final target position as a linear stretch
        _activePath.Add(
            new NodePathStep
            {
                NodeId = "TRAINING_TARGET",
                Position = targetPos,
                Edge = new LinkageEdge { Type = "walk" },
            }
        );

        // Clear linkage data once path is cached to save memory
        _linkage = null;
        GC.Collect();

        return true;
    }

    private static string GetNearestNodeId(Position pos)
    {
        string nearestId = null;
        double minDistance = double.MaxValue;

        foreach (var kvp in _linkage.Nodes)
        {
            var node = kvp.Value;
            var nodePos = new Position(node.X, node.Y, node.Region);
            var dist = pos.DistanceTo(nodePos);

            if (dist < minDistance)
            {
                minDistance = dist;
                nearestId = kvp.Key;
            }
        }

        return minDistance < 1000 ? nearestId : null;
    }

    private static List<NodePathStep> FindPath(
        string startSid,
        string endSid,
        Dictionary<string, List<SemanticEdge>> adjSid,
        Dictionary<string, List<string>> sidToNids
    )
    {
        var openSet = new PriorityQueue<string, float>();
        var cameFrom = new Dictionary<string, (string PrevSid, LinkageEdge Edge)>();
        var gScore = new Dictionary<string, float>();
        var fScore = new Dictionary<string, float>();

        gScore[startSid] = 0;
        fScore[startSid] = Heuristic(startSid, endSid, sidToNids);
        openSet.Enqueue(startSid, fScore[startSid]);

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current == endSid)
                return ReconstructPath(cameFrom, startSid, current, sidToNids);

            if (!adjSid.ContainsKey(current))
                continue;

            foreach (var sEdge in adjSid[current])
            {
                var neighborSid = sEdge.ToSid;
                var tentativeGScore = gScore[current] + EdgeWeight(sEdge.EdgeData);

                if (!gScore.ContainsKey(neighborSid) || tentativeGScore < gScore[neighborSid])
                {
                    cameFrom[neighborSid] = (current, sEdge.EdgeData);
                    gScore[neighborSid] = tentativeGScore;
                    fScore[neighborSid] = tentativeGScore + Heuristic(neighborSid, endSid, sidToNids);
                    openSet.Enqueue(neighborSid, fScore[neighborSid]);
                }
            }
        }

        return null;
    }

    private static float Heuristic(string aSid, string bSid, Dictionary<string, List<string>> sidToNids)
    {
        var nodeA = _linkage.Nodes[sidToNids[aSid][0]];
        var nodeB = _linkage.Nodes[sidToNids[bSid][0]];

        if (nodeA.Region != nodeB.Region)
            return 0; // Cross-region: Use Dijkstra mode

        var posA = new Position(nodeA.X, nodeA.Y, nodeA.Region);
        var posB = new Position(nodeB.X, nodeB.Y, nodeB.Region);
        return (float)posA.DistanceTo(posB);
    }

    private static float EdgeWeight(LinkageEdge edge)
    {
        if (edge.Type == "teleport")
            return 100f;

        var from = _linkage.Nodes[edge.From];
        var to = _linkage.Nodes[edge.To];
        var posFrom = new Position(from.X, from.Y, from.Region);
        var posTo = new Position(to.X, to.Y, to.Region);
        return (float)posFrom.DistanceTo(posTo);
    }

    private static List<NodePathStep> ReconstructPath(
        Dictionary<string, (string PrevSid, LinkageEdge Edge)> cameFrom,
        string startSid,
        string endSid,
        Dictionary<string, List<string>> sidToNids
    )
    {
        var totalPath = new List<NodePathStep>();
        var current = endSid;

        while (cameFrom.ContainsKey(current))
        {
            var entry = cameFrom[current];
            var node = _linkage.Nodes[sidToNids[current][0]];

            totalPath.Insert(
                0,
                new NodePathStep
                {
                    NodeId = sidToNids[current][0],
                    Position = new Position(node.X, node.Y, node.Region),
                    Edge = entry.Edge,
                }
            );
            current = entry.PrevSid;
        }

        var startNode = _linkage.Nodes[sidToNids[startSid][0]];
        totalPath.Insert(
            0,
            new NodePathStep
            {
                NodeId = sidToNids[startSid][0],
                Position = new Position(startNode.X, startNode.Y, startNode.Region),
                Edge = new LinkageEdge { Type = "walk" },
            }
        );

        return totalPath;
    }

    /// <summary>
    /// Generates an RBS file from the calculated path.
    /// </summary>
    public static string GenerateRBSFile()
    {
        if (_activePath == null || _activePath.Count == 0)
            return null;

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        var fileName = $"{timestamp}.rbs";
        var dynamicScriptsDir = Path.Combine(ScriptManager.InitialDirectory, "Dynamic");

        if (!Directory.Exists(dynamicScriptsDir))
            Directory.CreateDirectory(dynamicScriptsDir);

        var filePath = Path.Combine(dynamicScriptsDir, fileName);
        var rbsLines = new List<string>();

        foreach (var step in _activePath)
        {
            if (step.Edge.Type == "teleport")
            {
                rbsLines.Add($"teleport {step.Edge.Npc} {step.Edge.Dest}");
            }
            else
            {
                // move XOffset YOffset ZOffset XSector YSector
                rbsLines.Add(
                    $"move {step.Position.XOffset} {step.Position.YOffset} {step.Position.ZOffset} {step.Position.Region.X} {step.Position.Region.Y}"
                );
            }
        }

        File.WriteAllLines(filePath, rbsLines);
        Log.Notify($"Generated dynamic walk script: {fileName}");
        LogLinkageMetadata();

        _activePath = null; // Clear path after generation
        return filePath;
    }

    private class SemanticEdge
    {
        public string ToSid { get; set; }
        public LinkageEdge EdgeData { get; set; }
    }
}

internal class NodePathStep
{
    public string NodeId { get; set; }
    public Position Position { get; set; }
    public LinkageEdge Edge { get; set; }
}
