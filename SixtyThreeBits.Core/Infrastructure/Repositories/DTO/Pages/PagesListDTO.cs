using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record PagesListDTO
    {
        #region Properties
        public int? PageID { get; init; }
        public string PageSlug { get; init; }
        public string PageTitle { get; init; }
        public string PageTitleEng { get; init; }
        public bool PageIsPublished { get; init; }
        public DateTime? PageDateCreated { get; init; }
        #endregion
    }
}
