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
        public async Task<IActionResult> Properties(BlogPostPropertiesModel.ViewModel viewModel)
        {
            var result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).EnableDevextreme(true).Enable63BitsSuccessErrorToast(true);
            viewModel = Model.GetViewModel(viewModel: viewModel);

            Model.PageTitle.Set(Model.BlogPost.BlogPostTitle);
            Model.Breadcrumbs.RenameLastItem(Model.BlogPost.BlogPostTitle);

            await Model.Validate(viewModel);
            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsValid)
                {
                    Model.ShowSuccessToastNotification();
                    result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.BlogPostPropertiesController.Properties, new { blogPostID = Model.BlogPost.BlogPostID }));
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