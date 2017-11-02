using System;

namespace Samarium.PluginFramework {

    using JetBrains.Annotations;

    using Samarium.PluginFramework.Command;
    using Samarium.PluginFramework.Config;

    /// <summary>
    /// Delegate for handling command execution requests from plugins.
    /// </summary>
    /// <param name="sender">The calling plugin.</param>
    /// <param name="requestedCommand">The command requested for execution.</param>
    /// <returns></returns>
    public delegate ICommandResult CommandExecutionRequestedHandler(IPlugin sender, ICommand requestedCommand, params string[] execArgs);

    /// <summary>
    /// Handler delegate for command actions.
    /// </summary>
    /// <param name="sender">The plugin sending the command.</param>
    /// <param name="parentCommand"></param>
    /// <param name="execArgs"></param>
    /// <returns></returns>
    public delegate ICommandResult CommandExecutedHandler([NotNull]IPlugin sender, ICommand parentCommand, params string[] execArgs);

    /// <summary>
    /// Handler delegate for handling <see cref="IConfig.ConfigSet"/> events.
    /// </summary>
    /// <param name="sender">The calling <see cref="IConfig"/> object</param>
    /// <param name="key">The key of the modified config.</param>
    public delegate void ConfigSetEventHandler([NotNull]IConfig sender, string key);

    /// <summary>
    /// Handler delegate for handling the (re-)loading of configurations.
    /// </summary>
    /// <param name="sender">The calling <see cref="IConfig"/> object.</param>
    public delegate void ConfigsLoadedEventHandler([NotNull]IConfig sender);

}