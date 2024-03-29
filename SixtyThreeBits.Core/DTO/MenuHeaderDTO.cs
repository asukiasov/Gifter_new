namespace SixtyThreeBits.Core.DTO
{
    public record MenuHeaderDTO
    {
        #region Properties
        public int? MenuHeaderID { get; init; }
        public int? MenuHeaderParentID { get; init; }
        public string MenuHeaderTitle { get; init; }
        public string MenuHeaderTitleEng { get; init; }
        public string MenuHeaderTitleRus { get; init; }        
        public bool MenuHeaderIsExternalPage { get; init; }
        public string MenuHeaderExternalPageUrl { get; init; }
        public bool MenuHeaderIsPublished { get; init; }
        public int? MenuHeaderSortIndex { get; set; }

        public int? PageID { get; init; }
        public string PageSlug { get; init; }
        public string PageTitle { get; init; }
        public string PageTitleEng { get; init; }
        public string PageTitleRus { get; init; }
        public bool PageIsPublished { get; init; }
        #endregion
    }

    public record MenuHeaderIudDTO
    {
        #region Properties
        public int? MenuHeaderID { get; init; }
        public int? MenuHeaderParentID { get; init; }
        public string MenuHeaderTitle { get; init; }
        public string MenuHeaderTitleEng { get; init; }
        public string MenuHeaderTitleRus { get; init; }
        public int? PageID { get; init; }
        public bool? MenuHeaderIsExternalPage { get; init; }
        public string MenuHeaderExternalPageUrl { get; init; }
        public bool? MenuHeaderIsPublished { get; init; }        
        #endregion
    }
}
