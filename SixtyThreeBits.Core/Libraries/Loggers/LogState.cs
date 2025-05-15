namespace SixtyThreeBits.Core.Libraries.Loggers
{
    public class LogState
    {
        #region Properties
        public string LogString { get; init; }
        public string CallerFilePath { get; init; }
        public int? CallerLineNumber { get; init; } 
        #endregion
    }
}
