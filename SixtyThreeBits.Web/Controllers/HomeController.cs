using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Reusables.Core;

namespace SixtyThreeBits.Web.Controllers
{
    public class HomeController : Controller
    {
        [Route("", Name = ControllerActionRouteNames.Website.Home.Page)]
        public IActionResult Index()
        {
            return Redirect("/admin/");
            //return View(ViewNames.Website.Home.Page);
        }

        [Route("error/404/")]
        public IActionResult Error()
        {
            return View(ViewNames.Shared.NotFound);
        }
    }
}