using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record GiftsListDTO
    {
        #region Properties
        public int? GiftID { get; init; }
        public int? GiftGiftListID { get; init; }
        public string GiftTitle { get; init; }
        public string GiftDescription { get; init; }
        public decimal? GiftPrice { get; init; }
        public string GiftCurrency { get; init; }
        public string GiftUrl { get; init; }
        public string GiftImageUrl { get; init; }
        public bool GiftIsReserved { get; init; }
        public string ReservedByFullname { get; init; }
        public DateTime? GiftDateCreated { get; init; }
        public string WishlistTitle { get; init; }
        public string OwnerFullname { get; init; }
        #endregion
    }
}
