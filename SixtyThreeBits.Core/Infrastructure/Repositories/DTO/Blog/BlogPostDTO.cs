using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record BlogPostDTO
    {
        #region Properties
        public int? BlogPostID { get; init; }
        public string BlogPostSlug { get; init; }
        public string BlogPostTitle { get; init; }
        public string BlogPostShortText { get; init; }
        public string BlogPostText { get; init; }
        public string BlogPostAuthorName { get; init; }
        public string BlogPostImageFilename { get; init; }
        public DateTime? BlogPostDate { get; init; }
        public bool BlogPostIsPublished { get; init; }
        public DateTime? BlogPostDateCreated { get; init; }
        #endregion
    }
}
