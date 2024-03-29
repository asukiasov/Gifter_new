using System;

namespace SixtyThreeBits.Core.DTO
{
    public class PageDTO
    {
        #region Properties
        public int? PageID { get; set; }
        public int? ParentID { get; set; }
        public string PageSlug { get; set; }
        public string PageSlugHierarchy { get; set; }
        public string PageTitle { get; set; }
        public string PageTitleEng { get; set; }
        public string PageText { get; set; }
        public string PageTextEng { get; set; }
        public string PageTextHeaderHtml { get; set; }
        public string PageTextHeaderHtmlEng { get; set; }
        public string PageTextFooterHtml { get; set; }
        public string PageTextFooterHtmlEng { get; set; }
        public string PageData { get; set; }
        public string PageDataEng { get; set; }
        public string PageShortDescription { get; set; }
        public string PageShortDescriptionEng { get; set; }
        public string PageImageFilename { get; set; }
        public int? PageCode { get; set; }
        public bool PageIsPublished { get; set; }
        public int? PageSortIndex { get; set; }
        public bool PageIsMenuItem { get; set; }
        public bool PageIsFooterItem { get; set; }
        public bool PageIsExternalUrl { get; set; }
        public string PageExternalUrl { get; set; }
        #endregion        
    }

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

    public record PagesListDTO
    {
        #region Properties
        public int? PageID { get; init; }
        public int? PageParentID { get; init; }
        public string PageSlugHierarchy { get; init; }
        public string PageTitle { get; init; }
        public string PageTitleEng { get; init; }
        public string PageShortDescription { get; init; }
        public string PageShortDescriptionEng { get; init; }
        public string PageImageFilename { get; init; }
        public bool PageIsPublished { get; init; }
        public int? PageSortIndex { get; init; }
        public bool PageIsMenuItem { get; init; }
        public bool PageIsFooterItem { get; init; }
        public bool PageIsExternalUrl { get; init; }
        public string PageExternalUrl { get; init; }
        public DateTime? PageDateCreated { get; init; }
        #endregion
    }
}
