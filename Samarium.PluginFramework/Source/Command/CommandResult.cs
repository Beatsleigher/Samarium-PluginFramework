using System;

namespace Samarium.PluginFramework.Command {
    
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CommandResult<T> : ICommandResult<T> {

        /// <summary>
        /// Gets this command result's result.
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// Gets this command result's message.
        /// </summary>
        public string Message { get; set; }

    }
}
