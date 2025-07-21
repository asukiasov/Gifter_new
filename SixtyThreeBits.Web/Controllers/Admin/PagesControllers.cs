using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/pages-management")]
    public class PagesManagementController : AdminControllerBase<PagesManagementModule>
    {
        #region Actions
        [Route("", Name = ControllerActionRouteNames.Admin.PagesManagementController.RedirectToChild)]
        public IActionResult RedirectToChild()
        {
            var redirectUrl = Model.GetRedirectUrl();
            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                return Model.GetNotFoundAdminViewResult();
            }
            else
            {
                return Redirect(redirectUrl);
            }
        }
        #endregion
    }

    [Route("admin/pages")]
    [TypeFilter(typeof(PagesManagementFilterAttribute), Order = 2)]
    public class PagesController : AdminControllerBase<PagesModel>
    {
        #region Actions
        [Route("", Name = ControllerActionRouteNames.Admin.PagesController.Pages)]
        public IActionResult Pages()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.Pages.PagesView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.PagesController.Grid)]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.GetGridModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.PagesController.GridAdd)]
        public async Task<IActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<PagesModel.ViewModel.GridModel.GridItem>() ?? new PagesModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.INSERT, pageID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.PagesController.GridUpdate)]
        public async Task<IActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<PagesModel.ViewModel.GridModel.GridItem>() ?? new PagesModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, pageID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.PagesController.GridDelete)]
        public async Task<IActionResult> GridDelete(int? key)
        {
            var viewModel = await Model.Delete(pageID: key);
            return DevExtremeGridActionResult(viewModel);
        }

        [Route("get", Name = ControllerActionRouteNames.Admin.PagesController.Get)]
        public async Task<ActionResult> Get()
        {
            var viewModel = await Model.GetPagesData();
            return Json(viewModel);
        }
        #endregion
    }

    [Route("admin/pages/{PageID:int}/data")]
    [TypeFilter(typeof(PageFilterAttribute), Order = 2)]
    public class PageDataController : AdminControllerBase<PageDataModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.PageDataController.Get)]
        public IActionResult Get()
        {            
            var viewModel = Model.GetPageData();
            return Json(viewModel);
        }        
        #endregion
    }

    [Route("admin/pages/{PageID:int}/properties")]
    [TypeFilter(typeof(PageFilterAttribute), Order = 2)]
    public class PagePropertiesController : AdminControllerBase<PagePropertiesModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.PagePropertiesController.Properties)]
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetViewModel(viewModel: null);
            return View(ViewNames.Admin.Pages.Page.PagePropertiesView, viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(PagePropertiesModel.ViewModel submitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetViewModel(viewModel: submitModel);

            await Model.ValidateViewModel(viewModel);
            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsValid)
                {
                    Model.ShowSuccessToastNotification();
                    return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.PagePropertiesController.Properties, new { pageID = Model.DBItem.PageID }));
                }
                else
                {
                    Model.ShowErrorToastNotification();
                }
            }

            return View(ViewNames.Admin.Pages.Page.PagePropertiesView, viewModel);
        }

        [HttpPost]
        [Route("delete-image", Name = ControllerActionRouteNames.Admin.PagePropertiesController.DeleteImage)]
        public async Task<IActionResult> DeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }        
        #endregion
    }

    [Route("admin/pages/{pageID:int}/page-builder")]
    [TypeFilter(typeof(PageFilterAttribute), Order = 2)]
    public class PageBuilderController : AdminControllerBase<PageBuilderModel>
    {
        #region Actions
        [Route("", Name = ControllerActionRouteNames.Admin.PageBuilderController.Builder)]
        [Route("{Language:length(2)}", Name = ControllerActionRouteNames.Admin.PageBuilderController.BuilderLanguage)]
        public IActionResult PageBuilder(int? pageID, string language)
        {
            var viewModel = Model.GetViewModel(pageID, language);
            viewModel.PluginsClient.EnableJsClient(true).EnableJQuery(true).EnableBootstrap(true).EnableFancybox(true).EnablePreloader(true).EnableTemplate7(true).EnableTinyMce(true).EnableUtils(true).EnablePageBuilderEditor(true).EnableJWPlayer(true).EnableFontAwesome(true).EnableMalihuScroll(true).EnableSortableJS(true);
            return View(ViewNames.Admin.Pages.Page.PageBuilderView, viewModel);
        }

        [HttpPost]
        [Route("")]
        [Route("{Language:length(2)}")]
        public async Task<IActionResult> PageBuilder(PageBuilderModel.SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var errors = Model.Validate(submitModel);
            if (errors.HasErrors)
            {
                viewModel.Data = errors;
            }
            else
            {
                viewModel = await Model.Save(submitModel);
            }
            return Json(viewModel);
        }
        #endregion
    }
}