using System;

namespace Samarium.PluginFramework.Plugin {
    
    using Samarium.PluginFramework.Command;
    using Samarium.PluginFramework.Config;
    using Samarium.PluginFramework.Logger;
    using Samarium.PluginFramework.Rest;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceModel.Description;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for plugins used by Samarium and/or its derivatives.
    /// It is recommended this abstract class be used as a direct base class instead of the interface this class
    /// inherits from.
    /// </summary>
    public abstract class Plugin : IPlugin {

        private readonly Logger log;
        internal event GetSystemWideConfigEventHandler GetSystemWideConfigEvent;

        /// <summary>
        /// Constructor; instantiates an instance of this class. 
        /// </summary>
        protected Plugin() {
            log = Logger.CreateInstance(PluginName, PluginRegistry.Instance.SystemConfig.GetString("log_directory"));
        }

        private ICommandResult Plugin_CommandExecutionRequested(IPlugin sender, ICommand requestedCommand, params string[] execArgs) {
            return PluginCommands?
                .FirstOrDefault(x => x.CommandTag.ToLowerInvariant() == requestedCommand.CommandTag.ToLowerInvariant())?
                .Execute(execArgs);
        }

        /// <summary>
        /// Gets this plugin's configuration.
        /// </summary>
        internal protected abstract IConfig PluginConfig { get; }

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

        /// <summary>
        /// Gets the (RESTful) service endpoints provided by this plugin.
        /// It is mandatory that every plugin at least assign a non-null value to this auto-property.
        /// </summary>
        public abstract List<IEndpointContainer> ServiceEndpointContainer { get; }
        
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
        /// Log a message which is shows desirable behavior.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void Ok(string format, params object[] args) => log.Ok(format, args);

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
        /// Essentially a shortcut to <see cref="Console.WriteLine(string, object[])"/>
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void ToConsole(string format, params object[] args) => log.Output(format, args);
        
        /// <summary>
        /// Essentially a shortcut to <see cref="Console.WriteLine(string)"/>
        /// </summary>
        /// <param name="msg">The message to print</param>
        protected void ToConsole(string msg) => log.Output(msg);

        /// <summary>
        /// Log a message used for tracing errors to their point of origin.
        /// </summary>
        /// <param name="fmt">The format string</param>
        /// <param name="args">The arguments for the format string</param>
        protected void Trace(string fmt, params object[] args) => log.Trace(fmt, args);

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

        /// <summary>
        /// Attempts to get a command from this plugin.
        /// </summary>
        /// <param name="command">The command to retrieve.</param>
        /// <returns>The command.</returns>
        public ICommand GetCommand(ICommand command) => PluginCommands?.FirstOrDefault(x => x.CommandTag == command.CommandTag);

        /// <summary>
        /// Attempts to get a command from this plugin.
        /// </summary>
        /// <param name="commandTag">The command to retrieve.</param>
        /// <returns>The command.</returns>
        public ICommand GetCommand(string commandTag) => GetCommand(new Command { CommandTag = commandTag });

        /// <summary>
        /// Execute a command in Samarium.
        /// This command can be one of your own, a Samarium command, or even a command from another plugin!
        /// </summary>
        /// <param name="commandTag">The command's tag.</param>
        /// <param name="args">The arguments for the command in string form.</param>
        /// <returns>The command's result.</returns>
        protected ICommandResult ExecuteCommand(string commandTag, params string[] args) => PluginRegistry.Instance.ExecuteCommand(this, commandTag, args);

        /// <summary>
        /// Executes a command in Samarium asynchronously.
        /// This command can be one of your own, a Samarium command, or even a command from another plugin.
        /// </summary>
        /// <param name="commandTag">The command to execute.</param>
        /// <param name="args">The command's arguments.</param>
        /// <returns>The command's result.</returns>
        protected async Task<ICommandResult> ExecuteCommandAsync(string commandTag, params string[] args) => await PluginRegistry.Instance.ExecuteCommandAsync(this, commandTag, args);

        /// <summary>
        /// Attempts to retrieve a config from somwhere in the application.
        /// This method is usually used to retrieve configurations from other plugins, without having to directly read their configuration files,
        /// which may potentially damage the application, or resources it is using.
        /// </summary>
        /// <param name="configKey">The key of the desired config</param>
        /// <param name="pluginName" >(Optional) The name of the plugin</param>
        /// <returns></returns>
        protected object GetSystemWideConfig(string configKey, string pluginName = default) => GetSystemWideConfigEvent?.Invoke(configKey, pluginName);
        #endregion

        #region Properties
        internal IPlugin SetSystemConfig(IConfig cfg) {
            SystemConfig = cfg;
            return this;
        }

        /// <summary>
        /// Gets the main system's configuration.
        /// </summary>
        protected IConfig SystemConfig { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this plugin will log to the console or not.
        /// </summary>
        public bool LogToConsole {
            get => log.PrintToConsole;
            set => log.PrintToConsole = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this plugin will log to its respective log file or not.
        /// </summary>
        public bool LogToFile {
            get => log.PrintToFile;
            set => log.PrintToFile = value;
        }
        #endregion

    }
}
