using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.BusinessLogics;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class ProductsModel : ModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.GridAdd);
            viewModel.ShowExcelUploadButton = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.ExcelUpload) && User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.ExcelDownload);
            viewModel.UrlExcelUpload = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.ExcelUpload);
            viewModel.UrlExcelDownload = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.ExcelDownload);

            viewModel.Grid = new ViewModel.GridModel();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.GridDelete);
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.GridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.GridDelete);

            var repository = RepositoriesFactory.CreateProductsRepository();
            viewModel.Grid.Categories = (await repository.ProductCategoriesListWithTitlePaddindHierarchy(padChar: '-'))?
            .Select(item => new KeyValueTuple<int?, string>
            {
                Key = item.ProductCategoryID,
                Value = item.ProductCategoryName
            }).ToList();

            return viewModel;
        }

        public async Task<List<ViewModel.GridModel.GridItem>> GetGridModel()
        {
            var repository = RepositoriesFactory.CreateProductsRepository();
            var viewModel = (await repository.ProductsList())?
            .Select(Item => new ViewModel.GridModel.GridItem
            {
                ProductID = Item.ProductID,
                ProductName = Item.ProductName,
                ProductIsPublished = Item.ProductIsPublished,
                ProductPrice = Item.ProductPrice,
                ProductPriceOld = Item.ProductPriceOld,
                ProductRemainder = Item.ProductRemainder,
                ProductIsFeatured = Item.ProductIsFeatured,
                ProductCategoryID = Item.ProductCategoryID,
                UrlProductsProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.Properties, new { productID = Item.ProductID })
            }).ToList();
            return viewModel;
        }

        public async Task<AjaxResponse> InsertOrUpdate(Enums.DatabaseActions databaseAction, int? productID, ViewModel.GridModel.GridItem submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateProductsRepository();

            await repository.ProductsIUD(
                databaseAction: databaseAction,
                productID: productID,
                product: new ProductIudDTO
                {
                    ProductCategoryID = submitModel.ProductCategoryID,
                    ProductName = submitModel.ProductName,
                    ProductIsPublished = submitModel.ProductIsPublished,
                    ProductPrice = submitModel.ProductPrice,
                    ProductPriceOld = submitModel.ProductPriceOld,
                    ProductRemainder = submitModel.ProductRemainder,
                    ProductIsFeatured = submitModel.ProductIsFeatured
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> Delete(int? productID)
        {
            var viewModel = new AjaxResponse();
            var bl = new ProductDeleteBusinessLogic(
                productID: productID,
                repositoryFactory: RepositoriesFactory,
                fileStorage: FileStorage
            );
            var result = await bl.Execute();
            if (result.IsError)
            {
                viewModel.Data = result.ErrorMessage;
            }
            else
            {
                viewModel.IsSuccess = true;
            }
            return viewModel;
        }

        public async Task<byte[]> GetProductsSyncExcelFileBytes()
        {
            var bl = new ProductsBusinessLogic.GetProductsPricesAndRemaindersExcelFile(
                dataAccessFactory: RepositoriesFactory,
                appSettings: AppSettings
            );
            var result = await bl.Execute();
            return result.ExcelFileBytes;
        }

        public async Task<AjaxResponse> SyncExcel(ExcelUploadSubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var bl = new ProductsBusinessLogic.SyncProductPricesAndRemainders(
                excelFileBytes: submitModel.ExcelFileBytes,
                isXslx: submitModel.ExcelFilename?.EndsWith(".xlsx") == true,
                dataAccessFactory: RepositoriesFactory
            );
            var result = await bl.Execute();
            if (result.IsError)
            {
                if (result.HasExcelErrors)
                {
                    viewModel.Data = new
                    {
                        result.HasExcelErrors,
                        result.ExcelErrors
                    };
                }
                else
                {
                    viewModel.Data = result.ErrorMessage;
                }

            }
            else
            {
                viewModel.IsSuccess = true;
            }

            return viewModel;

        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public bool ShowExcelUploadButton { get; set; }
            public string UrlExcelDownload { get; set; }
            public string UrlExcelUpload { get; set; }
            public GridModel Grid { get; set; }
            public readonly string TextRemainderUpload = Resources.TextRemainderUpload;
            public readonly string TextExcelUpload = Resources.TextExcelUpload;
            public readonly string TextExcelDownloadTemplate = Resources.TextExcelDownloadTemplate;
            public readonly string TextExport = Resources.TextExport;
            public readonly string TextUpload = Resources.TextUpload;
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase<GridModel.GridItem>
            {
                #region Properties
                public List<KeyValueTuple<int?, string>> Categories { get; set; }
                #endregion

                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.ProductID));

                    grid
                   .ID("ProductsGrid")
                   .OnInitialized("model.onGridInit")
                   .Columns(columns =>
                   {
                       columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlProductsProperties)).AllowExporting(false);
                       columns.AddFor(m => m.ProductName).Caption(Resources.TextName).Width(350).ValidationRules(options =>
                       {
                           options.AddRequired();
                       });
                       columns.AddFor(m => m.ProductCategoryID).Caption(Resources.TextCategory).Width(250).InitLookupColumn(data: Categories, isRequired: true);
                       columns.AddFor(m => m.ProductPrice).Caption(Resources.TextPrice).Width(100).Alignment(HorizontalAlignment.Right).DataType(GridColumnDataType.Number);
                       columns.AddFor(m => m.ProductPriceOld).Caption(Resources.TextPriceOld).Width(100).Alignment(HorizontalAlignment.Right).DataType(GridColumnDataType.Number);
                       columns.AddFor(m => m.ProductRemainder).Caption(Resources.TextRemainder).Width(120).Alignment(HorizontalAlignment.Right).DataType(GridColumnDataType.Number);
                       columns.AddFor(m => m.ProductIsPublished).Caption(Resources.TextPublished).Width(120).InitCheckboxColumn();
                       columns.AddFor(m => m.ProductIsFeatured).Caption(Resources.TextFeatured).Width(120).InitCheckboxColumn();
                       columns.Add();
                   });

                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? ProductID { get; set; }
                    public string ProductName { get; set; }
                    public bool? ProductIsPublished { get; set; }
                    public decimal? ProductPrice { get; set; }
                    public decimal? ProductPriceOld { get; set; }
                    public decimal? ProductRemainder { get; set; }
                    public bool? ProductIsFeatured { get; set; }
                    public int? ProductCategoryID { get; set; }
                    public string UrlProductsProperties { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }

        public class ExcelUploadSubmitModel
        {
            #region Properties
            public byte[] ExcelFileBytes { get; set; }
            public string ExcelFilename { get; set; }
            #endregion
        }
        #endregion
    }
}