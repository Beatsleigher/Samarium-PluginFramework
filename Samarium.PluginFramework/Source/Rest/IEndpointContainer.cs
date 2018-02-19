using System;

namespace Samarium.PluginFramework.Rest {

    /// <summary>
    /// Interface for defining RESTful routes and their subsequent 
    /// handlers.
    /// </summary>
    public interface IEndpointContainer {

        /// <summary>
        /// Gets the handler that is called to process the request.
        /// </summary>
        RestulRouteHandler Handler { get; set; }

        /// <summary>
        /// Gets the HTTP method required for this transaction.
        /// </summary>
        HttpMethod Method { get; set; }
        
        /// <summary>
        /// Gets the URI template for this endpoint (route).
        /// </summary>
        UriTemplate UriTemplate { get; set; }

        /// <summary>
        /// Gets the string representation for this endpoint's template.
        /// </summary>
        string UriTemplateString { get; set; }

    }
}
