using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/blog/{blogPostID:int}/properties")]
    [TypeFilter(typeof(BlogFilterAttribute), Order = 2)]
    public class BlogPostPropertiesController : AdminControllerBase<BlogPostPropertiesModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.BlogPostPropertiesController.Properties)]
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).EnableDevextreme(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetViewModel(viewModel: null);
            Model.PageTitle.Set(Model.BlogPost.BlogPostTitle);
            Model.Breadcrumbs.RenameLastItem(Model.BlogPost.BlogPostTitle);
            return View(ViewNames.Admin.BlogPosts.BlogPostPropertiesView, viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(BlogPostPropertiesModel.ViewModel submitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).EnableDevextreme(true).Enable63BitsSuccessErrorToast(true);

            var viewModel = await Model.Save(submitModel);

            if (viewModel.HasErrors)
            {
                Model.PageTitle.Set(Model.BlogPost.BlogPostTitle);
                Model.Breadcrumbs.RenameLastItem(Model.BlogPost.BlogPostTitle);
                return View(ViewNames.Admin.BlogPosts.BlogPostPropertiesView, viewModel);
            }
            else
            {
                Model.ShowSuccessToastNotification();
                return Redirect(Model.UrlCurrentPageWithDomain);
            }
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