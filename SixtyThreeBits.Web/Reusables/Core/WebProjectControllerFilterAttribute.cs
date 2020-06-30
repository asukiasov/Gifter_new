using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Utilities;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class BeforeWebProjectControllerLoaded : ActionFilterAttribute
    {
        DataAccessFactory DataAccessFactory;
        AppSettingsCollection AppSettings;
        UtilityCollection Utilities;

        public BeforeWebProjectControllerLoaded(AppSettingsCollection AppSettings, UtilityCollection Utilities, DataAccessFactory DataAccessFactory)
        {
            this.AppSettings = AppSettings;
            this.Utilities = Utilities;
            this.DataAccessFactory = DataAccessFactory;
        }

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

                Model.IsHttps = C.Request.IsHttps;

                Model.AppSettings = this.AppSettings;
                Model.Utilities = this.Utilities;                
                Model.DataAccessFactory = this.DataAccessFactory;

                Model.SessionAssistance = new SessionAssistance(C.HttpContext.Session);
                Model.CookieAssistance = new CookieAssistance(C.Request, C.Response);
                Model.Url = C.Url;
                Model.PluginsClient = new PluginsClient();
                Model.Form = new FormViewModelBase();
            }
        }
    }    
}
