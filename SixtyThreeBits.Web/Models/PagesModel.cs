using SixtyThreeBits.Web.Domain;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models
{
    public class PagesModel : WebProjectModelBase
    {
        #region Methods
        public async Task<PageViewModel> GetPageViewModel(string PageSlug)
        {
            var viewModel = default(PageViewModel);
            var repository = RepositoriesFactory.GetPagesRepository(); ;            
            var dbItem = await repository.PagesGetSingleBySlugHierarchy(pageSlug: PageSlug?.Trim('/'));
            if (dbItem !=null && (dbItem.PageIsPublished || User?.UserIsSuperAdmin == true))
            {
                viewModel = new PageViewModel();
                viewModel.PageTitle = Utilities.GetValuesByLanguage(LanguageCultureCode, dbItem.PageTitle, dbItem.PageTitleEng);
                viewModel.PageShortDescription = Utilities.GetValuesByLanguage(LanguageCultureCode, dbItem.PageShortDescription, dbItem.PageShortDescriptionEng);
                viewModel.PageText = Utilities.GetValuesByLanguage(LanguageCultureCode, dbItem.PageText, dbItem.PageTextEng);
                viewModel.PageTextHeaderHtml = Utilities.GetValuesByLanguage(LanguageCultureCode, dbItem.PageTextHeaderHtml, dbItem.PageTextHeaderHtmlEng);
                viewModel.PageTextFooterHtml = Utilities.GetValuesByLanguage(LanguageCultureCode, dbItem.PageTextFooterHtml, dbItem.PageTextFooterHtmlEng);
                viewModel.PageImageHttpPath = FileStorage.GetUploadedFileHttpPathOrDefault(dbItem.PageImageFilename);                
            }
            return viewModel;
        }        
        #endregion

        #region Nested Classes
        public class PageViewModel
        {
            #region Properties
            public string PageTitle { get; set; }
            public string PageShortDescription { get; set; }
            public string PageText { get; set; }
            public string PageTextHeaderHtml { get; set; }
            public string PageTextFooterHtml { get; set; }
            public string PageImageHttpPath { get; set; }
            #endregion
        } 
        #endregion
    }
}
