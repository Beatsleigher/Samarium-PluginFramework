﻿using System;

namespace Samarium.PluginFramework {

    using Command;
    using Config;
    using Plugin;
    using System.IO;
    using System.ServiceModel.Web;
    using System.Threading.Tasks;

    /// <summary>
    /// Delegate for handling command execution requests from plugins.
    /// </summary>
    /// <param name="sender">The calling plugin.</param>
    /// <param name="requestedCommand">The command requested for execution.</param>
    /// <param name="execArgs">The command's arguments.</param>
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

    /// <summary>
    /// Delegate void used to retrieve configurations system-wide.
    /// This method should be avoided when requesting plugin-specific configurations, that 
    /// can be easily accessed via the PluginConfigs property!
    /// </summary>
    /// <param name="configKey">The config's key</param>
    /// <param name="pluginName" >(Optional): Specifically target a single plugin's configuration.</param>
    /// <returns>The value of the config or the default value.</returns>
    internal delegate object GetSystemWideConfigEventHandler(string configKey, string pluginName = default);

    /// <summary>
    /// Delegate function used to create and execute handlers for RESTful service endpoints (routes).
    /// </summary>
    /// <param name="webReq">The incoming web request.</param>
    /// <param name="webResp">The outgoing response.</param>
    /// <param name="state">The internal state. This will only contain data, if data was passed via the request!</param>
    /// <returns>A <see cref="Stream"/> containing the data that will be returned to the client.</returns>
    /// <remarks>
    /// It is recommended that streams be prepared using the provided extensions.
    /// </remarks>
    public delegate Stream RestulRouteHandler(IncomingWebRequestContext webReq, OutgoingWebResponseContext webResp, object state);

}