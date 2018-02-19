using System;

namespace Samarium.PluginFramework.ElasticSearch {

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an object used for indexing entire files to Elasticsearch.
    /// This object contains fields useful for indexing raw data, as well as other content.
    /// </summary>
    public class FileElasticDocument {

        /// <summary>
        /// Gets or sets the name of the file being indexed.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the original path of the file.
        /// </summary>
        public string OriginalPath { get; set; }

        /// <summary>
        /// Gets or sets the MIME/Content type of the file.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the text content of the file (if any).
        /// </summary>
        public string TextContent { get; set; }

        /// <summary>
        /// Gets or sets meta information about the file.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Gets or sets the raw data of the file.
        /// </summary>
        public byte[] RawData { get; set; }

        /// <summary>
        /// Gets the length of the file as a <code>long?</code>.
        /// </summary>
        public long? FileSizeInBytes => RawData?.Length;

    }
}
