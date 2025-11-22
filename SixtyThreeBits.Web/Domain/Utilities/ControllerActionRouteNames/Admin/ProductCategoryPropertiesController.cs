namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class ProductCategoryPropertiesController
            {
                public const string Properties = $"{nameof(Admin)}{nameof(ProductCategoryPropertiesController)}{nameof(Properties)}";
                public const string DeleteImage = $"{nameof(Admin)}{nameof(ProductCategoryPropertiesController)}{nameof(DeleteImage)}";
            }
        }
    }
}
