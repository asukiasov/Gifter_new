using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.Validation;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using System.Threading.Tasks;


namespace SixtyThreeBits.Web.Models.Admin
{
    public class BrandPropertiesModel : BrandModelBase
    {
        #region Methods
        public ViewModel GetViewModel(ViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
                viewModel.BrandName = DBItem.BrandName;
                viewModel.BrandNameEng = DBItem.BrandNameEng;
            }
            viewModel.BrandImageFilename = DBItem.BrandImageFilename;
            viewModel.BrandImageHttpPath = FileStorage.GetUploadedFileHttpPath(DBItem.BrandImageFilename);
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.BrandPropertiesController.DeleteImage, new { brandID = DBItem.BrandID });

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
                var brandImageFilename = default(string);
                var hasBrandImage = viewModel.BrandImageFile?.Length > 0;
                if (hasBrandImage)
                {
                    await FileStorage.DeleteFile(viewModel.BrandImageFilename);
                    brandImageFilename = GetFilenameFromUploadedFile(viewModel.BrandImageFile);
                }

                var repository = RepositoriesFactory.CreateBrandsRepository();
                await repository.BrandsIUD(
                    databaseAction: Enums.DatabaseActions.UPDATE,
                    brandID: DBItem.BrandID,
                    brand: new BrandIudDTO
                    {
                        BrandName = viewModel.BrandName,
                        BrandNameEng = viewModel.BrandNameEng,
                        BrandImageFilename = brandImageFilename
                    }
                );

                if (repository.IsError)
                {
                    viewModel.AddToastError(repository.ErrorMessage);
                }
                else
                {
                    if (hasBrandImage)
                    {
                        await FileStorage.SaveUploadedFile(
                            sourceFileStream: viewModel.BrandImageFile.OpenReadStream(),
                            filename: brandImageFilename
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

            error = Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(submitModel.BrandName)), valueToValidate: submitModel.BrandName);
            validationResult.AddError(error);

            return validationResult;
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateBrandsRepository();

            await FileStorage.DeleteFile(DBItem.BrandImageFilename);

            await repository.BrandsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                brandID: DBItem.BrandID,
                brand: new BrandIudDTO
                {
                    BrandImageFilename = Constants.NullValueFor.String
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
            public string BrandName { get; set; }
            public string BrandNameEng { get; set; }
            public string BrandImageFilename { get; set; }
            public string BrandImageHttpPath { get; set; }
            public bool HasBrandImage => !string.IsNullOrWhiteSpace(BrandImageFilename);
            public string UrlDeleteImage { get; set; }
            public IFormFile BrandImageFile { get; set; }

            public readonly string TextName = Resources.TextName;
            public readonly string TextNameEng = Resources.TextNameEng;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            #endregion
        }
        #endregion
    }
}