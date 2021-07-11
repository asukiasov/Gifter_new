using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Admin.Filters;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Controllers
{
    [Route("admin/categories")]
    public class CategoriesController : AdminControllerBase<CategoriesModel>
    {
        #region Constructors
        public CategoriesController()
        {
            Model = new CategoriesModel();
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Categories.Index)]
        public async Task<IActionResult> Categories()
        {
            Model.PluginsClient.EnableJQueryUI(EnableJs:true).EnableJQueryNestedSortable(true).EnableTemplate7(true);
            var ViewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.Categories.Category, ViewModel);
        }    

        [Route("add", Name =ControllerActionRouteNames.Admin.Categories.Add)]
        public async Task<IActionResult> Create(int? CategoryParentID, string CategoryName)
        {
            var ViewModel = await Model.CreateCategory(CategoryParentID, CategoryName);
            return Json(ViewModel);
        }

        [Route("sync", Name = ControllerActionRouteNames.Admin.Categories.Sync)]
        public async Task<IActionResult> Sync(SyncSortIndexesModel SubmitModel)
        {
            var ViewModel = await Model.SyncParentsAndSortIndexes(SubmitModel);
            return Json(ViewModel);
        }

        [Route("delete", Name = ControllerActionRouteNames.Admin.Categories.Delete)]
        public async Task<IActionResult> Delete(int? CategoryID)
        {
            var ViewModel = await Model.DeleteRecursive(CategoryID);
            return Json(ViewModel);
        }
        #endregion
    }

    [Route("admin/categories/{CategoryID:int}/properties")]
    [TypeFilter(typeof(BeforeCategoryPageLoad), Order = 2)]
    public class CategoriesPropertiesController: AdminControllerBase<CategoryPropertiesModel>
    {
        #region Constructors
        public CategoriesPropertiesController()
        {
            Model = new CategoryPropertiesModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Categories.Category.Properties)]
        public IActionResult Properties(int? CategoryID)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true);
            var ViewModel = Model.GetPageViewModel(CategoryID, ViewModel: null);
            if (ViewModel == null)
            {
                return Model.GetNotFoundAdminViewResult();
            }
            else
            {
                return View(ViewNames.Admin.Categories.CategoryProperties, ViewModel);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(CategoryPropertiesModel.CategoryPropertiesViewModel SubmitModel, int? CategoryID)
        {
            var Result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true);
            var ViewModel = Model.GetPageViewModel(CategoryID, SubmitModel);
            Model.ValidatePageViewModel(ViewModel);
            if (ViewModel.IsValid)
            {
                await Model.SaveCategoryProperties(CategoryID, ViewModel);
                if (ViewModel.IsSaved)
                {
                    Model.ShowSuccess();
                    Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.Category.Properties, new { CategoryID = CategoryID }));
                }
                else
                {
                    Model.ShowError();
                    Result = View(ViewNames.Admin.Categories.CategoryProperties, ViewModel);
                }
            }
            else
            {
                Result = View(ViewNames.Admin.Categories.CategoryProperties, ViewModel);
            }
            return Result;
        }

        [HttpPost]
        [Route("delete-image", Name = ControllerActionRouteNames.Admin.Categories.Category.DeleteImage)]
        public async Task<IActionResult> CategoryDeleteImage(int? CategoryID)
        {
            var Result = await Model.DeleteImage(CategoryID);
            return Json(Result);
        }
        #endregion
    }
}
