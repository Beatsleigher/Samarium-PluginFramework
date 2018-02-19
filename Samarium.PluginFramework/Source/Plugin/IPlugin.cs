
using System;

namespace Samarium.PluginFramework.Plugin {

    using Samarium.PluginFramework.Command;
    using Samarium.PluginFramework.Rest;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Description;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the basic structure of the main class of a Samarium plugin.
    /// Every plugin for Samarium must contain a single point of entry where the object
    /// inherits this interface (more specifically the Plugin abstract class).
    /// </summary>
    public interface IPlugin {

        /// <summary>
        /// Attempts to get a command from this plugin.
        /// </summary>
        /// <param name="command">The command to retrieve.</param>
        /// <returns>The command.</returns>
        ICommand GetCommand(ICommand command);

        /// <summary>
        /// Attempts to get a command from this plugin.
        /// </summary>
        /// <param name="commandTag">The command to retrieve.</param>
        /// <returns>The command.</returns>
        ICommand GetCommand(string commandTag);

        /// <summary>
        /// Gets a value indicating whether the inheriting plugin contains
        /// a given command.
        /// </summary>
        /// <param name="command">A command to check against.</param>
        /// <returns><code >true</code> if the command was found in this plugin.</returns>
        bool HasCommand(ICommand command);

        /// <summary>
        /// Gets a value indicating whether the inheriting plugin contains a given command.
        /// </summary>
        /// <param name="commandTag">A command to check against.</param>
        /// <returns><code >true</code> if the command was found in this plugin.</returns>
        bool HasCommand(string commandTag);

        /// <summary>
        /// Gets or sets a value indicating whether a plugin will log to its respective log file or not.
        /// </summary>
        bool LogToFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a plugin will log to the console or not.
        /// </summary>
        bool LogToConsole { get; set; }

        /// <summary>
        /// Method called by main system once the plugin has been successfully loaded.
        /// This method can then be used to initialise certain features that the plugin provides automatically, 
        /// such as beginning a threaded operation, designed to live as long as the main system.
        /// </summary>
        void OnLoaded();

        /// <summary>
        /// Plugin initialisation.
        /// Handles all plugin init routines and returns either one of two states. (See returns section)
        /// </summary>
        /// <returns>
        /// Returns <code >true</code> if plugin initialisation was successful, <code >false</code> otherwise.
        /// </returns>
        bool OnStart();

        /// <summary>
        /// Plugin termination.
        /// Similar to <see cref="IDisposable.Dispose"/>.
        /// Frees resources and allows plugins (and thus the application) to cleany shut down.
        /// </summary>
        /// <returns><code >true</code> if plugin termination was successful. <code >false</code> otherwise.</returns>
        bool OnStop();

        /// <summary>
        /// Gets the commands this plugin provides.
        /// </summary>
        List<ICommand> PluginCommands { get; }

        /// <summary>
        /// Gets the name of the plugin.
        /// Ideally has the name of the class inheriting this interface!
        /// </summary>
        string PluginName { get; }

        /// <summary>
        /// Retrieves the RESTful endpoints defined by this plugin which
        /// are then loaded in to the application's RESTful service.
        /// These endpoints MUST be defined before the plugin hits the <see cref="OnLoaded"/> method!
        /// </summary>
        List<IEndpointContainer> ServiceEndpointContainer { get; }

        
    }
}
