namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record PageIudDTO
    {
        #region Properties
        public int? PageID { get; init; }
        public string PageSlug { get; init; }
        public string PageTitle { get; init; }
        public string PageTitleEng { get; init; }        
        public string PageText { get; init; }
        public string PageTextEng { get; init; }
        public string PageData { get; init; }
        public string PageDataEng { get; init; }
        public string PageShortDescription { get; init; }
        public string PageShortDescriptionEng { get; init; }
        public string PageTextHeaderHtml { get; init; }
        public string PageTextHeaderHtmlEng { get; init; }
        public string PageTextFooterHtml { get; init; }
        public string PageTextFooterHtmlEng { get; init; }
        public string PageImageFilename { get; init; }
        public bool? PageIsPublished { get; init; }        
        #endregion        
    }    
}
