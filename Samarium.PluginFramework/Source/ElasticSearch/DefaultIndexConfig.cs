using System;

namespace Samarium.PluginFramework.ElasticSearch {

    using Nest;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines default values for new indices created in ElasticSearch.
    /// </summary>
    public class DefaultIndexConfig {

        /// <summary>
        /// Gets or sets a value indicating whether to return stack traces or not.
        /// </summary>
        public bool ReturnStackTraces { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to return human-readable data or not.
        /// </summary>
        public bool HumanReadable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to return pretty data.
        /// </summary>
        public bool PrettyPrint { get; set; }

        /// <summary>
        /// Defines default index settings.
        /// </summary>
        public SamariumIndexConfig IndexSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to update all types or not.
        /// </summary>
        public bool UpdateAllTypes { get; set; }
        
        /// <summary>
        /// Gets or sets the total amount of shards to wait for, before attempting to restore an index.
        /// </summary>
        public int WaitForActiveShards { get; set; }

        /// <summary>
        /// Serialises this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);

        /// <summary>
        /// Generates a CreateIndexRequest from this object.
        /// </summary>
        /// <param name="idxName"></param>
        /// <returns></returns>
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

    /// <summary>
    /// Defines configs for indices created by Samarium.
    /// </summary>
    public class SamariumIndexConfig {

        /// <summary>
        /// Gets or sets the number of replicas.
        /// </summary>
        public int? NumberOfReplicas { get; set; }

        /// <summary>
        /// Gets or sets the number of shards.
        /// </summary>
        public int? NumberOfShards { get; set; }

        /// <summary>
        /// Gets or sets the amount of replicas.
        /// </summary>
        public AutoExpandReplicas AutoExpandReplicas { get; set; }

        /// <summary>
        /// Gets or sets the file system storage implementation
        /// </summary>
        public FileSystemStorageImplementation FileSystemStorageImplementation { get; set; }

        /// <summary>
        /// Gets or sets the refresh interval of each index created with Samarium.
        /// </summary>
        public TimeSpan RefreshInterval { get; set; }

        /// <summary>
        /// Generates an IndexSettings object from these values.
        /// </summary>
        /// <returns></returns>
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
