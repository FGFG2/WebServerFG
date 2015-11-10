using System;
using System.Collections.Generic;
using NLog;

namespace WebServer.Logging
{
    /// <summary>
    /// LoggerNLog using the NLog framework. Configured inside the NLog.config.
    /// </summary>
    public class LoggerNLog : ILoggerFacade
    {
        private Logger _logger;
        private IDictionary<LogLevel, Action<string>> _logLevelToActionMap; 

        public LoggerNLog()
        {
            _logger = LogManager.GetLogger("Logger");
            _initLogLevelToActionMap();
        }

        private void _initLogLevelToActionMap()
        {
            _logLevelToActionMap = new Dictionary<LogLevel, Action<string>>
            {
                {LogLevel.Debug, _logger.Debug},
                {LogLevel.Info, _logger.Info},
                {LogLevel.Warn, _logger.Warn},
                {LogLevel.Error, _logger.Error}
            };
        }

        public void Log(string message, LogLevel loglevel)
        {
            var logAction = _logLevelToActionMap[loglevel];
            logAction(message);
        }
    }
}