namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class BrandPropertiesController
            {
                public const string Properties = $"{nameof(Admin)}{nameof(BrandPropertiesController)}{nameof(Properties)}";
                public const string DeleteImage = $"{nameof(Admin)}{nameof(BrandPropertiesController)}{nameof(DeleteImage)}";
            }
        }
    }
}
