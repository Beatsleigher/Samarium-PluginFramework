using System;

namespace Samarium.PluginFramework.Common {

    using MimeKit;

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a RAW file (attachment).
    /// </summary>
    public class RawEmailAttachment {

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the contents of the file.
        /// </summary>
        public IReadOnlyCollection<byte> Content { get; set; }

        /// <summary>
        /// Gets or sets the content type of this file.
        /// </summary>
        public ContentType ContentType { get; set; }

    }
}
