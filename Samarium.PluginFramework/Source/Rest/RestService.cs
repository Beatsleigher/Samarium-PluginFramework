using System;

namespace Samarium.PluginFramework.Rest {

    using Logger;

    using PluginFramework;
    using Config;
    using Plugin;
    
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;
    using System.ServiceModel.Description;
    using System.Collections.ObjectModel;
    using System.ServiceModel.Channels;

    /// <summary>
    /// 
    /// </summary>
    public sealed partial class RestService {

        /// <summary>
        /// Defines a constant for the HTT protocol.
        /// </summary>
        public const string HTTP = "http://";

        /// <summary>
        /// Defines a constant for the secure HTT protocol.
        /// </summary>
        public const string HTTPS = "https://";

        /// <summary>
        /// Defines the root route for the RESTful services.
        /// This route is provided by the Samarium Plugin Framework.
        /// </summary>
        public const string RestRootRoute = "/";

        #region Instance Variables
        private Logger logger;
        private Uri baseUrl;
        private WebServiceHost serviceHost;
        #endregion

        #region Static Variables
        /// <summary>
        /// Gets or sets the system configuration used with this module.
        /// </summary>
        public static IConfig SystemConfig { get; set; }
        #endregion

        #region Singleton
        private RestService(IEndpointContainer[] endpoints, IPAddress listenerIp, ushort port = 80, string routePrefix = "", bool useHttps = false) {
            logger = Logger.CreateInstance(nameof(RestService), SystemConfig.GetString("config_dir"));

            logger.Info("RESTful services are starting...");
            logger.Info("Checking listener IP...");

#if !ALLOW_PUBLIC_ADDR
            if (listenerIp.IsIPv4LinkLocal() || listenerIp.IsIPv6LinkLocal) {
#endif
                // SAFE IP
                logger.Ok("IP checks out... initializing service host...");
                var tmp = string.Format("{0}{1}:{2:d}", useHttps ? HTTPS : HTTP, listenerIp.ToString(), port);
                baseUrl = new Uri(tmp);

                logger.Info("Loading {0} endpoints!", endpoints.Length);
                var factory = new ExtendedServiceHostFactory();
                var serviceHost = factory.CreateServiceHost(default, new[] { baseUrl }) as WebServiceHost;

                logger.Info("Adding endpoints...");
                /*foreach (var endpoint in endpoints) {
                    if (useHttps) {
                        var binding = new BasicHttpsBinding();
                        
                        var baseUrl = string.Format("{0}/{1}", tmp, endpoint.ServiceEndpointBase.Trim('/'));
                        var svHost = serviceHost.AddServiceEndpoint(endpoint.GetType(), binding, endpoint.ServiceEndpointBase);
                        svHost.EndpointBehaviors.Add(new RequestInspector());
                    }
                }*/

                serviceHost.Open();
                logger.Ok("Service host started!");
                logger.Info("Registering default routes... please wait!");
#if !ALLOW_PUBLIC_ADDR
            } else {
                // UNSAFE! ABORT!
                throw new ArgumentException("REST service is only allowed to listen to LINK-LOCAL IP addresses!");
            }
#endif

        }

        /// <summary>
        /// Default constructor because it's required.
        /// Foxtrott Unicorn Charlie Kilo!
        /// </summary>
        public RestService() { }

        private static RestService _instance;

        /// <summary>
        /// Attempts to retrieve an instance of this class.
        /// If no instance was generated previously, will throw a <see cref="NullReferenceException"/>
        /// </summary>
        public static RestService Instance => _instance ?? throw new NullReferenceException($"No instance of { nameof(RestService) } was created!");

        /// <summary>
        /// Attempts to create a new instance of this class.
        /// If an instance was previously generated, that instance is returned.
        /// </summary>
        /// <param name="endpoints">The different endpoints found within the application.</param>
        /// <param name="listenerIp">The address to listen to.</param>
        /// <param name="port">The port to listen to</param>
        /// <param name="routePrefix">An optional route prefix (unused).</param>
        /// <returns>An instance of this class.</returns>
        public static RestService CreateInstance(IEndpointContainer[] endpoints, IPAddress listenerIp, ushort port = 80, string routePrefix = "") => _instance ?? (_instance = new RestService(endpoints, listenerIp, port, routePrefix));
        #endregion

        

    }
}
