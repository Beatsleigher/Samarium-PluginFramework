using System;

namespace Samarium.PluginFramework.Command {

    using Plugin;

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Command : ICommand {

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

        public Command() { }

        public Command(IPlugin parentPlugin, string commandTag, string desc, string[] args, Dictionary<string, string[]> switches, CommandExecutedHandler cmdHandler) {
            ParentPlugin = parentPlugin;
            CommandTag = commandTag;
            Description = desc;
            ShortDescription = desc.Substring(0, desc.IndexOf('\n') - 1);
            Arguments = args;
            Switches = switches;
            Handler = cmdHandler;
        }

        public Command(IPlugin parentPlugin, string commandTag, string desc, string shortDesc, string[] args, Dictionary<string, string[]> switches, CommandExecutedHandler cmdHandler) {
            ParentPlugin = parentPlugin;
            CommandTag = commandTag;
            Description = desc;
            ShortDescription = shortDesc;
            Arguments = args;
            Switches = switches;
            Handler = cmdHandler;
        }

        public IPlugin ParentPlugin { get; set; }
        public string CommandTag { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string[] Arguments { get; set; }
        public Dictionary<string, string[]> Switches { get; set; }
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
        public bool IsSwitch(string @switch) => Switches.ContainsKey(@switch); // Hack, I guess

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

            foreach (var arg in cmdArgs) {
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
