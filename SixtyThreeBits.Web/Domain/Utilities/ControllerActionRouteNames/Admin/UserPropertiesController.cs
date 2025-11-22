namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class UserPropertiesController
            {
                public const string Properties = $"{nameof(Admin)}{nameof(UserPropertiesController)}{nameof(Properties)}";
            }
        }
    }
}
