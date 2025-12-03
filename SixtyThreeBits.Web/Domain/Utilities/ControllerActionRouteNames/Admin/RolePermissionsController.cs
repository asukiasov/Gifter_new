namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class RolePermissionsController
            {
                #region Properties
                public const string RolesPermissions = $"{nameof(Admin)}{nameof(RolePermissionsController)}{nameof(RolesPermissions)}";
                public const string RolesGrid = $"{nameof(Admin)}{nameof(RolePermissionsController)}{nameof(RolesGrid)}";
                public const string PermissionsTree = $"{nameof(Admin)}{nameof(RolePermissionsController)}{nameof(PermissionsTree)}";
                public const string GetPermissionsByRole = $"{nameof(Admin)}{nameof(RolePermissionsController)}{nameof(GetPermissionsByRole)}";
                public const string Save = $"{nameof(Admin)}{nameof(RolePermissionsController)}{nameof(Save)}";
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
