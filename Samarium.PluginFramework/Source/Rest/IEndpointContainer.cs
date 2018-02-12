using System;

namespace Samarium.PluginFramework.Rest {

    /// <summary>
    /// Interface for defining RESTful routes and their subsequent 
    /// handlers.
    /// </summary>
    /// <example >
    /// Example usage of this interface:
    /// 
    /// <code >
    /// [ServiceContract(Name = "MyUniqueServiceContractName")]
    /// public class MyPluginRoutes: IEndpointContainer {
    ///     
    ///     [OperationContract]
    ///     [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/mygetroute")]
    ///     public Stream MyGetRoute() {
    ///         return GetJsonStream(new { /* example data */ });
    ///     }
    ///     
    ///     [OperationContract]
    ///     [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/mypostroute")]
    ///     public Stream MyPostRoute(MyPostRequest request) {
    ///         // do something with request
    ///         return GetJsonStream(new { /* example data */ });
    ///     }
    /// 
    /// }
    /// </code>
    /// 
    /// </example>
    public interface IEndpointContainer {
        
        /// <summary>
        /// Gets or sets the base address for the endpoint container the plugin exposes.
        /// This must NOT contain the protocol, host, port or anything concerning the actual routing to the application.
        /// Essentially the base URL will be built as such:
        /// application-provided: http://hostname:port/{your_base_here}[/your_other_routes]!
        /// </summary>
        string ServiceEndpointBase { get; }

    }
}
