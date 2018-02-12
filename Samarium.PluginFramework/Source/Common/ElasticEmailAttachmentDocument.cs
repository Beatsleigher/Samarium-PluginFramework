using System;

namespace Samarium.PluginFramework.Common {

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an email attachment in an indexable format.
    /// </summary>
    public class ElasticEmailAttachmentDocument {

        /// <summary>
        /// The attachment's MIME type.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// The attachment's metadata.
        /// </summary>
        public IDictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// The attachment's file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The attachment's string contents.
        /// </summary>
        public string StringContent { get; set; }

    }

}
