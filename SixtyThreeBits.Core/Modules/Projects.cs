using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class ProjectsDataAccess : DataAccessBase
    {
        #region Properties
        readonly UtilityCollection Utilities;
        #endregion
        #region Constructors
        public ProjectsDataAccess(ConnectionFactory ConnectionFactory, UtilityCollection Utilities) : base(ConnectionFactory)
        {
            this.Utilities = Utilities;
        }
        #endregion

        #region Methods
        public async Task<Projects> GetSingleProjectByID(int? ProjectID)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSingleProjectByID)}({nameof(ProjectID)} = {ProjectID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.ProjectsGetSingleByID(ProjectID);
                    return Result?.DeserializeTo<Projects>();
                }
            });
        }
        public async Task<List<DBCoreDataContext.ProjectsListResultItem>> ListProjects()
        {
            return await TryToReturn($"{nameof(ListProjects)}()", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.ProjectsList().OrderByDescending(Item=>Item.ProjectDateCreated) .ToListAsync();
                }
            });
        }

        public async Task<bool> IsProjectSlugUniq(string ProjectSlug, int? ProjectID = null)
        {
            return await TryToReturnAsyncTask($"{nameof(IsProjectSlugUniq)}({nameof(ProjectSlug)} = {ProjectSlug}, {nameof(ProjectID)} = {ProjectID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.ProjectsIsSlugUniq(ProjectSlug, ProjectID);
                }
            });
        }
        public async Task<int?> ProjectsIUD(Enums.DatabaseActions DatabaseAction, int? ProjectID = null, string ProjectSlug = null, string ProjectCaption = null, string ProjectCaptionEng = null, string ProjectCaptionRus = null, string ProjectShortDescription = null, string ProjectShortDescriptionEng = null, string ProjectShortDescriptionRus = null, string ProjectDescription = null, string ProjectDescriptionEng = null, string ProjectDescriptionRus = null, string ProjectCoverImageFilename = null, string ProjectVideoUrl = null, bool? ProjectIsPublished = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ProjectsIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(ProjectID)} = {ProjectID}, {nameof(ProjectSlug)} = {ProjectSlug}, {nameof(ProjectCaption)} = {ProjectCaption}, {nameof(ProjectCaptionEng)} = {ProjectCaptionEng}, {nameof(ProjectCaptionRus)} = {ProjectCaptionRus}, {nameof(ProjectShortDescription)} = {ProjectShortDescription}, {nameof(ProjectShortDescriptionEng)} = {ProjectShortDescriptionEng}) {nameof(ProjectShortDescriptionRus)} = {ProjectShortDescriptionRus}) {nameof(ProjectDescription)} = {ProjectDescription}) {nameof(ProjectDescriptionEng)} = {ProjectDescriptionEng}) {nameof(ProjectDescriptionRus)} = {ProjectDescriptionRus}) {nameof(ProjectCoverImageFilename)} = {ProjectCoverImageFilename}) {nameof(ProjectVideoUrl)} = {ProjectVideoUrl}) {nameof(ProjectIsPublished)} = {ProjectIsPublished}))", async () =>
            {
                if (DatabaseAction== Enums.DatabaseActions.DELETE)
                {
                    var DBItem =  await GetSingleProjectByID(ProjectID);
                    Utilities.DeleteUploadedFile(DBItem?.ProjectCoverImageFilename);
                }

                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    ProjectID = await db.ProjectsIUD(DatabaseAction, ProjectID, ProjectSlug, ProjectCaption, ProjectCaptionEng, ProjectCaptionRus, ProjectShortDescription, ProjectShortDescriptionEng, ProjectShortDescriptionRus, ProjectDescription, ProjectDescriptionEng, ProjectDescriptionRus, ProjectCoverImageFilename, ProjectVideoUrl, ProjectIsPublished);
                    return ProjectID;
                }
            });
        }
        #endregion
    }
    public class Projects
    {
        #region Properties
        public int? ProjectID { get; set; }
        public string ProjectSlug { get; set; }
        public string ProjectCaption { get; set; }
        public string ProjectCaptionEng { get; set; }
        public string ProjectCaptionRus { get; set; }
        public string ProjectShortDescription { get; set; }
        public string ProjectShortDescriptionEng { get; set; }
        public string ProjectShortDescriptionRus { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectDescriptionEng { get; set; }
        public string ProjectDescriptionRus { get; set; }
        public string ProjectCoverImageFilename { get; set; }
        public string ProjectVideoUrl { get; set; }
        public bool ProjectIsPublished { get; set; }
        #endregion
    }
}
