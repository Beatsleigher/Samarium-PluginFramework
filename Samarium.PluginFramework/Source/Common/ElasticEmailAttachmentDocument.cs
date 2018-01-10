using System;

namespace Samarium.PluginFramework.Common {

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ElasticEmailAttachmentDocument {

        public string MimeType { get; set; }

        public IDictionary<string, string> Metadata { get; set; }

        public string FileName { get; set; }

        public string StringContent { get; set; }

    }

}
