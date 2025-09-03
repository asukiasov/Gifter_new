using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{      
    public class ProductCategoryPropertiesModel : ProductCategoryModelBase
    {
        #region Methods
        public ViewModel GetViewModel(ViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();

                viewModel.ProductCategoryParentID = DBItem.ProductCategoryParentID;
                viewModel.ProductCategoryName = DBItem.ProductCategoryName;
                viewModel.ProductCategoryNameEng = DBItem.ProductCategoryNameEng;
                viewModel.ProductCategoryDescriptionShort = DBItem.ProductCategoryDescriptionShort;
                viewModel.ProductCategoryDescriptionShortEng = DBItem.ProductCategoryDescriptionShortEng;

            }
            viewModel.ProductCategoryImageFilename = DBItem.ProductCategoryImageFilename;
            viewModel.ProductCategoryImageHttpPath = FileStorage.GetUploadedFileHttpPath(DBItem.ProductCategoryImageFilename);
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductCategoryPropertiesController.DeleteImage, new { productCategoryID = DBItem.ProductCategoryID });

            return viewModel;
        }

        public async Task<ViewModel> Save(ViewModel submitModel)
        {
            var viewModel = GetViewModel(submitModel);

            var validationResult = validateSubmitModel(submitModel);

            if (validationResult.HasErrors)
            {
                viewModel.AddFormErrors(validationResult.Errors);
            }
            else
            {
                var hasCategoryImage = submitModel.ProductCategoryImageFile?.Length > 0;
                var categoryImageFilename = hasCategoryImage ? GetFilenameFromUploadedFile(submitModel.ProductCategoryImageFile) : null;

                if (hasCategoryImage)
                {
                    await FileStorage.DeleteFile(submitModel.ProductCategoryImageFilename);
                }

                var repository = RepositoriesFactory.CreateProductsRepository();
                await repository.ProductCategoriesIUD(
                    databaseAction: Enums.DatabaseActions.UPDATE,
                    productCategoryID: DBItem.ProductCategoryID,
                    productCategory: new ProductCategoryIudDTO
                    {
                        ProductCategoryParentID = submitModel.ProductCategoryParentID,
                        ProductCategoryName = submitModel.ProductCategoryName,
                        ProductCategoryNameEng = submitModel.ProductCategoryNameEng ?? Constants.NullValueFor.String,
                        ProductCategoryImageFilename = categoryImageFilename,
                        ProductCategoryDescriptionShort = submitModel.ProductCategoryDescriptionShort ?? Constants.NullValueFor.String,
                        ProductCategoryDescriptionShortEng = submitModel.ProductCategoryDescriptionShortEng ?? Constants.NullValueFor.String
                    }
                );

                if (repository.IsError)
                {
                    viewModel.AddToastError(repository.ErrorMessage);
                }
                else
                {
                    if (hasCategoryImage)
                    {
                        await FileStorage.SaveUploadedFile(
                            sourceFileStream: submitModel.ProductCategoryImageFile.OpenReadStream(),
                            filename: categoryImageFilename
                        );
                    }
                }
            }

            return viewModel;
        }

        ValidationResult63 validateSubmitModel(ViewModel submitModel)
        {
            var validationResult = new ValidationResult63();
            var error = default(Error63);
            
            error = Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(submitModel.ProductCategoryName)), valueToValidate: submitModel.ProductCategoryName);
            validationResult.AddError(error);

            return validationResult;
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();

            await FileStorage.DeleteFile(DBItem.ProductCategoryImageFilename);

            var repository = RepositoriesFactory.CreateProductsRepository();
            await repository.ProductCategoriesIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                productCategoryID: DBItem.ProductCategoryID,
                productCategory: new ProductCategoryIudDTO
                {
                    ProductCategoryImageFilename = Constants.NullValueFor.String
                }                
            );

            viewModel.IsSuccess = !repository.IsError;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel : FormViewModelBase
        {
            #region Properties
            public int? ProductCategoryID { get; set; }
            public int? ProductCategoryParentID { get; set; }
            public string ProductCategoryName { get; set; }
            public string ProductCategoryNameEng { get; set; }
            public string CategoryImageFilenameProduct { get; set; }
            public string ProductCategoryDescriptionShort { get; set; }
            public string ProductCategoryDescriptionShortEng { get; set; }
            public string ProductCategoryImageFilename { get; set; }
            public string ProductCategoryImageHttpPath { get; set; }
            public bool HasProductCategoryImage => !string.IsNullOrWhiteSpace(ProductCategoryImageFilename);
            public string UrlDeleteImage { get; set; }
            public IFormFile ProductCategoryImageFile { get; set; }

            public readonly string TextName = Resources.TextName;
            public readonly string TextNameEng = Resources.TextNameEng;
            public readonly string TextDescriptionShort = Resources.TextDescriptionShort;
            public readonly string TextDescriptionShortEng = Resources.TextDescriptionShortEng;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            #endregion
        }
        #endregion
    }
}