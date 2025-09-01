namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ViewNames
    {
        public static partial class Admin
        {
            public static class Pages
            {
                #region Properties
                public const string PagesView = "~/Views/Admin/Pages/PagesView.cshtml";
                #endregion

                #region Nested Classes
                public static class Page
                {
                    #region Properties
                    public const string PageBuilderView = "~/Views/Admin/Pages/Page/PageBuilderView.cshtml";
                    public const string PagePropertiesView = "~/Views/Admin/Pages/Page/PagePropertiesView.cshtml";                    
                    #endregion
                }
                #endregion
            }
        }
    }
}