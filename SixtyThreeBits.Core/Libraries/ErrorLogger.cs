using Microsoft.Extensions.Logging;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SixtyThreeBits.Core.Libraries
{
    public class ErrorLogger : ILogger
    {
        #region Properties
        private List<LogLevel> _enabledLogLevels = new List<LogLevel> {
            LogLevel.Error,
            LogLevel.Critical,
            LogLevel.Warning
        };
        #endregion

        #region Constructors
        public ErrorLogger()
        {
        }
        #endregion

        #region Methods
        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            return _enabledLogLevels.Any(item => item == logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var errorMessage = formatter(state, exception);
            errorMessage.LogString();
        }
        #endregion
    }
}
