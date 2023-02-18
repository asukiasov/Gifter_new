using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforeBlogPageLoad : IAsyncActionFilter
    {        

        public BeforeBlogPageLoad()
        {            
        }
        
        public async Task OnActionExecutionAsync(ActionExecutingContext FilterContext, ActionExecutionDelegate next)
        {
            var Model = LocalUtilities.GetModelFromController<BlogModelBase>(FilterContext.Controller);
            var BlogPostID = FilterContext.RouteData.Values[Constants.RouteValues.BlogPostID].ToString().ToInt();

            Model.DBItemBlog = await Model.DataAccessFactory.Blog.GetSingleBlogByID(BlogPostID);
            if (Model.DBItemBlog == null)
            {                
                FilterContext.Result = Model.GetNotFoundAdminViewResult();
            }
            else
            {
                ReinitBreadCrumbs(Model);
                await next();
            }
        }

        void ReinitBreadCrumbs(BlogModelBase Model)
        {
            Model.Breadcrumbs.DeleteLastItem();
            Model.Breadcrumbs.RenameLastItem(Model.DBItemBlog.BlogPostTitle);
        } 
    }
}