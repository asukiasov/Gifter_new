namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class FileManagerController
            {
                #region Properties
                public const string FileManager = $"{nameof(Admin)}{nameof(FileManagerController)}{nameof(FileManager)}";
                public const string Files = $"{nameof(Admin)}{nameof(FileManagerController)}{nameof(Files)}";
                public const string Upload = $"{nameof(Admin)}{nameof(FileManagerController)}{nameof(Upload)}";
                public const string Delete = $"{nameof(Admin)}{nameof(FileManagerController)}{nameof(Delete)}"; 
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
