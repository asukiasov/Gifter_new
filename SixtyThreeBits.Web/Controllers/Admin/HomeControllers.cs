using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin")]
    public class HomeController : AdminControllerBase<HomeModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.HomeController.Index)]
        public IActionResult Index()
        {
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.Home.IndexView, viewModel);
        } 
        #endregion
    }
}