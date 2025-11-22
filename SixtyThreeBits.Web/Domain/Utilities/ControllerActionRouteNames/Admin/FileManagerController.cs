namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class FileManagerController
            {
                public const string FileManager = $"{nameof(Admin)}{nameof(FileManagerController)}{nameof(FileManager)}";
                public const string Files = $"{nameof(Admin)}{nameof(FileManagerController)}{nameof(Files)}";
                public const string Upload = $"{nameof(Admin)}{nameof(FileManagerController)}{nameof(Upload)}";
                public const string Delete = $"{nameof(Admin)}{nameof(FileManagerController)}{nameof(Delete)}";
            }
        }
    }
}
