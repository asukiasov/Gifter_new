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
using System;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class NewsPropertiesModel : NewsModelBase
    {
        #region Methods
        public ViewModel GetViewModel(ViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
                viewModel.NewsSlug = NewsItem.NewsSlug;
                viewModel.NewsTitle = NewsItem.NewsTitle;
                viewModel.NewsTitleEng = NewsItem.NewsTitleEng;
                viewModel.NewsShortDescription = NewsItem.NewsShortDescription;
                viewModel.NewsShortDescriptionEng = NewsItem.NewsShortDescriptionEng;
                viewModel.NewsText = NewsItem.NewsText;
                viewModel.NewsTextEng = NewsItem.NewsTextEng;
                viewModel.NewsIsPublished = NewsItem.NewsIsPublished;
                viewModel.NewsDatePublished = NewsItem.NewsDatePublished;
            }

            viewModel.NewsImageFilename = NewsItem.NewsImageFilename;
            viewModel.NewsImageHttpPath = FileStorage.GetUploadedFileHttpPath(
                filename: NewsItem.NewsImageFilename, 
                folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.News)
            );
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.NewsPropertiesController.DeleteImage, new { newsID = NewsItem.NewsID });            
            viewModel.UrlFileManager = UrlFactory.CreateFileManagerAdminUrl(fileManagerModule: FileManagerModules.News);

            return viewModel;
        }

        public async Task ValidateViewModel(ViewModel viewModel)
        {
            viewModel.AddError(Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.NewsTitle)), valueToValidate: viewModel.NewsTitle));
            viewModel.AddError(Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.NewsSlug)), valueToValidate: viewModel.NewsSlug));
            viewModel.AddError(
                await Validation63.ValidateAsync(
                    errorAction: async () =>
                    {
                        var repository = RepositoriesFactory.CreateNewsRepository();
                        var IsUniq = await repository.NewsIsSlugUniq(newsSlug: viewModel.NewsSlug, newsID: NewsItem.NewsID);
                        return !IsUniq;
                    },
                    errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.NewsSlug)),
                    errorMessage: Resources.ValidationSlugNotUniq
                )
            );
        }

        public async Task Save(ViewModel viewModel)
        {
            var hasNewsImage = viewModel.NewsImageFile?.Length > 0;
            var newsImageFilename = hasNewsImage ? GetFilenameFromUploadedFile(viewModel.NewsImageFile) : null;
            if (hasNewsImage)
            {
                await FileStorage.DeleteFile(
                    filename: newsImageFilename,
                    folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.News)
                );
            }

            var repository = RepositoriesFactory.CreateNewsRepository();
            await repository.NewsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                newsID: NewsItem.NewsID,
                news: new NewsIudDTO
                {
                    NewsSlug = viewModel.NewsSlug,
                    NewsTitle = viewModel.NewsTitle,
                    NewsTitleEng = viewModel.NewsTitleEng ?? Constants.NullValueFor.String,
                    NewsShortDescription = viewModel.NewsShortDescription ?? Constants.NullValueFor.String,
                    NewsShortDescriptionEng = viewModel.NewsShortDescriptionEng ?? Constants.NullValueFor.String,
                    NewsText = viewModel.NewsText ?? Constants.NullValueFor.String,
                    NewsTextEng = viewModel.NewsTextEng ?? Constants.NullValueFor.String,
                    NewsImageFilename = newsImageFilename,
                    NewsDatePublished = Utilities.FormatDateSqlParseFriendly(viewModel.NewsDatePublished),
                    NewsIsPublished = viewModel.NewsIsPublished
                }
            );

            if (repository.IsError)
            {
                viewModel.AddError(repository.ErrorMessage);
            }
            else
            {
                if (hasNewsImage)
                {
                    await FileStorage.SaveUploadedFile(
                       sourceFileStream: viewModel.NewsImageFile.OpenReadStream(),
                       filename: newsImageFilename,
                       folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.News)
                   );                    
                }
            }
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();

            await FileStorage.DeleteFile(
                filename: NewsItem.NewsImageFilename,
                folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.News)
            );

            var repository = RepositoriesFactory.CreateNewsRepository();
            await repository.NewsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                newsID: NewsItem.NewsID,
                news: new NewsIudDTO
                {
                    NewsImageFilename = Constants.NullValueFor.String
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
            public string NewsSlug { get; set; }
            public string NewsTitle { get; set; }
            public string NewsTitleEng { get; set; }
            public string NewsShortDescription { get; set; }
            public string NewsShortDescriptionEng { get; set; }
            public string NewsText { get; set; }
            public string NewsTextEng { get; set; }
            public DateTime? NewsDatePublished { get; set; }
            public bool NewsIsPublished { get; set; }
            public string NewsImageFilename { get; set; }
            public string NewsImageHttpPath { get; set; }
            public bool HasNewsImage => !string.IsNullOrWhiteSpace(NewsImageFilename);
            public IFormFile NewsImageFile { get; set; }
            public string UrlDeleteImage { get; set; }
            public string UrlFileManager { get; set; }

            public readonly string FormatDate = Constants.Formats.Date;

            public readonly string TextPublished = Resources.TextPublished;
            public readonly string TextTitle = Resources.TextTitle;
            public readonly string TextTitleEng = Resources.TextTitleEng;
            public readonly string TextSlug = Resources.TextSlug;
            public readonly string TextGenerateFromTitle = Resources.TextGenerateFromTitle;
            public readonly string TextDate = Resources.TextDate;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            public readonly string TextDescriptionShort = Resources.TextDescriptionShort;
            public readonly string TextDescriptionShortEng = Resources.TextDescriptionShortEng;
            public readonly string TextDescription = Resources.TextDescription;
            public readonly string TextDescriptionEng = Resources.TextDescriptionEng;
            #endregion
        }
        #endregion
    }
}