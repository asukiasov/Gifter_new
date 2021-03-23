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
                    ViewModel.PageImageHttpPath = DBItem.HasPageImage ? $"{WebsiteDomain}{DBItem.PageImageFilenameHttpPath}" : $"{WebsiteDomain}{AppSettings.OgImageDefaultHttpPath}";
                }
            }
            return ViewModel;
        }

        public async Task<StaticPageViewModel> GetStaticPagePreviewViewModel(int? PageID)
        {
            var ViewModel = default(StaticPageViewModel);
            var DBItem = await DataAccessFactory.Pages.GetSinglePageByID(PageID: PageID);
            if (DBItem != null)
            {
                ViewModel = new StaticPageViewModel();
                ViewModel.PageTitle = Utilities.GetValuesByLanguage(Culture, DBItem.PageTitle, DBItem.PageTitleEng, DBItem.PageTitleRus);
                ViewModel.PageShortDescription = Utilities.GetValuesByLanguage(Culture, DBItem.PageShortDescription, DBItem.PageShortDescriptionEng, DBItem.PageShortDescriptionRus);
                ViewModel.PageText = Utilities.GetValuesByLanguage(Culture, DBItem.PageText, DBItem.PageTextEng, DBItem.PageTextRus);
                ViewModel.PageImageHttpPath = DBItem.HasPageImage ? $"{WebsiteDomain}{DBItem.PageImageFilenameHttpPath}" : $"{WebsiteDomain}{AppSettings.OgImageDefaultHttpPath}";
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
            public string PageImageHttpPath { get; set; }
            #endregion
        } 
        #endregion
    }
}
