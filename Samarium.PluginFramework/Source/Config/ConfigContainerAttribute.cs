using System;

namespace Samarium.PluginFramework.Config {

    /// <summary>
    /// Attribute to define objects containing config Properties
    /// for static configurations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = true)]
    public sealed class ConfigContainerAttribute : Attribute {

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigContainerAttribute() { }
    }

}
