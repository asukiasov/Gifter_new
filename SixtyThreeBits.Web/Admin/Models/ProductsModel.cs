using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.BusinessLogics;
using SixtyThreeBits.Core.Infrastructure.DTO;
using SixtyThreeBits.Core.Infrastructure.Libraries;
using SixtyThreeBits.Core.Infrastructure.Libraries.FileStorages;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class ProductsModel : WebProjectModelBase
    {
        #region Properties
        readonly string _folderPath = FileStorageManager.Modules[Enums.FileManagerModules.Products].FolderName;
        #endregion

        #region Methods
        public async Task<PageViewModel> GetPageViewModel()
        {
            var viewModel = new PageViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Products.GridAdd);
            viewModel.ShowExcelUploadButton = User.HasPermission(ControllerActionRouteNames.Admin.Products.ExcelUpload) && User.HasPermission(ControllerActionRouteNames.Admin.Products.ExcelDownload);
            viewModel.UrlExcelUpload = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.ExcelUpload);
            viewModel.UrlExcelDownload = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.ExcelDownload);            

            viewModel.Grid = new PageViewModel.GridModel();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.GridDelete);
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.Products.GridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Products.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Products.GridDelete);

            var repository = RepositoriesFactory.GetProductsRepository();
            viewModel.Grid.Categories = (await repository.ProductCategoriesListWithTitlePaddindHierarchy(padChar:'-')).Select(item => new KeyValueTuple<int?, string>
            {
                Key = item.ProductCategoryID,
                Value = item.ProductCategoryName
            }).ToList();

            return viewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var repository = RepositoriesFactory.GetProductsRepository();
            var viewModel = (await repository.ProductsList())?.Select(Item => new PageViewModel.GridModel.GridItem
            {
                ProductID = Item.ProductID,
                ProductName = Item.ProductName,
                ProductIsPublished = Item.ProductIsPublished,
                ProductPrice = Item.ProductPrice,
                ProductPriceOld = Item.ProductPriceOld,
                ProductRemainder = Item.ProductRemainder,
                ProductIsFeatured = Item.ProductIsFeatured,
                ProductCategoryID = Item.ProductCategoryID,
                UrlProductsProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.Product.Properties, new { ProductID = Item.ProductID })
            }).ToList();
            return viewModel;
        }

        public async Task CRUD(Enums.DatabaseActions databaseAction, int? productID, PageViewModel.GridModel.GridItem submitModel)
        {
            var repository = RepositoriesFactory.GetProductsRepository();

            if (databaseAction == Enums.DatabaseActions.DELETE)
            {                
                var DBItem = await repository.ProductsGetSingleByID(productID);
                if(DBItem?.ProductImages?.Any() == true)
                {
                    foreach(var Item in DBItem.ProductImages)
                    {
                        await DeleteUploadedFile(Item.ProductImageFilename, _folderPath);
                    }
                }
            }
            
            await repository.ProductsIUD(
                databaseAction: databaseAction,
                productID: productID,
                productCategoryID: submitModel.ProductCategoryID,
                productName: submitModel.ProductName,
                productIsPublished: submitModel.ProductIsPublished,
                productPrice: submitModel.ProductPrice,
                productPriceOld: submitModel.ProductPriceOld,
                productRemainder: submitModel.ProductRemainder,
                productIsFeatured: submitModel.ProductIsFeatured
            );

            if (repository.IsError)
            {
                Form.AddError(Resources.TextError);
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
						HasExcelErrors = result.HasExcelErrors,
						ExcelErrors = result.ExcelErrors				
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
        public class PageViewModel
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
            public readonly string TextUpload = Resources.TextUpload;            
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<GridModel.GridItem>
            {
                #region Properties
                public List<KeyValueTuple<int?,string>> Categories { get; set; }
                #endregion

                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = GetGridWithStartupValues<GridItem>(html: html, keyFieldName: nameof(GridItem.ProductID));

                    grid
                   .ID("ProductsGrid")                   
                   .OnInitialized("productsModel.onGridInit")
                   .Columns(columns =>
                   {
                       columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlProductsProperties));
                       columns.AddFor(m => m.ProductName).Caption(Resources.TextName).Width(350).ValidationRules(options =>
                       {
                           options.AddRequired();
                       });
                       columns.AddFor(m => m.ProductCategoryID).Caption(Resources.TextCategory).Width(250).InitLookupColumn(data: Categories, isRequired: true);
                       columns.AddFor(m => m.ProductPrice).Caption(Resources.TextPrice).Width(100).Alignment(HorizontalAlignment.Right).DataType(GridColumnDataType.Number);
                       columns.AddFor(m => m.ProductPriceOld).Caption(Resources.TextPriceOld).Width(100).Alignment(HorizontalAlignment.Right).DataType(GridColumnDataType.Number);
                       columns.AddFor(m => m.ProductRemainder).Caption(Resources.TextRemainder).Width(100).Alignment(HorizontalAlignment.Right).DataType(GridColumnDataType.Number);
                       columns.AddFor(m => m.ProductIsPublished).Caption(Resources.TextPublished).Width(80).InitCheckboxColumn();
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
                    public bool ProductIsFeatured { get; set; }
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

    public class ProductsModelBase : WebProjectModelBase
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
        public async Task<ProductsPropertiesViewModel> GetPageViewModel(ProductsPropertiesViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new ProductsPropertiesViewModel();
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

            var repositoryBrands = RepositoriesFactory.GetBrandsRepository();
            viewModel.Brands = (await repositoryBrands.BrandsList()).Select(item => new KeyValueSelectedTuple<int?, string> 
            { 
                Key = item.BrandID, 
                Value = item.BrandName, 
                IsSelected = item.BrandID == viewModel.BrandID
            }).ToList();

            var repositoryProducts = RepositoriesFactory.GetProductsRepository();
            viewModel.Categories = (await repositoryProducts.ProductCategoriesListWithTitlePaddindHierarchy('-')).Select(item => new KeyValueSelectedTuple<int?, string>
            {
                Key = item.ProductCategoryID,
                Value = item.ProductCategoryName,
                IsSelected = item.ProductCategoryID == viewModel.ProductCategoryID
            }).ToList();

            var repositoryCountries = RepositoriesFactory.GetCountriesRepository();
            viewModel.ProductProducerCountries = (await repositoryCountries.CountriesListAsSimpleKeyValue(SelectedCountryID: DBItem.CountryIDProducer));

            viewModel.ProductImages = DBItem.ProductImages?.Select(Item => new ProductsPropertiesViewModel.ProductImage
            {
                ProductImageID = Item.ProductImageID,
                ProductImageFilename = Item.ProductImageFilename,
                ProductImageFileHttpPath = FileStorage.GetUploadedFileHttpPath(Item.ProductImageFilename, _folderPath)
            }).ToList();

            viewModel.UrlImageUpload = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.Product.PropertiesImagesUpload, new { productID = DBItem.ProductID });
            viewModel.UrlImageSort = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.Product.PropertiesImagesSort, new { productID = DBItem.ProductID });
            viewModel.UrlImageDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.Product.PropertiesImagesDelete, new { productID = DBItem.ProductID });

            return viewModel;
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();

            await DeleteUploadedFile(DBItem.ProductImageFilename, _folderPath);

            var repository = RepositoriesFactory.GetProductsRepository();
            await repository.ProductsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                productID: DBItem.ProductID,
                productImageFilename: Constants.NullValueFor.String
            );

            viewModel.IsSuccess = !repository.IsError;

            return viewModel;
        }

        public void ValidatePageViewModel(ProductsPropertiesViewModel viewModel)
        {
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.ProductName)), valueToValidate: viewModel.ProductName));            
        }

        public async Task Save(ProductsPropertiesViewModel viewModel)
        {
            var repository = RepositoriesFactory.GetProductsRepository();
            await repository.ProductsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                productID: DBItem.ProductID,
                productCategoryID: viewModel.ProductCategoryID ?? Constants.NullValueFor.Int,
                countryIDProducer: viewModel.CountryIDProducer ?? Constants.NullValueFor.Int,
                brandID: viewModel.BrandID ?? Constants.NullValueFor.Int,                                
                productName: viewModel.ProductName,
                productPrice: viewModel.ProductPrice.ToDecimal() ?? Constants.NullValueFor.Int,
                productPriceOld: viewModel.ProductPriceOld.ToDecimal() ?? Constants.NullValueFor.Int,
                productRemainder: viewModel.ProductRemainder.ToDecimal() ?? Constants.NullValueFor.Int,
                productNameEng: viewModel.ProductNameEng ?? Constants.NullValueFor.String,
                productDescriptionShort: viewModel.ProductDescriptionShort ?? Constants.NullValueFor.String,
                productDescriptionShortEng: viewModel.ProductDescriptionShortEng ?? Constants.NullValueFor.String,
                productDescription: viewModel.ProductDescription ?? Constants.NullValueFor.String,
                productDescriptionEng: viewModel.ProductDescriptionEng ?? Constants.NullValueFor.String,
                productIsPublished: viewModel.ProductIsPublished,
                productIsFeatured: viewModel.ProductIsFeatured,
                productSKU: viewModel.ProductSKU ?? Constants.NullValueFor.String
            );
            viewModel.IsSaved = !repository.IsError;            
        }

        public async Task<AjaxResponse> UploadImages()
        {
            var viewModel = new AjaxResponse();

            var postedFile = Request.Form.Files[0];
            var productImageFilenameOriginal = postedFile.FileName;
            var productImageFilename = GetFilenameFromUploadedFile(postedFile);

            var repository = RepositoriesFactory.GetProductsRepository();
            var productImageID = await repository.ProductsImagesIUD(
                databaseAction: Enums.DatabaseActions.CREATE,
                productID: DBItem.ProductID,
                productImageFilename: productImageFilename
            );

            if (productImageID > 0)
            {
                await SaveUploadedFile(postedFile: postedFile, filename: productImageFilename, folderPath: _folderPath);

                viewModel.Data = new ProductsPropertiesViewModel.ProductImage
                {
                    ProductImageID = productImageID,
                    ProductImageFilename = productImageFilenameOriginal,
                    ProductImageFileHttpPath = FileStorage.GetUploadedFileHttpPath(productImageFilename,_folderPath)
                };
                viewModel.IsSuccess = true;
            }

            return viewModel;
        }

        public async Task<AjaxResponse> DeleteImage(DeleteImageSubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();

            var productImage = DBItem.ProductImages?.FirstOrDefault(Item => Item.ProductImageID == submitModel.ProductImageID);       
            if (productImage != null)
            {
                await DeleteUploadedFile(productImage.ProductImageFilename, _folderPath);

                var repository = RepositoriesFactory.GetProductsRepository();
                await repository.ProductsImagesIUD(
                    databaseAction: Enums.DatabaseActions.DELETE,
                    productImageID: submitModel.ProductImageID
                );
                viewModel.IsSuccess = !repository.IsError;
            }
            return viewModel;
        }

        public async Task<AjaxResponse> SortImages(SyncSortIndexesSubmitModel SubmitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.GetProductsRepository();
            await repository.ProductsImagesSyncSortIndex(DBItem.ProductID, SubmitModel.SortIndexes);
            viewModel .IsSuccess = !repository.IsError;
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ProductsPropertiesViewModel : FormViewModelBase
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
                #endregion
            }
            #endregion
        }

        public class DeleteImageSubmitModel
        {
            #region Properties
            public int? ProductImageID { get; set; }
            #endregion
        }
        #endregion
    }    
}