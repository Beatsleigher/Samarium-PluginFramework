using System;

namespace Samarium.PluginFramework.Config {

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using ServiceStack.Text;

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using YamlDotNet.Serialization.NodeDeserializers;

    /// <summary>
    /// Defines a configuration that is inherintly dynamic.
    /// </summary>
    public class DynamicConfig : IConfig {

        #region Static members
        /// <summary>
        /// Gets a regular expression matching YAML boolean values.
        /// </summary>
        public static Regex YamlBoolRegex { get; }

        static DynamicConfig() {
            YamlBoolRegex = new Regex(
                @"y|Y|yes|Yes|YES|n|N|no|No|NO|true|True|TRUE|false|False|FALSE|on|On|ON|off|Off|OFF", 
                RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant
            );
        }
        #endregion

        const string TrueString = "true";
        const string FalseString = "false";

        /// <summary>
        /// Event gets called when a single config is set.
        /// </summary>
        public event ConfigSetEventHandler ConfigSet;

        /// <summary>
        /// Event gets called when the configs are loaded.
        /// </summary>
        public event ConfigsLoadedEventHandler ConfigsLoaded;
        
        /// <summary>
        /// The directory in which the configs are saved.
        /// </summary>
        public DirectoryInfo ConfigDirectory { get; }

        Dictionary<string, object> cfgHashMap;
        Dictionary<string, object> defCfgHashMap;
        FileInfo cfgFile;
        FileInfo defCfgFile;

        /// <summary>
        /// Recommended constructor.
        /// </summary>
        /// <param name="configDir">The config dir</param>
        /// <param name="name">The name of the config manager</param>
        /// <param name="defConfigs">A serialised string containing the default configs.</param>
        public DynamicConfig(DirectoryInfo configDir, string name, string defConfigs) {
            ConfigDirectory = configDir;
            Name = name;

            cfgFile = new FileInfo(Path.Combine(ConfigDirectory.FullName, Name));
            if (defCfgFile is default)
                defCfgFile = new FileInfo(Path.Combine(ConfigDirectory.FullName, string.Concat(Name.Replace(Path.GetExtension(Name), ""), ".def", Path.GetExtension(Name))));
            
            Init(defConfigs);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configDir"></param>
        /// <param name="name"></param>
        /// <param name="defConfigs"></param>
        public DynamicConfig(string configDir, string name, FileInfo defConfigs): this(new DirectoryInfo(configDir), name, File.ReadAllText(defConfigs.FullName)) {
            defCfgFile = defConfigs;
        }

        void Init(string defConfig) {
            ConfigDirectory.Create();
            if (!cfgFile.Exists) {
                File.WriteAllText(cfgFile.FullName, defConfig);
            } if (!defCfgFile.Exists || defCfgFile.Length < defConfig.Length) {
                File.WriteAllText(defCfgFile.FullName, defConfig);
            }
            LoadConfigs();
            LoadDefaults(defConfig);
        }

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this config manager is inherintly dynamic.
        /// Always <code>true</code>.
        /// </summary>
        public bool IsDynamic => true;

        /// <summary>
        /// Counts the configurations found.
        /// </summary>
        public int ConfigCount => cfgHashMap.Count;

        /// <summary>
        /// Gets a list containing all the keys found.
        /// </summary>
        public List<string> Keys => cfgHashMap?.Keys.ToList();

        /// <summary>
        /// Gets the name of this config manager.
        /// </summary>
        public string Name { get; }
        #endregion

        /// <summary>
        /// Gets a boolean value.
        /// </summary>
        /// <param name="key">The config's key</param>
        /// <returns></returns>
        public bool GetBool(string key) => GetConfig<bool>(key);

        /// <summary>
        /// Attempts to get a config's default value.
        /// </summary>
        /// <typeparam name="T">The config's type</typeparam>
        /// <param name="key">The config's key</param>
        /// <param name="cfg">The config's value</param>
        /// <returns><code>true</code> if the config was found.</returns>
        public bool TryGetDefault<T>(string key, out T cfg) {
            var hasKey = cfgHashMap.TryGetValue(key, out var value);

            if (hasKey && value is T tVal) {
                cfg = tVal;
                return true;
            } else if (hasKey && (typeof(T) == typeof(bool) && value is string str)) {
                // Ugly hack to work around YamlDotNet's shortcomings :/
                str = str.ConvertYamlBool();
                if (bool.TryParse(str, out var @bool)) {
                    cfg = (T)Convert.ChangeType(@bool, typeof(T));
                    return true;
                }
            } else if (!hasKey) {
                cfg = default;
                return false;
            }

            // Brute-force attempt
            try {
                cfg = ((JObject)value).ToObject<T>();
                return true;
            } catch (InvalidCastException) {
                cfg = default;
                return false;
            }
        }

        /// <summary>
        /// Attempts to get a config.
        /// </summary>
        /// <typeparam name="T">The config's type.</typeparam>
        /// <param name="key">The config's key.</param>
        /// <returns>The value of the config.</returns>
        public T GetConfig<T>(string key) {
            var hasKey = cfgHashMap.TryGetValue(key, out var value);

            if (hasKey && value is T tVal) {
                return tVal;
            } else if (hasKey && (typeof(T) == typeof(bool) && value is string str)) {
                // Ugly hack to work around YamlDotNet's shortcomings :/
                str = str.ConvertYamlBool();
                if (bool.TryParse(str, out var @bool))
                    return (T)Convert.ChangeType(@bool, typeof(T));
            } else if (!hasKey) {
                if (TryGetDefault(key, out T cfg)) {
                    return cfg;
                } else throw new KeyNotFoundException(string.Format("The key \"{0}\" could not be found!", key));
            }

            try {
                return ((JObject)value).ToObject<T>();
            } catch (InvalidCastException) {
                throw new InvalidCastException($"Cannot cast { value.GetType().Name } to { typeof(T).Name }!");
            }
        }

        /// <summary>
        /// Attempts to get a config.
        /// </summary>
        /// <typeparam name="T">The config's type.</typeparam>
        /// <param name="key">The config's key.</param>
        /// <param name="cfg">The config's value.</param>
        /// <returns><code>true</code>if the config was found.</returns>
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
        /// Gets a <code>double</code> from the configs.
        /// </summary>
        /// <param name="key">The config's key.</param>
        /// <returns></returns>
        public double GetDouble(string key) => GetConfig<double>(key);

        /// <summary>
        /// Gets an integer value from the configs.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInt(string key) => GetConfig<int>(key);

        /// <summary>
        /// Gets a string value from the configs.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key) => GetConfig<string>(key);

        /// <summary>
        /// Gets a value indicating whether this config utility contains a config
        /// with a given key.
        /// </summary>
        /// <param name="key">The key to check against.</param>
        /// <returns><code>true</code> if the config was found.</returns>
        public bool HasKey(string key) => cfgHashMap.ContainsKey(key);

        /// <summary>
        /// Loads the configs in to memory.
        /// </summary>
        public void LoadConfigs() {
#if USE_YAMLDOTNET
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            using (var reader = cfgFile.OpenText()) {
                cfgHashMap = deserializer.Deserialize<Dictionary<string, object>>(reader);
            }
#else
            cfgHashMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(cfgFile.FullName));
#endif
            cfgHashMap["config_dir"] = ConfigDirectory.FullName;
            ConfigsLoaded?.Invoke(this);
        }

