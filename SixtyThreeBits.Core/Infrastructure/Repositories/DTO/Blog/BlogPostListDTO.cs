using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record BlogPostListDTO
    {
        #region Properties
        public int? BlogPostID { get; init; }
        public string BlogPostTitle { get; init; }
        public bool BlogPostIsPublished { get; init; }
        public string BlogPostAuthorName { get; init; }
        public DateTime? BlogPostDate { get; init; }
        public DateTime? BlogPostDateCreated { get; init; } 
        #endregion
    }
}