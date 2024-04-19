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
        #region Constructors
        public PagesManagementController()
        {
            Model = new PagesManagementModule();
        }
        #endregion

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
    [TypeFilter(typeof(BeforePagesManagementPageLoad), Order = 2)]
    public class PagesController : AdminControllerBase<PagesModel>
    {
        #region Constructors
        public PagesController()
        {
            Model = new PagesModel();
        }
        #endregion

        #region Actions
        [Route("", Name = ControllerActionRouteNames.Admin.PagesController.Pages)]
        public ActionResult Pages()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.Pages.PagesView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.PagesController.Grid)]
        public async Task<ActionResult> Grid()
        {
            var viewModel = await Model.ListGridItems();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.PagesController.GridAdd)]
        public async Task<ActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<PagesModel.ViewModel.GridViewModel.GridItem>() ?? new PagesModel.ViewModel.GridViewModel.GridItem();
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                await Model.IUD(databaseAction: Enums.DatabaseActions.CREATE, pageID: key, submitModel: submitModel);
                return GetDevexpressSuccessResult();
            }
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.PagesController.GridUpdate)]
        public async Task<ActionResult> GridUpdate(int? key, string values)
        {
            var result = default(ActionResult);
            var submitModel = values.DeserializeJsonTo<PagesModel.ViewModel.GridViewModel.GridItem>() ?? new PagesModel.ViewModel.GridViewModel.GridItem();

            if (Model.Form.HasErrors)
            {
                result = GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, pageID: key, submitModel: submitModel);
                if (Model.Form.HasErrors)
                {
                    result = GetDevexpressErrorResult(Model.Form.ErrorMessage);
                }
                else
                {
                    result = GetDevexpressSuccessResult();
                }
            }

            return result;
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.PagesController.GridDelete)]
        public async Task<ActionResult> GridDelete(int? key)
        {
            await Model.Delete(pageID: key);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
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
    [TypeFilter(typeof(BeforePagePageLoad), Order = 2)]
    public class PageDataController : AdminControllerBase<PageDataModel>
    {
        #region Constructors
        public PageDataController()
        {
            Model = new PageDataModel();
        }
        #endregion

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
    [TypeFilter(typeof(BeforePagePageLoad), Order = 2)]
    public class PagePropertiesController : AdminControllerBase<PagePropertiesModel>
    {
        #region Constructors
        public PagePropertiesController()
        {
            Model = new PagePropertiesModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.PagePropertiesController.Properties)]
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetPageViewModel(viewModel: null);
            return View(ViewNames.Admin.Pages.Page.PagePropertiesView, viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(PagePropertiesModel.PageViewModel submitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetPageViewModel(viewModel: submitModel);

            await Model.ValidatePageViewModel(viewModel);
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
    [TypeFilter(typeof(BeforePagePageLoad), Order = 2)]
    public class PageBuilderController : AdminControllerBase<PageBuilderModel>
    {
        #region Constructors
        public PageBuilderController()
        {
            Model = new PageBuilderModel();
        }
        #endregion

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