using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

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

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> TestUpload(int? ProductID, string ProductName)
        {
            foreach (var File in Request.Form.Files)
            {                
                await Model.SaveUploadedFile(File, File.FileName);
            }
            var Result = new AjaxResponse { IsSuccess = true };
            return Json(Result);
        }        
    }
}