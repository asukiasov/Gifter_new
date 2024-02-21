using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/partners")]
    public class PartnersController : AdminControllerBase<PartnersModel>
    {
        #region Constructors
        public PartnersController()
        {
            Model = new PartnersModel();
        }
        #endregion

        #region Actions
        [Route("", Name = ControllerActionRouteNames.Admin.Partners.Page)]
        public IActionResult Partners()
        {
            Model.PluginsClient.EnableDevextreme(true).Enable63BitsForms(true).Enable63BitsComponents(true).EnableTinyMce(true).EnableFancybox(true);
            var viewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.Partners.Page, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Partners.PartnersGrid)]
        public async Task<IActionResult> PartnersGrid()
        {
            var viewModel = await Model.GetGridViewModel();
            return Json(viewModel);
        }

        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Partners.PartnersGridAdd)]
        public async Task<IActionResult> PartnersGridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<PartnersModel.PageViewModel.GridModel.GridItem>() ?? new PartnersModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.CREATE, partnerID: key, submitModel: submitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [Route("grid/update", Name = ControllerActionRouteNames.Admin.Partners.PartnersGridUpdate)]
        public async Task<IActionResult> PartnersGridUpdate(int key, string values)
        {
            var submitModel = values.DeserializeJsonTo<PartnersModel.PageViewModel.GridModel.GridItem>() ?? new PartnersModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.UPDATE, partnerID: key, submitModel: submitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.Partners.PartnersGridDelete)]
        public async Task<IActionResult> PartnersGridDelete(int key)
        {
            await Model.CRUD(databaseAction: Enums.DatabaseActions.DELETE, partnerID: key, submitModel: new PartnersModel.PageViewModel.GridModel.GridItem());
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }
        #endregion 
    }

    [Route("admin/partners/{partnerID:int}")]
    [TypeFilter(typeof(BeforePartnerPageLoad), Order = 2)]
    public class PartnerPropertiesController : AdminControllerBase<PartnerPropertiesModel>
    {
        #region Constructors
        public PartnerPropertiesController()
        {
            Model = new PartnerPropertiesModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("properties", Name = ControllerActionRouteNames.Admin.Partners.Partner.Properties)]
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableTinyMce(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetPageViewModel(viewModel: null);
            return View(ViewNames.Admin.Partners.Partner, viewModel);
        }

        [HttpPost]
        [Route("properties")]
        public async Task<IActionResult> Properties(PartnerPropertiesModel.PageViewModel SubmitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetPageViewModel(viewModel: SubmitModel);
            var result = default(IActionResult);
            Model.ValidatePageViewModel(viewModel: SubmitModel);
            if (viewModel.IsValid)
            {
                await Model.Save(SubmitModel);
                if (viewModel.IsSaved)
                {
                    Model.ShowSuccessToastNotification();
                    result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Partners.Partner.Properties, new { Model.DBItem.PartnerID }));
                }
                else
                {
                    Model.ShowErrorToastNotification();
                    result = View(ViewNames.Admin.Partners.Partner, viewModel);
                }
            }
            else
            {
                result = View(ViewNames.Admin.Partners.Partner, viewModel);
            }
            return result;
        }

        [HttpPost]
        [Route("properties/delete-image", Name = ControllerActionRouteNames.Admin.Partners.Partner.PropertiesDeleteImage)]
        public async Task<IActionResult> PartnerPropertiesDeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion
    }
}
