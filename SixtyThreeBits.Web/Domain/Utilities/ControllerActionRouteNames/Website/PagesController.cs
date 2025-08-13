namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Website
        {
            #region Nested Classes
            public static class PagesController
            {
                #region Properties
                public const string Page = $"{nameof(Website)}{nameof(PagesController)}{nameof(Page)}";
                public const string PageCulture = $"{nameof(Website)}{nameof(PagesController)}{nameof(PageCulture)}";
                #endregion
            }
            #endregion
        }        
    }
}
