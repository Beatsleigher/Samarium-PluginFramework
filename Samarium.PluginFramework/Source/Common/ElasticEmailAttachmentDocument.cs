using System;

namespace Samarium.PluginFramework.Common {

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ElasticEmailAttachmentDocument {

        string MimeType { get; set; }

        IEnumerable<string> MetaData { get; set; }

        string FileName { get; set; }

        string StringContent { get; set; }

    }

}
