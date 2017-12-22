using System;

namespace Samarium.PluginFramework.Config {

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ConfigMemberAttribute : Attribute {
        
        public ConfigMemberAttribute() { }

        public string Name { get; set; }

    }
}
