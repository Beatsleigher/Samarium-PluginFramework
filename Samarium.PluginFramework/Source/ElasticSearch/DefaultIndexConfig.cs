using System;

namespace Samarium.PluginFramework.ElasticSearch {

    using Nest;
    using Newtonsoft.Json;

    public class DefaultIndexConfig {

        public bool ReturnStackTraces { get; set; }

        public bool HumanReadable { get; set; }

        public bool PrettyPrint { get; set; }

        public SamariumIndexConfig IndexSettings { get; set; }

        public bool UpdateAllTypes { get; set; }
        
        public int WaitForActiveShards { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);

        public CreateIndexRequest ToCreateIndexRequest(string idxName) {
            return new CreateIndexRequest(idxName) {
                ErrorTrace = ReturnStackTraces,
                Human = HumanReadable,
                Pretty = PrettyPrint,
                Settings = IndexSettings.ToIndexSettings(),
                UpdateAllTypes = UpdateAllTypes,
                WaitForActiveShards = WaitForActiveShards.ToString()
            };
        }

    }

    public class SamariumIndexConfig {

        public int? NumberOfReplicas { get; set; }

        public int? NumberOfShards { get; set; }

        public AutoExpandReplicas AutoExpandReplicas { get; set; }

        public FileSystemStorageImplementation FileSystemStorageImplementation { get; set; }

        public TimeSpan RefreshInterval { get; set; }

        public IndexSettings ToIndexSettings() {
            return new IndexSettings {
                FileSystemStorageImplementation = FileSystemStorageImplementation,
                NumberOfReplicas = NumberOfReplicas,
                NumberOfShards = NumberOfShards,
                RefreshInterval = RefreshInterval
            };
        }

    }

}
