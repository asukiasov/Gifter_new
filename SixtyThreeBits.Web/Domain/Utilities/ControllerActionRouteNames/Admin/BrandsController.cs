namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class BrandsController
            {
                #region Properties
                public const string Brands = $"{nameof(Admin)}{nameof(BrandsController)}{nameof(Brands)}";
                public const string Grid = $"{nameof(Admin)}{nameof(BrandsController)}{nameof(Grid)}";
                public const string GridAdd = $"{nameof(Admin)}{nameof(BrandsController)}{nameof(GridAdd)}";
                public const string GridUpdate = $"{nameof(Admin)}{nameof(BrandsController)}{nameof(GridUpdate)}";
                public const string GridDelete = $"{nameof(Admin)}{nameof(BrandsController)}{nameof(GridDelete)}";
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
