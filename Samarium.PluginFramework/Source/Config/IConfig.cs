using System;

namespace Samarium.PluginFramework.Config {

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Basic prototype for configurations.
    /// </summary>
    public interface IConfig {

        /// <summary>
        /// Event called when a config is set to a new value.
        /// </summary>
        event ConfigSetEventHandler ConfigSet;

        /// <summary>
        /// Event called when the configs in an inherited object are loaded.
        /// </summary>
        event ConfigsLoadedEventHandler ConfigsLoaded;

        /// <summary>
        /// Gets a value indicating whether this confiuration is dynamic
        /// in the sense that new configurations can be added file the file
        /// and still be recognized.
        /// </summary>
        bool IsDynamic { get; }

        /// <summary>
        /// Gets the amount of configurations loaded.
        /// </summary>
        int ConfigCount { get; }

        /// <summary>
        /// Gets the keys of all loaded configs.
        /// </summary>
        List<string> Keys { get; }

        /// <summary>
        /// Gets the (file) name of this configuration.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// gets a value indicating whether this configuration contains a given key.
        /// </summary>
        /// <param name="key">The key to check against.</param>
        /// <returns><code >true</code> if key was found.</returns>
        bool HasKey(string key);

        /// <summary>
        /// Gets a config of type T.
        /// </summary>
        /// <typeparam name="T">The type of the stored config.</typeparam>
        /// <param name="key">The config's key.</param>
        /// <returns>The value of the config in deserialized form.</returns>
        T GetConfig<T>(string key);

        /// <summary>
        /// Gets a string config.
        /// </summary>
        /// <param name="key">The config's key</param>
        /// <returns>The value of the config.</returns>
        string GetString(string key);

        /// <summary>
        /// Gets an integer config.
        /// </summary>
        /// <param name="key">The config's key</param>
        /// <returns>The value of the config.</returns>
        int GetInt(string key);

        /// <summary>
        /// Gets a double-precision floating point config.
        /// </summary>
        /// <param name="key">The config's key.</param>
        /// <returns>The value of the config.</returns>
        double GetDouble(string key);

        /// <summary>
        /// Gets a boolean config.
        /// </summary>
        /// <param name="key">The config's key.</param>
        /// <returns>The value of the config.</returns>
        bool GetBool(string key);

        /// <summary>
        /// Loads the configuration file in to memory.
        /// Can also be used to re-load configurations.
        /// </summary>
        void LoadConfigs();

        /// <summary>
        /// Loads the default configurations in to memory, replacing the old configs.
        /// This change is temporary, until they are saved to disk; when they become permanent.
        /// </summary>
        void LoadDefaults();

        /// <summary>
        /// Saves all configurations modified in memory to disk.
        /// </summary>
        void SaveConfigs();

        /// <summary>
        /// Sets the value of a configuration, or (if config is dynamic) creates a new configuration.
        /// </summary>
        /// <typeparam name="T">The (value) type of the config.</typeparam>
        /// <param name="key">The config's key.</param>
        /// <param name="value">The (new) value of the config.</param>
        void SetConfig<T>(string key, T value);

        /// <summary>
        /// Gets all configurations where the predicate (keys) return true.
        /// </summary>
        /// <typeparam name="T">The config type</typeparam>
        /// <param name="predicate">The filter</param>
        /// <returns>A list of all configs where the predictate returned true.</returns>
        IEnumerable<T> Where<T>(Func<string, bool> predicate);

        /// <summary>
        /// Gets a string representation of the configurations stored in this object.
        /// </summary>
        /// <param name="serializationType">The serialisation language to serialise to. Default: YAML</param>
        /// <returns>The serialised data stored in this object.</returns>
        string ToString(ConfigSerializationType serializationType = ConfigSerializationType.Yaml);

    }
}
