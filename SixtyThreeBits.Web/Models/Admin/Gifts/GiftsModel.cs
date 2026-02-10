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
    public class GiftsModel : ModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();

            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.GiftsController.GridAdd);
            viewModel.Grid = new ViewModel.GridModel();

            var giftListsRepo = RepositoriesFactory.CreateGiftListsRepository();
            viewModel.Grid.GiftLists = (await giftListsRepo.GiftListsList())
                ?.OrderBy(item => item.GiftListTitle)
                .Select(item => new KeyValueTuple<int?, string>
                {
                    Key = item.GiftListID,
                    Value = $"{item.GiftListTitle} ({item.OwnerFullname})"
                })
                .ToList();

            var dictionariesRepo = RepositoriesFactory.CreateDictionariesRepository();
            viewModel.Grid.Currencies = (await dictionariesRepo.DictionariesListByLevelCodeIsVisible(dictionaryLevel: 1, dictionaryCode: Enums.DictionaryCodes.Currencies))
                ?.Select(item => new KeyValueTuple<string, string>
                {
                    Key = item.DictionaryStringCode,
                    Value = item.DictionaryCaption
                })
                .ToList();

            viewModel.Grid.AllowAdd = viewModel.ShowAddNewButton;
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.GiftsController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.GiftsController.GridDelete);
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.GiftsController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.GiftsController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.GiftsController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.GiftsController.GridDelete);

            return viewModel;
        }

        public async Task<AjaxResponse> GetGridItems()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateGiftsRepository();

            var gifts = await repository.GiftsList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : gifts.Select(item => new ViewModel.GridModel.GridItem
            {
                GiftID = item.GiftID,
                GiftGiftListID = item.GiftGiftListID,
                GiftTitle = item.GiftTitle,
                OwnerFullname = item.OwnerFullname,
                GiftPrice = item.GiftPrice,
                GiftCurrency = item.GiftCurrency,
                GiftUrl = item.GiftUrl,
                GiftIsReserved = item.GiftIsReserved,
                GiftDateCreated = item.GiftDateCreated
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> IUD(Enums.DatabaseActions databaseAction, int? giftID, ViewModel.GridModel.GridItem submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateGiftsRepository();

            giftID = await repository.GiftsIUD(
                databaseAction: databaseAction,
                giftID: giftID,
                gift: new GiftIudDTO
                {
                    GiftGiftListID = submitModel.GiftGiftListID,
                    GiftTitle = submitModel.GiftTitle,
                    GiftPrice = submitModel.GiftPrice,
                    GiftCurrency = submitModel.GiftCurrency,
                    GiftUrl = submitModel.GiftUrl
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            if (!repository.IsError && databaseAction == Enums.DatabaseActions.INSERT && User?.UserID != null)
            {
                var ordersRepository = RepositoriesFactory.CreateOrdersRepository();
                await ordersRepository.OrdersInsert(
                    orderUserID: User.UserID.Value,
                    orderType: 2,
                    orderGiftListID: submitModel.GiftGiftListID,
                    orderGiftID: giftID
                );
            }

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
                public List<KeyValueTuple<int?, string>> GiftLists { get; set; }
                public List<KeyValueTuple<string, string>> Currencies { get; set; }
                #endregion

                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.GiftID));

                    grid
                    .ID("GiftsGrid")
                    .Columns(columns =>
                    {
                        columns.AddFor(m => m.GiftID).Caption("#").Width(60).AllowEditing(false).FormItem(fi => fi.Visible(false));
                        columns.AddFor(m => m.GiftTitle).Caption("Title").Width(250).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.GiftGiftListID).Caption("Wishlist").Width(200).InitLookupColumn(data: GiftLists, isRequired: true);
                        columns.AddFor(m => m.OwnerFullname).Caption("Owner").Width(150).AllowEditing(false);
                        columns.AddFor(m => m.GiftPrice).Caption("Price").Width(100);
                        columns.AddFor(m => m.GiftCurrency).Caption("Currency").Width(80).InitLookupColumn(data: Currencies);
                        columns.AddFor(m => m.GiftUrl).Caption("URL").Width(200);
                        columns.AddFor(m => m.GiftIsReserved).Caption("Reserved").Width(80).InitCheckboxColumn().AllowEditing(false);
                        columns.AddFor(m => m.GiftDateCreated).Caption("Created").Width(150).InitDateColumn(format: DevExtremeExtensions63.DateColumnFormat.DateTime).AllowEditing(false);
                        columns.Add();
                    })
                    .OnInitialized("model.onGridInit")
                    .OnRowUpdating("model.onGridRowUpdating");

                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? GiftID { get; set; }
                    public int? GiftGiftListID { get; set; }
                    public string GiftTitle { get; set; }
                    public string OwnerFullname { get; set; }
                    public decimal? GiftPrice { get; set; }
                    public string GiftCurrency { get; set; }
                    public string GiftUrl { get; set; }
                    public bool GiftIsReserved { get; set; }
                    public DateTime? GiftDateCreated { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
