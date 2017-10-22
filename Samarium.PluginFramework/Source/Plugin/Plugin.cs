using System;

namespace Samarium.PluginFramework.Plugin {

    using log4net;

    using Samarium.PluginFramework.Command;
    using Samarium.PluginFramework.Config;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class Plugin : IPlugin {

        private readonly ILog log;

        /// <summary>
        /// Constructor; instantiates an instance of this class. 
        /// </summary>
        protected Plugin() {
            log = LogManager.GetLogger(PluginName);
            log.Logger.IsEnabledFor(log4net.Core.Level.All);
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
        protected void Error(string format, params object[] args) => log.ErrorFormat(format, args);

        /// <summary>
        /// Log a debugging message. Should only be enabled if a debugger is connected!
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void Debug(string format, params object[] args) { if (Debugger.IsAttached) log.DebugFormat(format, args); }

        /// <summary>
        /// Log an informational message.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void Info(string format, params object[] args) => log.InfoFormat(format, args);

        /// <summary>
        /// Log a potentially lethal (fatal) message.
        /// E.g. settings could not be loaded.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void Fatal(string format, params object[] args) => log.FatalFormat(format, args);

        /// <summary>
        /// Log a warning message.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void Warn(string format, params object[] args) => log.InfoFormat(format, args);

        /// <summary>
        /// Essentially a shortcut to <see cref="Console.WriteLine"/>
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected void ToConsole(string format, params object[] args) => Console.WriteLine(format, args);
        #endregion

    }
}
