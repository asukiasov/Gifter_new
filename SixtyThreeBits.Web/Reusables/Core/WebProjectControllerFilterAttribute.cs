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
                Model.WebsiteDomain = Model.GetWebsiteDomain(C.Request);

                var db = FilterContext.HttpContext.RequestServices.GetService(typeof(DBCoreDataContext)) as DBCoreDataContext;
                Model.DataAccessFactory = new DataAccessFactory(db);                
                Model.AppSettings = FilterContext.HttpContext.RequestServices.GetService(typeof(AppSettingsModel)) as AppSettingsModel;
                Model.Utilities = FilterContext.HttpContext.RequestServices.GetService(typeof(UtilityCollection)) as UtilityCollection;

                Model.SessionAssistance = FilterContext.HttpContext.RequestServices.GetService(typeof(ISessionAssistance)) as ISessionAssistance;
                Model.CookieAssistance = new CookieAssistance(C.Request, C.Response);
                Model.Url = C.Url;
                Model.PluginClient = new PluginClient();
            }
        }
    }
}
