namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class OrdersController
            {
                #region Properties
                public const string Orders = $"{nameof(Admin)}{nameof(OrdersController)}{nameof(Orders)}";
                public const string Grid = $"{nameof(Admin)}{nameof(OrdersController)}{nameof(Grid)}";
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
