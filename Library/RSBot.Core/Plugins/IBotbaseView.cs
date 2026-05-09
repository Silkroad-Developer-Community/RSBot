using System.Windows.Forms;
using RSBot.Core.Objects;

namespace RSBot.Core.Plugins;

public interface IBotbaseView
{
    /// <summary>
    ///     Gets internal the name.
    /// </summary>
    /// <value>
    ///     The name.
    /// </value>
    public string Name { get; }

    /// <summary>
    ///     Gets the display name (label).
    ///     This value will be displayed as item text botbase ComboBox in the main window.
    /// </summary>
    /// <value>
    ///     The display name.
    /// </value>
    public string DisplayName { get; }

    /// <summary>
    ///     This value will be displayed as TabPage text in the main window.
    /// </summary>
    /// <value>
    ///     The tab text.
    /// </value>
    public string TabText { get; }

    /// <summary>
    ///     Gets the view.
    /// </summary>
    /// <returns></returns>
    Control View { get; }

    /// <summary>
    ///     Translate the botbase plugin
    /// </summary>
    void Translate();
}
