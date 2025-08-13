namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Website
        {
            #region Nested Classes
            public static class HomeController
            {
                #region Properties
                public const string Index = $"{nameof(Website)}{nameof(HomeController)}{nameof(Index)}";
                public const string IndexCulture = $"{nameof(Website)}{nameof(HomeController)}{nameof(IndexCulture)}";
                #endregion
            }
            #endregion
        }
    }
}
