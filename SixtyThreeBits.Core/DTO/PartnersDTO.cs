using System;

namespace SixtyThreeBits.Core.DTO
{
    public class PartnerDTO
    {
        #region Properties
        public int? PartnerID { get; set; }
        public string PartnerName { get; set; }
        public string PartnerNameEng { get; set; }
        public string PartnerShortDescription { get; set; }
        public string PartnerShortDescriptionEng { get; set; }
        public string PartnerFullDescription { get; set; }
        public string PartnerFullDescriptionEng { get; set; }
        public string PartnerWebSite { get; set; }
        public string PartnerImageFilename { get; set; }
        public bool PartnerIsPublished { get; set; }
        public DateTime? PartnerDateCreated { get; set; }
        #endregion
    }

    public record PartnersListDTO
    {
        #region Properties
        public int? PartnerID { get; init; }
        public string PartnerName { get; init; }
        public string PartnerNameEng { get; init; }
        public string PartnerWebSite { get; init; }
        public bool PartnerIsPublished { get; init; }
        public DateTime? PartnerDateCreated { get; init; }
        #endregion
    }
}
