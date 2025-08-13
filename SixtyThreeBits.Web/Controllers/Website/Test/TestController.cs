using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;
using System;

namespace SixtyThreeBits.Web.Controllers.Website
{
    [Route("test")]
    public class TestController : WebsiteControllerBase<TestModel>
    {
        #region Actions
        [Route("")]
        public IActionResult Test()
        {
            Model.PluginsClient.Enable63BitsComponents(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Website.Test.TestView, viewModel);
        }

		[Route("exception/{a}/{b}")]
		public IActionResult Test(int a, int b)
		{
			try
			{
				var c = a / b;
			}
			catch (Exception ex)
			{
				throw new Exception("There is an error in the system", ex);
			}

			return Ok();
		}        
		#endregion
	}
}