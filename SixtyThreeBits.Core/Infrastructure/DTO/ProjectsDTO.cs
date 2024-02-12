using System;

namespace SixtyThreeBits.Core.Infrastructure.DTO
{
    public class ProjectDTO
    {
        #region Properties
        public int? ProjectID { get; set; }
        public string ProjectSlug { get; set; }
        public string ProjectCaption { get; set; }
        public string ProjectCaptionEng { get; set; }
        public string ProjectShortDescription { get; set; }
        public string ProjectShortDescriptionEng { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectDescriptionEng { get; set; }
        public string ProjectCoverImageFilename { get; set; }
        public string ProjectVideoUrl { get; set; }
        public bool ProjectIsPublished { get; set; }
        #endregion
    }

    public record ProjectsListDTO
    {
        #region Properties
        public int? ProjectID { get; set; }
        public string ProjectCaption { get; set; }
        public string ProjectCaptionEng { get; set; }
        public bool ProjectIsPublished { get; set; }
        public int? ProjectSortIndex { get; set; }
        public DateTime ProjectDateCreated { get; set; }
        #endregion
    }
}
