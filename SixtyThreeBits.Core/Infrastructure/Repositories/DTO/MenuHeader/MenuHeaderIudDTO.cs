namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record MenuHeaderIudDTO
    {
        #region Properties
        public int? MenuHeaderID { get; init; }
        public int? MenuHeaderParentID { get; init; }
        public string MenuHeaderTitle { get; init; }
        public string MenuHeaderTitleEng { get; init; }        
        public int? PageID { get; init; }
        public bool? MenuHeaderIsExternalPage { get; init; }
        public string MenuHeaderExternalPageUrl { get; init; }
        public bool? MenuHeaderIsPublished { get; init; }
        public bool? MenuHeaderIsTargetBlank { get; set; }
        #endregion
    }
}
