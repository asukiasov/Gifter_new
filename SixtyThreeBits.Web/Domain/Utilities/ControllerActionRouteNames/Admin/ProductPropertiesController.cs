namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class ProductPropertiesController
            {
                #region Properties
                public const string Properties = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(Properties)}";
                public const string DeleteImage = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(DeleteImage)}";
                public const string ProductImagesUpload = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(ProductImagesUpload)}";
                public const string ProductImagesUpdate = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(ProductImagesUpdate)}";
                public const string ProductImagesSort = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(ProductImagesSort)}";
                public const string ProductImagesDelete = $"{nameof(Admin)}{nameof(ProductPropertiesController)}{nameof(ProductImagesDelete)}";
                #endregion
            }
        }
    }
}
