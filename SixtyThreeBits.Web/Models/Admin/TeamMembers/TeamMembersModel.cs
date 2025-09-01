using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
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

        public async Task<AjaxResponse> GetGridItems()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateTeamMembersRepository();

            var teamMembers = await repository.TeamMembersList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : teamMembers.Select(item => new ViewModel.GridModel.GridItem
            {
                TeamMemberID = item.TeamMemberID,
                TeamMemberFirstname = item.TeamMemberFirstname,
                TeamMemberLastname = item.TeamMemberLastname,
                TeamMemberPosition = item.TeamMemberPosition,
                TeamMemberIsPublished = item.TeamMemberIsPublished,
                TeamMemberCategoryID = item.TeamMemberCategoryID,
                TeamMemberSortIndex = item.TeamMemberSortIndex,
                UrlTeamMemberProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMemberPropertiesController.Properties, new { teamMemberID = item.TeamMemberID })
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
}
