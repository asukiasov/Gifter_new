using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Utilities;
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
            var NewsID = FilterContext.RouteData.Values[Constants.RouteValues.NewsID].ToString().ToInt();

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
            Model.Breadcrumbs.RenameAt(2, Model.DBItemNews.NewsTitle);
        }
    }
}