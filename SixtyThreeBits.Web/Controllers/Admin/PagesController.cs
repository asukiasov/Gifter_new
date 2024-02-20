using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/pages")]
    public class PagesController : AdminControllerBase<PagesTreeModel>
    {
        #region Constructors
        public PagesController()
        {
            Model = new PagesTreeModel();
        }
        #endregion

        #region Actions
        [Route("", Name = ControllerActionRouteNames.Admin.Pages.Index)]
        public async Task<ActionResult> Pages()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableTemplate7(true).EnableSortableJS(true);
            var viewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.Pages.Tree, viewModel);
        }

        [HttpPost]
        [Route("add", Name = ControllerActionRouteNames.Admin.Pages.AddNew)]
        public async Task<ActionResult> PagesAdd(PagesTreeModel.SubmitModel submitModel)
        {
            var viewModel = await Model.CreatePage(submitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("update", Name = ControllerActionRouteNames.Admin.Pages.Update)]
        public async Task<ActionResult> PagesUpdate(PagesTreeModel.SubmitModel submitModel)
        {
            var viewModel = await Model.UpdatePage(submitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("sync-parents-sort-indexes", Name = ControllerActionRouteNames.Admin.Pages.SyncParentsAndSortIndexes)]
        public async Task<ActionResult> PagesSyncParentsAndSortIndexes(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = await Model.SyncParentsAndSortIndexes(submitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("delete", Name = ControllerActionRouteNames.Admin.Pages.Delete)]
        public async Task<ActionResult> PagesDelete(int? pageID)
        {
            var viewModel = await Model.DeleteRecursive(pageID);
            return Json(viewModel);
        }
        #endregion
    }

    [Route("admin/pages/{PageID:int}/properties")]
    [TypeFilter(typeof(BeforePagesPageLoad), Order = 2)]
    public class PageController : AdminControllerBase<PagePropertiesModel>
    {
        #region Constructors
        public PageController()
        {
            Model = new PagePropertiesModel();
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Pages.Page.Properties)]
        public IActionResult PageProperties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true);
            var viewModel = Model.GetPageViewModel(viewModel: null);
            return View(ViewNames.Admin.Pages.Page.Properties, viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> PageProperties(PagePropertiesModel.PageViewModel SubmitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true);
            var viewModel = Model.GetPageViewModel(viewModel: SubmitModel);

            Model.ValidatePageViewModel(viewModel);
            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsSaved)
                {
                    Model.ShowSuccessToastNotification();
                    return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.Properties, new { pageID = Model.DBItem.PageID }));
                }
                else
                {
                    Model.ShowErrorToastNotification();
                }
            }

            return View(ViewNames.Admin.Pages.Page.Properties, viewModel);
        }

        [HttpPost]
        [Route("delete-image", Name = ControllerActionRouteNames.Admin.Pages.Page.DeleteImage)]
        public async Task<IActionResult> PageDeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion        
    }

    [Route("admin/pages/{pageID:int}/page-builder")]
    [TypeFilter(typeof(BeforePagesPageLoad), Order = 2)]
    public class PageBuilderController : AdminControllerBase<PageBuilderModel>
    {
        #region Constructors
        public PageBuilderController()
        {
            Model = new PageBuilderModel();
        }
        #endregion

        #region Actions
        [Route("", Name = ControllerActionRouteNames.Admin.Pages.Page.Builder)]
        [Route("{Language:length(2)}", Name = ControllerActionRouteNames.Admin.Pages.Page.BuilderLanguage)]
        public IActionResult PageBuilder(int? pageID, string language)
        {
            var viewModel = Model.GetPageViewModel(pageID, language);
            viewModel.PluginsClient.EnableGoogleFonts(true).EnableJsClient(true).EnableJQuery(true).EnableBootstrap(true).EnableFancybox(true).EnablePreloader(true).EnableTemplate7(true).EnableTinyMce(true).EnableUtils(true).EnablePageBuilderEditor(true).EnableJWPlayer(true).EnableFontAwesome(true).EnableMalihuScroll(true).EnableSortableJS(true);
            return View(ViewNames.Admin.Pages.Page.Builder, viewModel);
        }

        [HttpPost]
        //[ValidateInput(false)]
        [Route("")]
        [Route("{Language:length(2)}")]
        public async Task<IActionResult> PageBuilder(PageBuilderModel.SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var errors = Model.ValidatePageViewModel(submitModel);
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