using System;

namespace Samarium.PluginFramework.Config {

    using Samarium.PluginFramework.Exceptions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class StaticConfig : IConfig {

        IConfigContainer configContainer;

        public event ConfigSetEventHandler ConfigSet;
        public event ConfigsLoadedEventHandler ConfigsLoaded;

        public bool IsDynamic => false;

        public int ConfigCount => (int)configContainer?.GetType().GetProperties().Count(IsConfigMember);

        public List<string> Keys => configContainer?.GetType().GetProperties().OfType<ConfigMemberAttribute>().Select(x => x.Name).ToList();

        public abstract string Name { get; }

        protected IConfigContainer ConfigContainer { get => configContainer; set => configContainer = value; }

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

        public bool GetBool(string key) => GetConfig<bool>(key);
        public double GetDouble(string key) => GetConfig<double>(key);
        public int GetInt(string key) => GetConfig<int>(key);
        public string GetString(string key) => GetConfig<string>(key);

        bool IsConfigMember(PropertyInfo propInfo) => propInfo.GetCustomAttributes<ConfigMemberAttribute>()?.Count() > 0;
        bool IsConfigContainer(Type obj) => obj?.GetCustomAttributes<ConfigContainerAttribute>()?.Count() > 0;

        public bool HasKey(string key) => !(configContainer?.GetType().GetProperties()
                                                   .Where(IsConfigMember)
                                                   .Where(x => x.GetCustomAttribute<ConfigMemberAttribute>().Name == key)
                                                   .FirstOrDefault() is default);

        public abstract void LoadConfigs();
        public abstract void LoadDefaults();
        public abstract void SaveConfigs();

        public void SetConfig<T>(string key, T value) {
            configContainer?.GetType().GetProperties()
                            .Where(IsConfigMember)
                            .Where(x => x.GetCustomAttribute<ConfigMemberAttribute>().Name == key)
                            .FirstOrDefault().SetValue(configContainer, value);

        }

        public abstract string ToString(ConfigSerializationType serializationType = ConfigSerializationType.Yaml);

        public bool TryGetConfig<T>(string key, out T cfg) {
            try {
                cfg = GetConfig<T>(key);
                return true;
            } catch {
                cfg = default;
                return false;
            }
        }

        public IEnumerable<T> Where<T>(Func<string, bool> predicate) => throw new NotImplementedException();

    }
}
