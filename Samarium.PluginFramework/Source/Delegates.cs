using System;

namespace Samarium.PluginFramework {

    using Command;
    using Config;
    using Plugin;

    using System.Threading.Tasks;

    /// <summary>
    /// Delegate for handling command execution requests from plugins.
    /// </summary>
    /// <param name="sender">The calling plugin.</param>
    /// <param name="requestedCommand">The command requested for execution.</param>
    /// <returns></returns>
    public delegate ICommandResult CommandExecutionRequestedHandler(IPlugin sender, string requestedCommand, params string[] execArgs);

    /// <summary>
    /// Delegate for asynchronous handling of command execution requests from plugins.
    /// </summary>
    /// <param name="sender">The request sender.</param>
    /// <param name="requestedCommand">The command requested for execution</param>
    /// <param name="execArgs">The command's arguments.</param>
    /// <returns>The command's result.</returns>
    public delegate Task<ICommandResult> AsyncCommandExecutionRequestedHandler(IPlugin sender, string requestedCommand, params string[] execArgs);

    /// <summary>
    /// Handler delegate for command actions.
    /// </summary>
    /// <param name="sender">The plugin sending the command.</param>
    /// <param name="parentCommand"></param>
    /// <param name="execArgs"></param>
    /// <returns></returns>
    public delegate ICommandResult CommandExecutedHandler(IPlugin sender, ICommand parentCommand, params string[] execArgs);

    /// <summary>
    /// Handler delegate for handling <see cref="IConfig.ConfigSet"/> events.
    /// </summary>
    /// <param name="sender">The calling <see cref="IConfig"/> object</param>
    /// <param name="key">The key of the modified config.</param>
    public delegate void ConfigSetEventHandler(IConfig sender, string key);

    /// <summary>
    /// Handler delegate for handling the (re-)loading of configurations.
    /// </summary>
    /// <param name="sender">The calling <see cref="IConfig"/> object.</param>
    public delegate void ConfigsLoadedEventHandler(IConfig sender);

}