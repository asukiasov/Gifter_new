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
        public bool IsError { private set; get; }
        public string ErrorMessage { private set; get; }
        public string ErrorMessageExtended { private set; get; }
        public Exception ExceptionObject { private set; get; }
        public bool IsCustomDatabaseMessage { private set; get; }
        #endregion

        #region Methods
        protected void TryExecute(string Logger = null, Action ActionToTry = null, Action ActionForCatch = null, [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            try
            {
                IsError = false;
                ActionToTry();
            }
            catch (Exception ex)
            {
                if (ActionForCatch == null)
                {
                    ProcessException(Logger, ex, CallerFilePath, CallerLineNumber);
                }
                else
                {
                    ExceptionObject = ex;
                    ActionForCatch.Invoke();
                }
            }
        }

        protected async Task TryExecuteAsyncTask(string Logger, Func<Task> AsyncActionToTry, Func<Task> AsyncActionForCatch = null, Action ActionForCatch = null, [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            try
            {
                IsError = false;
                await AsyncActionToTry();
            }
            catch (Exception ex)
            {
                if (ActionForCatch == null)
                {
                    ProcessException(Logger, ex, CallerFilePath, CallerLineNumber);
                }
                else if (AsyncActionForCatch != null)
                {
                    await AsyncActionForCatch();
                }
                else
                {
                    ActionForCatch();
                }
            }
        }

        protected T TryToReturn<T>(string Logger = null, Func<T> ActionToTry = null, Func<T> ActionForCatch = null, [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            try
            {
                IsError = false;
                return ActionToTry();
            }
            catch (Exception ex)
            {
                if (ActionForCatch == null)
                {
                    ProcessException(Logger, ex, CallerFilePath, CallerLineNumber);
                }
                else
                {
                    ExceptionObject = ex;
                    return ActionForCatch();
                }
                return default;
            }
        }

        protected async Task<T> TryToReturnAsyncTask<T>(string Logger = null, Func<Task<T>> AsyncActionToTry = null, Func<Task<T>> AsyncActionForCatch = null, Func<T> ActionForCatch = null, [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            try
            {
                IsError = false;
                return await AsyncActionToTry();
            }
            catch (Exception ex)
            {
                ExceptionObject = ex;

                if (AsyncActionForCatch == null && ActionForCatch == null)
                {
                    ProcessException(Logger, ex, CallerFilePath, CallerLineNumber);
                    return default;
                }
                else if (AsyncActionForCatch != null)
                {
                    return await AsyncActionForCatch();
                }
                else
                {
                    return ActionForCatch();
                }
            }
        }

        void ProcessException(string Logger, Exception ex, [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            IsError = true;

            if (ex is SqlException)
            {
                ProcessSqlException(Logger, (SqlException)ex, CallerFilePath, CallerLineNumber);
            }
            else
            {
                ProcessRegularException(Logger, ex, CallerFilePath, CallerLineNumber);
            }
        }

        void ProcessRegularException(string Logger, Exception ex, string CallerFilePath, int CallerLineNumber)
        {
            if (ex.InnerException == null)
            {
                ErrorMessage = $"{ex.Message}{Environment.NewLine}";
            }
            else
            {
                ErrorMessage = $"Exception: {ex.Message}{Environment.NewLine}InnerException: {ex.InnerException.Message}{Environment.NewLine}";
            }
            ErrorMessageExtended = string.Format("Source File - {0}{4}Line Number - {1}{4}{2} --- {3}{4}", CallerFilePath, CallerLineNumber, Logger, ErrorMessage, Environment.NewLine);
            if (!IsCustomDatabaseMessage)
            {
                ErrorMessageExtended.LogString();
            }
        }

        void ProcessSqlException(string Logger, SqlException ex, string CallerFilePath, int CallerLineNumber)
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
            ErrorMessageExtended = string.Format("Source File - {0}{4}Line Number - {1}{4}{2} --- {3}{4}", CallerFilePath, CallerLineNumber, Logger, ErrorMessage, Environment.NewLine);
            if (!IsCustomDatabaseMessage)
            {
                ErrorMessageExtended.LogString();
            }
        }
        #endregion
    }
}
