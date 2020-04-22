using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Areas.Admin.Controllers;
using SixtyThreeBits.Web.Reusables;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforePagesPageLoad : IAsyncActionFilter
    {
        DataAccessFactory DataAccessFactory;

        public BeforePagesPageLoad(DataAccessFactory DataAccessFactory)
        {
            this.DataAccessFactory = DataAccessFactory;
        }
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context,ActionExecutionDelegate next)
        {
            var PageID = context.RouteData.Values["PageID"].ToString().ToInt();
            var DBItem = await DataAccessFactory.Pages.GetSinglePageByID(PageID);
            var C = context.Controller as PageController;

            if (DBItem == null)
            {
                context.Result = C.NotFoundAdmin();                
            }
            else
            {
                C.Model.Breadcrumbs.DeleteLastItem();
                C.Model.DBItemPage = DBItem;
                await next();
            }
        }        
    }
}