namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class RolesController
            {
                #region Properties
                public const string Roles = $"{nameof(Admin)}{nameof(RolesController)}{nameof(Roles)}";
                public const string Grid = $"{nameof(Admin)}{nameof(RolesController)}{nameof(Grid)}";
                public const string GridAdd = $"{nameof(Admin)}{nameof(RolesController)}{nameof(GridAdd)}";
                public const string GridUpdate = $"{nameof(Admin)}{nameof(RolesController)}{nameof(GridUpdate)}";
                public const string GridDelete = $"{nameof(Admin)}{nameof(RolesController)}{nameof(GridDelete)}";
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
