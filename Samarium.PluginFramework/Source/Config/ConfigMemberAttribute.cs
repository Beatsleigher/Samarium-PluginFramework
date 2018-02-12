using System;

namespace Samarium.PluginFramework.Config {

    /// <summary>
    /// Defines configuration members in static configuration objects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ConfigMemberAttribute : Attribute {
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigMemberAttribute() { }

        /// <summary>
        /// The name/key of the config.
        /// </summary>
        public string Name { get; set; }

    }
}
