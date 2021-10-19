using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
//using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Services;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Reusables.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class ProductsModel : WebProjectModelBase
    {
        #region Methods
        public async Task<PageViewModel> GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.UploadExcelFileUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.ProductsRemainderSync);
            ViewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Products.ProductsGridAdd);

            ViewModel.Grid = new PageViewModel.GridModel();
            ViewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.ProductsGrid);
            ViewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.ProductsGridAdd);
            ViewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.ProductsGridUpdate);
            ViewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.ProductsGridDelete);
            ViewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.Products.ProductsGridAdd);
            ViewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Products.ProductsGridUpdate);
            ViewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Products.ProductsGridDelete);

            ViewModel.Grid.Categories = (await DataAccessFactory.Categories.ListCategoriesWithTitlePaddindHierarchy(PadChar:'-')).Select(Item => new SimpleKeyValue<int?, string>
            {
                Key = Item.CategoryID,
                Value = Item.CategoryName
            }).ToList();

            return ViewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var Products = (await DataAccessFactory.Products.ListProducts())?.Select(Item => new PageViewModel.GridModel.GridItem
            {
                ProductID = Item.ProductID,
                ProductName = Item.ProductName,
                ProductCode = Item.ProductCode,
                ProductIsPublished = Item.ProductIsPublished,
                ProductPrice = Item.ProductPrice,
                ProductPriceOld = Item.ProductPriceOld,
                ProductRemainder = Item.ProductRemainder,
                ProductIsFeatured = Item.ProductIsFeatured,
                CategoryID = Item.CategoryID,
                UrlProductsProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.Product.Properties, new { ProductID = Item.ProductID })
            }).ToList();
            return Products;
        }

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? ProductID, PageViewModel.GridModel.GridItem SubmitModel)
        {
            if(DatabaseAction == Enums.DatabaseActions.DELETE)
            {
                var DBItem = await DataAccessFactory.Products.GetSingleProductByID(ProductID);
                if(DBItem?.ProductImages?.Any() == true)
                {
                    foreach(var Item in DBItem.ProductImages)
                    {
                        Utilities.DeleteUploadedFile(Item.ProductImageFilename);
                    }
                }
            }

            await DataAccessFactory.Products.ProductsIUD(
                DatabaseAction: DatabaseAction,
                ProductID: ProductID,
                CategoryID: SubmitModel.CategoryID,
                ProductName: SubmitModel.ProductName,
                ProductCode: SubmitModel.ProductCode,
                ProductIsPublished: SubmitModel.ProductIsPublished,
                ProductPrice: SubmitModel.ProductPrice,
                ProductPriceOld: SubmitModel.ProductPriceOld,
                ProductRemainder: SubmitModel.ProductRemainder,
                ProductIsFeatured: SubmitModel.ProductIsFeatured
            );

            if (DataAccessFactory.Products.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }

        //public List<Product.ProductSyncItem> GetListFromExcelFile(IFormFile PostedFile)
        //{
        //    var IsXlsx = PostedFile.FileName.EndsWith(".xlsx");
        //    var ProductSyncItemList = new List<Product.ProductSyncItem>();

        //    using (var InputStream = PostedFile.OpenReadStream())
        //    {
        //        using (var ExcelReader = IsXlsx ? ExcelReaderFactory.CreateOpenXmlReader(InputStream) : ExcelReaderFactory.CreateBinaryReader(InputStream))
        //        {
        //            ExcelReader.Read();

        //            while (ExcelReader.Read())
        //            {
        //                var Item = new Product.ProductSyncItem();
        //                Item.ProductName = ExcelReader.GetValue(0)?.ToString();
        //                Item.ProductNameEng = ExcelReader.GetValue(1)?.ToString();
        //                Item.ProductNameRus = ExcelReader.GetValue(2)?.ToString();
        //                Item.ProductCode = ExcelReader.GetValue(3)?.ToString();
        //                Item.ProductCategory = ExcelReader.GetValue(4)?.ToString();
        //                Item.ProductBrand = ExcelReader.GetValue(5)?.ToString();
        //                Item.ProductPrice = ExcelReader.GetValue(6)?.ToString().ToDecimal();
        //                Item.ProductRemainder = ExcelReader.GetValue(7)?.ToString().ToDecimal();

        //                ProductSyncItemList.Add(Item);
        //            }
        //        }
        //    }

        //    return ProductSyncItemList;
        //}

        public async Task InitListWithBrandIDAndCategoryID(List<Product.ProductSyncItem> ProductSyncItems)
        {
            await DataAccessFactory.Products.InitListWithBrandIDAndCategoryID(ProductSyncItems, DataAccessFactory);
        }

        public async Task<bool> InsertOrUpdateProducts(List<Product.ProductSyncItem> ProductSyncItems)
        {

            await DataAccessFactory.Products.ProductsSync(ProductSyncItems);

            var IsSuccess = !DataAccessFactory.Products.IsError;

            return IsSuccess;

        }

        //public async Task<List<SimpleKeyValue<string, string>>> ValidateProductSyncItems(List<Product.ProductSyncItem> ProductSyncItems)
        //{
        //    var ErrorList = new List<SimpleKeyValue<string, string>>();
        //    var Index = 0;
        //    var BrandsList = await DataAccessFactory.Brands.ListBrands();
        //    var CategoriesList = await DataAccessFactory.Categories.ListCategories();

        //    Regex objNotIntPattern = new Regex("[^0-9-]");

        //    foreach (var Product in ProductSyncItems)
        //    {
        //        var PreviousProducts = ProductSyncItems.GetRange(0, Index);
        //        var CategoryExists = CategoriesList.Any(CategoryItem => CategoryItem.CategoryName == Product.ProductCategory);
        //        var BrandExists = BrandsList.Any(BrandItem => BrandItem.BrandName == Product.ProductBrand);

        //        if (!CategoryExists)
        //        {
        //            ErrorList.Add(new SimpleKeyValue<string, string> { Key = $"{"ხაზი"} {(Index + 2)} {","}", Value = $"{Product.ProductCategory} {Resources.TextCategoryDoesNotExists}" });
        //        }

        //        if (!BrandExists)
        //        {
        //            ErrorList.Add(new SimpleKeyValue<string, string> { Key = $"{"ხაზი"} {(Index + 2)} {","}", Value = $"{Product.ProductBrand} {Resources.TextBrandDoesNotExists}" });
        //        }
               
        //        if (Product.ProductPrice == null)
        //        {
        //            ErrorList.Add(new SimpleKeyValue<string, string> { Key = $"{"ხაზი"} {(Index + 2)} {","}", Value = Resources.ValidationPriceMustBeNumeric });
        //        }
        //        if (Product.ProductRemainder == null)
        //        {
        //            ErrorList.Add(new SimpleKeyValue<string, string> { Key = $"{"ხაზი"} {(Index + 2)} {","}", Value = Resources.ValidationRemainderMustBeInt });
        //        }
        //        if (objNotIntPattern.IsMatch(Product.ProductRemainder.ToString()))
        //        {
        //            ErrorList.Add(new SimpleKeyValue<string, string> { Key = $"{"ხაზი"} {(Index + 2)} {","}", Value = Resources.ValidationRemainderMustBeInt });
        //        }
        //        if (string.IsNullOrEmpty(Product.ProductName))
        //        {
        //            ErrorList.Add(new SimpleKeyValue<string, string> { Key = $"{"ხაზი"} {(Index + 2)} {","}", Value = Resources.ValidationNameIsRequired });
        //        }
        //        else
        //        {
        //            var HasDuplicate = PreviousProducts.Any(Item => Item.ProductName == Product.ProductName);

        //            if (HasDuplicate)
        //            {
        //                ErrorList.Add(new SimpleKeyValue<string, string> { Key = $"{"ხაზი"} {(Index + 2)} {","}", Value = $"{Product.ProductName} {Resources.TextDublicateItemError}" });
        //            }
        //        }
        //        Index++;

        //    }           

        //    return ErrorList;
        //}
        #endregion

        #region Sub Classes
        public class PageViewModel : FormViewModelBase
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public string UploadExcelFileUrl { get; set; }
            public GridModel Grid { get; set; }
            public string IsError => (HasErrors).ToString().ToLower();            
            #endregion

            #region Sub Classes
            public class GridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<GridModel.GridItem>
            {
                #region Properties
                public List<SimpleKeyValue<int?,string>> Categories { get; set; }
                #endregion

                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.ProductID));

                    Grid
                   .ID("ProductsGrid")
                   .Scrolling(Options =>
                   {
                       Options.Mode(GridScrollingMode.Standard);
                       Options.ShowScrollbar(ShowScrollbarMode.Always);
                   })
                   .OnInitialized("ProductsModel.OnProductsGridInit")
                   .Columns(Columns =>
                   {
                       Columns.Add().Width(30).Caption(" ").CellTemplate(new JS("ProductsModel.GetDetailsButtonColumnCellHtml"));
                       Columns.AddFor(m => m.ProductName).Caption("დასახელება").Width(350).ValidationRules(Options =>
                       {
                           Options.AddRequired();
                       });
                       Columns.AddFor(m => m.CategoryID).Caption("კატეგორია").Width(250).InitLookupColumn(Data: Categories, IsRequired: true);
                       Columns.AddFor(m => m.ProductCode).Caption("კოდი").Width(150);
                       Columns.AddFor(m => m.ProductPrice).Caption("ფასი").Width(100).Alignment(HorizontalAlignment.Right).DataType(GridColumnDataType.Number);
                       Columns.AddFor(m => m.ProductPriceOld).Caption("ძველი ფასი").Width(100).Alignment(HorizontalAlignment.Right).DataType(GridColumnDataType.Number);
                       Columns.AddFor(m => m.ProductRemainder).Caption("ნაშთი").Width(100).Alignment(HorizontalAlignment.Right).DataType(GridColumnDataType.Number);
                       Columns.AddFor(m => m.ProductIsPublished).Caption("აქტიური").Width(80).InitCheckboxColumn();
                       Columns.AddFor(m => m.ProductIsFeatured).Caption("სპეც შეთავაზება").Width(120).InitCheckboxColumn();                       
                       Columns.Add();
                   });

                    return Grid;

                }
                #endregion

                #region Sub Classes
                public class GridItem
                {
                    #region Properties
                    public int? ProductID { get; set; }
                    public string ProductName { get; set; }
                    public string ProductCode { get; set; }
                    public bool? ProductIsPublished { get; set; }
                    public decimal? ProductPrice { get; set; }
                    public decimal? ProductPriceOld { get; set; }
                    public decimal? ProductRemainder { get; set; }
                    public bool ProductIsFeatured { get; set; }
                    public int? CategoryID { get; set; }
                    public string UrlProductsProperties { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }

    public class ProductsModelBase : WebProjectModelBase
    {
        #region Properties        
        public Product DBItemProduct { get; set; }
        #endregion
    }

    public class ProductPropertiesModel : ProductsModelBase
    {
        #region Methods        
        public async Task<ProductsPropertiesViewModel> GetPageViewModel(int? ProductID, ProductsPropertiesViewModel ViewModel)
        {
            if (ViewModel == null)
            {
                ViewModel = new ProductsPropertiesViewModel();
                ViewModel.ProductIsPublished = DBItemProduct.ProductIsPublished;
                ViewModel.ProductIsFeatured = DBItemProduct.ProductIsFeatured;
                ViewModel.BrandID = DBItemProduct.BrandID;
                ViewModel.CategoryID = DBItemProduct.CategoryID;
                ViewModel.ProductName = DBItemProduct.ProductName;
                ViewModel.ProductCode = DBItemProduct.ProductCode;
                ViewModel.ProductNameEng = DBItemProduct.ProductNameEng;
                ViewModel.ProductNameRus = DBItemProduct.ProductNameRus;
                ViewModel.ProductPriceString = string.Format(Constants.Formats.DecimalNoTrailingZerosEval, DBItemProduct.ProductPrice);
                ViewModel.ProductPriceOldString = string.Format(Constants.Formats.DecimalNoTrailingZerosEval, DBItemProduct.ProductPriceOld);
                ViewModel.ProductRemainderString = string.Format(Constants.Formats.DecimalNoTrailingZerosEval, DBItemProduct.ProductRemainder);
                ViewModel.ProductSKU = DBItemProduct.ProductSKU;
                ViewModel.ProductDescriptionShort = DBItemProduct.ProductDescriptionShort;
                ViewModel.ProductDescriptionShortEng = DBItemProduct.ProductDescriptionShortEng;
                ViewModel.ProductDescriptionShortRus = DBItemProduct.ProductDescriptionShortRus;
                ViewModel.ProductDescription = DBItemProduct.ProductDescription;
                ViewModel.ProductDescriptionEng = DBItemProduct.ProductDescriptionEng;
                ViewModel.ProductDescriptionRus = DBItemProduct.ProductDescriptionRus;
            }
            ViewModel.ProductImageFilename = DBItemProduct.ProductImageFilename;
            ViewModel.ProductImageHttpPath = Utilities.GetUploadedFileHttpPath(DBItemProduct.ProductImageFilename);            

            ViewModel.Brands = (await DataAccessFactory.Brands.ListBrands()).Select(Item => new SimpleKeyValue<int?, string> { Key = Item.BrandID, Value = Item.BrandName }).ToList();
            ViewModel.Categories = (await DataAccessFactory.Categories.ListCategoriesWithTitlePaddindHierarchy()).Select(Item => new SimpleKeyValue<int?, string> 
            { 
                Key = Item.CategoryID, 
                Value = Item.CategoryName?.Replace(" ","&nbsp;")
            }).ToList();
            foreach (var Item in ViewModel.Brands)
            {
                Item.IsSelected = ViewModel.BrandID == Item.Key;
            }

            foreach (var Item in ViewModel.Categories)
            {
                Item.IsSelected = ViewModel.CategoryID == Item.Key;
            }


            ViewModel.ProductProducerCountries = (await DataAccessFactory.Dictionaries.ListCountriesAsSimpleKeyValue(SelectedCountryID: DBItemProduct.ProductProducerCountryID));

            ViewModel.ProductImages = DBItemProduct.ProductImages?.Select(Item => new ProductsPropertiesViewModel.ProductImage
            {
                ProductImageID = Item.ProductImageID,
                ProductImageFilename = Item.ProductImageFilename,
                ProductImageHttpPath = Utilities.GetUploadedFileHttpPath(Item.ProductImageFilename)
            }).ToList();

            ViewModel.UrlImageUpload = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.Product.PropertiesImagesUpload, new { ProductID = ProductID });
            ViewModel.UrlImageSort = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.Product.PropertiesImagesSort, new { ProductID = ProductID });
            ViewModel.UrlImageDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Products.Product.PropertiesImagesDelete, new { ProductID = ProductID });

            return ViewModel;
        }

        public async Task<AjaxResponse> DeleteImage(int? ProductID)
        {

            Utilities.DeleteUploadedFile(DBItemProduct.ProductImageFilename);

            var AR = new AjaxResponse();

            await DataAccessFactory.Products.ProductsIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                ProductID: ProductID,
                ProductImageFilename: Constants.NullValueFor.String
            );

            AR.IsSuccess = !DataAccessFactory.Products.IsError;

            return AR;
        }

        public void ValidatePageViewModel(ProductsPropertiesViewModel ViewModel)
        {
            ViewModel.Errors = new List<SimpleKeyValue<string, string>>
            {
                //Validation.ValidateRequired(ErrorKey:$"[name=\"{nameof(ViewModel.ProductSlug)}\"]", ValueToValidate:ViewModel.ProductSlug),
                Validation.ValidateRequired(ErrorKey:$"[name=\"{nameof(ViewModel.ProductName)}\"]", ValueToValidate:ViewModel.ProductName)                
            };
            ViewModel.Errors.RemoveAll(Item => Item == null);
        }

        public async Task SaveProductsProperties(int? ProductID, ProductsPropertiesViewModel ViewModel)
        {            
            await DataAccessFactory.Products.ProductsIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                ProductID: ProductID,
                BrandID: ViewModel.BrandID ?? Constants.NullValueFor.Int,
                CategoryID: ViewModel.CategoryID ?? Constants.NullValueFor.Int,
                ProductName: ViewModel.ProductName,
                ProductCode: ViewModel.ProductCode ?? Constants.NullValueFor.String,
                ProductPrice: ViewModel.ProductPrice,
                ProductPriceOld: ViewModel.ProductPriceOld,
                ProductRemainder: ViewModel.ProductRemainder,
                ProductNameEng: ViewModel.ProductNameEng ?? Constants.NullValueFor.String,
                ProductNameRus: ViewModel.ProductNameRus ?? Constants.NullValueFor.String,
                ProductDescriptionShort: ViewModel.ProductDescriptionShort ?? Constants.NullValueFor.String,
                ProductDescriptionShortEng: ViewModel.ProductDescriptionShortEng ?? Constants.NullValueFor.String,
                ProductDescriptionShortRus: ViewModel.ProductDescriptionShortRus ?? Constants.NullValueFor.String,
                ProductDescription: ViewModel.ProductDescription ?? Constants.NullValueFor.String,
                ProductDescriptionEng: ViewModel.ProductDescriptionEng ?? Constants.NullValueFor.String,
                ProductDescriptionRus: ViewModel.ProductDescriptionRus ?? Constants.NullValueFor.String,
                ProductIsPublished: ViewModel.ProductIsPublished,
                ProductIsFeatured: ViewModel.ProductIsFeatured,
                ProductSKU: ViewModel.ProductSKU,
                ProductProducerCountryID: ViewModel.ProductProducerCountryID
            );
            ViewModel.IsSaved = !DataAccessFactory.Products.IsError;            
        }

        public async Task<AjaxResponse> UploadImages(int? ProductID)
        {
            var AR = new AjaxResponse();
            var Images = new List<SimpleKeyValue<string, IFormFile>>();
            var ImagesDBItems = new List<Product.ProductImage>();

            foreach (var PostedFile in Request.Form.Files)
            {
                var Filename = GetFilenameFromUploadedFile(PostedFile);
                ImagesDBItems.Add(new Product.ProductImage { ProductImageFilename = Filename });
                Images.Add(new SimpleKeyValue<string, IFormFile> { Key = Filename, Value = PostedFile });
            }

            await DataAccessFactory.Products.ProductsImagesInsert(ProductID, ImagesDBItems);

            if (!DataAccessFactory.Products.IsError)
            {
                foreach (var Item in Images)
                {
                    await SaveUploadedFile(PostedFile: Item.Value, Filename: Item.Key);                    
                };
                AR.IsSuccess = true;
            }
            return AR;
        }

        public async Task<AjaxResponse> DeleteImage(int? ProductID, int? ProductImageID, string ProductImageFilename)
        {
            var AR = new AjaxResponse();
            Utilities.DeleteUploadedFile(ProductImageFilename);            
            await DataAccessFactory.Products.ProductsImagesIUD(
                DatabaseAction: Enums.DatabaseActions.DELETE,
                ProductImageID: ProductImageID
            );
            AR.IsSuccess = !DataAccessFactory.Products.IsError;
            return AR;
        }

        public async Task<AjaxResponse> SortImage(int? ProductID, SyncSortIndexesModel SubmitModel)
        {
            var AR = new AjaxResponse();
            await DataAccessFactory.Products.ProductsImagesSyncSortIndex(SubmitModel.SortIndexes);
            AR.IsSuccess = !DataAccessFactory.Products.IsError;
            return AR;
        }
        #endregion

        #region Sub Classes
        public class ProductsPropertiesViewModel : FormViewModelBase
        {
            #region Properties
            public int? BrandID { get; set; }
            public int? CategoryID { get; set; }            
            public string ProductName { get; set; }
            public string ProductCode { get; set; }
            public string ProductNameEng { get; set; }
            public decimal? ProductPrice { get; set; }
            public string ProductPriceString { get; set; }
            public decimal? ProductPriceOld { get; set; }
            public string ProductPriceOldString { get; set; }
            public decimal? ProductRemainder { get; set; }
            public string ProductRemainderString { get; set; }
            public string ProductNameRus { get; set; }
            public bool ProductIsPublished { get; set; }
            public bool ProductIsFeatured { get; set; }
            public string ProductDescriptionShort { get; set; }
            public string ProductDescriptionShortEng { get; set; }
            public string ProductDescriptionShortRus { get; set; }
            public string ProductDescription { get; set; }
            public string ProductDescriptionEng { get; set; }
            public string ProductDescriptionRus { get; set; }
            public string ProductImageFilename { get; set; }
            public string ProductImageHttpPath { get; set; }
            public bool HasProductImage => !string.IsNullOrWhiteSpace(ProductImageFilename);

            public string ProductSKU { get; set; }
            public int? ProductProducerCountryID { get; set; }
            
            public List<ProductImage> ProductImages { get; set; }
            public bool HasProductImages => ProductImages?.Any() == true;

            public List<SimpleKeyValue<int?, string>> Brands { get; set; }
            public bool HasBrands => Brands?.Any() == true;
            public List<SimpleKeyValue<int?, string>> Categories { get; set; }
            public bool HasCategories => Categories?.Any() == true;
            public List<SimpleKeyValue<int?, string>> ProductProducerCountries { get; set; }
            public bool HasProductProducerCountries => ProductProducerCountries?.Any() == true;

            public string UrlImageUpload { get; set; }
            public string UrlImageDelete { get; set; }
            public string UrlImageSort { get; set; }


            public readonly string TextConfirmDelete = Resources.TextConfirmDelete;
            public readonly string TextConfirmDeleteImage = Resources.TextConfirmDeleteImage;            
            public readonly string TextDropzone = Resources.TextDropzone;

            #endregion

            #region Sub Classes
            public class ProductImage
            {
                #region Properties
                public int? ProductImageID { get; set; }
                public string ProductImageFilename { get; set; }
                public string ProductImageHttpPath { get; set; }
                #endregion
            }
            #endregion
        }
        #endregion

    }    
}