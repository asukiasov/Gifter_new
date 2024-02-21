using System;

namespace SixtyThreeBits.Core.Utilities
{
    public static class Constants
    {
        #region Nested Classes        
        public static class Formats
        {
            #region Properties

            #region Date
            public const string Date = "MMM dd, yyyy";
            public const string DateEval = "{0:MMM dd, yyyy}";
            public const string DateTime = "MMM dd, yyyy  HH:mm";
            public const string DateTimeEval = "{0:MMM dd, yyyy  HH:mm}";

            public const string DateGeo = "dd/MM/yyyy";
            public const string DateGeoEval = "{0:dd/MM/yyyy}";
            public const string DateTimeGeo = "dd/MM/yyyy HH:mm";
            public const string DateTimeGeoEval = "{0:dd/MM/yyyy HH:mm}";
            #endregion

            #region Decimal
            public const string Decimal2Fractions = "n2";
            public const string Decimal2FractionsEval = "{0:n2}";
            public const string Decimal4Fractions = "n4";
            public const string Decimal4FractionsEval = "{0:n4}";
            public const string DecimalNoFraction = "n0";
            public const string DecimalNoFractionEval = "{0:n0}";
            public const string DecimalNoTrailingZeros = "#,0.############";
            public const string DecimalNoTrailingZerosEval = "{0:#,0.############}";
            #endregion

            #region Time
            public const string Time = "HH:mm";
            public const string TimeEval = "{0:HH:mm}";
            #endregion

            #endregion
        }

        public static class Languages
        {
            #region Properties
            public const string GEORGIAN = "ka";
            public const string ENGLISH = "en";
            public const string RUSSIAN = "ru";
            #endregion
        }

        public static class NullValueFor
        {
            #region Properties
            public const string String = "";
            public const int Numeric = -1;
            public static readonly DateTime Date = new (1900, 1, 1);
            #endregion
        }

        public static class RegularExpressions
        {
            #region Properties
            public const string Base64 = "^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$";
            public const string Email = @"^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$";
            #endregion
        }        
        #endregion
    }
}
