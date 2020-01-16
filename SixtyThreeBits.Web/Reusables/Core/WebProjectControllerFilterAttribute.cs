using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class BeforeWebProjectControllerLoaded : System.Attribute, IActionFilter
    {

        void IActionFilter.OnActionExecuted(ActionExecutedContext FilterContext)
        {

        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext FilterContext)
        {
            var C = FilterContext.Controller as Controller;
            var Model = LocalUtilities.GetWebProjectModelBaseFromController(C);                        
            if (Model != null)
            {
                var ActionDescriptor = FilterContext.ActionDescriptor as ControllerActionDescriptor;

                Model.ActionName = ActionDescriptor.ActionName;                
                Model.ControllerName = ActionDescriptor.ControllerTypeInfo.Name;

                Model.UrlCurrentPage = LocalUtilities.GetCurrentPageUrl(C.Request);
                Model.WebsiteDomain = LocalUtilities.GetWebsiteDomain(C.Request);

                Model.db = FilterContext.HttpContext.RequestServices.GetService(typeof(DBCoreDataContext)) as DBCoreDataContext;
                Model.AppSettings = FilterContext.HttpContext.RequestServices.GetService(typeof(AppSettingsModel)) as AppSettingsModel;

                var HttpContextAccessor = FilterContext.HttpContext.RequestServices.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                Model.SessionAssistance = new SessionAssistance(HttpContextAccessor);
                Model.Url = C.Url;
            }
        }
    }
}
