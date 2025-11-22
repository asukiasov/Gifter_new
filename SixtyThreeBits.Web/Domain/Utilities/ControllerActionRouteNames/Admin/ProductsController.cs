namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class ProductsController
            {
                #region Properties
                public const string Products = $"{nameof(Admin)}{nameof(ProductsController)}{nameof(Products)}";
                public const string Grid = $"{nameof(Admin)}{nameof(ProductsController)}{nameof(Grid)}";
                public const string GridAdd = $"{nameof(Admin)}{nameof(ProductsController)}{nameof(GridAdd)}";
                public const string GridUpdate = $"{nameof(Admin)}{nameof(ProductsController)}{nameof(GridUpdate)}";
                public const string GridDelete = $"{nameof(Admin)}{nameof(ProductsController)}{nameof(GridDelete)}";
                public const string ExcelDownload = $"{nameof(Admin)}{nameof(ProductsController)}{nameof(ExcelDownload)}";
                public const string ExcelUpload = $"{nameof(Admin)}{nameof(ProductsController)}{nameof(ExcelUpload)}"; 
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
