using System;

namespace Samarium.PluginFramework.Config {

    using Newtonsoft.Json;

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

    public class DynamicConfig : IConfig {

        #region Static members
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

        public event ConfigSetEventHandler ConfigSet;
        public event ConfigsLoadedEventHandler ConfigsLoaded;
        
        public DirectoryInfo ConfigDirectory { get; }

        Dictionary<string, object> cfgHashMap;
        FileInfo cfgFile;
        FileInfo defCfgFile;

        public DynamicConfig(DirectoryInfo configDir, string name, string defConfigs) {
            ConfigDirectory = configDir;
            Name = name;

            cfgFile = new FileInfo(Path.Combine(ConfigDirectory.FullName, Name));
            if (defCfgFile is default)
                defCfgFile = new FileInfo(Path.Combine(ConfigDirectory.FullName, string.Concat(Name.Replace(Path.GetExtension(Name), ""), ".def", Path.GetExtension(Name))));
            
            Init(defConfigs);
        }

        public DynamicConfig(string configDir, string name, FileInfo defConfigs): this(new DirectoryInfo(configDir), name, File.ReadAllText(defConfigs.FullName)) {
            defCfgFile = defConfigs;
        }

        void Init(string defConfig) {
            if (!cfgFile.Exists) {
                File.WriteAllText(cfgFile.FullName, defConfig);
            }
            LoadConfigs();
        }

        #region Properties
        public bool IsDynamic => true;

        public int ConfigCount => cfgHashMap.Count;

        public List<string> Keys => cfgHashMap?.Keys.ToList();

        public string Name { get; }
        #endregion

        public bool GetBool(string key) => GetConfig<bool>(key);

        public T GetConfig<T>(string key) {
            var hasKey = cfgHashMap.TryGetValue(key, out var value);

            if (hasKey && value is T tVal) {
                return tVal;
            } else if (hasKey && (typeof(T) == typeof(bool) && value is string str)) {
                // Ugly hack to work around YamlDotNet's shortcomings :/
                str = str.ConvertYamlBool();
                if (bool.TryParse(str, out var @bool))
                    return (T)Convert.ChangeType(@bool, typeof(T));
            }

            throw new InvalidCastException($"Cannot cast { value.GetType().Name } to { typeof(T).Name }!");
        }

        public double GetDouble(string key) => GetConfig<double>(key);

        public int GetInt(string key) => GetConfig<int>(key);

        public string GetString(string key) => GetConfig<string>(key);

        public bool HasKey(string key) => cfgHashMap.ContainsKey(key);

        public void LoadConfigs() {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            using (var reader = cfgFile.OpenText()) {
                cfgHashMap = deserializer.Deserialize<Dictionary<string, object>>(reader);
            }
            return;
        }

        /// <summary>
        /// Loads the default configurations in to memory, replacing the old configs.
        /// This change is temporary, until they are saved to disk; when they become permanent.
        /// </summary>
        public void LoadDefaults() {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            using (var reader = defCfgFile.OpenText()) {
                cfgHashMap = deserializer.Deserialize<Dictionary<string, object>>(reader);
            }
        }

        public async void SaveConfigs() {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            using (var writer = cfgFile.OpenWrite()) {
                var resultingText = Encoding.UTF8.GetBytes(serializer.Serialize(cfgHashMap));
                await writer.WriteAsync(resultingText, 0, resultingText.Length);
            }

        }

        public void SetConfig<T>(string key, T value) {
            if (HasKey(key)) {
                cfgHashMap.TryGetValue(key, out var curValue);
                if (!(curValue is T))
                    throw new InvalidDataException($"Cannot convert { typeof(T).Name } to { curValue.GetType().Name }!");
                cfgHashMap.Remove(key);
            }


            cfgHashMap.Add(key, value);

        }

        public IEnumerable<T> Where<T>(Func<string, bool> predicate) {
            List<T> values = new List<T>();

            foreach (var key in Keys.Where(predicate))
                values.Add(GetConfig<T>(key));

            return values;
        }

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
