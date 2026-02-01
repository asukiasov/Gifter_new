using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record GiftListsListDTO
    {
        #region Properties
        public int? GiftListID { get; init; }
        public int? GiftListUserID { get; init; }
        public string GiftListTitle { get; init; }
        public string GiftListDescription { get; init; }
        public string GiftListOccasionType { get; init; }
        public bool GiftListIsSecret { get; init; }
        public bool GiftListIsPublished { get; init; }
        public DateTime? GiftListEndDate { get; init; }
        public DateTime? GiftListDateCreated { get; init; }
        public string OwnerFullname { get; init; }
        #endregion
    }
}
