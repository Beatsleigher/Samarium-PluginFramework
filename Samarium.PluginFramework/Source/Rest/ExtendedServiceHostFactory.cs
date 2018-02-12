using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Samarium.PluginFramework.Rest {

    /// <summary>
    /// Extends the standard service host factory and adds additional, required, behaviour.
    /// This class may be extended to suit the individual needs of other applications.
    /// </summary>
    public class ExtendedServiceHostFactory: WebServiceHostFactory {

        /// <summary>
        /// A collection of service endpoints.
        /// </summary>
        public IEndpointContainer[] EndpointCollection { get; set; }

        /// <summary>
        /// Creates a new ServiceHost object.
        /// </summary>
        /// <param name="serviceType">The service's type.</param>
        /// <param name="baseAddresses">The service's base addresses.</param>
        /// <returns>The newly generated ServiceHost object</returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses) {
            Debug.WriteLine("Creating Samarium RESTful service host...");
            var serviceHost = (WebServiceHost)base.CreateServiceHost(serviceType, baseAddresses);
            serviceHost.Description.Behaviors.Add(new ServiceMetadataBehavior {
                HttpGetEnabled = true,
                HttpsGetEnabled = true
            });
            
            return serviceHost;
        }

        /// <summary>
        /// Creates a new ServiceHost object.
        /// </summary>
        /// <param name="constructorString"></param>
        /// <param name="baseAddresses"></param>
        /// <returns></returns>
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses) {
            return CreateServiceHost(typeof(RestService), baseAddresses);
        }

    }
}
