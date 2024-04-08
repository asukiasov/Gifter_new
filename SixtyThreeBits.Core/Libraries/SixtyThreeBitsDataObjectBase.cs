using Microsoft.Data.SqlClient;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries
{
    public class SixtyThreeBitsDataObjectBase
    {
        #region Properties
        string _errorMessageForLog;

        public bool IsError { private set; get; }
        public string ErrorMessage { private set; get; }        
        public Exception ExceptionObject { private set; get; }
        public bool IsCustomDatabaseMessage { private set; get; }
        #endregion

        #region Methods
        protected void TryExecute(string logString, Action actionToTry, Action actionForCatch = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            try
            {
                IsError = false;
                actionToTry();
            }
            catch (Exception ex)
            {
                if (actionForCatch == null)
                {
                    processException(
                        logString: logString,
                        ex: ex,
                        callerFilePath: callerFilePath,
                        callerLineNumber: callerLineNumber
                    );
                }
                else
                {
                    ExceptionObject = ex;
                    actionForCatch.Invoke();
                }
            }
        }

        protected async Task TryExecuteAsyncTask(string logString, Func<Task> asyncFuncToTry, Func<Task> asyncFuncForCatch = null, Action actionForCatch = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            try
            {
                IsError = false;
                await asyncFuncToTry();
            }
            catch (Exception ex)
            {
                if (actionForCatch == null)
                {
                    processException(
                        logString: logString,
                        ex: ex,
                        callerFilePath: callerFilePath,
                        callerLineNumber: callerLineNumber
                    );
                }
                else if (asyncFuncForCatch != null)
                {
                    await asyncFuncForCatch();
                }
                else
                {
                    actionForCatch();
                }
            }
        }

        protected T TryToReturn<T>(string logString, Func<T> funcToTry, Func<T> funcForCatch = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var result = default(T);
            try
            {
                IsError = false;
                result = funcToTry();
            }
            catch (Exception ex)
            {
                if (funcForCatch == null)
                {
                    processException(
                        logString: logString,
                        ex: ex,
                        callerFilePath: callerFilePath,
                        callerLineNumber: callerLineNumber
                    );
                }
                else
                {
                    ExceptionObject = ex;
                    result = funcForCatch();
                }                
            }
            return result;
        }

        protected async Task<T> TryToReturnAsyncTask<T>(string logString = null, Func<Task<T>> asyncFuncToTry = null, Func<Task<T>> asyncFuncForCatch = null, Func<T> funcForCatch = null, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var result = default(T);
            try
            {
                IsError = false;
                result = await asyncFuncToTry();
            }
            catch (Exception ex)
            {
                ExceptionObject = ex;

                if (asyncFuncForCatch == null && funcForCatch == null)
                {
                    processException(
                        logString: logString,
                        ex: ex,
                        callerFilePath: callerFilePath,
                        callerLineNumber: callerLineNumber
                    );                    
                }
                else if (asyncFuncForCatch != null)
                {
                    result = await asyncFuncForCatch();
                }
                else
                {
                    result = funcForCatch();
                }
            }
            return result;
        }

        void processException(string logString, Exception ex, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            IsError = true;

            if (ex is SqlException)
            {
                processSqlException(logString, (SqlException)ex, callerFilePath, callerLineNumber);
            }
            else
            {
                processSystemException(logString, ex, callerFilePath, callerLineNumber);
            }
        }

        void processSystemException(string logString, Exception ex, string callerFilePath, int callerLineNumber)
        {
            if (ex.InnerException == null)
            {
                ErrorMessage = $"{ex.Message}{Environment.NewLine}";
            }
            else
            {
                ErrorMessage = $"Exception: {ex.Message}{Environment.NewLine}InnerException: {ex.InnerException.Message}{Environment.NewLine}";
            }
            _errorMessageForLog = string.Format("Source File - {0}{4}Line Number - {1}{4}{2} --- {3}", callerFilePath, callerLineNumber, logString, ErrorMessage, Environment.NewLine);
            if (!IsCustomDatabaseMessage)
            {
                _errorMessageForLog.LogString();
            }
        }

        void processSqlException(string logString, SqlException ex, string callerFilePath, int callerLineNumber)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < ex.Errors.Count; i++)
            {
                if (ex.Errors[i].Number > 50000)
                {
                    IsCustomDatabaseMessage = true;
                    sb.Append(ex.Message);
                }
                else
                {
                    sb.Append(ex.Errors[i].Message).Append(Environment.NewLine);
                }
            }
            ErrorMessage = sb.ToString();
            _errorMessageForLog = string.Format("Source File - {0}{4}Line Number - {1}{4}{2} --- {3}", callerFilePath, callerLineNumber, logString, ErrorMessage, Environment.NewLine);
            if (!IsCustomDatabaseMessage)
            {
                _errorMessageForLog.LogString();
            }
        }
        #endregion
    }
}