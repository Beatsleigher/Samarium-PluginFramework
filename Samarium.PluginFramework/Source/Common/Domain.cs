using System;

namespace Samarium.PluginFramework.Common {

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the domain a user is located in.
    /// </summary>
    public class Domain {

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Domain() { }

        /// <summary>
        /// Recommended constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="users"></param>
        public Domain(string name, IEnumerable<DomainUser> users) {
            DomainName = name;
            Users = users;
        }

        /// <summary>
        /// Shorthand constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="users"></param>
        /// <param name="inputDir"></param>
        /// <param name="outputDir"></param>
        public Domain(string name, IEnumerable<DomainUser> users, string inputDir, string outputDir): this(name, users, new DirectoryInfo(inputDir), new DirectoryInfo(outputDir)) { }

        /// <summary>
        /// Shorthand constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="users"></param>
        /// <param name="inputDir"></param>
        /// <param name="outputDir"></param>
        public Domain(string name, IEnumerable<DomainUser> users, DirectoryInfo inputDir, DirectoryInfo outputDir): this(name, users) {
            InputDirectory = inputDir;
            OutputDirectory = outputDir;
        }

        /// <summary>
        /// Gets or sets the name of this domain.
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// Gets or sets the users associated with this domain.
        /// </summary>
        public IEnumerable<DomainUser> Users { get; set; }

        /// <summary>
        /// Gets or sets the input directory.
        /// </summary>
        public DirectoryInfo InputDirectory { get; set; }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        public DirectoryInfo OutputDirectory { get; set; }

        /// <summary>
        /// Adds multiple users to this domain.
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public Domain AddUsers(params DomainUser[] users) {
            foreach (var user in users)
                user.ParentDomain = this;
            if (Users is default)
                Users = new List<DomainUser>();
            Users.ToList().AddRange(users);
            return this;
        }

        /// <summary>
        /// Adds multiple users to this domain.
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public Domain AddUsers(params string[] users) {
            if (Users is default)
                Users = new List<DomainUser>();
            var tmp = Users.ToList();

            foreach (var usr in users)
                tmp.Add(new DomainUser(usr, this));
            Users = tmp;

            return this;
        }

    }
}
