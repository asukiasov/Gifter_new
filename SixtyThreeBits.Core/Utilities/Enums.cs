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

        public class CurrencyISOCodes
        {
            #region Properties
            public const string GEL = "981";
            #endregion
        }

        public class Dictionaries
        {
            #region Sub Classes
            public class DiscountTypes
            {
                #region Properties
                public const int Percent = 1;
                public const int FixedAmount = 2;
                #endregion
            }

            public class OrderDeliveryTypes
            {
                #region Properties
                public const int DeliverToAddress = 1;
                public const int Takeaway = 2;
                #endregion
            }

            public class OrderStatuses
            {
                #region Properties
                public const int Created = 1;
                public const int PendingPayment = 2;
                public const int Paid = 3;
                public const int Canceled = 100;
                #endregion
            }

            public class OrderPaymentTypes
            {
                #region Properties
                public const int CreditCard = 1;
                public const int Cash = 2;
                public const int Invoice = 3;
                #endregion
            }

            public class Services
            {
                #region Properties
                public const int UFC = 2;
                #endregion
            }

            public class ServiceOperations
            {
                #region Properties
                public const int UFC_TRANSACTION_REGISTRATION = 200;
                public const int UFC_RETURN_TO_WEBSITE = 201;
                public const int UFC_TRANSACTION_STATUS_CHECK = 202;
                public const int UFC_TRANSACTION_REVERSAL = 203;
                public const int UFC_TRANSACTION_REFUND = 204;
                public const int UFC_END_OF_BUSINESS_DAY = 299;
                #endregion
            }
            #endregion
        }

        public class DictionaryCodes
        {
            #region Properties
            public const int OrderStatuses = 1;
            public const int DiscountCouponTypes = 2;
            public const int OrderPaymentTypes = 3;
            public const int Services = 4;
            public const int ServiceOperations = 5;
            public const int ProductSortOptions = 6;
            public const int EnergyRatings = 7;
            public const int ProductMaterials = 8;
            public const int OrderDeliveryTypes = 9;
            public const int OrderDeliveryCities = 10;
            public const int PaymentTypes = 11;
            public const int TeamMemberCategories = 12;
            #endregion
        }

        public class EmailTemplates
        {
            #region Properties
            public const int SignUpVerification = 1;
            public const int SignUpWelcome = 2;
            public const int PasswordReset = 3;
            public const int EmailChange = 4;
            public const int OrderToUser = 10;
            public const int OrderToAdmins = 11;
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