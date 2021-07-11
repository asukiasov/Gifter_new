using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Filters;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Controllers
{
    [Route("admin/brands")]
    public class BrandsController: AdminControllerBase<BrandsModel>
    {
        #region Constructors
        public BrandsController()
        {
            Model = new BrandsModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Brands.Index)]
        public ActionResult Brands()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var ViewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.Brands.Page, ViewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Brands.BrandsGrid)]
        public async Task<ActionResult> BrandsGrid()
        {
            var ViewModel = await Model.GetGridViewModel();
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Brands.BrandsGridAdd)]
        public async Task<ActionResult> BrandsGridAdd(int? key, string values)
        {
            var SubmitModel = values.FromJsonTo<BrandsModel.PageViewModel.GridModel.GridItem>() ?? new BrandsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, BrandID: key, SubmitModel: SubmitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.Brands.BrandsGridUpdate)]
        public async Task<ActionResult> BrandsGridUpdate(int? key, string values)
        {
            var SubmitModel = values.FromJsonTo<BrandsModel.PageViewModel.GridModel.GridItem>() ?? new BrandsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, BrandID: key, SubmitModel: SubmitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.Brands.BrandsGridDelete)]
        public async Task<ActionResult> BrandsGridDelete(int? key)
        {

            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.DELETE, BrandID: key, SubmitModel: new BrandsModel.PageViewModel.GridModel.GridItem());
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

    [Route("admin/brands/{BrandID:int}/properties")]
    [TypeFilter(typeof(BeforeBrandPageLoad), Order = 2)]
    public class BrandsPropertiesController : AdminControllerBase<BrandsPropertiesModel>
    {
        #region Constructors
        public BrandsPropertiesController()
        {
            Model = new BrandsPropertiesModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Brands.Brand.Properties)]
        public IActionResult Properties(int? BrandID)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true);
            var ViewModel = Model.GetPageViewModel(BrandID, ViewModel: null);
            return View(ViewNames.Admin.Brands.Brand.Properties, ViewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(int? BrandID, BrandsPropertiesModel.BrandsPropertiesViewModel SubmitModel)
        {
            var Result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true);
            var ViewModel = Model.GetPageViewModel(BrandID, SubmitModel);
            Model.ValidatePageViewModel(ViewModel);
            if (ViewModel.IsValid)
            {
                await Model.SaveBrandsProperties(BrandID, ViewModel);
                if (ViewModel.IsSaved)
                {
                    Model.ShowSuccess();
                    Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Brands.Brand.Properties, new { BrandID = BrandID }));
                }
                else
                {
                    Model.ShowError();
                    Result = View(ViewNames.Admin.Brands.Brand.Properties, ViewModel);
                }
            }
            else
            {
                Result = View(ViewNames.Admin.Brands.Brand.Properties, ViewModel);
            }
            return Result;
        }

        [HttpPost]
        [Route("delete-image", Name = ControllerActionRouteNames.Admin.Brands.Brand.DeleteCoverImage)]
        public async Task<IActionResult> BrandsItemDeleteImage(int? BrandID)
        {
            var Result = await Model.DeleteImage(BrandID);
            return Json(Result);
        }
        #endregion
    }
}
