using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Admin.Filters;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Domain;
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
            var viewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.Brands.Page, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Brands.BrandsGrid)]
        public async Task<ActionResult> BrandsGrid()
        {
            var viewModel = await Model.GetGridViewModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Brands.BrandsGridAdd)]
        public async Task<ActionResult> BrandsGridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<BrandsModel.PageViewModel.GridModel.GridItem>() ?? new BrandsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, brandID: key, submitModel: submitModel);
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
            var submitModel = values.DeserializeJsonTo<BrandsModel.PageViewModel.GridModel.GridItem>() ?? new BrandsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, brandID: key, submitModel: submitModel);
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
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.DELETE, brandID: key, submitModel: new BrandsModel.PageViewModel.GridModel.GridItem());
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

    [Route("admin/brands/{brandID:int}/properties")]
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
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true);
            var viewModel = Model.GetPageViewModel(viewModel: null);
            return View(ViewNames.Admin.Brands.BrandProperties, viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(BrandsPropertiesModel.PageViewModel submitModel)
        {
            var result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true);
            var viewModel = Model.GetPageViewModel(submitModel);
            Model.ValidatePageViewModel(viewModel);
            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsSaved)
                {
                    Model.ShowSuccess();
                    result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Brands.Brand.Properties, new { brandID = Model.DBItem.BrandID }));
                }
                else
                {
                    Model.ShowError();
                    result = View(ViewNames.Admin.Brands.BrandProperties, viewModel);
                }
            }
            else
            {
                result = View(ViewNames.Admin.Brands.BrandProperties, viewModel);
            }
            return result;
        }

        [HttpPost]
        [Route("delete-image", Name = ControllerActionRouteNames.Admin.Brands.Brand.DeleteCoverImage)]
        public async Task<IActionResult> DeleteCoverImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion
    }
}
