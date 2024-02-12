using System;

namespace SixtyThreeBits.Core.Infrastructure.Utilities
{
    public class Constants
    {
        #region Nested Classes
        public class Cache
        {

        }

        public class Cookies
        {
            #region Properties
            public const string IsAdminSideBarCollapsed = "IsAdminSideBarCollapsed";
            public const string User = "User";
            public const string AdminLanguageCultureCode = "AdminLanguage";
            #endregion
        }

        public class Formats
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

        public class Languages
        {
            #region Properties
            public const string GEORGIAN = "ka";
            public const string ENGLISH = "en";
            public const string RUSSIAN = "ru";
            #endregion
        }

        public class NullValueFor
        {
            #region Properties
            public const string String = "";
            public const int Int = -1;
            public static readonly DateTime Date = new DateTime(1900, 1, 1);
            #endregion
        }

        public class QueryStringKeys
        {
            #region Properties            
            public const string FileManagerAllowedExtensions = "ext";
            public const string FileManagerAllowChooseMultiple = "multichoice";
            public const string FileManagerOnSelectedFilesChooseClientCallback = "callback";
            #endregion
        }

        public class QueryStringValues
        {
            #region Properties            
            public const string FileManagerOnSelectedFilesChooseClientCallback = "tinyMCEHelper.onSelectedImageChoose";
            #endregion
        }

        public class RegularExpressions
        {
            #region Properties
            public const string Base64 = "^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$";
            public const string Email = @"^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$";
            #endregion
        }

        public class RouteValues
        {
            #region Properties
            public const string BlogPostID = "blogPostID";
            public const string BrandID = "brandID";
            public const string Culture = "culture";
            public const string NewsID = "newsID";
            public const string PageID = "pageID";
            public const string ProductCategoryID = "productCategoryID";
            public const string ProductID = "productID";
            public const string PartnerID = "partnerID";            
            public const string TeamMemberID = "teamMemberID";
            public const string UserID = "userID";
            #endregion
        }

        public class Session
        {
            #region Properties
            public const string User = "User";
            #endregion

            #region Nested Classes
            public class SuccessErrorMessage
            {
                #region Properties
                public const string Error = "SuccessErrorMessageError";
                public const string Success = "SuccessErrorMessageSuccess";
                #endregion
            }
            #endregion
        }

        public class ViewData
        {
            #region Properties
            public const string LayoutViewModel = "LayoutViewModel";
            public const string UserLayoutViewModel = "UserLayoutViewModel";
            public const string TabsViewModel = "TabsViewModel";
            #endregion
        }

        public class ViewSections
        {
            public const string HeaderSection = "HeaderSection";
            public const string FooterSection = "FooterSection";
        }
        #endregion
    }
}
