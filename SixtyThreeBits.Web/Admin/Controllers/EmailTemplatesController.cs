using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
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
            var ViewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.EmailTemplates.Page, ViewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.EmailTemplates.Grid)]
        public async Task<IActionResult> EmailTemplateGrid()
        {
            var ViewModel = await Model.GetGridModel();
            return Json(ViewModel);
        }
        #endregion
    }

    [Route("admin/email-templates/{EmailTemplateID}/properties")]
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
        public async Task<IActionResult> EmailTemplateProperties(int? EmailTemplateID)
        {
            var Result = default(IActionResult);
            var ViewModel = await Model.GetPageViewModel(EmailTemplateID);
            if(ViewModel == null)
            {
                Result = Model.GetNotFoundViewResult();
            }
            else
            {
                Model.PluginsClient.EnableTinyMce(true).Enable63BitsForms(true);
                Model.PageTitle.Set(ViewModel.EmailTemplateName);
                Model.Breadcrumbs.RenameLastItem(ViewModel.EmailTemplateName);
                Result = View(ViewNames.Admin.EmailTemplates.EmailTemplate.Properties, ViewModel);
            }
            return Result;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> EmailTemplateProperties(int? EmailTemplateID, EmailTemplatePropertiesModel.PageViewModel SubmitModel)
        {
            var Result = default(IActionResult);
            var ViewModel = await Model.GetPageViewModel(EmailTemplateID, SubmitModel);
            if (ViewModel == null)
            {
                Result = Model.GetNotFoundViewResult();
            }
            else
            {
                Model.PluginsClient.EnableTinyMce(true).Enable63BitsForms(true);
                Model.PageTitle.Set(ViewModel.EmailTemplateName);
                Model.Breadcrumbs.RenameLastItem(ViewModel.EmailTemplateName);

                Model.ValidatePageViewModel(ViewModel);
                if (ViewModel.IsValid)
                {
                    await Model.Save(EmailTemplateID, ViewModel);
                    if (ViewModel.IsSaved)
                    {
                        Result = Redirect(Model.Url.RouteUrl(ControllerActionRouteNames.Admin.EmailTemplates.EmailTemplate.Properties, new { EmailTemplateID = EmailTemplateID }));
                        Model.ShowSuccess();
                    }
                    else
                    {
                        Result = View(ViewNames.Admin.EmailTemplates.EmailTemplate.Properties, ViewModel);
                        Model.ShowError(ViewModel.ErrorMessage);
                    }
                }
                else
                {
                    Result = View(ViewNames.Admin.EmailTemplates.EmailTemplate.Properties, ViewModel);
                }
            }

            return Result;
        }
        #endregion
    }
}