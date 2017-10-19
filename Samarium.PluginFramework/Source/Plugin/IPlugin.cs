using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samarium.PluginFramework {

    /// <summary>
    /// Defines the basic structure of the main class of a Samarium plugin.
    /// Every plugin for Samarium must contain a single point of entry where the object
    /// inherits this interface (more specifically the Plugin abstract class).
    /// </summary>
    public interface IPlugin {

        /// <summary>
        /// Gets the name of the plugin.
        /// Ideally has the name of the class inheriting this interface!
        /// </summary>
        string PluginName { get; }

        /// <summary>
        /// Plugin initialisation.
        /// Handles all plugin init routines and returns either one of two states. (See returns section)
        /// </summary>
        /// <returns>
        /// Returns <code >true</code> if plugin initialisation was successful, <code >false</code> otherwise.
        /// </returns>
        bool OnStart();

        /// <summary>
        /// Method called by main system once the plugin has been successfully loaded.
        /// This method can then be used to initialise certain features that the plugin provides automatically, 
        /// such as beginning a threaded operation, designed to live as long as the main system.
        /// </summary>
        void OnLoaded();

        /// <summary>
        /// Plugin termination.
        /// Similar to <see cref="IDisposable.Dispose"/>.
        /// Frees resources and allows plugins (and thus the application) to cleany shut down.
        /// </summary>
        /// <returns><code >true</code> if plugin termination was successful. <code >false</code> otherwise.</returns>
        bool OnStop();
        


    }
}
