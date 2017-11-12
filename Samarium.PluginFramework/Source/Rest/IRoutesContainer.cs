using System;

namespace Samarium.PluginFramework.Rest {

    using Newtonsoft.Json;

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for defining RESTful routes and their subsequent 
    /// handlers.
    /// </summary>
    /// <example >
    /// Example usage of this interface:
    /// 
    /// <code >
    /// public class MyPluginRoutes: IRoutesContainer {
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
    public abstract class IRoutesContainer {



    }
}
