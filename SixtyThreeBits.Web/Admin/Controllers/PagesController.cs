using Microsoft.AspNetCore.Mvc;
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
            Model.PluginClient.EnableJQueryUI(true).EnableJQueryNestedSortable(true).Enable63BitsForms(true).EnableTemplate7(true);
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
        public async Task<ActionResult> PagesUpdate(int? PageID,string PageTitle = null, bool? IsPublished = null, bool? IsMenuItem = null)
        {
            var ViewModel = await Model.UpdatePage(PageID, PageTitle, IsPublished, IsMenuItem);
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
    [TabsInitialization(Order = 2, ParentRoute = ControllerActionRouteNames.Admin.Pages.Page.Root)]
    public class PageController : AdminControllerBase<PageModel>
    {

        #region Constructors
        public PageController()
        {
            Model = new PageModel();
        }
        #endregion


        #region Page Properties
        [Route("properties", Name = ControllerActionRouteNames.Admin.Pages.Page.Properties)]
        public IActionResult PageProperties()
        {
            Model.PluginClient.Enable63BitsForms(true).EnableFancybox(true);
            var ViewModel = Model.GetPagePropertiesViewModel();
            Model.SetPageTitle(Model.DBItemPage.PageTitle);
            return View(ViewNames.Admin.Pages.Page.Properties, ViewModel);
        }
        #endregion

        #region Page Builder
        [Route("page-builder", Name = ControllerActionRouteNames.Admin.Pages.Page.Builder)]
        [Route("page-builder/{Language:length(2)}", Name = ControllerActionRouteNames.Admin.Pages.Page.BuilderLanguage)]
        public IActionResult PageBuilder(int? PageID, string Language)
        {
            var ViewModel = Model.GetPageBuilderViewModel(PageID, Language);
            return View(ViewNames.Admin.Pages.Page.Builder, ViewModel);
        }

        [HttpPost]
        //[ValidateInput(false)]
        [Route("page-builder")]
        [Route("page-builder/{Language:length(2)}")]
        public IActionResult PageBuilder(int? PageID, string Language, PageModel.PageBuilderSubmitModel SubmitModel)
        {
            //var Errors = PageBuilderModel.Validate(PageID, SubmitModel);
            //var Model = new AjaxResponse();
            //if (Errors.Count == 0)
            //{
            //    Model = PageBuilderModel.Save(PageID, SubmitModel);
            //}
            //else
            //{
            //    Model.Data = Errors;
            //}
            //return Json(Model);
            return Json("OK");
        }
        #endregion
    }    
}