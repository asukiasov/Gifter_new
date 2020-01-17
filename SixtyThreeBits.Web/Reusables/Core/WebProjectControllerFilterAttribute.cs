using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Utilities;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class BeforeWebProjectControllerLoaded : ActionFilterAttribute
    {

        public override void OnActionExecuted(ActionExecutedContext FilterContext)
        {

        }

        public override void OnActionExecuting(ActionExecutingContext FilterContext)
        {
            var C = FilterContext.Controller as Controller;
            var Model = LocalUtilities.GetWebProjectModelBaseFromController(C);                        
            if (Model != null)
            {
                var ActionDescriptor = FilterContext.ActionDescriptor as ControllerActionDescriptor;

                Model.ActionName = ActionDescriptor.ActionName;                
                Model.ControllerName = ActionDescriptor.ControllerTypeInfo.Name;

                Model.UrlCurrentPage = C.Request.Path;
                Model.WebsiteDomain = LocalUtilities.GetWebsiteDomain(C.Request);

                var db = FilterContext.HttpContext.RequestServices.GetService(typeof(DBCoreDataContext)) as DBCoreDataContext;
                Model.DataAccessFactory = new DataAccessFactory(db);                
                Model.AppSettings = FilterContext.HttpContext.RequestServices.GetService(typeof(AppSettingsModel)) as AppSettingsModel;

                var HttpContextAccessor = FilterContext.HttpContext.RequestServices.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                Model.SessionAssistance = new SessionAssistance(HttpContextAccessor);
                Model.CookieAssistance = new CookieAssistance(C.Request, C.Response);
                Model.Url = C.Url;
            }
        }
    }
}
