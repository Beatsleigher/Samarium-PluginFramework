using System;

namespace Samarium.PluginFramework.Plugin {
    
    using Samarium.PluginFramework.Command;
    using Samarium.PluginFramework.Config;
    using Samarium.PluginFramework.Logger;

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class Plugin : IPlugin {

        internal static event CommandExecutionRequestedHandler CommandExecutionRequested;

        private readonly Logger log;

        /// <summary>
        /// Constructor; instantiates an instance of this class. 
        /// </summary>
        protected Plugin() {
            log = Logger.CreateInstance(PluginName, PluginRegistry.Instance.SystemConfig.GetString("log_directory"));
            CommandExecutionRequested += Plugin_CommandExecutionRequested;
        }

        private ICommandResult Plugin_CommandExecutionRequested(IPlugin sender, ICommand requestedCommand, params string[] execArgs) {
            return PluginCommands?
                .FirstOrDefault(x => x.CommandTag.ToLowerInvariant() == requestedCommand.CommandTag.ToLowerInvariant())?
                .Execute(execArgs);
        }

        protected abstract IConfig PluginConfig { get; }

        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        public abstract string PluginName { get; }

        /// <summary>
        /// Gets the commands this plugin provides.
        /// </summary>
        public abstract List<ICommand> PluginCommands { get; }

        /// <summary>
        /// Called by the main system once the plugin has been loaded.
        /// </summary>
        public abstract void OnLoaded();

        /// <summary>
        /// Called by the main system once the plugin has been detected and is being loaded.
        /// </summary>
        /// <returns><code>true</code> if the plugin started successfully. <code>false</code> otherwise.</returns>
        public abstract bool OnStart();

        /// <summary>
        /// Called by the main system to stop this plugin-
        /// </summary>
        /// <returns><code>true</code> if the plugin shut down correctly.</returns>
        public abstract bool OnStop();

        #region Implemented methods
        /// <summary>
        /// Log an error
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void Error(string format, params object[] args) => log.Error(format, args);

        /// <summary>
        /// Log a debugging message. Should only be enabled if a debugger is connected!
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void Debug(string format, params object[] args) { if (Debugger.IsAttached) log.Debug(format, args); }

        /// <summary>
        /// Log an informational message.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void Info(string format, params object[] args) => log.Info(format, args);

        /// <summary>
        /// Log a potentially lethal (fatal) message.
        /// E.g. settings could not be loaded.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void Fatal(string format, params object[] args) => log.Fatal(format, args);

        /// <summary>
        /// Log a warning message.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void Warn(string format, params object[] args) => log.Warn(format, args);

        /// <summary>
        /// Essentially a shortcut to <see cref="Console.WriteLine"/>
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void ToConsole(string format, params object[] args) => Console.WriteLine(format, args);

        /// <summary>
        /// Gets a value indicating whether the inheriting plugin contains
        /// a given command.
        /// </summary>
        /// <param name="command">A command to check against.</param>
        /// <returns><code >true</code> if the command was found in this plugin.</returns>
        public bool HasCommand(ICommand command) => PluginCommands?.Count(x => x.CommandTag == command.CommandTag) > 0;

        /// <summary>
        /// Gets a value indicating whether the inheriting plugin contains
        /// a given command.
        /// </summary>
        /// <param name="commandTag">A command to check against.</param>
        /// <returns><code >true</code> if the command was found in this plugin.</returns>
        public bool HasCommand(string commandTag) => HasCommand(new Command { CommandTag = commandTag });
        #endregion

    }
}
