using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samarium.PluginFramework.Rest {

    /// <summary>
    /// Contains information about routing, HTTP methods, templating 
    /// and the action to execute for a given RESTful route.
    /// </summary>
    public class EndpointContainer : IEndpointContainer {

        /// <summary>
        /// Gets or sets the handler that is called to process an incoming request.
        /// </summary>
        public RestulRouteHandler Handler { get; set; }

        /// <summary>
        /// Gets or sets the HTTP method required for this transaction.
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// Gets or sets the URI template for this endpoint (route).
        /// </summary>
        public UriTemplate UriTemplate { get; set; }

        /// <summary>
        /// Gets the string representation for this endpoint's template.
        /// </summary>
        public string UriTemplateString { get; set; }
    }
}
