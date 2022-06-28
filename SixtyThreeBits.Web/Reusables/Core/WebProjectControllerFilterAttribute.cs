using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Utilities;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class BeforeWebProjectControllerLoaded : IAsyncActionFilter
    {        
        AppSettingsCollection AppSettings;
        UtilityCollection Utilities;

        public BeforeWebProjectControllerLoaded(AppSettingsCollection AppSettings, UtilityCollection Utilities)
        {
            this.AppSettings = AppSettings;
            this.Utilities = Utilities;            
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext FilterContext, ActionExecutionDelegate next)
        {            
            var C = FilterContext.Controller as Controller;            
            var Model = LocalUtilities.GetModelFromController<WebProjectModelBase>(C);
            if (Model != null)
            {
                var ActionDescriptor = FilterContext.ActionDescriptor as ControllerActionDescriptor;

                Model.ActionName = ActionDescriptor.ActionName;                
                Model.ControllerName = ActionDescriptor.ControllerTypeInfo.Name;
                
                Model.WebsiteDomain = LocalUtilities.GetWebsiteDomain(C.Request);
                Model.UrlCurrentPage = $"{Model.WebsiteDomain}{C.Request.Path}";

                Model.IsHttps = C.Request.IsHttps;
                Model.IP = LocalUtilities.GetClientIP(C.Request);

                Model.AppSettings = AppSettings;
                Model.Utilities = Utilities;
                Model.DataAccessFactory = new DataAccessFactory(AppSettings, Utilities);

                Model.SessionAssistance = new SessionAssistance(C.HttpContext.Session);
                Model.CookieAssistance = new CookieAssistance(C.Request, C.Response);
                Model.Url = C.Url;
                Model.Request = C.Request;
                Model.Response = C.Response;
                Model.PluginsClient = new PluginsClient();
                Model.Form = new FormViewModelBase();

                await InitUser(Model);
                await next();
            }
        }

        async Task InitUser(WebProjectModelBase Model)
        {
            Model.User = Model.SessionAssistance.Get<User>(Constants.Session.User);

            if (Model.User == null)
            {
                var UserID = Model.CookieAssistance.Get<int?>(Constants.Cookies.User);
                if (UserID != null)
                {
                    Model.User = await Model.DataAccessFactory.Users.GetSingleUserByID(UserID);
                    Model.SessionAssistance.Set(Constants.Session.User, Model.User);
                }
            }
        }
    }    
}
