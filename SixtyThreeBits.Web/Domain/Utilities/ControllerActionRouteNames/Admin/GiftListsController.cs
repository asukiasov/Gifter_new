namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class GiftListsController
            {
                #region Properties
                public const string GiftLists = $"{nameof(Admin)}{nameof(GiftListsController)}{nameof(GiftLists)}";
                public const string Grid = $"{nameof(Admin)}{nameof(GiftListsController)}{nameof(Grid)}";
                public const string GridAdd = $"{nameof(Admin)}{nameof(GiftListsController)}{nameof(GridAdd)}";
                public const string GridUpdate = $"{nameof(Admin)}{nameof(GiftListsController)}{nameof(GridUpdate)}";
                public const string GridDelete = $"{nameof(Admin)}{nameof(GiftListsController)}{nameof(GridDelete)}";
                public const string Followers = $"{nameof(Admin)}{nameof(GiftListsController)}{nameof(Followers)}";
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
