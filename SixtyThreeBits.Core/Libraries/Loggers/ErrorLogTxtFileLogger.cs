using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Libraries.Loggers.DTO;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.IO;
using System.Text;

namespace SixtyThreeBits.Core.Libraries.Loggers
{
    public class ErrorLogTxtFileLogger : ILogger
    {
        #region Properties
        readonly string _errorLogTxtFileDirectoryPath;
        readonly string _errorLogTxtFilePath;
        static object _lockFileCreate = new object();
        static object _lockFileWrite = new object();
        #endregion

        #region Constructors
        public ErrorLogTxtFileLogger()
        {
            _errorLogTxtFileDirectoryPath = $"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}App_Data";
            _errorLogTxtFilePath = $"{_errorLogTxtFileDirectoryPath}{Path.DirectorySeparatorChar}ErrorLog.txt";
        } 
        #endregion

        #region Methods
        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == LogLevel.Error;            
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter = null)
        {
            if (IsEnabled(logLevel))
            {
                var isErrorLogFileExists = this.isErrorLogFileExists();
                if (isErrorLogFileExists)
                {
                    var logState = state?.ToString().DeserializeJsonTo<LogStateDTO>();
                    var errorMessage = formatErrorMessage(logState: logState, exception: exception);
                    writeErrorMessageToErroLogTxtFile(errorMessage: errorMessage);                    
                }
            }            
        }

        bool isErrorLogFileExists()
        {            
            var isFileExists = false;

            if (File.Exists(_errorLogTxtFilePath))
            {
                isFileExists = true;
            }
            else  
            {

                lock (_lockFileCreate)
                {
                    try
                    {
                        if (!Directory.Exists(_errorLogTxtFileDirectoryPath))
                        {
                            Directory.CreateDirectory(_errorLogTxtFileDirectoryPath);
                        }
                        File.Create(_errorLogTxtFilePath).Close();
                        isFileExists = true;
                    }
                    catch { }
                }
            }

            return isFileExists;
        }

        string formatErrorMessage(LogStateDTO logState, Exception exception)
        {
            var errorMessageBuilder = new StringBuilder();
            if (exception is SqlException)
            {
                var ex = exception as SqlException;
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    if (ex.Errors[i].Number > 50000)
                    {                        
                        errorMessageBuilder.Append(ex.Message);
                    }
                    else
                    {
                        errorMessageBuilder.Append(ex.Errors[i].Message).Append(Environment.NewLine);
                    }
                }
            }
            else
            {
                if (exception.InnerException == null)
                {
                    errorMessageBuilder.Append($"{exception.Message}{Environment.NewLine}");
                }
                else
                {                    
                    errorMessageBuilder.Append($"Exception: {exception.Message}{Environment.NewLine}InnerException: {exception.InnerException.Message}{Environment.NewLine}");
                }
                errorMessageBuilder.Append($"{Environment.NewLine}StackTrace:{Environment.NewLine}{exception.StackTrace}{Environment.NewLine}");
            }
            
            var logMessage = string.Format("Source File - {0}{4}Line Number - {1}{4}{2} --- {3}", logState.CallerFilePath, logState.CallerLineNumber, logState.LogString, errorMessageBuilder.ToString(), Environment.NewLine);

            return logMessage;
        }

        void writeErrorMessageToErroLogTxtFile(string errorMessage)
        {
            lock (_lockFileWrite)
            {
                try
                {
                    File.AppendAllText(_errorLogTxtFilePath, $"------------------------------------\r\n{DateTime.Now}\r\n{errorMessage}------------------------------------\r\n\r\n", Encoding.UTF8);
                }
                catch { }
            }
        }
        #endregion        
    }
}
