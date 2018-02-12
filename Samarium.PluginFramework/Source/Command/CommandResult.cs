using System;

namespace Samarium.PluginFramework.Command {
    
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the result of a command executed within Samarium.
    /// </summary>
    /// <typeparam name="T">The type contained within the result.</typeparam>
    public class CommandResult<T> : ICommandResult<T> {

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CommandResult() { }

        /// <summary>
        /// Parameterised constructor.
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="result">The actual result.</param>
        public CommandResult(string msg, T result) {
            Message = msg;
            Result = result;
        }

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
