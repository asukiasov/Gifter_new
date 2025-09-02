namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record MenuFooterIudDTO
    {
        #region Properties
        public int? MenuFooterID { get; init; }
        public string MenuFooterTitle { get; init; }
        public string MenuFooterTitleEng { get; init; }
        public int? PageID { get; init; }
        public bool? MenuFooterIsExternalPage { get; init; }
        public string MenuFooterExternalPageUrl { get; init; }
        public bool? MenuFooterIsPublished { get; init; }
        public bool? MenuFooterIsTargetBlank { get; set; }
        #endregion
    }
}
