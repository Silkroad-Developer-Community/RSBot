using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RSBot.Core.Event;
using RSBot.Core.Network;

namespace RSBot.Core.Plugins;

public class PluginManager
{
    /// <summary>
    ///     Gets the extension directory.
    /// </summary>
    /// <value>
    ///     The extension directory.
    /// </value>
    public string InitialDirectory => Path.Combine(Kernel.BasePath, "Data", "Plugins");

    /// <summary>
    ///     Gets the extensions.
    /// </summary>
    /// <value>
    ///     The extensions.
    /// </value>
    public Dictionary<string, IPlugin> Extensions { get; private set; }
    public Dictionary<string, IPluginView> ExtensionsViews { get; private set; }

    /// <summary>
    ///     Loads the assemblies.
    /// </summary>
    public bool LoadAssemblies()
    {
        if (Extensions != null)
            return false;

        Extensions = new Dictionary<string, IPlugin>();
        ExtensionsViews = new Dictionary<string, IPluginView>();

        try
        {
            var files = Directory.GetFiles(InitialDirectory, "*.dll");
            foreach (var file in files)
            {
                var (plugins, views) = GetExtensionsFromAssembly(file);

                foreach (var plugin in plugins)
                {
                    if (!Extensions.ContainsKey(plugin.Key))
                    {
                        Extensions.Add(plugin.Key, plugin.Value);
                    }
                }

                foreach (var view in views)
                {
                    if (!ExtensionsViews.ContainsKey(view.Key))
                    {
                        ExtensionsViews.Add(view.Key, view.Value);
                        Log.Debug($"Loaded view [{view.Value.InternalName}]");
                    }
                }
            }
            //order by index, not alphabeticaly
            ExtensionsViews = ExtensionsViews.OrderBy(entry => entry.Value.Index).ToDictionary(x => x.Key, x => x.Value);

            EventManager.FireEvent("OnLoadPlugins");

            return true;
        }
        catch (Exception ex)
        {
            File.WriteAllText(
                Path.Combine(Kernel.BasePath, "Data", "Logs", "boot-error.log"),
                $"The plugin manager encountered a problem: \n{ex.Message} at {ex.StackTrace}"
            );
            return false;
        }
    }

    /// <summary>
    ///     Gets the extensions from assembly.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns></returns>
    private static (Dictionary<string, IPlugin>, Dictionary<string, IPluginView>) GetExtensionsFromAssembly(string file)
    {
        var plugins = new Dictionary<string, IPlugin>();
        var views = new Dictionary<string, IPluginView>();
;

        try
        {
            var assembly = Assembly.LoadFrom(file);
            var assemblyTypes = assembly.GetTypes();

            foreach (var type in assemblyTypes.Where(t => t.IsPublic && !t.IsAbstract))
            {
                object instance = null;

                if (typeof(IPlugin).IsAssignableFrom(type))
                {
                    instance = Activator.CreateInstance(type);
                    var plugin = (IPlugin)instance;
                    plugins[plugin.InternalName] = plugin;
                }

                if (typeof(IPluginView).IsAssignableFrom(type))
                {
                    instance ??= Activator.CreateInstance(type);
                    var view = (IPluginView)instance;
                    views[view.InternalName] = view;
                }
            }
            if (plugins.Count == 0 && views.Count == 0)
                return (plugins, views);

            var handlerType = typeof(IPacketHandler);
            var hookType = typeof(IPacketHook);

            var types = assemblyTypes.Where(p => handlerType.IsAssignableFrom(p) && !p.IsInterface).ToArray();

            foreach (var handler in types)
                PacketManager.RegisterHandler((IPacketHandler)Activator.CreateInstance(handler));

            types = assemblyTypes.Where(p => hookType.IsAssignableFrom(p) && !p.IsInterface).ToArray();

            foreach (var hook in types)
                PacketManager.RegisterHook((IPacketHook)Activator.CreateInstance(hook));
        }
        catch
        {
            /* ignore, it's an invalid extension */
        }

        return (plugins, views);
    }
}
