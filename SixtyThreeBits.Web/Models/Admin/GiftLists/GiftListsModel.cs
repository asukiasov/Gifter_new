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
    public class GiftListsModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();

            viewModel.Grid = new ViewModel.GridModel();
            viewModel.Grid.AllowAdd = false;
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.GiftListsController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.GiftListsController.GridDelete);
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.GiftListsController.Grid);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.GiftListsController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.GiftListsController.GridDelete);

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
                GiftListTitle = item.GiftListTitle,
                OwnerFullname = item.OwnerFullname,
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

            await repository.GiftListsIUD(
                databaseAction: databaseAction,
                giftListID: giftListID,
                giftList: new GiftListIudDTO
                {
                    GiftListTitle = submitModel.GiftListTitle,
                    GiftListIsSecret = submitModel.GiftListIsSecret,
                    GiftListIsPublished = submitModel.GiftListIsPublished,
                    GiftListEndDate = submitModel.GiftListEndDate
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
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.GiftListID));

                    grid
                    .ID("GiftListsGrid")
                    .Columns(columns =>
                    {
                        columns.AddFor(m => m.GiftListID).Caption("#").Width(60);
                        columns.AddFor(m => m.GiftListTitle).Caption("Title").Width(300).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.OwnerFullname).Caption("Owner").Width(200).AllowEditing(false);
                        columns.AddFor(m => m.GiftListIsSecret).Caption("Secret").Width(80).InitCheckboxColumn();
                        columns.AddFor(m => m.GiftListIsPublished).Caption("Published").Width(90).InitCheckboxColumn();
                        columns.AddFor(m => m.GiftListEndDate).Caption("End Date").Width(150).InitDateColumn(format: DevExtremeExtensions63.DateColumnFormat.Date);
                        columns.AddFor(m => m.GiftListDateCreated).Caption("Created").Width(150).InitDateColumn(format: DevExtremeExtensions63.DateColumnFormat.DateTime).AllowEditing(false);
                        columns.Add();
                    });

                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? GiftListID { get; set; }
                    public string GiftListTitle { get; set; }
                    public string OwnerFullname { get; set; }
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
