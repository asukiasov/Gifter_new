using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record NewsIudDTO
    {
        #region Properties
        public int? NewsID { get; init; }
        public string NewsSlug { get; init; }
        public string NewsTitle { get; init; }
        public string NewsTitleEng { get; init; }
        public string NewsShortDescription { get; init; }
        public string NewsShortDescriptionEng { get; init; }
        public string NewsText { get; init; }
        public string NewsTextEng { get; init; }
        public DateTime? NewsDatePublished { get; init; }
        public string NewsImageFilename { get; init; }
        public bool? NewsIsPublished { get; init; }
        #endregion
    }
}
