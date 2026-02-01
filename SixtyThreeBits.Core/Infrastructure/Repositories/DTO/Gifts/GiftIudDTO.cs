namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record GiftIudDTO
    {
        #region Properties
        public string GiftTitle { get; init; }
        public string GiftDescription { get; init; }
        public decimal? GiftPrice { get; init; }
        public string GiftCurrency { get; init; }
        public string GiftUrl { get; init; }
        public string GiftImageUrl { get; init; }
        #endregion
    }
}
