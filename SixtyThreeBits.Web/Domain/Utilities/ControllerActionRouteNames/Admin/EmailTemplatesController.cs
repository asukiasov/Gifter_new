namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class EmailTemplatesController
            {
                public const string EmailTemplates = $"{nameof(Admin)}{nameof(EmailTemplatesController)}{nameof(EmailTemplates)}";
                public const string Grid = $"{nameof(Admin)}{nameof(EmailTemplatesController)}{nameof(Grid)}";
            }
        }
    }
}
