using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/blog")]
    public class BlogPostsController : AdminControllerBase<BlogPostsModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.BlogPostsController.BlogPosts)]
        public IActionResult BlogPosts()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.BlogPosts.BlogPostsView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.BlogPostsController.Grid)]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.GetGridItems();
            return DevExtremeGridResult(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.BlogPostsController.GridAdd)]
        public async Task<IActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<BlogPostsModel.ViewModel.GridModel.GridItem>() ?? new BlogPostsModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.INSERT, blogPostID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.BlogPostsController.GridUpdate)]
        public async Task<IActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<BlogPostsModel.ViewModel.GridModel.GridItem>() ?? new BlogPostsModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, blogPostID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.BlogPostsController.GridDelete)]
        public async Task<IActionResult> GridDelete(int? key)
        {
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.DELETE, blogPostID: key, submitModel: new BlogPostsModel.ViewModel.GridModel.GridItem());
            return DevExtremeGridActionResult(viewModel);
        }
        #endregion
    }    
}