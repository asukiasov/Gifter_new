namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class ProductCategoriesController
            {
                #region Properties
                public const string Categories = $"{nameof(Admin)}{nameof(ProductCategoriesController)}{nameof(Categories)}";
                public const string Add = $"{nameof(Admin)}{nameof(ProductCategoriesController)}{nameof(Add)}";
                public const string Delete = $"{nameof(Admin)}{nameof(ProductCategoriesController)}{nameof(Delete)}";
                public const string Sort = $"{nameof(Admin)}{nameof(ProductCategoriesController)}{nameof(Sort)}"; 
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
