using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class LocalUtilities
    {                        
        public static string GetClientIP(HttpRequest Request)
        {
            return Request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public static string GetWebsiteDomain(HttpRequest Request)
        {
            var Port = Request.Host.Port;
            var HostString = Request.Host.Host.TrimEnd(':');
            var PortString = (Port == 80 || Port == 443 || Port == null) ? "" : $":{Port}";

            var WebsiteDomain = $"{Request.Scheme}://{HostString}{PortString}";
            return WebsiteDomain;
        }                

        public static T GetLayoutViewModel<T>(ViewDataDictionary ViewData, string Key = Constants.ViewData.LayoutViewModel)
        {            
            return (T)ViewData[Key];
        }

        public static WebProjectModelBase GetWebProjectModelBaseFromController(object Controller)
        {
            dynamic C = Controller;
            var Model = C.Model as WebProjectModelBase;
            return Model;
        }

        public static bool IsAjaxRequest(HttpRequest Request)
        {
            var Header = Request?.Headers["X-Requested-With"].ToString();
            return Header == "XMLHttpRequest";
        }

        public void LogRequest(HttpRequest Request, string LogFilePhysicalPath = null)
        {
            var SB = new StringBuilder();
            SB.AppendLine($"QueryString: ");
            foreach (var Key in Request.Query.Keys)
            {
                SB.AppendLine($"{Key}={Request.Query[Key]}");
            }
            SB.AppendLine();

            SB.AppendLine($"Form: ");
            foreach (var Key in Request.Form.Keys)
            {
                SB.AppendLine($"{Key}={Request.Form[Key]}");
            }

            SB.Append($"Body: ");
            SB.Append(Request.Body);
            SB.AppendLine();

            var LogString = SB.ToString();
            if (!string.IsNullOrWhiteSpace(LogFilePhysicalPath))
            {
                LogString.LogString(LogFilePhysicalPath);
            }
        }

        public static void SetLayoutViewModel<T>(ViewDataDictionary ViewData, T ViewModel, string Key)
        {
            ViewData[Key] = ViewModel;
        }
    }    
}