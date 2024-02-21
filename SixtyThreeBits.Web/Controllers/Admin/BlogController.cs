using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/blog")]
    public class BlogController : AdminControllerBase<BlogModel>
    {
        #region Constructors
        public BlogController()
        {
            Model = new BlogModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Blog.Page)]
        public ActionResult Blog()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.Blog.Page, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Blog.Grid)]
        public async Task<ActionResult> BlogGrid()
        {
            var viewModel = await Model.GetGridViewModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Blog.GridAdd)]
        public async Task<ActionResult> BlogGridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<BlogModel.PageViewModel.GridModel.GridItem>() ?? new BlogModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.CREATE, blogPostID: key, submitModel: submitModel);
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
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.Blog.GridUpdate)]
        public async Task<ActionResult> BlogGridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<BlogModel.PageViewModel.GridModel.GridItem>() ?? new BlogModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.UPDATE, blogPostID: key, submitModel: submitModel);
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
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.Blog.GridDelete)]
        public async Task<ActionResult> BlogGridDelete(int? key)
        {
            await Model.CRUD(databaseAction: Enums.DatabaseActions.DELETE, blogPostID: key, submitModel: new BlogModel.PageViewModel.GridModel.GridItem());
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

    [Route("admin/blog/{blogPostID:int}")]
    [TypeFilter(typeof(BeforeBlogPageLoad), Order = 2)]
    public class BlogPropertiesController : AdminControllerBase<BlogPropertiesModel>
    {
        #region Constructors
        public BlogPropertiesController()
        {
            Model = new BlogPropertiesModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("properties", Name = ControllerActionRouteNames.Admin.Blog.PostProperties)]
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).EnableDevextreme(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetBlogPropertiesViewModel(viewModel: null);
            Model.PageTitle.Set(Model.DBItem.BlogPostTitle);
            Model.Breadcrumbs.RenameLastItem(Model.DBItem.BlogPostTitle);
            return View(ViewNames.Admin.Blog.BlogPostProperties, viewModel);
        }

        [HttpPost]
        [Route("properties")]
        public async Task<IActionResult> Properties(BlogPropertiesModel.BlogPropertiesViewModel submitModel)
        {
            var result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).EnableDevextreme(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetBlogPropertiesViewModel(viewModel: submitModel);

            Model.PageTitle.Set(Model.DBItem.BlogPostTitle);
            Model.Breadcrumbs.RenameLastItem(Model.DBItem.BlogPostTitle);

            await Model.ValidatePageViewModel(viewModel);
            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsSaved)
                {
                    Model.ShowSuccessToastNotification();
                    result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.PostProperties, new { blogPostID = Model.DBItem.BlogPostID }));
                }
                else
                {
                    Model.ShowErrorToastNotification();
                    result = View(ViewNames.Admin.Blog.BlogPostProperties, viewModel);
                }
            }
            else
            {
                result = View(ViewNames.Admin.Blog.BlogPostProperties, viewModel);
            }
            return result;
        }

        [HttpPost]
        [Route("properties/delete-image", Name = ControllerActionRouteNames.Admin.Blog.PostPropertiesDeleteImage)]
        public async Task<IActionResult> BlogItemDeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion
    }
}
