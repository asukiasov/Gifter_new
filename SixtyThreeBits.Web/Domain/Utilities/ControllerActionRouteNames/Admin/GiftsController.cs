namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class GiftsController
            {
                #region Properties
                public const string Gifts = $"{nameof(Admin)}{nameof(GiftsController)}{nameof(Gifts)}";
                public const string Grid = $"{nameof(Admin)}{nameof(GiftsController)}{nameof(Grid)}";
                public const string GridUpdate = $"{nameof(Admin)}{nameof(GiftsController)}{nameof(GridUpdate)}";
                public const string GridDelete = $"{nameof(Admin)}{nameof(GiftsController)}{nameof(GridDelete)}";
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
