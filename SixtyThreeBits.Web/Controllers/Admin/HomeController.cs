using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Models.Admin;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin")]
    public class HomeController : AdminControllerBase<HomeModel>
    {
        #region Constructors
        public HomeController()
        {
            Model = new HomeModel();
        }
        #endregion

        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Home.Page)]
        public IActionResult Index()
        {
            return View(ViewNames.Admin.Home.Index);
        }
    }
}