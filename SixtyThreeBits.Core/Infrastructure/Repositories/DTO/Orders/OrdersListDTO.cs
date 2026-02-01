using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record OrdersListDTO
    {
        #region Properties
        public int? OrderID { get; init; }
        public DateTime? OrderDateCreated { get; init; }
        public int? OrderUserID { get; init; }
        public string OrderUserFullname { get; init; }
        public int? OrderType { get; init; }
        public string OrderTypeName { get; init; }
        public int? OrderGiftListID { get; init; }
        public string OrderGiftListTitle { get; init; }
        public int? OrderGiftID { get; init; }
        public string OrderGiftTitle { get; init; }
        public int? OrderTargetUserID { get; init; }
        public string OrderTargetUserFullname { get; init; }
        #endregion
    }
}
