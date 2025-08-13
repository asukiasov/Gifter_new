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
        public async Task<IActionResult> EmailTemplateProperties(int? emailTemplateID, EmailTemplatePropertiesModel.ViewModel viewModel)
        {
            var result = default(IActionResult);
            viewModel = await Model.GetViewModel(emailTemplateID, viewModel);
            if (viewModel == null)
            {
                result = Model.GetNotFoundWebsiteViewResult();
            }
            else
            {
                Model.PluginsClient.EnableTinyMce(true).Enable63BitsForms(true).Enable63BitsSuccessErrorToast(true);
                Model.PageTitle.Set(viewModel.EmailTemplateName);
                Model.Breadcrumbs.RenameLastItem(viewModel.EmailTemplateName);

                Model.ValidateViewModel(viewModel);
                if (viewModel.IsValid)
                {
                    await Model.Save(emailTemplateID, viewModel);
                    if (viewModel.IsValid)
                    {
                        result = Redirect(Model.Url.RouteUrl(ControllerActionRouteNames.Admin.EmailTemplatePropertiesController.Properties, new { emailTemplateID }));
                        Model.ShowSuccessToastNotification();
                    }
                    else
                    {
                        result = View(ViewNames.Admin.EmailTemplates.EmailTemplatePropertiesView, viewModel);
                        Model.ShowErrorToastNotification(viewModel.ErrorMessage);
                    }
                }
                else
                {
                    result = View(ViewNames.Admin.EmailTemplates.EmailTemplatePropertiesView, viewModel);
                }
            }

            return result;
        }
        #endregion
    }
}