using System;
using System.IO;

namespace RSBot.Core;

public static class GeneralConfig
{
    /// <summary>
    /// The config
    /// </summary>
    private static Config _config;

    /// <summary>
    /// Load config from file
    /// </summary>
    public static void Load()
    {
        var path = Path.Combine(Kernel.BasePath, "User", "Settings.rs");
        _config = new Config(path);
    }

    /// <summary>
    /// Returns a value indicating if the given config key exists.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public static bool Exists(string key)
    {
        if (_config == null) Load();
        return _config.Exists(key);
    }

    /// <summary>
    /// Gets the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    public static T Get<T>(string key, T defaultValue = default)
    {
        if (_config == null) Load();
        return _config.Get(key, defaultValue);
    }

    /// <summary>
    /// Sets the specified key inside the config.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void Set<T>(string key, T value)
    {
        if (_config == null) Load();
        _config.Set(key, value);
    }

    /// <summary>
    /// Saves the specified file.
    /// </summary>
    public static void Save()
    {
        if (_config == null) return;

        try
        {
            _config.Save();
        }
        catch (Exception ex)
        {
            Log.Debug($"[GeneralConfig] Could not save settings: {ex.Message}");
        }
    }
}
