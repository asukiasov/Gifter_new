using Microsoft.AspNetCore.Mvc.Filters;
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
            var BlogID = FilterContext.RouteData.Values["BlogID"].ToString().ToInt();

            Model.DBItemBlog = await Model.DataAccessFactory.Blog.GetSingleBlogByID(BlogID);
            if (Model.DBItemBlog == null)
            {
                FilterContext.Result = Model.GetNotFoundAdminViewResult();
            }
            else
            {
                Model.DBItemBlog.SetAppSettings(Model.AppSettings);                
            }

            await next();
        }

        void ReinitBreadCrumbs(BlogModelBase Model)
        {
            Model.Breadcrumbs.DeleteLastItem();
        } 
    }
}