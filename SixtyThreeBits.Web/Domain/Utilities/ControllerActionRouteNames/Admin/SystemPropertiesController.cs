namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class SystemPropertiesController
            {
                #region Properties
                public const string SystemProperties = $"{nameof(Admin)}{nameof(SystemPropertiesController)}{nameof(SystemProperties)}";
                public const string TestEmailSmtp = $"{nameof(Admin)}{nameof(SystemPropertiesController)}{nameof(TestEmailSmtp)}";
                public const string TestEmailMailgun = $"{nameof(Admin)}{nameof(SystemPropertiesController)}{nameof(TestEmailMailgun)}";
                public const string TestEmailOffice365 = $"{nameof(Admin)}{nameof(SystemPropertiesController)}{nameof(TestEmailOffice365)}";
                public const string TestAws = $"{nameof(Admin)}{nameof(SystemPropertiesController)}{nameof(TestAws)}";
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
