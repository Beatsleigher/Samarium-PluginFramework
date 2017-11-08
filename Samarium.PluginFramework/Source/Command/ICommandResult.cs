﻿using System;

namespace Samarium.PluginFramework.Command {

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICommandResult {

        /// <summary>
        /// Gets this command result's message.
        /// </summary>
        string Message { get; set; }

    }

    public interface ICommandResult<T>: ICommandResult {

        /// <summary>
        /// Gets the command's result.
        /// </summary>
        T Result { get; set; }

    }

}
