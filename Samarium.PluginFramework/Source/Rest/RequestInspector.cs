using System;

namespace Samarium.PluginFramework.Rest {

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Text;
    using System.Threading.Tasks;

#if DEBUG
    /// <summary>
    /// Implements a basic request inspector class, useful for debugging.
    /// This class is NOT implemented in release versions of the library.
    /// </summary>
    class RequestInspector : IEndpointBehavior, IDispatchMessageInspector {

        const string DebugTag = "[RequestInspector]";
        const string ListSeparator = ", ";

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { /* Not implemented or required */ }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext) {

            Debug("Reached inspector...");
            Debug("Message headers:\n{0}", string.Join(ListSeparator, request.Headers.Select(h => h.ToString())));

            return default;
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { /* Not implemented or required */ }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { /* Not implemented or required */ }

        public void BeforeSendReply(ref Message reply, object correlationState) => throw new NotImplementedException();

        public void Validate(ServiceEndpoint endpoint) { /* Not implemented or required */ }

        void Debug(string fmt, params object[] args) => System.Diagnostics.Debug.WriteLine(string.Concat(DebugTag, " ", fmt), args);

        void Debug(string msg) => System.Diagnostics.Debug.WriteLine(string.Concat(DebugTag, " ", msg));

    }
#endif
}
