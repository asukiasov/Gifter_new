using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries.DevExtreme;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class GiftListsModel : ModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();

            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.GiftListsController.GridAdd);
            viewModel.Grid = new ViewModel.GridModel();

            var usersRepo = RepositoriesFactory.CreateUsersRepository();
            viewModel.Grid.Users = (await usersRepo.UsersList())
                ?.OrderBy(item => item.UserFullname)
                .Select(item => new KeyValueTuple<int?, string>
                {
                    Key = item.UserID,
                    Value = item.UserFullname
                })
                .ToList();

            viewModel.Grid.AllowAdd = viewModel.ShowAddNewButton;
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.GiftListsController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.GiftListsController.GridDelete);
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.GiftListsController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.GiftListsController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.GiftListsController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.GiftListsController.GridDelete);
            viewModel.Grid.UrlLoadFollowers = Url.RouteUrl(ControllerActionRouteNames.Admin.GiftListsController.Followers, new { followingUserID = 0 }).TrimEnd('0');

            return viewModel;
        }

        public async Task<AjaxResponse> GetGridItems()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateGiftListsRepository();

            var giftLists = await repository.GiftListsList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : giftLists.Select(item => new ViewModel.GridModel.GridItem
            {
                GiftListID = item.GiftListID,
                GiftListUserID = item.GiftListUserID,
                GiftListTitle = item.GiftListTitle,
                GiftListIsSecret = item.GiftListIsSecret,
                GiftListIsPublished = item.GiftListIsPublished,
                GiftListEndDate = item.GiftListEndDate,
                GiftListDateCreated = item.GiftListDateCreated
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> IUD(Enums.DatabaseActions databaseAction, int? giftListID, ViewModel.GridModel.GridItem submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateGiftListsRepository();

            giftListID = await repository.GiftListsIUD(
                databaseAction: databaseAction,
                giftListID: giftListID,
                giftList: new GiftListIudDTO
                {
                    GiftListUserID = submitModel.GiftListUserID,
                    GiftListTitle = submitModel.GiftListTitle,
                    GiftListIsSecret = submitModel.GiftListIsSecret,
                    GiftListIsPublished = submitModel.GiftListIsPublished,
                    GiftListEndDate = submitModel.GiftListEndDate
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            if (!repository.IsError && databaseAction == Enums.DatabaseActions.INSERT && User?.UserID != null)
            {
                var ordersRepository = RepositoriesFactory.CreateOrdersRepository();
                await ordersRepository.OrdersInsert(
                    orderUserID: User.UserID.Value,
                    orderType: 1,
                    orderGiftListID: giftListID
                );

                // TODO: Remove this diagnostic block after debugging
                if (ordersRepository.IsError)
                {
                    viewModel.IsSuccess = false;
                    viewModel.Data = $"GiftList created OK (ID={giftListID}), but OrdersInsert failed: {ordersRepository.ErrorMessage}";
                }
            }

            return viewModel;
        }

        public async Task<AjaxResponse> GetFollowers(int followedUserID)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateFollowersRepository();

            var followers = await repository.FollowersMyFollowers(followedUserID);

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : followers;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridModelBase<GridModel.GridItem>
            {
                #region Properties
                public List<KeyValueTuple<int?, string>> Users { get; set; }
                public string UrlLoadFollowers { get; set; }
                #endregion

                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.GiftListID));

                    grid
                    .ID("GiftListsGrid")
                    .Columns(columns =>
                    {
                        columns.AddFor(m => m.GiftListID).Caption("#").Width(60).AllowEditing(false).FormItem(fi => fi.Visible(false));
                        columns.AddFor(m => m.GiftListTitle).Caption("Title").Width(300).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.GiftListUserID).Caption("Owner").Width(200).InitLookupColumn(data: Users, isRequired: true);
                        columns.AddFor(m => m.GiftListIsSecret).Caption("Secret").Width(80).InitCheckboxColumn();
                        columns.AddFor(m => m.GiftListIsPublished).Caption("Published").Width(90).InitCheckboxColumn();
                        columns.AddFor(m => m.GiftListEndDate).Caption("End Date").Width(150).InitDateColumn(format: DevExtremeExtensions63.DateColumnFormat.Date);
                        columns.AddFor(m => m.GiftListDateCreated).Caption("Created").Width(150).InitDateColumn(format: DevExtremeExtensions63.DateColumnFormat.DateTime).AllowEditing(false);
                        columns.Add();
                    })
                    .OnInitialized("model.onGridInit")
                    .OnRowUpdating("model.onGridRowUpdating")
                    .MasterDetail(md =>
                    {
                        md.Enabled(true);
                        md.Template(new JS("model.renderFollowersDetail"));
                    });

                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? GiftListID { get; set; }
                    public int? GiftListUserID { get; set; }
                    public string GiftListTitle { get; set; }
                    public bool GiftListIsSecret { get; set; }
                    public bool GiftListIsPublished { get; set; }
                    public DateTime? GiftListEndDate { get; set; }
                    public DateTime? GiftListDateCreated { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
