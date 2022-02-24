using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Filters;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Controllers
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
            var ViewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.Partners.Page, ViewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Partners.PartnersGrid)]
        public async Task<IActionResult> PartnersGrid()
        {
            var ViewModel = await Model.GetGridViewModel();
            return Json(ViewModel);
        }

        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Partners.PartnersGridAdd)]
        public async Task<IActionResult> PartnersGridAdd(int? key, string values)
        {
            var SubmitModel = values.DeserializeJsonTo<PartnersModel.PageViewModel.GridModel.GridItem>() ?? new PartnersModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, PartnerID: key, SubmitModel: SubmitModel);
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
            var SubmitModel = values.DeserializeJsonTo<PartnersModel.PageViewModel.GridModel.GridItem>() ?? new PartnersModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, PartnerID: key, SubmitModel: SubmitModel);
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
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.DELETE, PartnerID: key, SubmitModel: new PartnersModel.PageViewModel.GridModel.GridItem());
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

    [Route("admin/partners/{PartnersID:int}")]
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
            Model.PluginsClient.Enable63BitsForms(true).EnableTinyMce(true).EnableFancybox(true);
            var ViewModel = Model.GetPartnerPropertiesViewModel(ViewModel: null);
            return View(ViewNames.Admin.Partners.Partner, ViewModel);
        }

        [HttpPost]
        [Route("properties")]
        public async Task<IActionResult> Properties(PartnerPropertiesModel.PartnerPropertiesViewModel SubmitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true);
            var ViewModel = Model.GetPartnerPropertiesViewModel(ViewModel: SubmitModel);
            var Result = default(IActionResult);
            Model.ValidatePartnerPropertiesViewModel(ViewModel: SubmitModel);
            if (ViewModel.IsValid)
            {
                var IsSaved = await Model.SavePartnerProperties(SubmitModel);
                if (IsSaved)
                {
                    Model.ShowSuccess();
                    Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Partners.Partner.Properties, new { PartnersID = Model.DBItemPartner.PartnerID }));
                }
                else
                {
                    Model.ShowError();
                    Result = View(ViewNames.Admin.Partners.Partner, ViewModel);
                }
            }
            else
            {
                Result = View(ViewNames.Admin.Partners.Partner, ViewModel);
            }
            return Result;
        }

        [HttpPost]
        [Route("properties/delete-image", Name = ControllerActionRouteNames.Admin.Partners.PartnersPartnerPropertiesDeleteImage)]
        public async Task<IActionResult> PartnerPropertiesDeleteImage(int? PartnersID)
        {
            var Result = await Model.DeleteImage(PartnersID);
            return Json(Result);
        }
        #endregion
    }
}
