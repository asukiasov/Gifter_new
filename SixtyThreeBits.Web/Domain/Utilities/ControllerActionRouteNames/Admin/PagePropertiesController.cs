namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class PagePropertiesController
            {
                public const string Properties = $"{nameof(Admin)}{nameof(PagePropertiesController)}{nameof(Properties)}";
                public const string DeleteImage = $"{nameof(Admin)}{nameof(PagePropertiesController)}{nameof(DeleteImage)}";
            }
        }
    }
}
