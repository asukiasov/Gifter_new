using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/email-templates/{emailTemplateID:int}/properties")]
    public class EmailTemplatePropertiesController : AdminControllerBase<EmailTemplatePropertiesModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.EmailTemplatePropertiesController.Properties)]
        public async Task<IActionResult> EmailTemplateProperties(int? emailTemplateID)
        {
            var result = default(IActionResult);
            var viewModel = await Model.GetViewModel(emailTemplateID);
            if (viewModel == null)
            {
                result = Model.GetNotFoundWebsiteViewResult();
            }
            else
            {
                Model.PluginsClient.EnableTinyMce(true).Enable63BitsForms(true).Enable63BitsSuccessErrorToast(true);
                Model.PageTitle.Set(viewModel.EmailTemplateName);
                Model.Breadcrumbs.RenameLastItem(viewModel.EmailTemplateName);
                result = View(ViewNames.Admin.EmailTemplates.EmailTemplatePropertiesView, viewModel);
            }
            return result;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> EmailTemplateProperties(int? emailTemplateID, EmailTemplatePropertiesModel.ViewModel submitModel)
        {
            Model.PluginsClient.EnableTinyMce(true).Enable63BitsForms(true).Enable63BitsSuccessErrorToast(true);

            var viewModel = await Model.Save(emailTemplateID, submitModel);

            if(viewModel.HasErrors)
            {
                Model.PageTitle.Set(viewModel.EmailTemplateName);
                Model.Breadcrumbs.RenameLastItem(viewModel.EmailTemplateName);
                return View(ViewNames.Admin.EmailTemplates.EmailTemplatePropertiesView, viewModel);
            }
            else
            {
                Model.ShowSuccessToastNotification();
                return Redirect(Model.UrlCurrentPageWithDomain);
            }
        }
        #endregion
    }
}