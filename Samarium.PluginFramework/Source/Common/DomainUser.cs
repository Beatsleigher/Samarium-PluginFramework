using System;

namespace Samarium.PluginFramework.Common {

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a user in a <see cref="Domain"/>.
    /// </summary>
    public class DomainUser {

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DomainUser() { }

        /// <summary>
        /// Recommended constructor.
        /// </summary>
        /// <param name="name">The name of the user</param>
        /// <param name="parent">The parent domain.</param>
        public DomainUser(string name, Domain parent) {
            Name = name;
            ParentDomain = parent;
        }

        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user's parent domain.
        /// </summary>
        public Domain ParentDomain { get; set; }

        /// <summary>
        /// Gets the user's ID.
        /// </summary>
        public string GetUserID => string.Format("{0}_a_{1}", ParentDomain.DomainName, Name);

    }
}