        /// <summary>
        /// Loads the default configurations in to memory, replacing the old configs.
        /// This change is temporary, until they are saved to disk; when they become permanent.
        /// </summary>
        public void LoadDefaults() {
#if USE_YAMLDOTNET
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            using (var reader = cfgFile.OpenText()) {
                cfgHashMap = deserializer.Deserialize<Dictionary<string, object>>(reader);
            }
#else
            cfgHashMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(defCfgFile.FullName));
#endif
            ConfigsLoaded?.Invoke(this);
        }

        void LoadDefaults(string defaults, bool overwrite = false) {
#if USE_YAMLDOTNET
            defCfgHashMap = deserializer.Deserialize<Dictionary<string, object>>(defaults);
            if (overwrite)
                cfgHashMap = defCfgHashMap;
#else
            defCfgHashMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(defaults);
            if (overwrite)
                cfgHashMap = defCfgHashMap;
#endif
            ConfigsLoaded?.Invoke(this);
        }

        /// <summary>
        /// Saves the configurations to disk.
        /// </summary>
        public async void SaveConfigs() {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            using (var writer = cfgFile.OpenWrite()) {
                var resultingText = Encoding.UTF8.GetBytes(serializer.Serialize(cfgHashMap));
                await writer.WriteAsync(resultingText, 0, resultingText.Length);
            }

        }

        /// <summary>
        /// Sets a given config.
        /// </summary>
        /// <typeparam name="T">The config's type-</typeparam>
        /// <param name="key">The config's key.</param>
        /// <param name="value">The new value of the config.</param>
        public void SetConfig<T>(string key, T value) {
            if (HasKey(key)) {
                cfgHashMap.TryGetValue(key, out var curValue);
                if (!(curValue is T))
                    throw new InvalidDataException($"Cannot convert { typeof(T).Name } to { curValue.GetType().Name }!");
                cfgHashMap.Remove(key);
            }


            cfgHashMap.Add(key, value);
            ConfigSet?.Invoke(this, key);

        }

        /// <summary>
        /// Enumerates configs matching a given predicate.
        /// </summary>
        /// <typeparam name="T">The type of the configs.</typeparam>
        /// <param name="predicate">The predicate to check against.</param>
        /// <returns></returns>
        public IEnumerable<T> Where<T>(Func<string, bool> predicate) {
            List<T> values = new List<T>();

            foreach (var key in Keys.Where(predicate))
                values.Add(GetConfig<T>(key));

            return values;
        }

        /// <summary>
        /// Serialises this object so it may be saved to disk.
        /// </summary>
        /// <param name="serializationType"></param>
        /// <returns></returns>
        public string ToString(ConfigSerializationType serializationType = ConfigSerializationType.Yaml) {
            switch (serializationType) {
                case ConfigSerializationType.Yaml:
                    return new SerializerBuilder().WithNamingConvention(new UnderscoredNamingConvention()).Build().Serialize(cfgHashMap);
                case ConfigSerializationType.Json:
                    return JsonConvert.SerializeObject(cfgHashMap, Formatting.Indented);
                case ConfigSerializationType.CSV:
                    return CsvSerializer.SerializeToCsv(cfgHashMap);
                case ConfigSerializationType.XML:
                    return XmlSerializer.SerializeToString(cfgHashMap);
                default:
                    throw new ArgumentOutOfRangeException(nameof(serializationType), "Invalid serialization type!");
            }
        }

    }
}
