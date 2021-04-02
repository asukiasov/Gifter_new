using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Services;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Reusables.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class ProjectsModel : WebProjectModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Projects.ProjectsGridAdd);

            ViewModel.Grid = new PageViewModel.GridModel();
            ViewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.Projects.ProjectsGridAdd);
            ViewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Projects.ProjectsGridUpdate);
            ViewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Projects.ProjectsGridDelete);
            ViewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Projects.ProjectsGrid);
            ViewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Projects.ProjectsGridAdd);
            ViewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Projects.ProjectsGridUpdate);
            ViewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Projects.ProjectsGridDelete);

            return ViewModel;
        }
        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var ViewModel = (await DataAccessFactory.Projects.ListProjects()).Select(Item => new PageViewModel.GridModel.GridItem
            {
                ProjectID = Item.ProjectID,
                ProjectCaption = Item.ProjectCaption,
                ProjectIsPublished = Item.ProjectIsPublished,
                UrlProjectsProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.Projects.Project.Properties, new { ProjectID = Item.ProjectID })
            }).ToList();
            return ViewModel;
        }
        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? ProjectID, PageViewModel.GridModel.GridItem SubmitModel)
        {
            await DataAccessFactory.Projects.ProjectsIUD(
                DatabaseAction: DatabaseAction,
                ProjectID: ProjectID,
                ProjectCaption: SubmitModel.ProjectCaption,
                ProjectIsPublished: SubmitModel.ProjectIsPublished
            );

            if (DataAccessFactory.Projects.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Sub Classes
            public class GridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<GridModel.GridItem>
            {
                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.ProjectID));

                    Grid
                    .ID("ProjectsGrid")
                    .OnInitialized("ProjectsModel.OnProjectsGridInit")
                    .Columns(Columns =>
                    {
                        Columns.Add().Width(30).Caption(" ").CellTemplate(new JS("ProjectsModel.GetDetailsButtonColumnCellHtml"));
                        Columns.AddFor(m => m.ProjectCaption).Caption("Caption").Width(400).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        });
                        Columns.AddFor(m => m.ProjectIsPublished).Caption("Published").DataType(GridColumnDataType.Boolean).Width(150);
                        Columns.Add();
                    });

                    return Grid;
                }
                #endregion

                #region Sub CLasses
                public class GridItem
                {
                    #region Properties
                    public int? ProjectID { get; set; }
                    public string ProjectCaption { get; set; }
                    public bool ProjectIsPublished { get; set; }
                    public string UrlProjectsProperties { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }

    public class ProjectsModelBase : WebProjectModelBase
    {
        #region Properties
        public Projects DBItemProjects { get; set; }
        #endregion
    }
    public class ProjectsPropertiesModel : ProjectsModelBase
    {
        #region Methods
        public async Task<ProjectsPropertiesViewModel> GetPageViewModel(int? ProjectID, ProjectsPropertiesViewModel ViewModel)
        {
            var DBItem = await DataAccessFactory.Projects.GetSingleProjectByID(ProjectID);
            DBItemProjects = DBItem;
            if (DBItem == null)
            {
                ViewModel = null;
            }
            else
            {
                if (ViewModel == null)
                {
                    ViewModel = new ProjectsPropertiesViewModel();
                    ViewModel.ProjectSlug = DBItem.ProjectSlug;
                    ViewModel.ProjectCaption = DBItem.ProjectCaption;
                    ViewModel.ProjectCaptionEng = DBItem.ProjectCaptionEng;
                    ViewModel.ProjectCaptionRus = DBItem.ProjectCaptionRus;
                    ViewModel.ProjectShortDescription = DBItem.ProjectShortDescription;
                    ViewModel.ProjectShortDescriptionEng = DBItem.ProjectShortDescriptionEng;
                    ViewModel.ProjectShortDescriptionRus = DBItem.ProjectShortDescriptionRus;
                    ViewModel.ProjectDescription = DBItem.ProjectDescription;
                    ViewModel.ProjectDescriptionEng = DBItem.ProjectDescriptionEng;
                    ViewModel.ProjectDescriptionRus = DBItem.ProjectDescriptionRus;
                    ViewModel.ProjectVideoUrl = DBItem.ProjectVideoUrl;
                    ViewModel.ProjectIsPublished = DBItem.ProjectIsPublished;
                }
                ViewModel.ProjectCoverImageFilename = DBItem.ProjectCoverImageFilename;
                ViewModel.ProjectImageHttpPath = Utilities.GetUploadedFileHttpPath(DBItem.ProjectCoverImageFilename);
                ViewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.Projects.Project.DeleteCoverImage, new { ProjectID = ProjectID });
            }
            return ViewModel;
        }

        public  async Task ValidatePageViewModel(ProjectsPropertiesViewModel ViewModel)
        {
            ViewModel.Errors = new List<SimpleKeyValue<string, string>>
            {
                Validation.ValidateRequired(ErrorKey:$"[name=\"{nameof(ViewModel.ProjectSlug)}\"]", ValueToValidate:ViewModel.ProjectSlug),
                await Validation.ValidateAsync(
                    ErrorAction: async () =>
                    {
                        var IsUniq = await DataAccessFactory.Projects.IsProjectSlugUniq(ProjectSlug:ViewModel.ProjectSlug, ProjectID: DBItemProjects.ProjectID);
                        return !IsUniq;
                    },
                    ErrorKey: $"[name=\"{nameof(ViewModel.ProjectSlug)}\"]",
                    ErrorMessage: Resources.ValidationProjectsSlugNotUniq
                )
            };
            ViewModel.Errors.RemoveAll(Item => Item == null);
        }

        public async Task SaveProjectsProperties(int? ProjectID, ProjectsPropertiesViewModel ViewModel)
        {
            var HasProjectImage = ViewModel.PostedFile?.Length > 0;
            var ProjectImageFilename = HasProjectImage ? GetFilenameFromUploadedFile(ViewModel.PostedFile) : null;
            if (HasProjectImage)
            {
                Utilities.DeleteUploadedFile(ViewModel.ProjectCoverImageFilename);
            }

            await DataAccessFactory.Projects.ProjectsIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                ProjectID: ProjectID,
                ProjectSlug: ViewModel.ProjectSlug,
                ProjectCaption: ViewModel.ProjectCaption,
                ProjectCaptionEng: ViewModel.ProjectCaptionEng,
                ProjectCaptionRus: ViewModel.ProjectCaptionRus,
                ProjectShortDescription: ViewModel.ProjectShortDescription,
                ProjectShortDescriptionEng: ViewModel.ProjectShortDescriptionEng,
                ProjectShortDescriptionRus: ViewModel.ProjectShortDescriptionRus,
                ProjectDescription: ViewModel.ProjectDescription,
                ProjectDescriptionEng: ViewModel.ProjectDescriptionEng,
                ProjectDescriptionRus: ViewModel.ProjectDescriptionRus,
                ProjectCoverImageFilename: ProjectImageFilename,
                ProjectVideoUrl: ViewModel.ProjectVideoUrl,
                ProjectIsPublished: ViewModel.ProjectIsPublished
            );

            if (!DataAccessFactory.Projects.IsError)
            {
                ViewModel.IsSaved = true;
                if (HasProjectImage)
                {
                    await SaveUploadedFile(PostedFile: ViewModel.PostedFile, Filename: ProjectImageFilename);
                }
            }
        }
        public async Task<AjaxResponse> DeleteImage(int? ProjectID)
        {
            var ProjectItem = await DataAccessFactory.Projects.GetSingleProjectByID(ProjectID);
            Utilities.DeleteUploadedFile(ProjectItem.ProjectCoverImageFilename);

            var AR = new AjaxResponse();
            await DataAccessFactory.Projects.ProjectsIUD(
               DatabaseAction: Enums.DatabaseActions.UPDATE,
               ProjectID: ProjectID,
               ProjectCoverImageFilename: Constants.NullValueFor.String
            );

            AR.IsSuccess = !DataAccessFactory.Projects.IsError;

            return AR;
        }

        #endregion

        #region Sub Classes
        public class ProjectsPropertiesViewModel : FormViewModelBase
        {
            #region Properties
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
            public string ProjectImageHttpPath { get; set; }
            public bool HasProjectImage => !string.IsNullOrWhiteSpace(ProjectCoverImageFilename);
            public IFormFile PostedFile { get; set; }
            public string UrlDeleteImage { get; set; }
            public string ProjectVideoUrl { get; set; }
            public bool ProjectIsPublished { get; set; }
            public readonly string TextConfirmDelete = Resources.TextConfirmDelete;
            #endregion
        }
        #endregion
    }
}
