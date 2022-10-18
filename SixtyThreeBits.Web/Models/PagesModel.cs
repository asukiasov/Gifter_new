using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models
{
    public class PagesModel : WebProjectModelBase
    {
        #region Methods
        public async Task<PageViewModel> GetPageViewModel(string PageSlug)
        {
            var ViewModel = default(PageViewModel);
            var DBItem = await DataAccessFactory.Pages.GetSinglePageBySlugHierarchy(PageSlug: PageSlug?.Trim('/'));
            if (DBItem !=null && (DBItem.PageIsPublished || User?.UserIsAdmin == true))
            {
                ViewModel = new PageViewModel();
                ViewModel.PageTitle = Utilities.GetValuesByLanguage(Culture, DBItem.PageTitle, DBItem.PageTitleEng, DBItem.PageTitleRus);
                ViewModel.PageShortDescription = Utilities.GetValuesByLanguage(Culture, DBItem.PageShortDescription, DBItem.PageShortDescriptionEng, DBItem.PageShortDescriptionRus);
                ViewModel.PageText = Utilities.GetValuesByLanguage(Culture, DBItem.PageText, DBItem.PageTextEng, DBItem.PageTextRus);
                ViewModel.PageTextHeaderHtml = Utilities.GetValuesByLanguage(Culture, DBItem.PageTextHeaderHtml, DBItem.PageTextHeaderHtmlEng, DBItem.PageTextHeaderHtmlRus);
                ViewModel.PageTextFooterHtml = Utilities.GetValuesByLanguage(Culture, DBItem.PageTextFooterHtml, DBItem.PageTextFooterHtmlEng, DBItem.PageTextFooterHtmlRus);
                ViewModel.PageImageHttpPath = string.IsNullOrWhiteSpace(DBItem.PageImageFilename)
                    ? $"{WebsiteDomain}{AppSettings.OgImageDefaultHttpPath}"
                    : $"{WebsiteDomain}{DataAccessFactory.Pages.GetPageFileHttpPath(DBItem.PageID, DBItem.PageImageFilename)}";
            }
            return ViewModel;
        }        
        #endregion

        #region Sub Classes
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
