using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforeNewsPageLoad : IAsyncActionFilter
    {        

        public BeforeNewsPageLoad()
        {            
        }
        
        public async Task OnActionExecutionAsync(ActionExecutingContext FilterContext, ActionExecutionDelegate next)
        {
            var Model = LocalUtilities.GetModelFromController<NewsModelBase>(FilterContext.Controller);
            var NewsID = FilterContext.RouteData.Values["NewsID"].ToString().ToInt();

            Model.DBItemNews = await Model.DataAccessFactory.News.GetSingleNewsByID(NewsID);
            if (Model.DBItemNews == null)
            {
                FilterContext.Result = Model.GetNotFoundAdminViewResult();
            }
            else
            {
                InitPageTitle(Model);
                ReinitBreadCrumbs(Model);
                await next();
            }
        }

        void InitPageTitle(NewsModelBase Model)
        {
            Model.PageTitle.Set(Model.DBItemNews.NewsTitle);
        }

        void ReinitBreadCrumbs(NewsModelBase Model)
        {
            if(Model.Breadcrumbs.ItemsCount > 2)
            {
                Model.Breadcrumbs.Items[2].Title = Model.DBItemNews.NewsTitle;
            }
            
        }
    }
}