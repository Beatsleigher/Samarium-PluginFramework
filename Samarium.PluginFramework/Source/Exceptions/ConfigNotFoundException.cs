using System;

namespace Samarium.PluginFramework.Exceptions {

    /// <summary>
    /// Defines an exception where a desired configuration could not be found.
    /// This exception inherits from <see cref="Exception"/>
    /// </summary>
    public class ConfigNotFoundException: Exception {

        /// <summary>
        /// The key that was not found.
        /// </summary>
        public string ConfigKey { get; internal set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="msg">The exception message.</param>
        public ConfigNotFoundException(string msg): base(msg) { }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="msg">The exception message.</param>
        /// <param name="configKey">The key that was not found.</param>
        public ConfigNotFoundException(string msg, string configKey): this(msg) {
            ConfigKey = configKey;
        }

    }
}
