using System;

namespace Samarium.PluginFramework {

    /// <summary >
    /// Defines several serialisation types available for config serialisation.
    /// </summary>
    /// <remarks >
    /// Simply because a value is listed in this enumeration, does NOT mean you have to implement each and every one.
    /// Samarium and plugins developed by the original creator/maintainer will only implement
    /// JSON and YAML, as they are the two most favoured serialisation languages.
    /// Feel free to implement the others, though, if you need to.
    /// </remarks>
    public enum ConfigSerializationType {

        /// <summary>
        /// Serialise configurations as JSON.
        /// (JavaScript Object Notation)
        /// </summary>
        Json,

        /// <summary>
        /// Serialise configurations as YAML.
        /// (Yaml Ain't Markup Language / Yet Another Markup Language)
        /// </summary>
        Yaml,

        /// <summary>
        /// Comma-separated values.
        /// Desired format:
        /// key=value,
        /// key=value,
        /// ...
        /// </summary>
        CSV,

        /// <summary>
        /// Why anyone'd use this is a mystery to me.
        /// But I'm a nice person, so I'm giving you this opportunity.
        /// </summary>
        XML

    }

}