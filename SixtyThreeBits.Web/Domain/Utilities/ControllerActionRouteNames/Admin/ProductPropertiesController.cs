namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class ProductPropertiesController
            {
                public const string Properties = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(Properties)}";
                public const string DeleteImage = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(DeleteImage)}";
                public const string ImagesUpload = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(ImagesUpload)}";
                public const string ImagesUpdate = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(ImagesUpdate)}";
                public const string ImagesSort = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(ImagesSort)}";
                public const string ImagesDelete = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(ImagesDelete)}";
            }
        }
    }
}
