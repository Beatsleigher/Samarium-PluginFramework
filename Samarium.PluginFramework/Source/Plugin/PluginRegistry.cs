using System;

namespace Samarium.PluginFramework.Plugin {

    using Command;
    using Config;
    using Exceptions;
    using Logger;

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>Plugin registry.</summary>
    public class PluginRegistry {

        private Logger log;
        private Dictionary<Assembly, Type> listOfPlugins;
        private List<IPlugin> pluginInstances;
        private List<ICommand> systemCommands;

        /// <summary>
        /// CommandExecutionRequested event; is called and fired when a plugin requests a command be executed.
        /// </summary>
        public event CommandExecutionRequestedHandler CommandExecutionRequested;

        /// <summary>
        /// AsyncCommandExecutionRequested event; is called and fired when a plugin requests a command be executed asynchronously.
        /// </summary>
        public event AsyncCommandExecutionRequestedHandler AsyncCommandExecutionRequested;
        
        /// <summary>Gets the instance.</summary>
        /// <value>The instance.</value>
        public static PluginRegistry Instance { get; private set; }

        public static PluginRegistry CreateInstance(IConfig config) => Instance ?? (Instance = new PluginRegistry(config));

        internal PluginRegistry(IConfig config) {
            SystemConfig = config;
            listOfPlugins = new Dictionary<Assembly, Type>();
            pluginInstances = new List<IPlugin>();
            log = Logger.CreateInstance(nameof(PluginRegistry), SystemConfig.GetString("log_directory"));
        }

        public async Task<ICommandResult> ExecuteCommandAsync(IPlugin caller, string commandTag, params string[] args) => await AsyncCommandExecutionRequested?.Invoke(caller, commandTag, args);

        public ICommandResult ExecuteCommand(IPlugin caller, string commandTag, params string[] args) => CommandExecutionRequested?.Invoke(caller, commandTag, args);

        /// <summary>Gets or sets the system config.</summary>
        /// <value>The system config.</value>
        public IConfig SystemConfig { get; private set; }

        /// <summary>Gets the list of plugins.</summary>
        /// <value>The list of plugins.</value>
        public Dictionary<Assembly, Type> ListOfPlugins {
            get {
                return this.listOfPlugins;
            }
        }

        /// <summary>Gets the plugin instances.</summary>
        /// <value>The plugin instances.</value>
        public List<IPlugin> PluginInstances {
            get {
                return this.pluginInstances;
            }
        }

        /// <summary>Gets or sets the main system commands.</summary>
        /// <value>The main system commands.</value>
        public List<ICommand> MainSystemCommands {
            get {
                return this.systemCommands;
            }
            set {
                this.systemCommands = value;
            }
        }

        /// <summary>Removes the plugin from the plugin registry.</summary>
        /// <returns>The plugin.</returns>
        /// <param name="assembly">Assembly.</param>
        public bool RemovePlugin(Assembly assembly) => RemovePlugin(PluginInstances.FirstOrDefault(x => x.GetType().Assembly == assembly));

        /// <summary>Removes the plugin.</summary>
        /// <returns><c>true</c>, if plugin was removed, <c>false</c> otherwise.</returns>
        /// <param name="pluginClass">Plugin class.</param>
        public bool RemovePlugin(IPlugin pluginClass) {
            if (pluginClass is default)
                throw new ArgumentNullException(nameof(pluginClass), "Plugin class reference must not be null!");

            ListOfPlugins.TryGetValue(pluginClass.GetType().Assembly, out var type);
            var assembly = pluginClass.GetType().Assembly;
            if (type is default)
                return false;
            ListOfPlugins.Remove(assembly);
            pluginInstances.Remove(pluginClass);
            return pluginClass.OnStop();
        }

        /// <summary>Registers the plugin.</summary>
        /// <returns>The plugin.</returns>
        /// <param name="pluginAssembly">Plugin assembly.</param>
        /// <param name="pluginClass">Plugin class.</param>
        public void RegisterPlugin(Assembly pluginAssembly, IPlugin pluginClass) {
            this.listOfPlugins.Add(pluginAssembly, pluginClass.GetType());
            this.pluginInstances.Add(pluginClass);
            var startSuccess = pluginClass.OnStart();

            if (startSuccess)
                pluginClass.OnLoaded();

        }

        /// <summary>Registers the plugin.</summary>
        /// <returns>The plugin.</returns>
        /// <param name="pluginAssembly">Plugin assembly.</param>
        /// <param name="pluginClass">Plugin class.</param>
        public void RegisterPlugin(Assembly pluginAssembly, Type pluginClass) {
            Debug.WriteLine(string.Format("sLoading plugin {0} ", (object)pluginClass.FullName));
            if (this.listOfPlugins.ContainsKey(pluginAssembly)) {
                this.log.Error(string.Format("Plugin {0} could not be loaded: Instance already exists!", (object)pluginClass.FullName));
                this.log.Error("To load the plugin, stop and then load it again.");
            } else {
                Plugin pluginClass1 = (Plugin)null;
                try {
                    pluginClass1 = (Plugin)Activator.CreateInstance(pluginClass);
                    this.RegisterPlugin(pluginAssembly, pluginClass1);
                } catch (TargetInvocationException ex) {
                    Console.WriteLine("A fatal error occurred!");
                    this.log.Fatal(string.Format("The plugin {0} could not be loaded! An exception occurred while starting the plugin:", (object)pluginClass1.PluginName));
                    this.log.Debug(ex.Message);
                    this.log.Debug(ex.Source);
                    this.log.Debug(ex.StackTrace);
                } catch (Exception ex) {
                    this.log.Fatal(string.Format("A fatal error occurred while loading plugin {0}!", (object)pluginClass1.PluginName));
                    this.log.Fatal(string.Format("The plugin {0} could not be loaded!", (object)pluginClass1.PluginName));
                    this.log.Debug(ex.Message);
                    this.log.Debug(ex.Source);
                    this.log.Debug(ex.StackTrace);
                }
            }
        }

