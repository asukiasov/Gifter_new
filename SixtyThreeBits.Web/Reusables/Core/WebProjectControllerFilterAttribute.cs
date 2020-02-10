using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Utilities;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class BeforeWebProjectControllerLoaded : ActionFilterAttribute
    {
        DBCoreDataContext db;
        AppSettingsModel AppSettings;
        UtilityCollection Utilities;
        ISessionAssistance SessionAssistance;

        public BeforeWebProjectControllerLoaded(DBCoreDataContext db, AppSettingsModel AppSettings, UtilityCollection Utilities, ISessionAssistance SessionAssistance)
        {
            this.db = db;
            this.AppSettings = AppSettings;
            this.Utilities = Utilities;
            this.SessionAssistance = SessionAssistance;
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

                Model.AppSettings = this.AppSettings;
                Model.Utilities = this.Utilities;
                Model.SessionAssistance = this.SessionAssistance;

                Model.DataAccessFactory = new DataAccessFactory(db);                                                
                Model.CookieAssistance = new CookieAssistance(C.Request, C.Response);
                Model.Url = C.Url;
                Model.PluginClient = new PluginClient();
                Model.Form = new FormViewModelBase(Model.Utilities);
            }
        }
    }    
}
