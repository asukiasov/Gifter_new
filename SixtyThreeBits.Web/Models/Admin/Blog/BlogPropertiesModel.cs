using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Libraries.FileStorages;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using System;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class BlogPropertiesModel : BlogModelBase
    {
        #region Properties
        readonly string _folderPath = FileStorageManager.Modules[Enums.FileManagerModules.Blog].FolderName;
        #endregion

        #region Methods
        public ViewModel GetViewModel(ViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
                viewModel.BlogPostIsPublished = DBItem.BlogPostIsPublished;
                viewModel.BlogPostSlug = DBItem.BlogPostSlug;
                viewModel.BlogPostTitle = DBItem.BlogPostTitle;
                viewModel.BlogPostShortText = DBItem.BlogPostShortText;
                viewModel.BlogPostText = DBItem.BlogPostText;
                viewModel.BlogPostAuthorName = DBItem.BlogPostAuthorName;
                viewModel.BlogPostDate = DBItem.BlogPostDate;                
            }

            viewModel.BlogPostImageFilename = DBItem.BlogPostImageFilename;
            viewModel.BlogPostImageHttpPath = FileStorage.GetUploadedFileHttpPath(DBItem.BlogPostImageFilename, _folderPath);
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.BlogPostPropertiesController.DeleteImage, new { blogPostID = DBItem.BlogPostID });

            return viewModel;
        }

        public async Task Validate(ViewModel viewModel)
        {
            viewModel.AddError(Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.BlogPostTitle)), valueToValidate: viewModel.BlogPostTitle));
            viewModel.AddError(Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.BlogPostSlug)), valueToValidate: viewModel.BlogPostSlug));
            viewModel.AddError(
                await Validation63.ValidateAsync(
                    errorAction: async () =>
                    {
                        var repository = RepositoriesFactory.CreateBlogRepository();
                        var isUniq = await repository.BlogPostIsSlugUniq(blogPostSlug: viewModel.BlogPostSlug, blogPostID: DBItem.BlogPostID);
                        return !isUniq;
                    },
                    errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.BlogPostSlug)),
                    errorMessage: Resources.ValidationSlugNotUniq
                )
            );
        }

        public async Task Save(ViewModel viewModel)
        {
            var hasBlogImage = viewModel.BlogImageFile?.Length > 0;
            var blogPostImageFilename = hasBlogImage ? GetFilenameFromUploadedFile(viewModel.BlogImageFile) : null;
            if (hasBlogImage)
            {
                await FileStorage.DeleteFile(DBItem.BlogPostImageFilename, _folderPath);
            }

            var repository = RepositoriesFactory.CreateBlogRepository();
            await repository.BlogPostsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                blogPostID: DBItem.BlogPostID,
                blogPost: new BlogPostIudDTO
                {
                    BlogPostSlug = viewModel.BlogPostSlug,
                    BlogPostTitle = viewModel.BlogPostTitle,
                    BlogPostShortText = viewModel.BlogPostShortText,
                    BlogPostText = viewModel.BlogPostText,
                    BlogPostAuthorName = viewModel.BlogPostAuthorName,
                    BlogPostImageFilename = blogPostImageFilename,
                    BlogPostDate = Utilities.FormatDateSqlParseFriendly(viewModel.BlogPostDate),
                    BlogPostIsPublished = viewModel.BlogPostIsPublished
                }                
            );

            if (repository.IsError)
            {
                viewModel.AddError(repository.ErrorMessage);
            }
            else
            {
                if (hasBlogImage)
                {
                    await SaveUploadedFile(viewModel.BlogImageFile, blogPostImageFilename, _folderPath);
                }
            }
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();
            await FileStorage.DeleteFile(DBItem.BlogPostImageFilename, _folderPath);

            var repository = RepositoriesFactory.CreateBlogRepository();
            await repository.BlogPostsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                blogPostID: DBItem.BlogPostID,
                blogPost: new BlogPostIudDTO
                {
                    BlogPostImageFilename = Constants.NullValueFor.String
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
            public bool BlogPostIsPublished { get; set; }
            public string BlogPostSlug { get; set; }
            public string BlogPostTitle { get; set; }
            public string BlogPostShortText { get; set; }
            public string BlogPostText { get; set; }
            public string BlogPostAuthorName { get; set; }
            public DateTime? BlogPostDate { get; set; }            
            public string BlogPostImageFilename { get; set; }
            public string BlogPostImageHttpPath { get; set; }
            public bool HasBlogPostImage => !string.IsNullOrWhiteSpace(BlogPostImageFilename);
            public IFormFile BlogImageFile { get; set; }
            public string UrlDeleteImage { get; set; }

            public readonly string TextPublished = Resources.TextPublished;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            public readonly string TextTitle = Resources.TextTitle;
            public readonly string TextSlug = Resources.TextSlug;
            public readonly string TextGenerateFromTitle = Resources.TextGenerateFromTitle;
            public readonly string TextAuthor = Resources.TextAuthor;
            public readonly string TextDate = Resources.TextDate;
            public readonly string TextDescriptionShort = Resources.TextDescriptionShort;
            public readonly string TextDescription = Resources.TextDescription;

            public readonly string FormatDate = Constants.Formats.Date;
            #endregion
        }
        #endregion
    }
}
