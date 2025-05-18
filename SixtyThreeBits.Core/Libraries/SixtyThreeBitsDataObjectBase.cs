using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Libraries.Loggers.DTO;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries
{
    public class SixtyThreeBitsDataObjectBase
    {
        #region Properties
        readonly ILogger _logger;

        public bool IsError { private set; get; }
        public string ErrorMessage { private set; get; }
        public Exception Exception { private set; get; }
        #endregion

        #region Constructors
        public SixtyThreeBitsDataObjectBase(ILogger logger)
        {
            _logger = logger;
        }
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
                        exception: ex,
                        callerFilePath: callerFilePath,
                        callerLineNumber: callerLineNumber
                    );
                }
                else
                {
                    Exception = ex;
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
                        exception: ex,
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
                        exception: ex,
                        callerFilePath: callerFilePath,
                        callerLineNumber: callerLineNumber
                    );
                }
                else
                {
                    Exception = ex;
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
                Exception = ex;

                if (asyncFuncForCatch == null && funcForCatch == null)
                {
                    processException(
                        logString: logString,
                        exception: ex,
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

        void processException(string logString, Exception exception, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            if (_logger != null)
            {
                _logger.LogError(
                    exception: exception,
                    message: new LogStateDTO
                    {
                        LogString = logString,
                        CallerFilePath = callerFilePath,
                        CallerLineNumber = callerLineNumber
                    }.ToJson()
                );
            }
            IsError = true;
            Exception = exception;
            ErrorMessage = exception.Message;            
        }
        #endregion
    }
}