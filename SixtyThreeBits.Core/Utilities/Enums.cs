using System.Collections.Generic;

namespace SixtyThreeBits.Core.Utilities
{
    public class Enums
    {
        public class AreaNames
        {
            #region Properties
            public const string Key = "AreaNamesKey";
            public const string Admin = "Admin";
            public const string Api = "Api";
            #endregion
        }

        public class Cultures
        {
            #region Properties        
            public static System.Globalization.CultureInfo CultureKA => new System.Globalization.CultureInfo("ka-ge");
            public static System.Globalization.CultureInfo CultureEN => new System.Globalization.CultureInfo("en-us");
            public static System.Globalization.CultureInfo CultureRU => new System.Globalization.CultureInfo("ru-ru");

            public static Dictionary<string, System.Globalization.CultureInfo> Culture = new Dictionary<string, System.Globalization.CultureInfo>
        {
            {Languages.GEORGIAN,CultureKA },
            {Languages.ENGLISH,CultureEN},
            {Languages.RUSSIAN,CultureRU},
        };
            #endregion
        }

        public enum DatabaseActions
        {
            #region Properties            
            CREATE = 0,
            UPDATE = 1,
            DELETE = 2,
            ARCHIVE = 3
            #endregion
        }

        public class Languages
        {
            #region Properties
            public const string GEORGIAN = "ka";
            public const string ENGLISH = "en";
            public const string RUSSIAN = "ru";
            #endregion
        }

        public class OrderStatuses
        {
            #region Properties
            public const int CANCELED = 0;
            public const int CREATED = 1;
            public const int PARTIALY_FINISHED = 2;
            public const int FINISHED = 3;
            #endregion
        }

        public class UfcTransactionTypes
        {
            #region Properties
            public const int TRANSACTION_REGISTRATION = 0;
            public const int UFC_POST_BACK = 1;
            public const int CHECK_TRANSACTION_STATUS = 2;
            public const int REVERSE = 3;
            public const int END_OF_BUSINESS_DAY = 10;
            #endregion
        }

        public enum TimeUnitCodes : byte
        {
            #region Properties
            MILLISECOND = 1,
            SECOND = 2,
            MINUTE = 3,
            HOUR = 4,
            DAY = 5,
            WEEK = 6,
            MONTH = 7,
            YEAR = 8
            #endregion
        }
    }
}