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
            var SubmitModel = values.FromJsonTo<BlogModel.PageViewModel.GridModel.GridItem>() ?? new BlogModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, BlogID: key, SubmitModel: SubmitModel);
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
            var SubmitModel = values.FromJsonTo<BlogModel.PageViewModel.GridModel.GridItem>() ?? new BlogModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, BlogID: key, SubmitModel: SubmitModel);
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
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.DELETE, BlogID: key, SubmitModel: new BlogModel.PageViewModel.GridModel.GridItem());
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

    }
    [Route("admin/blog/{BlogID:int}")]
    [TypeFilter(typeof(BeforeBlogPageLoad), Order = 2)]
    public class BlogPropertiesController : AdminControllerBase<BlogPropertiesModel>
        {
        #region Constructors
        public BlogPropertiesController()
        {
            Model = new BlogPropertiesModel();
        }
        #endregion

        #region Blog Properties
        [HttpGet]
        [Route("properties", Name = ControllerActionRouteNames.Admin.Blog.BlogItem)]
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true);
            var ViewModel = Model.GetBlogPropertiesViewModel(ViewModel: null);
            Model.PageTitle.Set(Model.DBItemBlog.BlogTitle);
            Model.Breadcrumbs.RenameLastItem(Model.DBItemBlog.BlogTitle);
            return View(ViewNames.Admin.Blog.BlogItem, ViewModel);
        }

        [HttpPost]
        [Route("properties")]
        public async Task<IActionResult> Properties(BlogPropertiesModel.BlogPropertiesViewModel SubmitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true);
            var ViewModel = Model.GetBlogPropertiesViewModel(ViewModel: SubmitModel);

            Model.PageTitle.Set(Model.DBItemBlog.BlogTitle);
            Model.Breadcrumbs.RenameLastItem(Model.DBItemBlog.BlogTitle);

            await Model.ValidateBlogPropertiesViewModel(ViewModel);
            if (ViewModel.IsValid)
            {
                var IsSaved = await Model.SaveBlogProperties(ViewModel);
                if (IsSaved)
                {
                    Model.ShowSuccess();
                    return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.BlogItem, new { BlogID = Model.DBItemBlog.BlogID }));
                }
                else
                {
                    Model.ShowError();
                }
            }

            return View(ViewNames.Admin.Blog.BlogItem, ViewModel);
        }

        [HttpPost]
        [Route("properties/delete-image", Name = ControllerActionRouteNames.Admin.Blog.BlogItemDeleteImage)]
        public async Task<IActionResult> BlogItemDeleteImage(int? BlogID)
        {   
            var Result = await Model.DeleteImage(BlogID);
            return Json(Result);
        }
        #endregion
    }


}
