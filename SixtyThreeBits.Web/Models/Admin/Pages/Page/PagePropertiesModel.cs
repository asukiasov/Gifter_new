using DevExpress.CodeParser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Libraries.FileStorages.Enums;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class PagePropertiesModel : PageModelBase
    {        
        #region Methods
        public ViewModel GetViewModel(ViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
                viewModel.PageIsPublished = Page.PageIsPublished;
                viewModel.PageSlug = Page.PageSlug;
                viewModel.PageTitle = Page.PageTitle;
                viewModel.PageTitleEng = Page.PageTitleEng;
                viewModel.PageShortDescription = Page.PageShortDescription;
                viewModel.PageShortDescriptionEng = Page.PageShortDescriptionEng;
            }

            viewModel.PageImageFilename = Page.PageImageFilename;
            viewModel.PageImageHttpPath = FileStorage.GetUploadedFileHttpPath(
                filename: Page.PageImageFilename,
                folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.Pages)
            );
            viewModel.UrlPreview = $"{WebsiteHttpPath}{Page.PageSlug}";
                
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.PagePropertiesController.DeleteImage, values: new { pageID = Page.PageID });

            return viewModel;
        }
        
        public async Task<ViewModel> Save(ViewModel submitModel)
        {
            var viewModel = GetViewModel(submitModel);

            var validationResult = await ValidateSubmitModel(submitModel);

            if (validationResult.HasErrors)
            {
                viewModel.AddFormErrors(validationResult.Errors);
            }
            else
            {
                var hasPageImage = submitModel.PageImageFile?.Length > 0;
                var pageImageFilename = hasPageImage ? GetFilenameFromUploadedFile(submitModel.PageImageFile) : null;
                if (hasPageImage)
                {
                    await FileStorage.DeleteFile(
                        filename: pageImageFilename,
                        folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.Pages)
                    );
                }

                var repository = RepositoriesFactory.CreatePagesRepository();
                await repository.PagesIUD(
                    databaseAction: Enums.DatabaseActions.UPDATE,
                    pageID: Page.PageID,
                    page: new PageIudDTO
                    {
                        PageSlug = submitModel.PageSlug,
                        PageTitle = submitModel.PageTitle,
                        PageTitleEng = submitModel.PageTitleEng ?? Constants.NullValueFor.String,
                        PageShortDescription = submitModel.PageShortDescription ?? Constants.NullValueFor.String,
                        PageShortDescriptionEng = submitModel.PageShortDescriptionEng ?? Constants.NullValueFor.String,
                        PageImageFilename = pageImageFilename,
                        PageIsPublished = submitModel.PageIsPublished
                    }
                );

                if (repository.IsError)
                {
                    submitModel.AddToastError(repository.ErrorMessage);
                }
                else
                {
                    if (hasPageImage)
                    {
                        await FileStorage.SaveUploadedFile(
                            sourceFileStream: submitModel.PageImageFile.OpenReadStream(),
                            filename: pageImageFilename,
                            folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.Pages)
                        );
                    }
                }
            }

            return viewModel;
        }
        async Task<ValidationResult63> ValidateSubmitModel(ViewModel submitModel)
        {
            var validationResult = new ValidationResult63();
            var error = default(Error63);

            error = Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(submitModel.PageTitle)), valueToValidate: submitModel.PageTitle);
            validationResult.AddError(error);

            error = Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(submitModel.PageSlug)), valueToValidate: submitModel.PageSlug);
            validationResult.AddError(error);

            error = await Validation63.ValidateAsync(
                errorAction: async () =>
                {
                    var repository = RepositoriesFactory.CreatePagesRepository();
                    var isUniq = await repository.PagesIsSlugUniq(pageSlug: submitModel.PageSlug, pageID: Page.PageID);
                    var isError = !isUniq;
                    return isError;
                },
                errorKey: Validation63.GetJQueryNameSelectorFor(nameof(submitModel.PageSlug)),
                errorMessage: Resources.ValidationPagesSlugNotUniq
            );
            validationResult.AddError(error);

            return validationResult;
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();

            await FileStorage.DeleteFile(
                filename: Page.PageImageFilename,
                folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.Pages)
            );

            var repository = RepositoriesFactory.CreatePagesRepository();
            await repository.PagesIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                pageID: Page.PageID,
                page: new PageIudDTO
                {
                    PageImageFilename = Constants.NullValueFor.String
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
            public string PageSlug { get; set; }
            public string PageTitle { get; set; }
            public string PageTitleEng { get; set; }
            public string PageShortDescription { get; set; }
            public string PageShortDescriptionEng { get; set; }
            public string PageImageFilename { get; set; }
            public string PageImageHttpPath { get; set; }
            public bool HasPageImage => !string.IsNullOrWhiteSpace(PageImageFilename);

            public bool PageIsPublished { get; set; }
            public IFormFile PageImageFile { get; set; }
            public string UrlDeleteImage { get; set; }
            public string UrlPreview { get; set; }

            public readonly string TextPreview = Resources.TextPreview;
            public readonly string TextGenerateFromTitle = Resources.TextGenerateFromTitle;
            public readonly string TextPublished = Resources.TextPublished;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            public readonly string TextTitle = Resources.TextTitle;
            public readonly string TextTitleEng = Resources.TextTitleEng;            
            public readonly string TextSlug = Resources.TextSlug;            
            public readonly string TextPageUrl = Resources.TextPageUrl;
            public readonly string TextDescriptionShort = Resources.TextDescriptionShort;
            public readonly string TextDescriptionShortEng = Resources.TextDescriptionShortEng;
            public readonly string TextPageShortDescriptionAndImageInfo = Resources.TextPageShortDescriptionAndImageInfo;
            #endregion
        }
        #endregion
    }
}