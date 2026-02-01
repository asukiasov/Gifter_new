using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries.DevExtreme;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class OrdersModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();

            viewModel.Grid = new ViewModel.GridModel();
            viewModel.Grid.AllowAdd = false;
            viewModel.Grid.AllowUpdate = false;
            viewModel.Grid.AllowDelete = false;
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.OrdersController.Grid);

            return viewModel;
        }

        public async Task<AjaxResponse> GetGridItems()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateOrdersRepository();

            var orders = await repository.OrdersList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : orders.Select(item => new ViewModel.GridModel.GridItem
            {
                OrderID = item.OrderID,
                OrderDateCreated = item.OrderDateCreated,
                OrderUserFullname = item.OrderUserFullname,
                OrderTypeName = item.OrderTypeName,
                OrderGiftListTitle = item.OrderGiftListTitle,
                OrderGiftTitle = item.OrderGiftTitle,
                OrderTargetUserFullname = item.OrderTargetUserFullname
            }).ToList();

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
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.OrderID));
                    grid
                    .ID("OrdersGrid")
                    .Columns(columns =>
                    {
                        columns.AddFor(m => m.OrderID).Caption("#").Width(60);
                        columns.AddFor(m => m.OrderDateCreated).Caption("Date").Width(150).InitDateColumn(format: DevExtremeExtensions63.DateColumnFormat.DateTime);
                        columns.AddFor(m => m.OrderUserFullname).Caption("User").Width(200);
                        columns.AddFor(m => m.OrderTypeName).Caption("Activity").Width(180);
                        columns.AddFor(m => m.OrderGiftListTitle).Caption("Wishlist").Width(200);
                        columns.AddFor(m => m.OrderGiftTitle).Caption("Gift").Width(200);
                        columns.AddFor(m => m.OrderTargetUserFullname).Caption("Target User").Width(200);
                        columns.Add();
                    });

                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? OrderID { get; set; }
                    public DateTime? OrderDateCreated { get; set; }
                    public string OrderUserFullname { get; set; }
                    public string OrderTypeName { get; set; }
                    public string OrderGiftListTitle { get; set; }
                    public string OrderGiftTitle { get; set; }
                    public string OrderTargetUserFullname { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
