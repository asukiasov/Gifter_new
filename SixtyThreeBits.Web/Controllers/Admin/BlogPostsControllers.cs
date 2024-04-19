using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/blog")]
    public class BlogPostsController : AdminControllerBase<BlogModel>
    {
        #region Constructors
        public BlogPostsController()
        {
            Model = new BlogModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.BlogPostsController.BlogPosts)]
        public ActionResult BlogPosts()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.BlogPosts.BlogPostsView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.BlogPostsController.Grid)]
        public async Task<ActionResult> Grid()
        {
            var viewModel = await Model.ListGridItems();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.BlogPostsController.GridAdd)]
        public async Task<ActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<BlogModel.ViewModel.GridViewModel.GridItem>() ?? new BlogModel.ViewModel.GridViewModel.GridItem();
            await Model.IUD(databaseAction: Enums.DatabaseActions.CREATE, blogPostID: key, submitModel: submitModel);
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
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.BlogPostsController.GridUpdate)]
        public async Task<ActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<BlogModel.ViewModel.GridViewModel.GridItem>() ?? new BlogModel.ViewModel.GridViewModel.GridItem();
            await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, blogPostID: key, submitModel: submitModel);
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
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.BlogPostsController.GridDelete)]
        public async Task<ActionResult> GridDelete(int? key)
        {
            await Model.IUD(databaseAction: Enums.DatabaseActions.DELETE, blogPostID: key, submitModel: new BlogModel.ViewModel.GridViewModel.GridItem());
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

    [Route("admin/blog/{blogPostID:int}/properties")]
    [TypeFilter(typeof(BeforeBlogPageLoad), Order = 2)]
    public class BlogPostPropertiesController : AdminControllerBase<BlogPropertiesModel>
    {
        #region Constructors
        public BlogPostPropertiesController()
        {
            Model = new BlogPropertiesModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.BlogPostPropertiesController.Properties)]
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).EnableDevextreme(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetViewModel(viewModel: null);
            Model.PageTitle.Set(Model.DBItem.BlogPostTitle);
            Model.Breadcrumbs.RenameLastItem(Model.DBItem.BlogPostTitle);
            return View(ViewNames.Admin.BlogPosts.BlogPostPropertiesView, viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(BlogPropertiesModel.ViewModel viewModel)
        {
            var result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).EnableDevextreme(true).Enable63BitsSuccessErrorToast(true);
            viewModel = Model.GetViewModel(viewModel: viewModel);

            Model.PageTitle.Set(Model.DBItem.BlogPostTitle);
            Model.Breadcrumbs.RenameLastItem(Model.DBItem.BlogPostTitle);

            await Model.Validate(viewModel);
            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsValid)
                {
                    Model.ShowSuccessToastNotification();
                    result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.BlogPostPropertiesController.Properties, new { blogPostID = Model.DBItem.BlogPostID }));
                }
                else
                {
                    Model.ShowErrorToastNotification();
                    result = View(ViewNames.Admin.BlogPosts.BlogPostPropertiesView, viewModel);
                }
            }
            else
            {
                result = View(ViewNames.Admin.BlogPosts.BlogPostPropertiesView, viewModel);
            }
            return result;
        }

        [HttpPost]
        [Route("delete-image", Name = ControllerActionRouteNames.Admin.BlogPostPropertiesController.DeleteImage)]
        public async Task<IActionResult> DeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion
    }
}
