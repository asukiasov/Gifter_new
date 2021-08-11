using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Controllers;
using SixtyThreeBits.Web.Admin.Filters;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Areas.Admin.Controllers
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

        [Route("", Name = ControllerActionRouteNames.Admin.Pages.Index)]
        public async Task<ActionResult> Pages()
        {
            Model.PluginsClient.EnableJQueryUI(EnableJs: true, EnableCss: false).EnableJQueryNestedSortable(true).Enable63BitsForms(true).EnableTemplate7(true);
            var ViewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.Pages.Tree, ViewModel);
        }

        [HttpPost]
        [Route("add", Name = ControllerActionRouteNames.Admin.Pages.AddNew)]
        public async Task<ActionResult> PagesAdd(int? ParentID,string PageTitle)
        {
            var ViewModel = await Model.CreatePage(ParentID, PageTitle);
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("update", Name = ControllerActionRouteNames.Admin.Pages.Update)]
        public async Task<ActionResult> PagesUpdate(int? PageID,string PageTitle = null, bool? PageIsPublished = null, bool? PageIsMenuItem = null, bool? PageIsFooterItem = null)
        {
            var ViewModel = await Model.UpdatePage(PageID, PageTitle, PageIsPublished, PageIsMenuItem, PageIsFooterItem);
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("sync-parents-sort-indexes", Name = ControllerActionRouteNames.Admin.Pages.SyncParentsAndSortIndexes)]
        public async Task<ActionResult> PagesSyncParentsAndSortIndexes(SyncSortIndexesModel SubmitModel)
        {
            var ViewModel = await Model.SyncParentsAndSortIndexes(SubmitModel);
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("delete", Name = ControllerActionRouteNames.Admin.Pages.Delete)]
        public async Task<ActionResult> PagesDelete(int? PageID)
        {
            var ViewModel = await Model.DeleteRecursive(PageID);            
            return Json(ViewModel);
        }
    }

    [Route("admin/pages/{PageID:int}")]
    [TypeFilter(typeof(BeforePagesPageLoad), Order = 2)]    
    public class PageController : AdminControllerBase<PageModel>
    {
        #region Constructors
        public PageController()
        {
            Model = new PageModel();
        }
        #endregion

        #region Page Properties
        [HttpGet]
        [Route("properties", Name = ControllerActionRouteNames.Admin.Pages.Page.Properties)]
        public IActionResult PageProperties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true);
            var ViewModel = Model.GetPagePropertiesViewModel(ViewModel: null);
            Model.PageTitle.Set(Model.DBItemPage.PageTitle);
            Model.Breadcrumbs.RenameLastItem(Model.DBItemPage.PageTitle);
            return View(ViewNames.Admin.Pages.Page.Properties, ViewModel);
        }

        [HttpPost]
        [Route("properties")]
        public async Task<IActionResult> PageProperties(PageModel.PagePropertiesViewModel SubmitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true);
            var ViewModel = Model.GetPagePropertiesViewModel(ViewModel: SubmitModel);

            Model.PageTitle.Set(Model.DBItemPage.PageTitle);
            Model.Breadcrumbs.RenameLastItem(Model.DBItemPage.PageTitle);

            Model.ValidatePagePropertiesViewModel(ViewModel);
            if(ViewModel.IsValid)
            {
                var IsSaved = await Model.SavePageProperties(ViewModel);
                if (IsSaved)
                {
                    Model.ShowSuccess();
                    return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.Properties, new { PageID = Model.DBItemPage.PageID }));
                }
                else
                {
                    Model.ShowError();
                }
            }
            
            return View(ViewNames.Admin.Pages.Page.Properties, ViewModel);
        }

        [HttpPost]
        [Route("properties/delete-image", Name = ControllerActionRouteNames.Admin.Pages.Page.DeleteImage)]
        public async Task<IActionResult> PageDeleteImage()
        {
            var Response = await Model.DeleteImage();
            return Json(Response);
        }
        #endregion

        #region Page Builder
        [Route("page-builder", Name = ControllerActionRouteNames.Admin.Pages.Page.Builder)]
        [Route("page-builder/{Language:length(2)}", Name = ControllerActionRouteNames.Admin.Pages.Page.BuilderLanguage)]
        public IActionResult PageBuilder(int? PageID, string Language)
        {
            var ViewModel = Model.GetPageBuilderViewModel(PageID, Language);
            ViewModel.PluginsClient.EnableGoogleFonts(true).EnableJsClient(true).EnableJQuery(true).EnableJQueryUI(EnableJs: true, EnableCss: false).EnableBootstrap(true).EnableFancybox(true).EnablePreloader(true).EnableTemplate7(true).EnableTinyMce(true).EnableFancybox(true).EnableUtils(true).EnablePageBuilderEditor(true).EnableJWPlayer(true).EnableFontAwesome(true).EnableMalihuScroll(true);
            return View(ViewNames.Admin.Pages.Page.Builder, ViewModel);
        }

        [HttpPost]
        //[ValidateInput(false)]
        [Route("page-builder")]
        [Route("page-builder/{Language:length(2)}")]
        public async Task<IActionResult> PageBuilder(int? PageID, PageModel.PageBuilderSubmitModel SubmitModel)
        {
            var ViewModel = new AjaxResponse();
            var Errors = Model.ValidatePageBuilderSubmitModel(SubmitModel);
            if (Errors.Count == 0)
            {
                ViewModel = await Model.SavePageBuilder(SubmitModel);
            }
            else
            {
                ViewModel.Data = Errors;
            }
            return Json(ViewModel);
        }
        #endregion

        #region Page Text
        [Route("page-text", Name = ControllerActionRouteNames.Admin.Pages.Page.Text)]
        [Route("page-text/{Language:length(2)}", Name = ControllerActionRouteNames.Admin.Pages.Page.TextLanguage)]
        public IActionResult PageText(int? PageID, string Language)
        {
            var ViewModel = Model.GetPageBuilderViewModel(PageID, Language);
            ViewModel.PluginsClient.EnableGoogleFonts(true).EnableJsClient(true).EnableJQuery(true).EnableBootstrap(true).EnableFancybox(true).EnablePreloader(true).EnableTinyMce(true).EnableUtils(true);
            return View(ViewNames.Admin.Pages.Page.Text, ViewModel);
        }

        [HttpPost]
        //[ValidateInput(false)]
        [Route("page-text")]
        [Route("page-text/{Language:length(2)}")]
        public async Task<IActionResult> PageText(int? PageID,PageModel.PageBuilderSubmitModel SubmitModel)
        {
            var ViewModel = new AjaxResponse();
            var Errors = Model.ValidatePageBuilderSubmitModel(SubmitModel);
            if (Errors.Count == 0)
            {
                ViewModel = await Model.SavePageBuilder(SubmitModel);
            }
            else
            {
                ViewModel.Data = Errors;
            }
            return Json(ViewModel);
        }
        #endregion
    }
}