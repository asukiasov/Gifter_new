using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.BusinessLogics;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Libraries.FileStorages;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
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
    public class ProductsModel : ModelBase
    {
        #region Properties
        readonly string _folderPath = FileStorageManager.Modules[Enums.FileManagerModules.Products].FolderName;
        #endregion

        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.GridAdd);
            viewModel.ShowExcelUploadButton = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.ExcelUpload) && User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.ExcelDownload);
            viewModel.UrlExcelUpload = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.ExcelUpload);
            viewModel.UrlExcelDownload = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.ExcelDownload);

            viewModel.Grid = new ViewModel.GridViewModel();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.GridDelete);
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.GridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.GridDelete);

            var repository = RepositoriesFactory.CreateProductsRepository();
            viewModel.Grid.Categories = (await repository.ProductCategoriesListWithTitlePaddindHierarchy(padChar: '-'))
            ?.Select(item => new KeyValueTuple<int?, string>
            {
                Key = item.ProductCategoryID,
                Value = item.ProductCategoryName
            })
            .ToList();

            return viewModel;
        }

        public async Task<List<ViewModel.GridViewModel.GridItem>> ListGridItems()
        {
            var repository = RepositoriesFactory.CreateProductsRepository();
            var viewModel = (await repository.ProductsList())
            ?.Select(Item => new ViewModel.GridViewModel.GridItem
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
            })
            .ToList();
            return viewModel;
        }

        public async Task IUD(Enums.DatabaseActions databaseAction, int? productID, ViewModel.GridViewModel.GridItem submitModel)
        {
            var repository = RepositoriesFactory.CreateProductsRepository();

            if (databaseAction == Enums.DatabaseActions.DELETE)
            {
                var DBItem = await repository.ProductsGetSingleByID(productID);
                if (DBItem?.ProductImages?.Any() == true)
                {
                    foreach (var Item in DBItem.ProductImages)
                    {
                        await DeleteUploadedFile(Item.ProductImageFilename, _folderPath);
                    }
                }
            }

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

            if (repository.IsError)
            {
                Form.AddError(repository.ErrorMessage);
            }
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
            public GridViewModel Grid { get; set; }
            public readonly string TextRemainderUpload = Resources.TextRemainderUpload;
            public readonly string TextExcelUpload = Resources.TextExcelUpload;
            public readonly string TextExcelDownloadTemplate = Resources.TextExcelDownloadTemplate;
            public readonly string TextExport = Resources.TextExport;
            public readonly string TextUpload = Resources.TextUpload;
            #endregion

            #region Nested Classes
            public class GridViewModel : DevExtremeGridViewModelBase<GridViewModel.GridItem>
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

    public class ProductsModelBase : ModelBase
    {
        #region Properties        
        public ProductDTO DBItem { get; set; }
        #endregion
    }

    public class ProductPropertiesModel : ProductsModelBase
    {
        #region Properties
        readonly string _folderPath = FileStorageManager.Modules[Enums.FileManagerModules.Products].FolderName;
        #endregion

        #region Methods        
        public async Task<ViewModel> GetViewModel(ViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
                viewModel.ProductIsPublished = DBItem.ProductIsPublished;
                viewModel.ProductIsFeatured = DBItem.ProductIsFeatured;
                viewModel.BrandID = DBItem.BrandID;
                viewModel.ProductCategoryID = DBItem.ProductCategoryID;
                viewModel.ProductName = DBItem.ProductName;
                viewModel.ProductNameEng = DBItem.ProductNameEng;
                viewModel.ProductPrice = Utilities.FormatPriceValue(DBItem.ProductPrice);
                viewModel.ProductPriceOld = Utilities.FormatPriceValue(DBItem.ProductPriceOld);
                viewModel.ProductRemainder = Utilities.FormatQuantityValue(DBItem.ProductRemainder);
                viewModel.ProductSKU = DBItem.ProductSKU;
                viewModel.ProductDescriptionShort = DBItem.ProductDescriptionShort;
                viewModel.ProductDescriptionShortEng = DBItem.ProductDescriptionShortEng;
                viewModel.ProductDescription = DBItem.ProductDescription;
                viewModel.ProductDescriptionEng = DBItem.ProductDescriptionEng;
            }
            viewModel.ProductImageFilename = DBItem.ProductImageFilename;
            viewModel.ProductImageHttpPath = FileStorage.GetUploadedFileHttpPath(DBItem.ProductImageFilename, _folderPath);

            var repositoryBrands = RepositoriesFactory.CreateBrandsRepository();
            viewModel.Brands = (await repositoryBrands.BrandsList())
            ?.Select(item => new KeyValueSelectedTuple<int?, string>
            {
                Key = item.BrandID,
                Value = item.BrandName,
                IsSelected = item.BrandID == viewModel.BrandID
            })
            .ToList();

            var repositoryProducts = RepositoriesFactory.CreateProductsRepository();
            viewModel.Categories = (await repositoryProducts.ProductCategoriesListWithTitlePaddindHierarchy('-'))
            ?.Select(item => new KeyValueSelectedTuple<int?, string>
            {
                Key = item.ProductCategoryID,
                Value = item.ProductCategoryName,
                IsSelected = item.ProductCategoryID == viewModel.ProductCategoryID
            })
            .ToList();

            var repositoryCountries = RepositoriesFactory.CreateCountriesRepository();
            viewModel.ProductProducerCountries = await repositoryCountries.CountriesListAsSimpleKeyValue(SelectedCountryID: DBItem.CountryIDProducer);

            viewModel.ProductImages = DBItem.ProductImages?.Select(item => new ViewModel.ProductImage
            {
                ProductImageID = item.ProductImageID,
                ProductImageFilename = item.ProductImageFilename,
                ProductImageFileHttpPath = FileStorage.GetUploadedFileHttpPath(item.ProductImageFilename, _folderPath),
                ProductImageAltText = item.ProductImageAltText
            })
            .ToList();

            viewModel.UrlImageUpload = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.ProductImagesUpload, new { productID = DBItem.ProductID });
            viewModel.UrlImageUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.ProductImagesUpdate, new { productID = DBItem.ProductID });
            viewModel.UrlImageSort = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.ProductImagesSort, new { productID = DBItem.ProductID });
            viewModel.UrlImageDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.ProductImagesDelete, new { productID = DBItem.ProductID });

            return viewModel;
        }

        public void Validate(ViewModel viewModel)
        {
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.ProductName)), valueToValidate: viewModel.ProductName));
        }

        public async Task Save(ViewModel viewModel)
        {
            var repository = RepositoriesFactory.CreateProductsRepository();
            await repository.ProductsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                productID: DBItem.ProductID,
                product: new ProductIudDTO
                {
                    ProductCategoryID = viewModel.ProductCategoryID ?? Constants.NullValueFor.Numeric,
                    CountryIDProducer = viewModel.CountryIDProducer ?? Constants.NullValueFor.Numeric,
                    BrandID = viewModel.BrandID ?? Constants.NullValueFor.Numeric,
                    ProductName = viewModel.ProductName,
                    ProductPrice = viewModel.ProductPrice.ToDecimal() ?? Constants.NullValueFor.Numeric,
                    ProductPriceOld = viewModel.ProductPriceOld.ToDecimal() ?? Constants.NullValueFor.Numeric,
                    ProductRemainder = viewModel.ProductRemainder.ToDecimal() ?? Constants.NullValueFor.Numeric,
                    ProductNameEng = viewModel.ProductNameEng ?? Constants.NullValueFor.String,
                    ProductDescriptionShort = viewModel.ProductDescriptionShort ?? Constants.NullValueFor.String,
                    ProductDescriptionShortEng = viewModel.ProductDescriptionShortEng ?? Constants.NullValueFor.String,
                    ProductDescription = viewModel.ProductDescription ?? Constants.NullValueFor.String,
                    ProductDescriptionEng = viewModel.ProductDescriptionEng ?? Constants.NullValueFor.String,
                    ProductIsPublished = viewModel.ProductIsPublished,
                    ProductIsFeatured = viewModel.ProductIsFeatured,
                    ProductSKU = viewModel.ProductSKU ?? Constants.NullValueFor.String
                }
            );

            if (repository.IsError)
            {
                viewModel.AddError(repository.ErrorMessage);
            }
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();

            await DeleteUploadedFile(DBItem.ProductImageFilename, _folderPath);

            var repository = RepositoriesFactory.CreateProductsRepository();
            await repository.ProductsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                productID: DBItem.ProductID,
                product: new ProductIudDTO
                {
                    ProductImageFilename = Constants.NullValueFor.String
                }

            );

            viewModel.IsSuccess = !repository.IsError;

            return viewModel;
        }

        public async Task<AjaxResponse> UploadProductImages()
        {
            var viewModel = new AjaxResponse();

            var postedFile = Request.Form.Files[0];

            if (postedFile.Length > 2097152) //2MB
            {
                viewModel.Data = "Max file size exceeded";
            }
            else
            {
                var productImageFilenameOriginal = postedFile.FileName;
                var productImageFilename = GetFilenameFromUploadedFile(postedFile);

                var repository = RepositoriesFactory.CreateProductsRepository();
                var productImageID = await repository.ProductsImagesIUD(
                    databaseAction: Enums.DatabaseActions.CREATE,
                    productImageID: null,
                    productImage: new ProductImageIudDTO
                    {
                        ProductID = DBItem.ProductID,
                        ProductImageFilename = productImageFilename
                    }
                );

                if (productImageID > 0)
                {
                    await SaveUploadedFile(postedFile: postedFile, filename: productImageFilename, folderPath: _folderPath);

                    DBItem = await repository.ProductsGetSingleByID(DBItem.ProductID);
                    var firstImage = DBItem.ProductImages?.FirstOrDefault();
                    if (firstImage != null)
                    {
                        await repository.ProductsIUD(
                            databaseAction: Enums.DatabaseActions.UPDATE,
                            productID: DBItem.ProductID,
                            product: new ProductIudDTO
                            {
                                ProductImageFilename = firstImage.ProductImageFilename,
                                ProductImageAltText = firstImage.ProductImageAltText
                            }
                        );
                    }

                    viewModel.Data = new ViewModel.ProductImage
                    {
                        ProductImageID = productImageID,
                        ProductImageFilename = productImageFilenameOriginal,
                        ProductImageFileHttpPath = FileStorage.GetUploadedFileHttpPath(productImageFilename, _folderPath)
                    };
                    viewModel.IsSuccess = true;
                }
            }

            return viewModel;
        }

        public async Task<AjaxResponse> UpdateProductImages(UpdateProductImageSubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();

            var productImage = DBItem.ProductImages?.FirstOrDefault(Item => Item.ProductImageID == submitModel.ProductImageID);
            if (productImage != null)
            {
                var repository = RepositoriesFactory.CreateProductsRepository();
                await repository.ProductsImagesIUD(
                    databaseAction: Enums.DatabaseActions.UPDATE,
                    productImageID: submitModel.ProductImageID,
                    productImage: new ProductImageIudDTO
                    {
                        ProductImageAltText = submitModel.ProductImageAltText ?? Constants.NullValueFor.String
                    }
                );

                if (!repository.IsError)
                {
                    var firstImage = DBItem.ProductImages?.FirstOrDefault();
                    if (firstImage != null && firstImage.ProductImageID == submitModel.ProductImageID)
                    {
                        await repository.ProductsIUD(
                            databaseAction: Enums.DatabaseActions.UPDATE,
                            productID: DBItem.ProductID,
                            product: new ProductIudDTO
                            {
                                ProductImageAltText = submitModel.ProductImageAltText ?? Constants.NullValueFor.String
                            }
                        );
                    }
                    viewModel.IsSuccess = true;
                }
            }
            return viewModel;
        }

        public async Task<AjaxResponse> DeleteProductImages(DeleteProductImageSubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();

            var productImage = DBItem.ProductImages?.FirstOrDefault(Item => Item.ProductImageID == submitModel.ProductImageID);
            if (productImage != null)
            {
                await DeleteUploadedFile(productImage.ProductImageFilename, _folderPath);

                var repository = RepositoriesFactory.CreateProductsRepository();
                await repository.ProductsImagesIUD(
                    databaseAction: Enums.DatabaseActions.DELETE,
                    productImageID: submitModel.ProductImageID,
                    productImage: null
                );
                viewModel.IsSuccess = !repository.IsError;
            }
            return viewModel;
        }

        public async Task<AjaxResponse> SortProductImages(SyncSortIndexesSubmitModel SubmitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateProductsRepository();
            await repository.ProductsImagesSyncSortIndex(DBItem.ProductID, SubmitModel.SortIndexes);
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel : FormViewModelBase
        {
            #region Properties
            public int? BrandID { get; set; }
            public int? ProductCategoryID { get; set; }
            public string ProductName { get; set; }
            public string ProductNameEng { get; set; }
            public string ProductPrice { get; set; }
            public string ProductPriceOld { get; set; }
            public string ProductRemainder { get; set; }
            public bool ProductIsPublished { get; set; }
            public bool ProductIsFeatured { get; set; }
            public string ProductDescriptionShort { get; set; }
            public string ProductDescriptionShortEng { get; set; }
            public string ProductDescription { get; set; }
            public string ProductDescriptionEng { get; set; }
            public string ProductImageFilename { get; set; }
            public string ProductImageHttpPath { get; set; }
            public bool HasProductImage => !string.IsNullOrWhiteSpace(ProductImageFilename);

            public string ProductSKU { get; set; }
            public int? CountryIDProducer { get; set; }

            public List<ProductImage> ProductImages { get; set; }
            public bool HasProductImages => ProductImages?.Any() == true;

            public List<KeyValueSelectedTuple<int?, string>> Brands { get; set; }
            public bool HasBrands => Brands?.Any() == true;
            public List<KeyValueSelectedTuple<int?, string>> Categories { get; set; }
            public bool HasCategories => Categories?.Any() == true;
            public List<KeyValueSelectedTuple<int?, string>> ProductProducerCountries { get; set; }
            public bool HasProductProducerCountries => ProductProducerCountries?.Any() == true;

            public string UrlImageUpload { get; set; }
            public string UrlImageUpdate { get; set; }
            public string UrlImageDelete { get; set; }
            public string UrlImageSort { get; set; }


            public readonly string TextPublished = Resources.TextPublished;
            public readonly string TextFeatured = Resources.TextFeatured;
            public readonly string TextCategory = Resources.TextCategory;
            public readonly string TextPrice = Resources.TextPrice;
            public readonly string TextPriceOld = Resources.TextPriceOld;
            public readonly string TextRemainder = Resources.TextRemainder;
            public readonly string TextBrand = Resources.TextBrand;
            public readonly string TextProducerCountry = Resources.TextProducerCountry;
            public readonly string TextCaption = Resources.TextCaption;
            public readonly string TextCaptionEng = Resources.TextCaptionEng;
            public readonly string TextDescriptionShort = Resources.TextDescriptionShort;
            public readonly string TextDescriptionShortEng = Resources.TextDescriptionShortEng;
            public readonly string TextDescription = Resources.TextDescription;
            public readonly string TextDescriptionEng = Resources.TextDescription;
            public readonly string TextPhotos = Resources.TextPhotos;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            public readonly string TextConfirmDeleteImage = Resources.TextConfirmDeleteImage;
            #endregion

            #region Nested Classes
            public class ProductImage
            {
                #region Properties
                public int? ProductImageID { get; set; }
                public string ProductImageFilename { get; set; }
                public string ProductImageFileHttpPath { get; set; }
                public string ProductImageAltText { get; set; }
                #endregion
            }
            #endregion
        }

        public class DeleteProductImageSubmitModel
        {
            #region Properties
            public int? ProductImageID { get; set; }
            #endregion
        }

        public class UpdateProductImageSubmitModel
        {
            #region Properties
            public int? ProductImageID { get; set; }
            public string ProductImageAltText { get; set; }
            #endregion
        }
        #endregion
    }
}