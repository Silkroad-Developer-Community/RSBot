using System.Windows.Forms;

namespace RSBot.Core.Plugins;

public interface IPlugin
{
    /// <summary>
    ///     Gets or sets the internal name of the plugin.
    ///     This value should be unique.
    /// </summary>
    /// <value>
    ///     The name.
    /// </value>
    public string InternalName { get; }

    /// <summary>
    ///     Initializes this instance.
    /// </summary>
    void Initialize();

    /// <summary>
    ///     Initialzes objects when user is loaded.
    /// </summary>
    void OnLoadCharacter();
}
