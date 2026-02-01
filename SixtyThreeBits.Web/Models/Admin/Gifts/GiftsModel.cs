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
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class GiftsModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();

            viewModel.Grid = new ViewModel.GridModel();
            viewModel.Grid.AllowAdd = false;
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.GiftsController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.GiftsController.GridDelete);
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.GiftsController.Grid);
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
                GiftTitle = item.GiftTitle,
                WishlistTitle = item.WishlistTitle,
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

            await repository.GiftsIUD(
                databaseAction: databaseAction,
                giftID: giftID,
                gift: new GiftIudDTO
                {
                    GiftTitle = submitModel.GiftTitle,
                    GiftPrice = submitModel.GiftPrice,
                    GiftCurrency = submitModel.GiftCurrency,
                    GiftUrl = submitModel.GiftUrl
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridModelBase<GridModel.GridItem>
            {
                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.GiftID));

                    grid
                    .ID("GiftsGrid")
                    .Columns(columns =>
                    {
                        columns.AddFor(m => m.GiftID).Caption("#").Width(60);
                        columns.AddFor(m => m.GiftTitle).Caption("Title").Width(250).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.WishlistTitle).Caption("Wishlist").Width(200).AllowEditing(false);
                        columns.AddFor(m => m.OwnerFullname).Caption("Owner").Width(150).AllowEditing(false);
                        columns.AddFor(m => m.GiftPrice).Caption("Price").Width(100);
                        columns.AddFor(m => m.GiftCurrency).Caption("Currency").Width(80);
                        columns.AddFor(m => m.GiftUrl).Caption("URL").Width(200);
                        columns.AddFor(m => m.GiftIsReserved).Caption("Reserved").Width(80).InitCheckboxColumn().AllowEditing(false);
                        columns.AddFor(m => m.GiftDateCreated).Caption("Created").Width(150).InitDateColumn(format: DevExtremeExtensions63.DateColumnFormat.DateTime).AllowEditing(false);
                        columns.Add();
                    });

                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? GiftID { get; set; }
                    public string GiftTitle { get; set; }
                    public string WishlistTitle { get; set; }
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
