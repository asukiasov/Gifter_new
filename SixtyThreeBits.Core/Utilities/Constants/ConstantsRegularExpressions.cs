namespace SixtyThreeBits.Core.Utilities
{
    public static partial class Constants
    {
        public static class RegularExpressions
        {
            #region Properties
            public const string Base64 = "^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$";
            public const string Email = @"^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$";
            #endregion
        }        
    }
}
