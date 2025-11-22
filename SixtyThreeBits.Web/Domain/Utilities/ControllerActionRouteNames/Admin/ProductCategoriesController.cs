namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class ProductCategoriesController
            {
                public const string Categories = $"{nameof(Admin)}{nameof(ProductCategoriesController)}{nameof(Categories)}";
                public const string Add = $"{nameof(Admin)}{nameof(ProductCategoriesController)}{nameof(Add)}";
                public const string Delete = $"{nameof(Admin)}{nameof(ProductCategoriesController)}{nameof(Delete)}";
                public const string Sort = $"{nameof(Admin)}{nameof(ProductCategoriesController)}{nameof(Sort)}";
            }
        }
    }
}
