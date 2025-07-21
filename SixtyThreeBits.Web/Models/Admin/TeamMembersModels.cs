using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class TeamMembersModel : ModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();

            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.TeamMembersController.GridAdd);
            viewModel.Grid = new ViewModel.GridModel();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembersController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembersController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembersController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembersController.GridDelete);
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.TeamMembersController.GridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.TeamMembersController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.TeamMembersController.GridDelete);

            var repository = RepositoriesFactory.CreateDictionariesRepository();
            viewModel.Grid.TeamMemberCategories = await repository.DictionariesListAsKeyValueTuple(Enums.DictionaryCodes.TeamMemberCategories);
            viewModel.UrlSync = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembersController.GridSort);

            return viewModel;
        }

        public async Task<List<ViewModel.GridModel.GridItem>> GetGridModel()
        {
            var repository = RepositoriesFactory.CreateTeamMembersRepository();
            var viewModel = (await repository.TeamMembersList())?
            .Select(item => new ViewModel.GridModel.GridItem
            {
                TeamMemberID = item.TeamMemberID,
                TeamMemberFirstname = item.TeamMemberFirstname,
                TeamMemberLastname = item.TeamMemberLastname,
                TeamMemberPosition = item.TeamMemberPosition,
                TeamMemberIsPublished = item.TeamMemberIsPublished,
                TeamMemberCategoryID = item.TeamMemberCategoryID,
                TeamMemberSortIndex = item.TeamMemberSortIndex,
                UrlTeamMemberProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembersPropertiesController.Properties, new { teamMemberID = item.TeamMemberID })
            })
            .OrderBy(item => item.TeamMemberSortIndex)
            .ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> IUD(Enums.DatabaseActions databaseAction, int? teamMemberID, ViewModel.GridModel.GridItem submitModel)
        {
            var viewModel = new AjaxResponse();

            await iudProcessTeamMemberImageFilename(databaseAction:databaseAction, teamMemberID: teamMemberID);

            var repository = RepositoriesFactory.CreateTeamMembersRepository();
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
            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;
            
            return viewModel;
        }
        async Task iudProcessTeamMemberImageFilename(Enums.DatabaseActions databaseAction, int? teamMemberID)
        {
            if (databaseAction == Enums.DatabaseActions.DELETE)
            {
                var repository = RepositoriesFactory.CreateTeamMembersRepository();
                var teamMember = await repository.TeamMembersGetSingleByID(teamMemberID);
                await FileStorage.DeleteFile(teamMember.TeamMemberImageFilename);
            }
        }

        public async Task<AjaxResponse> Sort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateTeamMembersRepository();
            await repository.TeamMembersSyncSortIndexes(submitModel.SortIndexes);
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            public string UrlSync { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase<GridModel.GridItem>
            {
                #region Properties
                public List<KeyValueTuple<int?, string>> TeamMemberCategories { get; set; }
                #endregion

                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var grid = CreateGridWithStartupValues(html: Html, keyFieldName: nameof(GridItem.TeamMemberID));

                    grid
                    .ID("TeamMembersGrid")
                    .OnInitialized("model.onGridInit")
                   .Sorting(sorting => sorting.Mode(GridSortingMode.None))
                   .Pager(options =>
                   {
                       options.Visible(false);
                   })
                   .RowDragging(options => options
                        .AllowReordering(true)
                        .OnReorder("model.onGridReorder")
                        .DropFeedbackMode(DropFeedbackMode.Push)
                        .ShowDragIcons(true)
                    )
                   .Paging(options =>
                   {
                       options.Enabled(false);
                   })                   
                   .FilterRow(options =>
                   {
                       options.Visible(false);
                   })                   
                   .Columns(Columns =>
                   {
                       Columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlTeamMemberProperties));
                       Columns.AddFor(m => m.TeamMemberFirstname).Caption(Resources.TextFirstname).Width(150).ValidationRules(options =>
                       {
                           options.AddRequired();
                       });
                       Columns.AddFor(m => m.TeamMemberLastname).Caption(Resources.TextLastname).Width(150).ValidationRules(options =>
                       {
                           options.AddRequired();
                       });
                       Columns.AddFor(m => m.TeamMemberPosition).Caption(Resources.TextPosition).Width(150);
                       Columns.AddFor(m => m.TeamMemberCategoryID).Caption(Resources.TextCategory).Width(250).InitLookupColumn(data: TeamMemberCategories, allowNull: true);
                       Columns.AddFor(m => m.TeamMemberIsPublished).Caption(Resources.TextPublished).DataType(GridColumnDataType.Boolean).Width(130).InitCheckboxColumn();
                       Columns.Add();
                   });

                    return grid;

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
        public async Task<ViewModel> GetViewModel(ViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
                viewModel.TeamMemberFirstname = DBItem.TeamMemberFirstname;
                viewModel.TeamMemberLastname = DBItem.TeamMemberLastname;
                viewModel.TeamMemberPosition = DBItem.TeamMemberPosition;
                viewModel.TeamMemberShortDescription = DBItem.TeamMemberShortDescription;
                viewModel.TeamMemberLongDescription = DBItem.TeamMemberLongDescription;
                viewModel.TeamMemberIsPublished = DBItem.TeamMemberIsPublished;
                viewModel.TeamMemberCategoryID = DBItem.TeamMemberCategoryID;
            }

            var repository = RepositoriesFactory.CreateDictionariesRepository();
            viewModel.TeamMemberCategories = await repository.DictionariesListAsKeyValueSelectedTuple(dictionaryCode: Enums.DictionaryCodes.TeamMemberCategories, selectedValue: viewModel.TeamMemberCategoryID);
            viewModel.TeamMemberImageFilename = DBItem.TeamMemberImageFilename;
            viewModel.TeamMemberImageHttpPath = FileStorage.GetUploadedFileHttpPath(DBItem.TeamMemberImageFilename);
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembersPropertiesController.DeleteImage, new { teamMemberID = DBItem.TeamMemberID });
            return viewModel;
        }

        public void Validate(ViewModel viewModel)
        {
            viewModel.AddError(Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.TeamMemberFirstname)), valueToValidate: viewModel.TeamMemberFirstname));
            viewModel.AddError(Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.TeamMemberLastname)), valueToValidate: viewModel.TeamMemberLastname));
        }

        public async Task Save(ViewModel viewModel)
        {
            var hasTeamMemberImage = viewModel.TeamMemberImageFile?.Length > 0;
            var teamMemberImageFilename = hasTeamMemberImage ? GetFilenameFromUploadedFile(viewModel.TeamMemberImageFile) : null;
            if (hasTeamMemberImage)
            {
                await FileStorage.DeleteFile(DBItem.TeamMemberImageFilename);
            }

            var repository = RepositoriesFactory.CreateTeamMembersRepository();
            await repository.TeamMembersIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                teamMemberID: DBItem.TeamMemberID,
                teamMember: new TeamMemberIudDTO
                {
                    TeamMemberFirstname = viewModel.TeamMemberFirstname,
                    TeamMemberLastname = viewModel.TeamMemberLastname,
                    TeamMemberPosition = viewModel.TeamMemberPosition ?? Constants.NullValueFor.String,
                    TeamMemberShortDescription = viewModel.TeamMemberShortDescription ?? Constants.NullValueFor.String,
                    TeamMemberLongDescription = viewModel.TeamMemberLongDescription ?? Constants.NullValueFor.String,
                    TeamMemberImageFilename = teamMemberImageFilename,
                    TeamMemberIsPublished = viewModel.TeamMemberIsPublished,
                    TeamMemberCategoryID = viewModel.TeamMemberCategoryID
                }
            );

            if (repository.IsError)
            {
                viewModel.AddError(repository.ErrorMessage);
            }
            else
            {
                if (hasTeamMemberImage)
                {
                    await SaveUploadedFile(viewModel.TeamMemberImageFile, teamMemberImageFilename);
                }
            }
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();
            await FileStorage.DeleteFile(DBItem.TeamMemberImageFilename);
            var repository = RepositoriesFactory.CreateTeamMembersRepository();
            await repository.TeamMembersIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                teamMemberID: DBItem.TeamMemberID,
                teamMember: new TeamMemberIudDTO
                {
                    TeamMemberImageFilename = Constants.NullValueFor.String
                }
            );
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel : FormViewModelBase
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

            public readonly int NullValueForInt = Constants.NullValueFor.Numeric;

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
