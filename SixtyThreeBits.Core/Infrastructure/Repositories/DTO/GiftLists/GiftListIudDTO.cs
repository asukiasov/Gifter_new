using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record GiftListIudDTO
    {
        #region Properties
        public int? GiftListUserID { get; init; }
        public string GiftListTitle { get; init; }
        public string GiftListDescription { get; init; }
        public string GiftListOccasionType { get; init; }
        public bool? GiftListIsSecret { get; init; }
        public bool? GiftListIsPublished { get; init; }
        public DateTime? GiftListEndDate { get; init; }
        #endregion
    }
}
