namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class NewsPropertiesController
            {
                #region Properties
                public const string Properties = $"{nameof(Admin)}{nameof(NewsPropertiesController)}{nameof(Properties)}";
                public const string DeleteImage = $"{nameof(Admin)}{nameof(NewsPropertiesController)}{nameof(DeleteImage)}";
                #endregion
            }
        }
    }
}
