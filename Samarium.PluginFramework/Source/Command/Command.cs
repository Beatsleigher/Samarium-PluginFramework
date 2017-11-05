using System;

namespace Samarium.PluginFramework.Command {

    using Plugin;

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Command : ICommand {

        public static ICommand GetExecutableCommand(string commandTag, params string[] args) => new Command(commandTag, args);
        
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

        public ICommandResult Execute(params string[] args) => Handler?.Invoke(ParentPlugin, this, args);
        public async Task<ICommandResult> ExecuteAsync(params string[] args) => await Task.Run(() => Execute(args));
        public void ExecuteNoReturn(params string[] args) => Execute(args);
        public async Task ExecuteNoReturnAsync(params string[] args) => await Task.Run(() => ExecuteNoReturn(args));
        public bool IsArgument(string arg) => Arguments.Contains(arg);
        public bool IsSwitch(string @switch) => Switches.ContainsKey(@switch);
        public void SortArgs(out IEnumerable<string> parameters, out IEnumerable<string> arguments, out Dictionary<string, string> switches, IEnumerable<string> cmdArgs) => throw new NotImplementedException();
    }
}
