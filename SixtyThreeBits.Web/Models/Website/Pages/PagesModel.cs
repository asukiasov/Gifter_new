using SixtyThreeBits.Core.Libraries.FileStorages.Enums;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System.Threading.Tasks;
using System.Web;

namespace SixtyThreeBits.Web.Models.Website
{
    public class PagesModel : ModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel(string PageSlug)
        {
            var viewModel = default(ViewModel);
            var repository = RepositoriesFactory.CreatePagesRepository(); ;
            var dbItem = await repository.PagesGetSingleBySlug(pageSlug: PageSlug?.Trim('/'));
            if (dbItem != null && (dbItem.PageIsPublished || User?.RoleCode == Enums.RolesCodes.Administrator))
            {
                viewModel = new ViewModel();
                viewModel.PageTitle = Utilities.GetValuesByLanguage(LanguageCultureCode, dbItem.PageTitle, dbItem.PageTitleEng);
                viewModel.PageShortDescription = Utilities.GetValuesByLanguage(LanguageCultureCode, dbItem.PageShortDescription, dbItem.PageShortDescriptionEng);
                viewModel.PageText = Utilities.GetValuesByLanguage(LanguageCultureCode, dbItem.PageText, dbItem.PageTextEng);
                viewModel.PageTextHeaderHtml = Utilities.GetValuesByLanguage(LanguageCultureCode, dbItem.PageTextHeaderHtml, dbItem.PageTextHeaderHtmlEng);
                viewModel.PageTextFooterHtml = Utilities.GetValuesByLanguage(LanguageCultureCode, dbItem.PageTextFooterHtml, dbItem.PageTextFooterHtmlEng);
                viewModel.PageImageHttpPath = FileStorage.GetUploadedFileHttpPath(
                    filename: dbItem.PageImageFilename,
                    folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.Pages)
                );
                    
                viewModel.PageOgImageHttpPath = string.IsNullOrWhiteSpace(dbItem.PageImageFilename) ? null : FileStorage.GetUploadedFileHttpPath(
                    filename: HttpUtility.UrlEncode(dbItem.PageImageFilename),
                    folderPath: FileStorage.GetFolderPathByModule(FileManagerModules.Pages)
                );
            }
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public string PageTitle { get; set; }
            public string PageShortDescription { get; set; }
            public string PageText { get; set; }
            public string PageTextHeaderHtml { get; set; }
            public string PageTextFooterHtml { get; set; }
            public string PageImageHttpPath { get; set; }
            public string PageOgImageHttpPath { get; set; }
            public bool HasPageImageHttpPath => !string.IsNullOrWhiteSpace(PageOgImageHttpPath);
            #endregion
        }
        #endregion
    }
}