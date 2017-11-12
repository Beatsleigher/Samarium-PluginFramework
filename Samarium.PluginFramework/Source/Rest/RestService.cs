using System;

namespace Samarium.PluginFramework.Rest {

    using PluginFramework;
    using Logger;
    
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading.Tasks;

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
            if (listenerIp.IsIPv4LinkLocal() || listenerIp.IsIPv6LinkLocal) {
                // SAFE IP
                serviceHost = new WebServiceHost(this, baseUrl = new Uri(string.Format("{0}:{1:d}{2}", listenerIp.ToString(), port, routePrefix)));
            } else {
                // UNSAFE! ABORT!
                throw new ArgumentException("REST service is only allowed to listen to LINK-LOCAL IP addresses!");
            }

            logger = Logger.CreateInstance(nameof(RestService), default);
        }

        private static RestService _instance;
        public static RestService Instance => _instance ?? throw new NullReferenceException($"No instance of { nameof(RestService) } was created!");
        public static RestService CreateInstance(IPAddress listenerIp, short port = 80, string routePrefix = "") => _instance ?? (_instance = new RestService(listenerIp, port, routePrefix));
        #endregion

        #region Dynamic routing
        internal void AddPluginRoutes(IRoutesContainer routesContainer) {

        }
        #endregion

    }
}
