using System;

namespace Samarium.PluginFramework.Command {

    using Plugin;

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a command executable within the application.
    /// Commands can be executed from within a plugin.
    /// </summary>
    public class Command : ICommand {

        /// <summary>
        /// Gets a basic command object from the passed data.
        /// </summary>
        /// <param name="commandTag">The command's name.</param>
        /// <param name="args">The arguments to execute the command with.</param>
        /// <returns></returns>
        public static ICommand GetExecutableCommand(string commandTag, params string[] args) => new Command(commandTag, args);

        /// <summary>
        /// Gets or sets the help command.
        /// </summary>
        /// <value>
        /// The help command.
        /// </value>
        public static ICommand HelpCommand { get; set; }
        
        Command(string tag, params string[] args) {
            CommandTag = tag;
            Arguments = args;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Command() { }

        /// <summary>
        /// 2nd Recommended constructor.
        /// </summary>
        /// <param name="parentPlugin">The parent plugin. (Default if application command)</param>
        /// <param name="commandTag">The command tag.</param>
        /// <param name="desc">Long description of the command and its features.</param>
        /// <param name="args">The arguments available with the command.</param>
        /// <param name="switches">The switches available with the command.</param>
        /// <param name="cmdHandler">The handler delegate.</param>
        public Command(IPlugin parentPlugin, string commandTag, string desc, string[] args, Dictionary<string, string[]> switches, CommandExecutedHandler cmdHandler) {
            ParentPlugin = parentPlugin;
            CommandTag = commandTag;
            Description = desc;
            ShortDescription = desc.Substring(0, desc.IndexOf('\n') - 1);
            Arguments = args;
            Switches = switches;
            Handler = cmdHandler;
        }

        /// <summary>
        /// Recommended constructor.
        /// </summary>
        /// <param name="parentPlugin">The parent plugin. (Default if application command)</param>
        /// <param name="commandTag">The command tag.</param>
        /// <param name="desc">Long description of the command and its features.</param>
        /// <param name="shortDesc">Short description of the command.</param>
        /// <param name="args">The arguments available with the command.</param>
        /// <param name="switches">The switches available with the command.</param>
        /// <param name="cmdHandler">The handler delegate.</param>
        public Command(IPlugin parentPlugin, string commandTag, string desc, string shortDesc, string[] args, Dictionary<string, string[]> switches, CommandExecutedHandler cmdHandler) {
            ParentPlugin = parentPlugin;
            CommandTag = commandTag;
            Description = desc;
            ShortDescription = shortDesc;
            Arguments = args;
            Switches = switches;
            Handler = cmdHandler;
        }

        /// <summary>
        /// The plugin this command is associated with.
        /// </summary>
        public IPlugin ParentPlugin { get; set; }

        /// <summary>
        /// The command's tag. The actual string you type in to execute (call) the command.
        /// </summary>
        public string CommandTag { get; set; }

        /// <summary>
        /// A long description of the command and its different features.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A short, one-line description of the command.
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// The arguments this command provides.
        /// </summary>
        public string[] Arguments { get; set; }

        /// <summary>
        /// The switches this command provides.
        /// </summary>
        public Dictionary<string, string[]> Switches { get; set; }

        /// <summary>
        /// The handler delegate.
        /// </summary>
        public CommandExecutedHandler Handler { get; set; }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="args">The arguments the command should use.</param>
        /// <returns>The command's result.</returns>
        public ICommandResult Execute(params string[] args) => Handler?.Invoke(ParentPlugin, this, args);

        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <param name="args">The arguments the command should use.</param>
        /// <returns>The command's result.</returns>
        public async Task<ICommandResult> ExecuteAsync(params string[] args) => await Task.Run(() => Execute(args));

        /// <summary>
        /// Executes the command without returning its result.
        /// </summary>
        /// <param name="args">The arguments the command should use.</param>
        public void ExecuteNoReturn(params string[] args) => Execute(args);

        /// <summary>
        /// Executes the command asynchronously without returning its result.
        /// </summary>
        /// <param name="args">The arguments the command should use.</param>
        /// <returns>The command's result.</returns>
        public async Task ExecuteNoReturnAsync(params string[] args) => await Task.Run(() => ExecuteNoReturn(args));

        /// <summary>
        /// Gets a value indicating whether a given string is a valid argument for this command.
        /// </summary>
        /// <param name="arg">The string to check against.</param>
        /// <returns><code >true</code> if the passed string is a valid argument.</returns>
        public bool IsArgument(string arg) => Arguments.Contains(arg.Split(' ')[0]);

        /// <summary>
        /// Gets a value indicating whether a given string is a valid switch for this command.
        /// </summary>
        /// <param name="switch">The string to check against.</param>
        /// <returns><code >true</code> if the passed string is a valid switch.</returns>
        public bool IsSwitch(string @switch) => Switches?.ContainsKey(@switch) == true; // Hack, I guess

        /// <summary>
        /// Calls the help command for this given command.
        /// </summary>
        /// <returns>
        /// The output of the help command.
        /// </returns>
        public ICommandResult GetHelp() => HelpCommand.Execute(CommandTag);

        /// <summary>
		/// Sorts the arguments.
		/// </summary>
		/// <param name="parameters">Parameters.</param>
		/// <param name="arguments">Arguments.</param>
		/// <param name="switches">Switches.</param>
		/// <param name="cmdArgs">Cmd arguments.</param>
		public void SortArgs(out IEnumerable<string> parameters, out IEnumerable<string> arguments, out Dictionary<string, string> switches, IEnumerable<string> cmdArgs) {
            var _parameters = new List<string>();
            var _arguments = new List<string>();
            var _switches = new Dictionary<string, string>();

            foreach (var arg in cmdArgs.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))) {
                if (IsArgument(arg)) {
                    _arguments.Add(arg);
                } else if (IsSwitch(arg.Split(new[] { '=' }, 2)[0] + "=")) {
                    var split = arg.Split(new[] { '=' }, 2); // Split at the = character; return max 2 substrings
                    _switches.Add(split[0], split[1]);
                } else {
                    _parameters.Add(arg);
                }
            }

            arguments = _arguments;
            parameters = _parameters;
            switches = _switches;
        }
    }
}
