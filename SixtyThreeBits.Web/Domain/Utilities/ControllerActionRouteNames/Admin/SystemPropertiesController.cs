namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class SystemPropertiesController
            {
                public const string SystemProperties = $"{nameof(Admin)}{nameof(SystemPropertiesController)}{nameof(SystemProperties)}";
                public const string TestEmailSmtp = $"{nameof(Admin)}{nameof(SystemPropertiesController)}{nameof(TestEmailSmtp)}";
                public const string TestEmailMailgun = $"{nameof(Admin)}{nameof(SystemPropertiesController)}{nameof(TestEmailMailgun)}";
                public const string TestEmailOffice365 = $"{nameof(Admin)}{nameof(SystemPropertiesController)}{nameof(TestEmailOffice365)}";
                public const string TestAws = $"{nameof(Admin)}{nameof(SystemPropertiesController)}{nameof(TestAws)}";
            }
        }
    }
}
