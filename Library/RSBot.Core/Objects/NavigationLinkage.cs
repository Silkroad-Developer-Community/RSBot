using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RSBot.Core.Objects;

public class NavigationLinkage
{
    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("date")]
    public string Date { get; set; }

    [JsonPropertyName("copyright")]
    public string Copyright { get; set; }

    [JsonPropertyName("license")]
    public string License { get; set; }

    [JsonPropertyName("maintainer")]
    public string Maintainer { get; set; }

    [JsonPropertyName("contributors")]
    public string Contributors { get; set; }

    [JsonPropertyName("nodes")]
    public Dictionary<string, LinkageNode> Nodes { get; set; }

    [JsonPropertyName("edges")]
    public Dictionary<string, LinkageEdge> Edges { get; set; }
}

public class LinkageNode
{
    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }

    [JsonPropertyName("region")]
    public ushort Region { get; set; }
}

public class LinkageEdge
{
    [JsonPropertyName("from")]
    public string From { get; set; }

    [JsonPropertyName("to")]
    public string To { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("npc")]
    public string Npc { get; set; }

    [JsonPropertyName("dest")]
    public int? Dest { get; set; }
}
