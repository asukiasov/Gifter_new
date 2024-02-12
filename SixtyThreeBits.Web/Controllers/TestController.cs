using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Models;

namespace SixtyThreeBits.Web.Controllers
{
    [Route("test")]
    public class TestController : WebsiteControllerBase<TestModel>
    {
        public TestController()
        {
            Model = new TestModel();
        }

        [Route("")]
        public IActionResult Test()
        {
            Model.PluginsClient.Enable63BitsComponents(true);
            return View(ViewNames.Website.Test.Page);
        }        
    }
}