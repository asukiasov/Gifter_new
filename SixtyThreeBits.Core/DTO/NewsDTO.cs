using System;

namespace SixtyThreeBits.Core.DTO
{
    public class NewsDTO
    {
        #region Properties
        public int? NewsID { get; set; }
        public string NewsSlug { get; set; }
        public string NewsTitle { get; set; }
        public string NewsTitleEng { get; set; }
        public string NewsText { get; set; }
        public string NewsTextEng { get; set; }
        public string NewsShortDescription { get; set; }
        public string NewsShortDescriptionEng { get; set; }
        public string NewsImageFilename { get; set; }
        public DateTime? NewsDatePublished { get; set; }
        public bool NewsIsPublished { get; set; }
        public DateTime? NewsDateCreated { get; set; }
        #endregion        
    }
}
