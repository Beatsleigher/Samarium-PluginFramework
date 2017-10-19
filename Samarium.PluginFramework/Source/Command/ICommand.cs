using System;

namespace Samarium.PluginFramework.Command {

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the most basic of structure for commands in Samarium.
    /// Commands can be called by the terminal user, piped in via stdin, or
    /// executed via plugins extending the application itself.
    /// 
    /// Commands consist of these following main components:
    /// 
    /// The command tag:        The command tag can also be called the command "name".
    ///                         This is what will be called to execute the command.
    ///                  
    /// The command args:       The command arguments are simple strings prefixed by two dashes (--).
    ///                         The arguments act as switches (yes, sounds confusing - it's a misnomer) for boolean values.
    ///                         If an argument is passed, a certain boolean is now true.
    ///                     
    /// The command switches:   The command switches are variable arguments. They are named switches, after switch-case structures.
    ///                         Switches allow multiple configuration possibilities within command execution, and allow
    ///                         these seemingly simple constructs to grow in functionality and complexity.
    /// </summary>
    public interface ICommand {

        #region Methods
        /// <summary>
        /// Execute this command with the given arguments.
        /// </summary>
        /// <param name="args">The arguments the command will enact upon.</param>
        /// <returns>The result of the command.</returns>
        ICommandResult Execute(params string[] args);

        /// <summary>
        /// Execute this command with the given arguments.
        /// </summary>
        /// <param name="args">The arguments the command will enact upon.</param>
        void ExecuteNoReturn(params string[] args);

        /// <summary>
        /// Execute this command with the given arguments asynchronously.
        /// </summary>
        /// <param name="args">The arguments the command will enact upon.</param>
        /// <returns>The result of the command.</returns>
        Task<ICommandResult> ExecuteAsync(params string[] args);

        /// <summary>
        /// Execute this command with the given arguments asynchronously.
        /// </summary>
        /// <param name="args">The arguments the command will enact upon.</param>
        Task ExecuteNoReturnAsync(params string[] args);

        /// <summary>
        /// Gets a value indicating whether the given string is a valid argument
        /// for this command.
        /// </summary>
        /// <param name="arg">The string to check against.</param>
        /// <returns><code >true</code> if the string is a valid argument, <code>false</code> otherwise.</returns>
        bool IsArgument(string arg);

        /// <summary>
        /// Gets a value indicating whether the given string is a valid switch for this command.
        /// </summary>
        /// <param name="switch"></param>
        /// <returns></returns>
        bool IsSwitch(string @switch);

        /// <summary>
		/// Sorts the arguments.
		/// </summary>
		/// <param name="parameters">Parameters.</param>
		/// <param name="arguments">Arguments.</param>
		/// <param name="switches">Switches.</param>
		/// <param name="cmdArgs">Cmd arguments.</param>
		void SortArgs(out IEnumerable<string> parameters, out IEnumerable<string> arguments, out Dictionary<string, string> switches, IEnumerable<string> cmdArgs);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the command tag.
        /// The effective "name" of the command.
        /// </summary>
        string CommandTag { get; set; }

        /// <summary>
        /// Gets or sets the command's description
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets an excerpt of the command's description.
        /// </summary>
        string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the command's arguments.
        /// </summary>
        string[] Arguments { get; set; }

        /// <summary>
        /// Gets or sets the command's switches.
        /// </summary>
        Dictionary<string, string[]> Switches { get; set; }

        /// <summary>
        /// Gets or sets the handler method (Func), which is called on command execution.
        /// </summary>
        Func<ICommandResult> Handler { get; set; }
        #endregion

    }
}
