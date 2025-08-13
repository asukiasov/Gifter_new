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
                PageID = DBItem.PageID,
                PageTitle = DBItem.PageTitle,
                PageTitleEng = DBItem.PageTitleEng,
                PageSlug = DBItem.PageSlug,
                PageIsPublished = DBItem.PageIsPublished
            };
            return viewModel;
        }
        #endregion
    }    
}