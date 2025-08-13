using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/pages/{PageID:int}/metadata")]
    [TypeFilter(typeof(PageFilterAttribute), Order = 2)]
    public class PageMetaDataController : AdminControllerBase<PageMetaDataModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.PageMetaDataController.Get)]
        public IActionResult Get()
        {            
            var viewModel = Model.GetPageData();
            return Json(viewModel);
        }        
        #endregion
    }    
}