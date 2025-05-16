namespace SixtyThreeBits.Core.Libraries.Loggers.DTO
{
    public class LogStateDTO
    {
        #region Properties
        public string LogString { get; init; }
        public string CallerFilePath { get; init; }
        public int? CallerLineNumber { get; init; } 
        #endregion
    }
}
