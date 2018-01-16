using System;

namespace Samarium.PluginFramework.ElasticSearch {

    using Nest;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a parsed email attachment to be stored in/retrieve from Elasticsearch.
    /// </summary>
    [ElasticsearchType(Name = "attachment_document")]
    public class AttachmentElasticDocument {

        /// <summary>
        /// Gets or sets the file's (text) content
        /// </summary>
        [Keyword]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the file's content type
        /// </summary>
        [Keyword]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the file's name.
        /// </summary>
        [Keyword]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file's size in bytes.
        /// </summary>
        [Number(NumberType.Long, IgnoreMalformed = true, Coerce = true)]
        public long FileSize { get; set; }

        /// <summary>
        /// Gets or sets the file's metadata.
        /// </summary>

        [Nested][Object]
        public Dictionary<string, string> Metadata { get; set; }

    }
}
