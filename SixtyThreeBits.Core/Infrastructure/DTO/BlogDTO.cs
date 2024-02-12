using System;

namespace SixtyThreeBits.Core.Infrastructure.DTO
{
    public class BlogPostDTO
    {
        #region Properties
        public int? BlogPostID { get; set; }
        public string BlogPostSlug { get; set; }
        public string BlogPostTitle { get; set; }
        public string BlogPostShortText { get; set; }
        public string BlogPostText { get; set; }
        public string BlogPostAuthorName { get; set; }
        public string BlogPostImageFilename { get; set; }
        public DateTime? BlogPostDate { get; set; }
        public bool BlogPostIsPublished { get; set; }
        public DateTime? BlogPostDateCreated { get; set; }
        #endregion
    }
}
