namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static class WebConstants
    {
        public static class Cookies
        {
            #region Properties
            public const string IsAdminSideBarCollapsed = "IsAdminSideBarCollapsed";
            public const string User = "User";
            public const string AdminLanguageCultureCode = "AdminLanguage";
            #endregion
        }

        public static class Permissions
        {
            public static string Page = "EA1EAEBA-9D1B-40E3-99C6-99A46061C050";
            public static string User = "020F4066-1451-4739-BC42-F6022EEB8881";
        }

        public static class QueryStringKeys
        {
            #region Properties            
            public const string FileManagerAllowedExtensions = "ext";
            public const string FileManagerAllowChooseMultiple = "multichoice";
            public const string FileManagerOnSelectedFilesChooseClientCallback = "callback";
            #endregion
        }

        public static class QueryStringValues
        {
            #region Properties            
            public const string FileManagerOnSelectedFilesChooseClientCallback = "tinyMCEHelper.onSelectedImageChoose";
            #endregion
        }

        public static class RouteValues
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

        public static class Session
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

        public static class ViewData
        {
            #region Properties
            public const string LayoutViewModel = "LayoutViewModel";
            public const string UserLayoutViewModel = "UserLayoutViewModel";
            #endregion
        }

        public static class ViewSections
        {
            #region Properties
            public const string HeaderSection = "HeaderSection";
            public const string FooterSection = "FooterSection";
            #endregion
        }
    }
}
