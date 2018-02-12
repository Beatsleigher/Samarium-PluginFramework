using System;

namespace Samarium.PluginFramework.Rest {

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading.Tasks;

    [ServiceContract(Name = nameof(RestService))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    partial class RestService: IEndpointContainer {

        public string ServiceEndpointBase => "/";

        [WebGet(UriTemplate = "/")]
        public Stream IndexRoute() {
            var htmlString = "<html ><head ><title >It Works!</title></head><body >Hooray!</body></html>";
            this.GetOutgoingWebResponse().SetAsOk("It worked!", htmlString.Length, "text/html");
            return this.GetHtmlStream(htmlString);
        }

    }
}
