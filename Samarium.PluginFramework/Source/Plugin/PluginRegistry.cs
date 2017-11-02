using System;

namespace Samarium.PluginFramework.Plugin {

    using log4net;

    using Samarium.PluginFramework.Command;
    using Samarium.PluginFramework.Config;
    using Samarium.PluginFramework.Exceptions;

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>Plugin registry.</summary>
    public class PluginRegistry {

        private ILog log = LogManager.GetLogger("Samarium");
        private Dictionary<Assembly, Type> listOfPlugins;
        private List<IPlugin> pluginInstances;
        private List<ICommand> systemCommands;

        /// <summary>Gets the instance.</summary>
        /// <value>The instance.</value>
        public static PluginRegistry Instance { get; } = new PluginRegistry();

        internal PluginRegistry() {
            listOfPlugins = new Dictionary<Assembly, Type>();
            pluginInstances = new List<IPlugin>();

        }

        public async Task<ICommandResult> ExecuteCommandAsync(string commandTag, params string[] args) {
            var command = MainSystemCommands.FirstOrDefault(x => x.CommandTag.ToLowerInvariant() == commandTag.ToLowerInvariant());

            if (command is default) {
                command = PluginInstances
                    .FirstOrDefault(x => x.HasCommand(commandTag.ToLowerInvariant()))?
                    .PluginCommands.FirstOrDefault(x => x.CommandTag.ToLowerInvariant() == commandTag.ToLowerInvariant());
            }

            if (command is default) {
                throw new CommandNotFoundException(commandTag, "Could not find command!");
            }

            return await command.ExecuteAsync(args);

        }

        public ICommandResult ExecuteCommand(string commandTag, params string[] args) => ExecuteCommandAsync(commandTag, args).Result;

        /// <summary>Gets or sets the system config.</summary>
        /// <value>The system config.</value>
        public IConfig SystemConfig { internal get; set; }

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
        public bool RemovePlugin(Assembly assembly) {
            Type type = (Type)null;
            this.ListOfPlugins.TryGetValue(assembly, out type);
            if (!(bool)type.GetMethod("OnStop").Invoke(Activator.CreateInstance(type), (object[])null))
                return false;
            this.ListOfPlugins.Remove(assembly);
            return true;
        }

        /// <summary>Removes the plugin.</summary>
        /// <returns><c>true</c>, if plugin was removed, <c>false</c> otherwise.</returns>
        /// <param name="pluginClass">Plugin class.</param>
        public bool RemovePlugin(IPlugin pluginClass) {
            Type type = (Type)null;
            this.ListOfPlugins.TryGetValue(pluginClass.GetType().Assembly, out type);
            Assembly assembly = pluginClass.GetType().Assembly;
            if (type == (Type)null)
                return false;
            this.ListOfPlugins.Remove(assembly);
            this.pluginInstances.Remove(pluginClass);
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
        public Plugin GetInstance(string pluginName) {
            foreach (Plugin pluginInstance in this.pluginInstances) {
                if (pluginInstance.PluginName.ToLower().Equals(pluginName.ToLower()))
                    return pluginInstance;
            }
            return (Plugin)null;
        }

        /// <summary>Gets an instance of the Plugin class.</summary>
        /// <returns>The instance.</returns>
        /// <param name="index">Index.</param>
        public IPlugin GetInstance(int index) {
            return this.pluginInstances[index];
        }

        /// <summary>Gets an instance of the Plugin class.</summary>
        /// <returns>The instance.</returns>
        /// <param name="assembly">Assembly.</param>
        public Plugin GetInstance(Assembly assembly) {
            return this.GetInstance(assembly.GetName().ToString().ToLowerInvariant().Split(',')[0].Trim());
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
    }
}
