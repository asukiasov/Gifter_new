using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Repositories;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SixtyThreeBits.Core.Utilities.Constants;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class TeamMembersModel : ModelBase
    {
        #region Methods
        public async Task<PageViewModel> GetPageViewModel()
        {
            var viewModel = new PageViewModel();

            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridAdd);
            viewModel.Grid = new PageViewModel.GridModel();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGrid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridDelete);
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridDelete);

            var repository = RepositoriesFactory.GetDictionariesRepository();
            viewModel.Grid.TeamMemberCategories = await repository.DictionariesListAsKeyValueTuple(Enums.DictionaryCodes.TeamMemberCategories);
            viewModel.UrlSync = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridSort);

            return viewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var repository = RepositoriesFactory.GetTeamMembersRepository();
            var viewModel = (await repository.TeamMembersList())
            ?.Select(item => new PageViewModel.GridModel.GridItem
            {
                TeamMemberID = item.TeamMemberID,
                TeamMemberFirstname = item.TeamMemberFirstname,
                TeamMemberLastname = item.TeamMemberLastname,
                TeamMemberPosition = item.TeamMemberPosition,
                TeamMemberIsPublished = item.TeamMemberIsPublished,
                TeamMemberCategoryID = item.TeamMemberCategoryID,
                TeamMemberSortIndex = item.TeamMemberSortIndex,
                UrlTeamMemberProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMember.Properties, new { teamMemberID = item.TeamMemberID })
            })
            .OrderBy(item => item.TeamMemberSortIndex)
            .ToList();

            return viewModel;
        }

        public async Task CRUD(Enums.DatabaseActions databaseAction, int? teamMemberID, PageViewModel.GridModel.GridItem submitModel)
        {
            var repository = RepositoriesFactory.GetTeamMembersRepository();

            if (databaseAction == Enums.DatabaseActions.DELETE)
            {
                var dbItem = await repository.TeamMembersGetSingleByID(teamMemberID);
                await DeleteUploadedFile(dbItem.TeamMemberImageFilename, folderPath: null);
            }

            await repository.TeamMembersIUD(
                databaseAction: databaseAction,                
                teamMemberID: teamMemberID,
                teamMember: new TeamMemberIudDTO
                {
                    TeamMemberFirstname = submitModel.TeamMemberFirstname,
                    TeamMemberLastname = submitModel.TeamMemberLastname,
                    TeamMemberPosition = submitModel.TeamMemberPosition,
                    TeamMemberIsPublished = submitModel.TeamMemberIsPublished,
                    TeamMemberCategoryID = submitModel.TeamMemberCategoryID
                }                
            );

            if (repository.IsError)
            {
                Form.AddError(repository.ErrorMessage);
            }
        }

        public async Task<AjaxResponse> TeamMembersSyncSortIndexes(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.GetTeamMembersRepository();
            await repository.TeamMembersSyncSortIndexes(submitModel.SortIndexes);
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }

        #endregion

        #region Nested Classes
        public class PageViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            public string UrlSync { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<GridModel.GridItem>
            {
                #region Properties
                public List<KeyValueTuple<int?, string>> TeamMemberCategories { get; set; }
                #endregion

                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var Grid = GetGridWithStartupValues<GridItem>(html: Html, keyFieldName: nameof(GridItem.TeamMemberID));

                    Grid
                   .Sorting(sorting => sorting.Mode(GridSortingMode.None))
                   .Pager(Options =>
                   {
                       Options.Visible(false);
                   })
                   .RowDragging(rd => rd
                        .AllowReordering(true)
                        .OnReorder("teamMembersModel.onGridReorder")
                        .DropFeedbackMode(DropFeedbackMode.Push)
                        .ShowDragIcons(true)
                    )
                   .Paging(Options =>
                   {
                       Options.Enabled(false);
                   })
                   .ID("TeamMembersGrid")
                   .FilterRow(Options =>
                   {
                       Options.Visible(false);
                   })
                   .OnInitialized("teamMembersModel.onGridInit")
                   .Columns(Columns =>
                   {
                       Columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlTeamMemberProperties));
                       Columns.AddFor(m => m.TeamMemberFirstname).Caption(Resources.TextFirstname).Width(150).ValidationRules(Options =>
                       {
                           Options.AddRequired();
                       });
                       Columns.AddFor(m => m.TeamMemberLastname).Caption(Resources.TextLastname).Width(150).ValidationRules(Options =>
                       {
                           Options.AddRequired();
                       });
                       Columns.AddFor(m => m.TeamMemberPosition).Caption(Resources.TextPosition).Width(150);
                       Columns.AddFor(m => m.TeamMemberCategoryID).Caption(Resources.TextCategory).Width(150).InitLookupColumn(data: TeamMemberCategories, allowNull: true);
                       Columns.AddFor(m => m.TeamMemberIsPublished).Caption(Resources.TextPublished).DataType(GridColumnDataType.Boolean).Width(130).InitCheckboxColumn();
                       Columns.Add();
                   });

                    return Grid;

                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? TeamMemberID { get; set; }
                    public string TeamMemberFirstname { get; set; }
                    public string TeamMemberLastname { get; set; }
                    public string TeamMemberPosition { get; set; }
                    public bool? TeamMemberIsPublished { get; set; }
                    public int? TeamMemberCategoryID { get; set; }
                    public int? TeamMemberSortIndex { get; set; }
                    public string UrlTeamMemberProperties { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
    public class TeamMembersModelBase : ModelBase
    {
        #region Properties        
        public TeamMemberDTO DBItem { get; set; }
        #endregion
    }

    public class TeamMemberPropertiesModel : TeamMembersModelBase
    {
        #region Methods

        public void ValidatePageViewModel(PageViewModel viewModel)
        {
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.TeamMemberFirstname)), valueToValidate: viewModel.TeamMemberFirstname));
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.TeamMemberLastname)), valueToValidate: viewModel.TeamMemberLastname));
        }

        public async Task<PageViewModel> GetTeamMembersPropertiesViewModel(PageViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new PageViewModel();
                viewModel.TeamMemberFirstname = DBItem.TeamMemberFirstname;
                viewModel.TeamMemberLastname = DBItem.TeamMemberLastname;
                viewModel.TeamMemberPosition = DBItem.TeamMemberPosition;
                viewModel.TeamMemberShortDescription = DBItem.TeamMemberShortDescription;
                viewModel.TeamMemberLongDescription = DBItem.TeamMemberLongDescription;
                viewModel.TeamMemberIsPublished = DBItem.TeamMemberIsPublished;
                viewModel.TeamMemberCategoryID = DBItem.TeamMemberCategoryID;
            }

            var repository = RepositoriesFactory.GetDictionariesRepository();
            viewModel.TeamMemberCategories = await repository.DictionariesListAsKeyValueSelectedTuple(dictionaryCode: Enums.DictionaryCodes.TeamMemberCategories, selectedValue: viewModel.TeamMemberCategoryID);
            viewModel.TeamMemberImageFilename = DBItem.TeamMemberImageFilename;
            viewModel.TeamMemberImageHttpPath = FileStorage.GetUploadedFileHttpPath(DBItem.TeamMemberImageFilename);
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMember.PropertiesDeleteImage, new { teamMemberID = DBItem.TeamMemberID });
            return viewModel;
        }

        public async Task SaveTeamMemberProperties(PageViewModel viewModel)
        {
            var hasTeamMemberImage = viewModel.TeamMemberImageFile?.Length > 0;
            var teamMemberImageFilename = hasTeamMemberImage ? GetFilenameFromUploadedFile(viewModel.TeamMemberImageFile) : null;
            if (hasTeamMemberImage)
            {
                await DeleteUploadedFile(DBItem.TeamMemberImageFilename, folderPath: null);
            }

            var repository = RepositoriesFactory.GetTeamMembersRepository();
            await repository.TeamMembersIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                teamMemberID: DBItem.TeamMemberID,
                teamMember: new TeamMemberIudDTO
                {
                    TeamMemberFirstname = viewModel.TeamMemberFirstname,
                    TeamMemberLastname = viewModel.TeamMemberLastname,
                    TeamMemberPosition = viewModel.TeamMemberPosition ?? NullValueFor.String,
                    TeamMemberShortDescription = viewModel.TeamMemberShortDescription ?? NullValueFor.String,
                    TeamMemberLongDescription = viewModel.TeamMemberLongDescription ?? NullValueFor.String,
                    TeamMemberImageFilename = teamMemberImageFilename,
                    TeamMemberIsPublished = viewModel.TeamMemberIsPublished,
                    TeamMemberCategoryID = viewModel.TeamMemberCategoryID
                }
            );

            if (!repository.IsError)
            {
                viewModel.IsSaved = true;
                if (hasTeamMemberImage)
                {
                    await SaveUploadedFile(viewModel.TeamMemberImageFile, teamMemberImageFilename, folderPath: null);
                }
            }
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();
            await DeleteUploadedFile(DBItem.TeamMemberImageFilename, folderPath: null);
            var repository = RepositoriesFactory.GetTeamMembersRepository();
            await repository.TeamMembersIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                teamMemberID: DBItem.TeamMemberID,
                teamMember: new TeamMemberIudDTO
                {
                    TeamMemberImageFilename = NullValueFor.String
                }
            );
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }

        #endregion

        #region Nested Classes
        public class PageViewModel : FormViewModelBase
        {
            #region Properties             
            public string TeamMemberFirstname { get; set; }
            public string TeamMemberLastname { get; set; }
            public string TeamMemberPosition { get; set; }
            public string TeamMemberShortDescription { get; set; }
            public string TeamMemberLongDescription { get; set; }
            public string TeamMemberImageFilename { get; set; }
            public bool TeamMemberIsPublished { get; set; }
            public int? TeamMemberCategoryID { get; set; }
            public bool HasTeamMemberCategories => TeamMemberCategories?.Count > 0;
            public List<KeyValueSelectedTuple<int?, string>> TeamMemberCategories { get; set; }
            public string TeamMemberImageHttpPath { get; set; }
            public bool HasTeamMemberImage => !string.IsNullOrWhiteSpace(TeamMemberImageFilename);
            public string UrlDeleteImage { get; set; }
            public IFormFile TeamMemberImageFile { get; set; }

            public readonly int NullValueForInt = NullValueFor.Numeric;

            public readonly string TextPublished = Resources.TextPublished;
            public readonly string TextFirstname = Resources.TextFirstname;
            public readonly string TextLastname = Resources.TextLastname;
            public readonly string TextPosition = Resources.TextPosition;
            public readonly string TextCategory = Resources.TextCategory;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            public readonly string TextDescriptionShort = Resources.TextDescriptionShort;
            public readonly string TextDescription = Resources.TextDescription;
            #endregion
        }
        #endregion
    }
}
