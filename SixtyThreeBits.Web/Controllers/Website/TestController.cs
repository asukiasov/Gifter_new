using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;

namespace SixtyThreeBits.Web.Controllers.Website
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