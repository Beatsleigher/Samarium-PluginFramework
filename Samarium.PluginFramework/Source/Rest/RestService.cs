using System;

namespace Samarium.PluginFramework.Rest {

    using Logger;
    using PluginFramework;
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

    [ServiceContract(Name = nameof(RestService), ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public sealed class RestService {

        #region Instance Variables
        private Logger logger;
        private Uri baseUrl;
        private WebServiceHost serviceHost;
        #endregion

        #region Singleton
        private RestService(IPAddress listenerIp, short port = 80, string routePrefix = "") {
            logger = Logger.CreateInstance(nameof(RestService), default);

            logger.Info("RESTful services are starting...");
            logger.Info("Checking listener IP...");

            if (listenerIp.IsIPv4LinkLocal() || listenerIp.IsIPv6LinkLocal) {
                // SAFE IP
                logger.Ok("IP checks out... initializing service host...");
                var tmp = string.Format("http://{0}:{1:d}{2}", listenerIp.ToString(), port, routePrefix);
                baseUrl = new Uri(tmp);
                serviceHost = new WebServiceHost(this, baseUrl);
                logger.Ok("Service host started!");
                logger.Info("Registering default routes... please wait!");
                AddPluginRoutes(null, new SamariumRoutesContainer());
            } else {
                // UNSAFE! ABORT!
                throw new ArgumentException("REST service is only allowed to listen to LINK-LOCAL IP addresses!");
            }

        }

        private static RestService _instance;
        public static RestService Instance => _instance ?? throw new NullReferenceException($"No instance of { nameof(RestService) } was created!");
        public static RestService CreateInstance(IPAddress listenerIp, short port = 80, string routePrefix = "") => _instance ?? (_instance = new RestService(listenerIp, port, routePrefix));
        #endregion

        #region Dynamic routing
        internal void AddPluginRoutes(IPlugin containerPlugin, IRoutesContainer routesContainer) {
            var newEndpointAddress = string.Format("{0}/_{1}", baseUrl.AbsolutePath, containerPlugin?.PluginName ?? new Random((int)DateTime.Now.ToFileTime()).Next().ToString());
            var binding = new WSHttpBinding();
            logger.Warn("Adding new service endpoint {0}!", newEndpointAddress);
            serviceHost.AddServiceEndpoint(routesContainer.GetType(), binding, newEndpointAddress);
            logger.Ok("New service endpoint added with no faults!");
        }
        #endregion

    }
}
