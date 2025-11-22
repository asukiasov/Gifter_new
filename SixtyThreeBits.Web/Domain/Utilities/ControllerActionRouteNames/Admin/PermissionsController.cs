namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class PermissionsController
            {
                #region Properties
                public const string Permissions = $"{nameof(Admin)}{nameof(PermissionsController)}{nameof(Permissions)}";
                public const string Tree = $"{nameof(Admin)}{nameof(PermissionsController)}{nameof(Tree)}";
                public const string TreeAdd = $"{nameof(Admin)}{nameof(PermissionsController)}{nameof(TreeAdd)}";
                public const string TreeUpdate = $"{nameof(Admin)}{nameof(PermissionsController)}{nameof(TreeUpdate)}";
                public const string TreeDelete = $"{nameof(Admin)}{nameof(PermissionsController)}{nameof(TreeDelete)}"; 
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
