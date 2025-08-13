using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Obsolete("NEED TO TAKE CARE FOR ROUTING")]
    [Route("admin/pages/{PageID:int}/data")]
    [TypeFilter(typeof(PageFilterAttribute), Order = 2)]
    public class PageMetaDataController : AdminControllerBase<PageDataModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.PageDataController.Get)]
        public IActionResult Get()
        {            
            var viewModel = Model.GetPageData();
            return Json(viewModel);
        }        
        #endregion
    }    
}