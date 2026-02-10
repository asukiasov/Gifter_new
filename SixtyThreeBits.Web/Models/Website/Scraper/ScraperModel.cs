using SixtyThreeBits.Web.Models.Base;
using SixtyThreeBits.Web.Services;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Website
{
    public class ScraperModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            return new ViewModel();
        }

        public async Task<ScraperService.ScrapeResult> ScrapeAsync(string url)
        {
            var scraper = new ScraperService();
            return await scraper.ScrapeAsync(url);
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
        }
        #endregion
    }
}
