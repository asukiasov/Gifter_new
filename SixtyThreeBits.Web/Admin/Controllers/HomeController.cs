using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;

namespace SixtyThreeBits.Web.Admin.Controllers
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
        public ActionResult Index()
        {            
            return View(ViewNames.Admin.Home.Index);
        }        
    }
}