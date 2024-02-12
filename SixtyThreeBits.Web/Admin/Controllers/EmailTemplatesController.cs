using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Domain;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Controllers
{
    [Route("admin/email-templates")]
    public class EmailTemplatesController : AdminControllerBase<EmailTemplatesModel>
    {
        #region Constructors
        public EmailTemplatesController()
        {
            Model = new EmailTemplatesModel();
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.EmailTemplates.Page)]
        public IActionResult EmailTemplates()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.EmailTemplates.Page, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.EmailTemplates.Grid)]
        public async Task<IActionResult> EmailTemplateGrid()
        {
            var viewModel = await Model.GetGridModel();
            return Json(viewModel);
        }
        #endregion
    }

    [Route("admin/email-templates/{emailTemplateID:int}/properties")]
    public class EmailTemplatePropertiesController : AdminControllerBase<EmailTemplatePropertiesModel>
    {
        #region Constructors
        public EmailTemplatePropertiesController()
        {
            Model = new EmailTemplatePropertiesModel();
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.EmailTemplates.EmailTemplate.Properties)]
        public async Task<IActionResult> EmailTemplateProperties(int? emailTemplateID)
        {
            var result = default(IActionResult);
            var viewModel = await Model.GetPageViewModel(emailTemplateID);
            if(viewModel == null)
            {
                result = Model.GetNotFoundViewResult();
            }
            else
            {
                Model.PluginsClient.EnableTinyMce(true).Enable63BitsForms(true);
                Model.PageTitle.Set(viewModel.EmailTemplateName);
                Model.Breadcrumbs.RenameLastItem(viewModel.EmailTemplateName);
                result = View(ViewNames.Admin.EmailTemplates.EmailTemplate.Properties, viewModel);
            }
            return result;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> EmailTemplateProperties(int? emailTemplateID, EmailTemplatePropertiesModel.PageViewModel submitModel)
        {
            var result = default(IActionResult);
            var viewModel = await Model.GetPageViewModel(emailTemplateID, submitModel);
            if (viewModel == null)
            {
                result = Model.GetNotFoundViewResult();
            }
            else
            {
                Model.PluginsClient.EnableTinyMce(true).Enable63BitsForms(true);
                Model.PageTitle.Set(viewModel.EmailTemplateName);
                Model.Breadcrumbs.RenameLastItem(viewModel.EmailTemplateName);

                Model.ValidatePageViewModel(viewModel);
                if (viewModel.IsValid)
                {
                    await Model.Save(emailTemplateID, viewModel);
                    if (viewModel.IsSaved)
                    {
                        result = Redirect(Model.Url.RouteUrl(ControllerActionRouteNames.Admin.EmailTemplates.EmailTemplate.Properties, new { emailTemplateID = emailTemplateID }));
                        Model.ShowSuccess();
                    }
                    else
                    {
                        result = View(ViewNames.Admin.EmailTemplates.EmailTemplate.Properties, viewModel);
                        Model.ShowError(viewModel.ErrorMessage);
                    }
                }
                else
                {
                    result = View(ViewNames.Admin.EmailTemplates.EmailTemplate.Properties, viewModel);
                }
            }

            return result;
        }
        #endregion
    }
}