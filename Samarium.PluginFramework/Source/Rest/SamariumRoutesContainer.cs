using System;

namespace Samarium.PluginFramework.Rest {

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading.Tasks;

    [ServiceContract(Name = nameof(SamariumRoutesContainer))]
    internal class SamariumRoutesContainer {

        public string ServiceEndpointBase => "/";

        [OperationContract]
        [WebGet(UriTemplate = "/", BodyStyle = WebMessageBodyStyle.Bare)]
        public Stream IndexRoute() {
            throw new NotImplementedException();
        }

        public Stream InvalidRoute() => throw new NotImplementedException();

    }
}
