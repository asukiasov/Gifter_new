using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Filters;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;

namespace SixtyThreeBits.Web.Admin.Controllers
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
            var ViewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.Blog.Page, ViewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Blog.Grid)]
        public async Task<ActionResult> BlogGrid()
        {
            var ViewModel = await Model.GetGridViewModel();
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Blog.GridAdd)]
        public async Task<ActionResult> BlogGridAdd(int? key, string values)
        {
            var SubmitModel = values.DeserializeJsonTo<BlogModel.PageViewModel.GridModel.GridItem>() ?? new BlogModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, BlogPostID: key, SubmitModel: SubmitModel);
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
            var SubmitModel = values.DeserializeJsonTo<BlogModel.PageViewModel.GridModel.GridItem>() ?? new BlogModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, BlogPostID: key, SubmitModel: SubmitModel);
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
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.DELETE, BlogPostID: key, SubmitModel: new BlogModel.PageViewModel.GridModel.GridItem());
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

    [Route("admin/blog/{BlogPostID:int}")]
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
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true);
            var ViewModel = Model.GetBlogPropertiesViewModel(ViewModel: null);
            Model.PageTitle.Set(Model.DBItemBlog.BlogPostTitle);
            Model.Breadcrumbs.RenameLastItem(Model.DBItemBlog.BlogPostTitle);
            return View(ViewNames.Admin.Blog.BlogItem, ViewModel);
        }

        [HttpPost]
        [Route("properties")]
        public async Task<IActionResult> Properties(BlogPropertiesModel.BlogPropertiesViewModel SubmitModel)
        {
            var Result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true);
            var ViewModel = Model.GetBlogPropertiesViewModel(ViewModel: SubmitModel);

            Model.PageTitle.Set(Model.DBItemBlog.BlogPostTitle);
            Model.Breadcrumbs.RenameLastItem(Model.DBItemBlog.BlogPostTitle);

            await Model.ValidateBlogPropertiesViewModel(ViewModel);
            if (ViewModel.IsValid)
            {
                await Model.SaveBlogProperties(ViewModel);
                if (ViewModel.IsSaved)
                {
                    Model.ShowSuccess();
                    Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.PostProperties, new { BlogPostID = Model.DBItemBlog.BlogPostID }));
                }
                else
                {
                    Model.ShowError();
                    Result = View(ViewNames.Admin.Blog.BlogItem, ViewModel);
                }
            }
            else
            {
                Result = View(ViewNames.Admin.Blog.BlogItem, ViewModel);
            }
            return Result;
        }

        [HttpPost]
        [Route("properties/delete-image", Name = ControllerActionRouteNames.Admin.Blog.PostPropertiesDeleteImage)]
        public async Task<IActionResult> BlogItemDeleteImage(int? BlogPostID)
        {
            var Result = await Model.DeleteImage(BlogPostID);
            return Json(Result);
        }
        #endregion
    }
}
