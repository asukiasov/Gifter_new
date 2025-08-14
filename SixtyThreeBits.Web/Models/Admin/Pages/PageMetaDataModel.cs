using SixtyThreeBits.Libraries;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class PageMetaDataModel : PageModelBase
    {
        #region Methods
        public AjaxResponse GetPageData()
        {
            var viewModel = new AjaxResponse();
            viewModel.IsSuccess = true;
            viewModel.Data = new
            {
                PageID = Page.PageID,
                PageTitle = Page.PageTitle,
                PageTitleEng = Page.PageTitleEng,
                PageSlug = Page.PageSlug,
                PageIsPublished = Page.PageIsPublished
            };
            return viewModel;
        }
        #endregion
    }    
}