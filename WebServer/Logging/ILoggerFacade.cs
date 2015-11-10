using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Logging
{
    /// <summary>
    /// Interface hiding the implementation of the used logging mechanism.
    /// </summary>
    public interface ILoggerFacade
    {
        /// <summary>
        /// Logs the message to all available logs.
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        /// <param name="loglevel">Log level of the message.</param>
        void Log(string message, LogLevel loglevel);
    }

    /// <summary>
    /// Available log levels.
    /// </summary>
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3
    }
}
