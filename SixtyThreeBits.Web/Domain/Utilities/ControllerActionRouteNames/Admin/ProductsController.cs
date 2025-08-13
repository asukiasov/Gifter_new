namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class ProductsController
            {
                #region Properties
                public const string Products = "AdminProductsControllerProducts";
                public const string Grid = "AdminProductsControllerGrid";
                public const string GridAdd = "AdminProductsControllerGridAdd";
                public const string GridUpdate = "AdminProductsControllerGridUpdate";
                public const string GridDelete = "AdminProductsControllerGridDelete";
                public const string ExcelDownload = "AdminProductsControllerExcelDownload";
                public const string ExcelUpload = "AdminProductsControllerExcelUpload";
                #endregion                
            }
        }
    }
}
