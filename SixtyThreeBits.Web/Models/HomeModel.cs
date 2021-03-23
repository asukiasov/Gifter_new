using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models
{
    public class HomeModel : WebProjectModelBase
    {
        #region Methods
        public async Task<StaticPageViewModel> GetStaticPageViewModel(string PageSlug)
        {
            var ViewModel = default(StaticPageViewModel);
            var DBItem = await DataAccessFactory.Pages.GetSinglePageBySlug(PageSlug: PageSlug, IsPublished: true);
            if (DBItem != null)
            {
                if (DBItem.PageIsPublished || User?.UserIsAdmin == true)
                {
                    ViewModel = new StaticPageViewModel();
                    ViewModel.PageTitle = Utilities.GetValuesByLanguage(Culture, DBItem.PageTitle, DBItem.PageTitleEng, DBItem.PageTitleRus);
                    ViewModel.PageShortDescription = Utilities.GetValuesByLanguage(Culture, DBItem.PageShortDescription, DBItem.PageShortDescriptionEng, DBItem.PageShortDescriptionRus);
                    ViewModel.PageText = Utilities.GetValuesByLanguage(Culture, DBItem.PageText, DBItem.PageTextEng, DBItem.PageTextRus);
                    ViewModel.PageTextHeaderHtml = Utilities.GetValuesByLanguage(Culture, DBItem.PageTextHeaderHtml, DBItem.PageTextHeaderHtmlEng, DBItem.PageTextHeaderHtmlRus);
                    ViewModel.PageTextFooterHtml = Utilities.GetValuesByLanguage(Culture, DBItem.PageTextFooterHtml, DBItem.PageTextFooterHtmlEng, DBItem.PageTextFooterHtmlRus);
                    ViewModel.PageImageHttpPath = DBItem.HasPageImage ? $"{WebsiteDomain}{DBItem.PageImageFilenameHttpPath}" : $"{WebsiteDomain}{AppSettings.OgImageDefaultHttpPath}";
                }
            }
            return ViewModel;
        }        
        #endregion

        #region Sub Classes
        public class StaticPageViewModel
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
