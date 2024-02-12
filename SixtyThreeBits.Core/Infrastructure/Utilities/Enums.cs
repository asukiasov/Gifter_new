namespace SixtyThreeBits.Core.Infrastructure.Utilities
{
    public static class Enums
    {
        public enum DatabaseActions
        {
            #region Properties            
            CREATE = 0,
            UPDATE = 1,
            DELETE = 2,
            ARCHIVE = 3
            #endregion
        }

        public static class Dictionaries
        {
            #region Nested Classes
            public static class Banks
            {
                #region Properties
                public const int TBC = 1;
                public const int BOG = 2;
                #endregion
            }

            public static class DiscountTypes
            {
                #region Properties
                public const int Percent = 1;
                public const int FixedAmount = 2;
                #endregion
            }

            public static class OrderStatuses
            {
                #region Properties
                public const int Created = 1;
                public const int PendingPayment = 2;
                public const int Paid = 3;
                public const int Canceled = 100;
                #endregion
            }

            public static class PaymentOptions
            {
                #region Properties
                public const int Consignation = 0;
                public const int TbcCard = 1;
                public const int TbcErtguli = 2;
                public const int BogCard = 10;
                public const int BogInstallment = 11;
                #endregion
            }

            public static class Services
            {
                #region Properties
                public const int UFC = 2;
                #endregion
            }

            public static class ServiceOperations
            {
                #region Properties
                public const int UFC_TRANSACTION_REGISTRATION = 200;
                public const int UFC_RETURN_TO_WEBSITE = 201;
                public const int UFC_TRANSACTION_STATUS_CHECK = 202;
                public const int UFC_TRANSACTION_REVERSAL = 203;
                public const int UFC_TRANSACTION_REFUND = 204;
                public const int UFC_END_OF_BUSINESS_DAY = 299;

                public const int IPAY_TRANSACTION_REGISTRATION = 300;
                public const int IPAY_RETURN_TO_WEBSITE = 301;
                public const int IPAY_TRANSACTION_STATUS_CHECK = 302;
                public const int IPAY_TRANSACTION_REFUND = 303;
                public const int IPAY_CALLBACK = 304;

                public const int IPAY_INSTALLMENT_CALCULATE = 400;
                public const int IPAY_INSTALLMENT_CHECKOUT = 401;
                public const int IPAY_INSTALLMENT_CHECKOUT_CALLBACK = 402;
                public const int IPAY_INSTALLMENT_STATUS_CHECK = 403;
                public const int IPAY_INSTALLMENT_REVERSE_CALLBACK = 404;
                #endregion
            }
            #endregion
        }

        public static class DictionaryCodes
        {
            #region Properties
            public const int TimeUnits = 1;
            public const int Services = 2;
            public const int ServiceOperations = 3;
            public const int TeamMemberCategories = 10;
            #endregion
        }

        public static class EmailTemplates
        {
            #region Properties
            public const int SignUpVerification = 1;
            public const int SignUpWelcome = 2;
            public const int PasswordReset = 3;
            public const int EmailChange = 4;
            public const int ContactFormToAdmins = 9;
            public const int OrderToUser = 10;
            public const int OrderToAdmins = 11;
            #endregion
        }

        public static class FileManagerModules
        {
            #region Properties
            public const string News = "news";
            public const string Blog = "blog";
            public const string Pages = "pages";
            public const string Products = "products";
            #endregion
        }

        public static class HttpStatusCodes
        {
            /// <summary>
            /// HTTP status code 100.
            /// </summary>
            public const int Status100Continue = 100;

            /// <summary>
            /// HTTP status code 101.
            /// </summary>
            public const int Status101SwitchingProtocols = 101;

            /// <summary>
            /// HTTP status code 102.
            /// </summary>
            public const int Status102Processing = 102;

            /// <summary>
            /// HTTP status code 200.
            /// </summary>
            public const int Status200OK = 200;

            /// <summary>
            /// HTTP status code 201.
            /// </summary>
            public const int Status201Created = 201;

            /// <summary>
            /// HTTP status code 202.
            /// </summary>
            public const int Status202Accepted = 202;

            /// <summary>
            /// HTTP status code 203.
            /// </summary>
            public const int Status203NonAuthoritative = 203;

            /// <summary>
            /// HTTP status code 204.
            /// </summary>
            public const int Status204NoContent = 204;

            /// <summary>
            /// HTTP status code 205.
            /// </summary>
            public const int Status205ResetContent = 205;

            /// <summary>
            /// HTTP status code 206.
            /// </summary>
            public const int Status206PartialContent = 206;

            /// <summary>
            /// HTTP status code 207.
            /// </summary>
            public const int Status207MultiStatus = 207;

            /// <summary>
            /// HTTP status code 208.
            /// </summary>
            public const int Status208AlreadyReported = 208;

            /// <summary>
            /// HTTP status code 226.
            /// </summary>
            public const int Status226IMUsed = 226;

            /// <summary>
            /// HTTP status code 300.
            /// </summary>
            public const int Status300MultipleChoices = 300;

            /// <summary>
            /// HTTP status code 301.
            /// </summary>
            public const int Status301MovedPermanently = 301;

            /// <summary>
            /// HTTP status code 302.
            /// </summary>
            public const int Status302Found = 302;

            /// <summary>
            /// HTTP status code 303.
            /// </summary>
            public const int Status303SeeOther = 303;

            /// <summary>
            /// HTTP status code 304.
            /// </summary>
            public const int Status304NotModified = 304;

            /// <summary>
            /// HTTP status code 305.
            /// </summary>
            public const int Status305UseProxy = 305;

            /// <summary>
            /// HTTP status code 306.
            /// </summary>
            public const int Status306SwitchProxy = 306;

            /// <summary>
            /// HTTP status code 307.
            /// </summary>
            public const int Status307TemporaryRedirect = 307;

            /// <summary>
            /// HTTP status code 308.
            /// </summary>
            public const int Status308PermanentRedirect = 308;

            /// <summary>
            /// HTTP status code 400.
            /// </summary>
            public const int Status400BadRequest = 400;

            /// <summary>
            /// HTTP status code 401.
            /// </summary>
            public const int Status401Unauthorized = 401;

            /// <summary>
            /// HTTP status code 402.
            /// </summary>
            public const int Status402PaymentRequired = 402;

            /// <summary>
            /// HTTP status code 403.
            /// </summary>
            public const int Status403Forbidden = 403;

            /// <summary>
            /// HTTP status code 404.
            /// </summary>
            public const int Status404NotFound = 404;

            /// <summary>
            /// HTTP status code 405.
            /// </summary>
            public const int Status405MethodNotAllowed = 405;

            /// <summary>
            /// HTTP status code 406.
            /// </summary>
            public const int Status406NotAcceptable = 406;

            /// <summary>
            /// HTTP status code 407.
            /// </summary>
            public const int Status407ProxyAuthenticationRequired = 407;

            /// <summary>
            /// HTTP status code 408.
            /// </summary>
            public const int Status408RequestTimeout = 408;

            /// <summary>
            /// HTTP status code 409.
            /// </summary>
            public const int Status409Conflict = 409;

            /// <summary>
            /// HTTP status code 410.
            /// </summary>
            public const int Status410Gone = 410;

            /// <summary>
            /// HTTP status code 411.
            /// </summary>
            public const int Status411LengthRequired = 411;

            /// <summary>
            /// HTTP status code 412.
            /// </summary>
            public const int Status412PreconditionFailed = 412;

            /// <summary>
            /// HTTP status code 413.
            /// </summary>
            public const int Status413RequestEntityTooLarge = 413;

            /// <summary>
            /// HTTP status code 413.
            /// </summary>
            public const int Status413PayloadTooLarge = 413;

            /// <summary>
            /// HTTP status code 414.
            /// </summary>
            public const int Status414RequestUriTooLong = 414;

            /// <summary>
            /// HTTP status code 414.
            /// </summary>
            public const int Status414UriTooLong = 414;

            /// <summary>
            /// HTTP status code 415.
            /// </summary>
            public const int Status415UnsupportedMediaType = 415;

            /// <summary>
            /// HTTP status code 416.
            /// </summary>
            public const int Status416RequestedRangeNotSatisfiable = 416;

            /// <summary>
            /// HTTP status code 416.
            /// </summary>
            public const int Status416RangeNotSatisfiable = 416;

            /// <summary>
            /// HTTP status code 417.
            /// </summary>
            public const int Status417ExpectationFailed = 417;

            /// <summary>
            /// HTTP status code 418.
            /// </summary>
            public const int Status418ImATeapot = 418;

            /// <summary>
            /// HTTP status code 419.
            /// </summary>
            public const int Status419AuthenticationTimeout = 419;

            /// <summary>
            /// HTTP status code 422.
            /// </summary>
            public const int Status421MisdirectedRequest = 421;

            /// <summary>
            /// HTTP status code 422.
            /// </summary>
            public const int Status422UnprocessableEntity = 422;

            /// <summary>
            /// HTTP status code 423.
            /// </summary>
            public const int Status423Locked = 423;

            /// <summary>
            /// HTTP status code 424.
            /// </summary>
            public const int Status424FailedDependency = 424;

            /// <summary>
            /// HTTP status code 426.
            /// </summary>
            public const int Status426UpgradeRequired = 426;

            /// <summary>
            /// HTTP status code 428.
            /// </summary>
            public const int Status428PreconditionRequired = 428;

            /// <summary>
            /// HTTP status code 429.
            /// </summary>
            public const int Status429TooManyRequests = 429;

            /// <summary>
            /// HTTP status code 431.
            /// </summary>
            public const int Status431RequestHeaderFieldsTooLarge = 431;

            /// <summary>
            /// HTTP status code 451.
            /// </summary>
            public const int Status451UnavailableForLegalReasons = 451;

            /// <summary>
            /// HTTP status code 500.
            /// </summary>
            public const int Status500InternalServerError = 500;

            /// <summary>
            /// HTTP status code 501.
            /// </summary>
            public const int Status501NotImplemented = 501;

            /// <summary>
            /// HTTP status code 502.
            /// </summary>
            public const int Status502BadGateway = 502;

            /// <summary>
            /// HTTP status code 503.
            /// </summary>
            public const int Status503ServiceUnavailable = 503;

            /// <summary>
            /// HTTP status code 504.
            /// </summary>
            public const int Status504GatewayTimeout = 504;

            /// <summary>
            /// HTTP status code 505.
            /// </summary>
            public const int Status505HttpVersionNotsupported = 505;

            /// <summary>
            /// HTTP status code 506.
            /// </summary>
            public const int Status506VariantAlsoNegotiates = 506;

            /// <summary>
            /// HTTP status code 507.
            /// </summary>
            public const int Status507InsufficientStorage = 507;

            /// <summary>
            /// HTTP status code 508.
            /// </summary>
            public const int Status508LoopDetected = 508;

            /// <summary>
            /// HTTP status code 510.
            /// </summary>
            public const int Status510NotExtended = 510;

            /// <summary>
            /// HTTP status code 511.
            /// </summary>
            public const int Status511NetworkAuthenticationRequired = 511;
        }

        public static class Languages
        {
            #region Properties
            public const string GEORGIAN = "ka";
            public const string ENGLISH = "en";
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