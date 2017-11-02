using System;

namespace Samarium.PluginFramework.Exceptions {
    public class CommandNotFoundException: SystemException {

        public string CommandTag { get; }

        public CommandNotFoundException() : base() { }

        public CommandNotFoundException(string commandTag, string message): base(message) {
            CommandTag = commandTag;
        }

    }
}