        /// <summary>Calls the method with no return value.</summary>
        /// <returns>Nothing.</returns>
        /// <param name="assemblyName">Assembly name.</param>
        /// <param name="methodName">Method name.</param>
        /// <param name="param">Parameter.</param>
        public void CallMethodNoReturn(string assemblyName, string methodName, params object[] @params) {
            var plugin1 = default(IPlugin);
            foreach (var listOfPlugin in this.ListOfPlugins) {
                if (listOfPlugin.Key.FullName.Equals(assemblyName))
                    plugin1 = (Plugin)listOfPlugin.Key.CreateInstance("Plugin", true);
            }
            MethodInfo method = plugin1.GetType().GetMethod(methodName);
            if (method == null)
                return;
            IPlugin plugin2 = plugin1;
            method.Invoke((object)plugin2, @params);
        }

        /// <summary>Calls a given method and returns the output.</summary>
        /// <returns>The output from the method.</returns>
        /// <param name="assemblyName">Assembly name.</param>
        /// <param name="methodName">Method name.</param>
        /// <param name="param">Parameters.</param>
        public object CallMethod(string assemblyName, string methodName, params object[] param) {
            Plugin plugin = (Plugin)null;
            foreach (KeyValuePair<Assembly, Type> listOfPlugin in this.ListOfPlugins) {
                if (listOfPlugin.Key.FullName.Equals(assemblyName))
                    plugin = (Plugin)listOfPlugin.Key.CreateInstance("Plugin", true);
            }
            return plugin.GetType().GetMethod(methodName).Invoke((object)plugin, param);
        }

        /// <summary>Gets an instance of a Plugin class.</summary>
        /// <returns>The instance.</returns>
        /// <param name="pluginName">Plugin name.</param>
        public IPlugin GetInstance(string pluginName) => PluginInstances.FirstOrDefault(x => x.PluginName.ToLowerInvariant() == pluginName.ToLowerInvariant());

        /// <summary>Gets an instance of the Plugin class.</summary>
        /// <returns>The instance.</returns>
        /// <param name="index">Index.</param>
        public IPlugin GetInstance(int index) {
            return pluginInstances[index];
        }

        /// <summary>Gets an instance of the Plugin class.</summary>
        /// <returns>The instance.</returns>
        /// <param name="assembly">Assembly.</param>
        public IPlugin GetInstance(Assembly assembly) {
            return GetInstance(assembly.GetName().ToString().ToLowerInvariant().Split(',')[0].Trim());
        }

        /// <summary>
        /// Gets a list of all the commands found in the application.
        /// </summary>
        /// <returns>The commands.</returns>
        public IEnumerable<ICommand> GetCommands() {
            var mainSystemCommands = this.MainSystemCommands;
            foreach (Plugin pluginInstance in this.pluginInstances)
                mainSystemCommands.AddRange(pluginInstance.PluginCommands.Where(x => x != null));
            return mainSystemCommands;
        }

        /// <summary>
        /// Gets a list of all the command tags found in the application.
        /// </summary>
        /// <returns>The commands.</returns>
        /// <param name="searchString">Search string.</param>
        public IEnumerable<string> GetCommands(string searchString = "") {
            List<string> stringList = new List<string>();
            foreach (var command in this.GetCommands().Where((x => {
                if (!string.IsNullOrEmpty(searchString))
                    return x.CommandTag.Contains(searchString);
                return true;
            })))
                stringList.Add(command.CommandTag);
            return stringList;
        }

        /// <summary>
        /// Gets a list of all the commands (,switches, and arguments) found in the application, matching the search string, if the search string is not null or empty.
        /// </summary>
        /// <returns>The commands with arguments.</returns>
        /// <param name="searchString">Search string.</param>
        public IEnumerable<Tuple<string, string[], Dictionary<string, string[]>>> GetCommandsWithArgs(string searchString = "") {
            var tupleList = new List<Tuple<string, string[], Dictionary<string, string[]>>>();
            foreach (var command in this.GetCommands().Where((x => {
                if (string.IsNullOrEmpty(searchString))
                    return true;
                bool flag1 = x.CommandTag.StartsWith(searchString, StringComparison.InvariantCultureIgnoreCase);
                string[] arguments = x.Arguments;
                int num;
                if ((arguments != null ? (((IEnumerable<string>)arguments).Count<string>((Func<string, bool>)(str => str.StartsWith(searchString, StringComparison.InvariantCultureIgnoreCase))) > 0 ? 1 : 0) : 0) != 0) {
                    var switches = x.Switches;
                    num = switches != null ? (((IEnumerable<string>)switches).Count<string>((Func<string, bool>)(str => str.StartsWith(searchString, StringComparison.InvariantCultureIgnoreCase))) > 0 ? 1 : 0) : 0;
                } else
                    num = 0;
                bool flag2 = num != 0;
                return flag1 | flag2;
            })))
                tupleList.Add(new Tuple<string, string[], Dictionary<string, string[]>>(command.CommandTag, command.Arguments, command.Switches));
            return tupleList;
        }

        /// <summary>
        /// Gets all the commands from all plugins loaded in to Samarium.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICommand> GetAllCommands() {
            var cmdList = new List<ICommand>();

            foreach (var plugin in PluginInstances)
                cmdList.AddRange(plugin.PluginCommands);

            return cmdList;
        }
    }
}
