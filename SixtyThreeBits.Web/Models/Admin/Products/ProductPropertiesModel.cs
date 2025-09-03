using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Libraries.FileStorages.Enums;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class ProductPropertiesModel : ProductModelBase
    {
        #region Methods        
        public async Task<ViewModel> GetViewModel(ViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
                viewModel.ProductIsPublished = Product.ProductIsPublished;
                viewModel.ProductIsFeatured = Product.ProductIsFeatured;
                viewModel.BrandID = Product.BrandID;
                viewModel.ProductCategoryID = Product.ProductCategoryID;
                viewModel.ProductName = Product.ProductName;
                viewModel.ProductNameEng = Product.ProductNameEng;
                viewModel.ProductPrice = Utilities.FormatPriceValue(Product.ProductPrice);
                viewModel.ProductPriceOld = Utilities.FormatPriceValue(Product.ProductPriceOld);
                viewModel.ProductRemainder = Utilities.FormatQuantityValue(Product.ProductRemainder);
                viewModel.ProductSKU = Product.ProductSKU;
                viewModel.ProductDescriptionShort = Product.ProductDescriptionShort;
                viewModel.ProductDescriptionShortEng = Product.ProductDescriptionShortEng;
                viewModel.ProductDescription = Product.ProductDescription;
                viewModel.ProductDescriptionEng = Product.ProductDescriptionEng;
            }
            viewModel.ProductImageFilename = Product.ProductImageFilename;
            viewModel.ProductImageHttpPath = FileStorage.GetUploadedFileHttpPath(
                filename: Product.ProductImageFilename, 
                folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.Products)
            );

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
            viewModel.ProductProducerCountries = await repositoryCountries.CountriesListAsSimpleKeyValue(SelectedCountryID: Product.CountryIDProducer);

            viewModel.ProductImages = Product.ProductImages?.Select(item => new ViewModel.ProductImage
            {
                ProductImageID = item.ProductImageID,
                ProductImageFilename = item.ProductImageFilename,
                ProductImageFileHttpPath = FileStorage.GetUploadedFileHttpPath(
                    filename: item.ProductImageFilename, 
                    folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.Products)
                ),
                ProductImageAltText = item.ProductImageAltText
            })
            .ToList();

            viewModel.UrlImageUpload = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.ImagesUpload, new { productID = Product.ProductID });
            viewModel.UrlImageUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.ImagesUpdate, new { productID = Product.ProductID });
            viewModel.UrlImageSort = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.ImagesSort, new { productID = Product.ProductID });
            viewModel.UrlImageDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.ImagesDelete, new { productID = Product.ProductID });

            return viewModel;
        }

        public async Task<ViewModel> Save(ViewModel submitModel)
        {
            var viewModel = await GetViewModel(submitModel);

            var validationResult = validateSubmitModel(submitModel);

            if(validationResult.HasErrors)
            {
                viewModel.AddFormErrors(validationResult.Errors);
            }
            else
            {
                var repository = RepositoriesFactory.CreateProductsRepository();
                await repository.ProductsIUD(
                    databaseAction: Enums.DatabaseActions.UPDATE,
                    productID: Product.ProductID,
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
                    viewModel.AddToastError(repository.ErrorMessage);
                }
            }

            return viewModel;
        }

        ValidationResult63 validateSubmitModel(ViewModel submitModel)
        {
            var validationResult = new ValidationResult63();
            var error = default(Error63);

            error = Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(submitModel.ProductName)), valueToValidate: submitModel.ProductName);
            validationResult.AddError(error);

            return validationResult;
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
                    databaseAction: Enums.DatabaseActions.INSERT,
                    productImageID: null,
                    productImage: new ProductImageIudDTO
                    {
                        ProductID = Product.ProductID,
                        ProductImageFilename = productImageFilename
                    }
                );

                if (productImageID > 0)
                {
                    await FileStorage.SaveUploadedFile(
                        sourceFileStream: postedFile.OpenReadStream(),
                        filename: productImageFilename,
                        folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.Products)
                    );

                    Product = await repository.ProductsGetSingleByID(Product.ProductID);
                    var firstImage = Product.ProductImages?.FirstOrDefault();
                    if (firstImage != null)
                    {
                        await repository.ProductsIUD(
                            databaseAction: Enums.DatabaseActions.UPDATE,
                            productID: Product.ProductID,
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
                        ProductImageFileHttpPath = FileStorage.GetUploadedFileHttpPath(
                            filename: productImageFilename,
                            folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.Products)
                        )
                    };
                    viewModel.IsSuccess = true;
                }
            }

            return viewModel;
        }

        public async Task<AjaxResponse> UpdateProductImages(UpdateProductImageSubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();

            var productImage = Product.ProductImages?.FirstOrDefault(Item => Item.ProductImageID == submitModel.ProductImageID);
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
                    var firstImage = Product.ProductImages?.FirstOrDefault();
                    if (firstImage != null && firstImage.ProductImageID == submitModel.ProductImageID)
                    {
                        await repository.ProductsIUD(
                            databaseAction: Enums.DatabaseActions.UPDATE,
                            productID: Product.ProductID,
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

            var productImage = Product.ProductImages?.FirstOrDefault(Item => Item.ProductImageID == submitModel.ProductImageID);
            if (productImage != null)
            {
                await FileStorage.DeleteFile(
                    filename: productImage.ProductImageFilename,
                    folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.Products)
                );

                var repository = RepositoriesFactory.CreateProductsRepository();
                await repository.ProductsImagesIUD(
                    databaseAction: Enums.DatabaseActions.DELETE,
                    productImageID: submitModel.ProductImageID,
                    productImage: null
                );

                if (!repository.IsError)
                {
                    viewModel.IsSuccess = true;

                    var isMainImageDeleted = productImage.ProductImageFilename == Product.ProductImageFilename;
                    if (isMainImageDeleted)
                    {
                        Product = await repository.ProductsGetSingleByID(Product.ProductID);
                        var firstImage = Product.ProductImages?.FirstOrDefault();
                        await repository.ProductsIUD(
                            databaseAction: Enums.DatabaseActions.UPDATE,
                            productID: Product.ProductID,
                            product: new ProductIudDTO
                            {
                                ProductImageFilename = firstImage?.ProductImageFilename ?? Constants.NullValueFor.String,
                                ProductImageAltText = firstImage?.ProductImageAltText ?? Constants.NullValueFor.String
                            }
                        );
                    }
                }
            }
            return viewModel;
        }

        public async Task<AjaxResponse> SortProductImages(SyncSortIndexesSubmitModel SubmitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateProductsRepository();
            await repository.ProductsImagesSyncSortIndex(Product.ProductID, SubmitModel.SortIndexes);
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