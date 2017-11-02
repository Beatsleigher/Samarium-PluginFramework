using System;

namespace Samarium.PluginFramework.Config {

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public class DynamicConfig : IConfig {

        public event ConfigSetEventHandler ConfigSet;

        public event ConfigsLoadedEventHandler ConfigsLoaded;

        public static DirectoryInfo ConfigDirectory { get; }

        static DynamicConfig() {
            ConfigDirectory = new DirectoryInfo(Path.GetDirectoryName(typeof(DynamicConfig).Assembly.Location));
        }

        Dictionary<string, object> cfgHashMap;
        FileInfo cfgFile;

        public DynamicConfig(string name, string defConfigs) {
            Name = name;

            cfgFile = new FileInfo(Path.Combine(ConfigDirectory.FullName, Name));
        }
        public DynamicConfig(string name, FileInfo defConfigs): this(name, File.ReadAllText(defConfigs.FullName)) { }

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
            } else throw new InvalidCastException($"Cannot cast { value.GetType().Name } to { typeof(T).Name }!");

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
    }
}
