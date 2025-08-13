namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ViewNames
    {
        public static partial class Admin
        {
            public static class Pages
            {
                #region Properties
                public const string PagesView = "~/Views/Admin/Pages/Pages.cshtml";
                #endregion

                #region Nested Classes
                public static class Page
                {
                    #region Properties
                    public const string PagePropertiesView = "~/Views/Admin/Pages/Page/PageProperties.cshtml";
                    public const string PageBuilderView = "~/Views/Admin/Pages/Page/PageBuilder.cshtml";
                    #endregion
                }
                #endregion
            }
        }
    }
}