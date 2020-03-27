using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Admin.Controllers;
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

    //[RoutePrefix("pages")]
    //[BeforePagesPageLoad(Order = 1)]
    //[TabsInitialization(Order = 2, ParentRoute = ControllerActionRouteNames.Admin.Pages.Page.Root)]
    //public class PageController : AdminAreaController
    //{
    //    #region Properties
    //    public SixtyThreeBits.Core.Page DBItemPage;
    //    #endregion

    //    #region Page Properties
    //    [HttpGet]
    //    [Route("{PageID:int}/properties", Name = ControllerActionRouteNames.Admin.Pages.Page.Properties)]
    //    public async Task<ActionResult> PageProperties(int? PageID)
    //    {
    //        var Model = PagePropertiesModel.GetPageViewModel(DBItemPage, null, Url);            
    //        return View(ViewNames.Admin.Pages.Page, Model);
    //    }

    //    [HttpPost]
    //    [ValidateInput(false)]
    //    [Route("{PageID:int}/properties")]
    //    public async Task<ActionResult> PageProperties(int? PageID, PagePropertiesModel.PageViewModel Model)
    //    {
    //        var Result = default(async Task<ActionResult>);

    //        Model = PagePropertiesModel.GetPageViewModel(DBItemPage, Model, Url);
    //        PagePropertiesModel.ValidatePageItemViewModel(PageID, Model);

    //        if (Model.Form.HasErrors)
    //        {
    //            Result = View(ViewNames.Admin.Pages.Page, Model);
    //        }
    //        else
    //        {
    //            Model = PagePropertiesModel.SavePageItem(PageID, Model, User.UserID);
    //            if (Model.Form.IsSaved)
    //            {
    //                SuccessErrorPartialViewAssistance.InitSuccessMessage(Session: Session);
    //                Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.Properties));
    //            }
    //            else
    //            {
    //                SuccessErrorPartialViewAssistance.InitErrorMessage<AdminLayoutModel>(ViewData: ViewData);
    //                Result = View(ViewNames.Admin.Pages.Page, Model);
    //            }
    //        }

    //        return Result;
    //    }

    //    [HttpPost]
    //    [Route("{PageID:int}/properties/delete-image", Name = ControllerActionRouteNames.Admin.Pages.Page.DeleteImage)]
    //    public async Task<ActionResult> PageDeleteImage(int? PageID)
    //    {
    //        var Response = PagePropertiesModel.DeleteImage(PageID);
    //        return Json(Response);
    //    }
    //    #endregion

    //    #region Page Builder
    //    [Route("{PageID:int}/page-builder", Name = ControllerActionRouteNames.Admin.Pages.Page.Builder)]
    //    [Route("{PageID:int}/page-builder/{Language:length(2)}", Name = ControllerActionRouteNames.Admin.Pages.Page.BuilderLanguage)]
    //    public async Task<ActionResult> PageBuilder(int? PageID, string Language)
    //    {
    //        var Model = PageBuilderModel.GetPageViewModel(PageID, Language, this);
    //        return View(ViewNames.Admin.Pages.PageBuilder, Model);
    //    }

    //    [HttpPost]
    //    [ValidateInput(false)]
    //    [Route("{PageID:int}/page-builder")]
    //    [Route("{PageID:int}/page-builder/{Language:length(2)}")]
    //    public async Task<ActionResult> PageBuilder(int? PageID, string Language, PageBuilderModel.PageSubmitModel SubmitModel)
    //    {
    //        var Errors = PageBuilderModel.Validate(PageID, SubmitModel);
    //        var Model = new AjaxResponse();
    //        if (Errors.Count == 0)
    //        {
    //            Model = PageBuilderModel.Save(PageID, SubmitModel);
    //        }
    //        else
    //        {
    //            Model.Data = Errors;
    //        }
    //        return Json(Model);
    //    }
    //    #endregion
    //}
}