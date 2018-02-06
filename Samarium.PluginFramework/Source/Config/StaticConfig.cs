using System;

namespace Samarium.PluginFramework.Config {

    using Samarium.PluginFramework.Exceptions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Abstract class containing a basic implementation of a static configuration class.
    /// Static configurations may not be modified during runtime, and extra/missing values will 
    /// cause severe issues with the plugin or application.
    /// </summary>
    public abstract class StaticConfig : IConfig {

        IConfigContainer configContainer;
        //CS0067
#pragma warning disable CS0067
        /// <summary>
        /// Event: occurs when a config was set.
        /// </summary>
        public event ConfigSetEventHandler ConfigSet;

        /// <summary>
        /// Event: occurs when configurations were (re-) loaded.
        /// </summary>
        public event ConfigsLoadedEventHandler ConfigsLoaded;
#pragma warning restore CS0067

        /// <summary>
        /// Gets a value indicating whether this configuration object is dynamic.
        /// Will always return false in this case.
        /// </summary>
        public bool IsDynamic => false;

        /// <summary>
        /// Gets the amount of configurations managed by this object.
        /// </summary>
        public int ConfigCount => (int)configContainer?.GetType().GetProperties().Count(IsConfigMember);

        /// <summary>
        /// Gets the configuration keys found in this object.
        /// </summary>
        public List<string> Keys => configContainer?.GetType().GetProperties().Where(IsConfigMember).Select(x => x.GetCustomAttribute<ConfigMemberAttribute>().Name).ToList();

        /// <summary>
        /// Gets the name of the configuration object.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the configuration container object.
        /// </summary>
        protected IConfigContainer ConfigContainer { get => configContainer; set => configContainer = value; }

        /// <summary>
        /// Object constructor.
        /// </summary>
        /// <param name="configContainer"></param>
        public StaticConfig(Type configContainer) {
            if (!IsConfigContainer(configContainer))
                throw new ArgumentException("Argument must be prefixed with the ConfigContainer attribute!");
        }
        
        /// <summary>
        /// Gets a config of the specific type with the given key.
        /// </summary>
        /// <typeparam name="T">The type of the config</typeparam>
        /// <param name="key">The config's key</param>
        /// <returns>The config or an exception</returns>
        public T GetConfig<T>(string key) {

            var config = configContainer?.GetType().GetProperties()
                                                   .Where(IsConfigMember)
                                                   .Where(x => x.GetCustomAttribute<ConfigMemberAttribute>().Name == key)
                                                   .FirstOrDefault();

            if (config is default)
                throw new ConfigNotFoundException(string.Format("The config \"{0}\" was not found!", key), key);
            if (config.PropertyType != typeof(T))
                throw new InvalidCastException(string.Format("Cannot cast {0} to {1}", config.PropertyType.Name, typeof(T).Name));

            // Try to return value
            return (T)config.GetValue(configContainer);
        }

        /// <summary>
        /// Gets a boolean value from the configuration object
        /// </summary>
        /// <param name="key">The configuration's key.</param>
        /// <returns></returns>
        public bool GetBool(string key) => GetConfig<bool>(key);

        /// <summary>
        /// Gets a double-precision decimal value from the configuration object.
        /// </summary>
        /// <param name="key">The configuration's key.</param>
        /// <returns></returns>
        public double GetDouble(string key) => GetConfig<double>(key);

        /// <summary>
        /// Gets an integer value from the configuration object.
        /// </summary>
        /// <param name="key">The configuration's key.</param>
        /// <returns></returns>
        public int GetInt(string key) => GetConfig<int>(key);

        /// <summary>
        /// Gets a string value from the configuration object.
        /// </summary>
        /// <param name="key">The configuration's key.</param>
        /// <returns></returns>
        public string GetString(string key) => GetConfig<string>(key);

        bool IsConfigMember(PropertyInfo propInfo) => propInfo.GetCustomAttributes<ConfigMemberAttribute>()?.Count() > 0;
        bool IsConfigContainer(Type obj) => obj?.GetCustomAttributes<ConfigContainerAttribute>()?.Count() > 0;

        /// <summary>
        /// Returns a value indicating whether this configuration manager's
        /// container contains a configuration with a given key.
        /// </summary>
        /// <param name="key">The key to check against.</param>
        /// <returns></returns>
        public bool HasKey(string key) => !(configContainer?.GetType().GetProperties()
                                                   .Where(IsConfigMember)
                                                   .Where(x => x.GetCustomAttribute<ConfigMemberAttribute>().Name == key)
                                                   .FirstOrDefault() is default);

        /// <summary>
        /// Loads the configurations.
        /// </summary>
        public abstract void LoadConfigs();

        /// <summary>
        /// Loads the default configurations.
        /// </summary>
        public abstract void LoadDefaults();

        /// <summary>
        /// Saves the configurations back to disk.
        /// </summary>
        public abstract void SaveConfigs();

        /// <summary>
        /// Sets a configuration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The configuration's key.</param>
        /// <param name="value">The new value of the configuration.</param>
        public void SetConfig<T>(string key, T value) {
            configContainer?.GetType().GetProperties()
                            .Where(IsConfigMember)
                            .Where(x => x.GetCustomAttribute<ConfigMemberAttribute>().Name == key)
                            .FirstOrDefault().SetValue(configContainer, value);

        }

        /// <summary>
        /// Serialises the configurations to a given type.
        /// </summary>
        /// <param name="serializationType">The format to serialise the data to.</param>
        /// <returns></returns>
        public abstract string ToString(ConfigSerializationType serializationType = ConfigSerializationType.Yaml);

        /// <summary>
        /// Attempts to retrieve a config's value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The config's key.</param>
        /// <param name="cfg">The config's value</param>
        /// <returns><code>true</code> if retrieval was successful.</returns>
        public bool TryGetConfig<T>(string key, out T cfg) {
            try {
                cfg = GetConfig<T>(key);
                return true;
            } catch {
                cfg = default;
                return false;
            }
        }

        /// <summary>
        /// LINQ-like qhere-clause.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<T> Where<T>(Func<string, bool> predicate) => throw new NotImplementedException();

    }
}
