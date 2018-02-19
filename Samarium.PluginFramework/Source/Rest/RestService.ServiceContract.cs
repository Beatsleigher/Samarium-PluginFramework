using System;

namespace Samarium.PluginFramework.Rest {
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading.Tasks;

    [ServiceContract(Name = nameof(RestService))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    partial class RestService {
        
        const string BaseRoute = "/";
        const string GetRoute = BaseRoute + "_get/*";
        const string PostRoute = BaseRoute + "_post/*";
        const string PutRoute = BaseRoute + "_put/*";
        const string DeleteRoute = BaseRoute + "_del/*";
        const string ListRoutesRoute = BaseRoute + "_routes";
        readonly object NotFound = new {
            Status = HttpStatusCode.NotFound,
            Description = "This is not the object you're looking for. Move along, move along..."
        };
        const string NotFoundFormatString = "The desired route ({0}) was not found on this server.";
        const string JsonMimeType = "application/json";

        /// <summary>
        /// Gets this webservice's endpoint base address.
        /// These addresses are relative to the configured
        /// addresses.
        /// </summary>
        public string ServiceEndpointBase => BaseRoute;

        /// <summary>
        /// The index route. Doesn't really do anything.
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = BaseRoute)]
        public Stream IndexRoute() {
            var htmlString = "<html ><head ><title >It Works!</title></head><body >Hooray!</body></html>";
            this.GetOutgoingWebResponse().SetAsOk("It worked!", htmlString.Length, "text/html");
            return this.GetHtmlStream(htmlString);
        }

        /// <summary>
        /// Gets a list containing all routes that were detected in the application.
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = ListRoutesRoute)]
        public Stream ListRoutes() {
            var jsonOutput = this.GetPrettyJsonStream(restfulEndpoints.Select(x => new { x.UriTemplateString, x.UriTemplate, x.Method }));
            this.GetOutgoingWebResponse().SetAsOk("Following routes were detected", jsonOutput.Length, JsonMimeType);
            return jsonOutput;
        }

        /// <summary>
        /// Calls a GET route within the application and returns the output.
        /// </summary>
        /// <returns></returns>
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = GetRoute)]
        public Stream CallGetRoute() {
            var webRequest = this.GetIncomingWebRequest();
            var webResponse = this.GetOutgoingWebResponse();

            var endpoint = restfulEndpoints.FirstOrDefault(x => GetMatchingRoute(x, webRequest, HttpMethod.GET));

            if (endpoint is default) {
                var json = this.GetPrettyJsonStream(NotFound);
                webResponse.SetAsNotFound(string.Format(NotFoundFormatString, webRequest.UriTemplateMatch.RequestUri), json.Length, JsonMimeType);
                return json;
                
            }

            return endpoint.Handler(webRequest, webResponse, webRequest.UriTemplateMatch.QueryParameters);
        }

        /// <summary>
        /// Calls a POST route within the application and returns the output.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = PostRoute)]
        public Stream CallPostRoute(Stream state) {
            var webRequest = this.GetIncomingWebRequest();
            var webResponse = this.GetOutgoingWebResponse();

            var endpoint = restfulEndpoints.FirstOrDefault(x => GetMatchingRoute(x, webRequest, HttpMethod.POST));

            if (endpoint is default) {
                var json = this.GetPrettyJsonStream(NotFound);
                webResponse.SetAsNotFound(string.Format(NotFoundFormatString, webRequest.UriTemplateMatch.RequestUri), json.Length, JsonMimeType);
                return json;
            }

            using (var sReader = new StreamReader(state)) {
                var jsonObj = (JsonConvert.DeserializeObject(sReader.ReadToEnd()) as JObject);

                return endpoint.Handler(webRequest, webResponse, jsonObj);
            }
        }

        /// <summary>
        /// Calls a PUT route within the application and returns the output.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = PutRoute)]
        public Stream CallPutRoute(object state) {
            var webRequest = this.GetIncomingWebRequest();
            var webResponse = this.GetOutgoingWebResponse();

            var endpoint = restfulEndpoints.FirstOrDefault(x => GetMatchingRoute(x, webRequest, HttpMethod.PUT));

            if (endpoint is default) {
                var json = this.GetPrettyJsonStream(NotFound);
                webResponse.SetAsNotFound(string.Format(NotFoundFormatString, webRequest.UriTemplateMatch.RequestUri), json.Length, JsonMimeType);
                return json;

            }

            return endpoint.Handler(webRequest, webResponse, state);
        }

        /// <summary>
        /// Calls a DELETE route within the application and returns the output.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = DeleteRoute)]
        public Stream CallDeleteRoute(object state) {
            var webRequest = this.GetIncomingWebRequest();
            var webResponse = this.GetOutgoingWebResponse();

            var endpoint = restfulEndpoints.FirstOrDefault(x => GetMatchingRoute(x, webRequest, HttpMethod.DELETE));

            if (endpoint is default) {
                var json = this.GetPrettyJsonStream(NotFound);
                webResponse.SetAsNotFound(string.Format(NotFoundFormatString, webRequest.UriTemplateMatch.RequestUri), json.Length, JsonMimeType);
                return json;
            }

            return endpoint.Handler(webRequest, webResponse, state);
        }

        private bool GetMatchingRoute(IEndpointContainer endpoint, IncomingWebRequestContext webRequest, HttpMethod method) {
            //x.Method == HttpMethod.GET && x.UriTemplate.Match(baseUrl, webRequest.UriTemplateMatch.RequestUri).
            var isGet = endpoint.Method == method;
            var uriTemplateMatch = endpoint.UriTemplate.Match(baseUrl, webRequest.UriTemplateMatch.RequestUri);
            var segments = new List<string> { string.Empty };
            segments.AddRange(webRequest.UriTemplateMatch.WildcardPathSegments);
            var foundUriSegments = string.Join("/", segments.ToArray());
            var isUriMatch = foundUriSegments == endpoint.UriTemplate.ToString();

            return isGet && isUriMatch;
        }

    }
}
