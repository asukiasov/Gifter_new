using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class ContactController : WebsiteControllerBase<ContactModel>
    {
        #region Actions
        [Route("contact", Name = ControllerActionRouteNames.Website.ContactController.Contact)]
        [Route($"{RouteValueLanguageCode}/contact", Name = ControllerActionRouteNames.Website.ContactController.ContactCulture)]
        public IActionResult Contact()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnablePreloader(true).EnableGoogleRecaptcha(true);
            var viewModel = Model.GetViewModel();
            Model.PageTitle.Set(viewModel.TextContactUs);
            return View(ViewNames.Website.Contact.ContactView, viewModel);
        }

        [HttpPost]
        [Route("contact")]
        [Route($"{RouteValueLanguageCode}/contact")]
        public async Task<IActionResult> ContactSubmit(ContactModel.SubmitModel submitModel)
        {
            var viewModel = await Model.SendContactEmail(submitModel);
            return Json(viewModel);
        }
        #endregion
    }
}
