using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class LocalUtilities
    {
        #region Methods
        public static bool IsAjaxRequest(HttpRequest Request)
        {
            var Header = Request?.Headers["X-Requested-With"].ToString();
            return Header == "XMLHttpRequest";
        }

        public static string GetClientIP(HttpRequest Request)
        {
            return Request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public static T GetModelFromController<T>(object Controller) where T : class
        {
            dynamic C = Controller;
            var Model = C.Model as T;
            return Model;
        }

        public static T GetLayoutViewModel<T>(ViewDataDictionary ViewData, string Key = null)
        {
            if (Key == null)
            {
                Key = Constants.ViewData.LayoutViewModel;
            }
            return (T)ViewData[Key];
        }

        public static List<DevExtremeGridFilterItem> GetDevExtremeGridFilterValues(string FilterString)
        {
            var Filters = string.IsNullOrWhiteSpace(FilterString) ? new List<DevExtremeGridFilterItem>() :
            Regex.Matches(FilterString, @"\[\""(?<key>\w+)\"",\""(?<operator>[^\""]+)\"",(\"")?(?<value>[^\""|\]]+)(\"")?\]").OfType<Match>()

            .Select(Item => new DevExtremeGridFilterItem
            {
                FieldName = Item.Groups["key"].Value,
                Operator = Item.Groups["operator"].Value,
                Value = Item.Groups["value"].Value,
            }).ToList() ?? new List<DevExtremeGridFilterItem>();

            return Filters;
        }

        public static List<DevExtremeGridSortItem> GetDevExtremeGridSortValues(string SortString)
        {
            var SortValues = string.IsNullOrWhiteSpace(SortString) ? new List<DevExtremeGridSortItem>() :
            //[{"selector":"CaseID","desc":false}]
            Regex.Matches(SortString, @"\{\""selector\"":\""(?<key>\w+)\"",\""desc\"":(?<value>\w+)\}")
            .OfType<Match>()
            .Select(Item => new DevExtremeGridSortItem
            {
                FieldName = Item.Groups["key"].Value,
                IsDescending = Item.Groups["value"].Value == "true",
            }).ToList() ?? new List<DevExtremeGridSortItem>();

            return SortValues;
        }

        public static string GetWebsiteDomain(HttpRequest Request)
        {
            var Port = Request.Host.Port;
            var HostString = Request.Host.Host.TrimEnd(':');
            var PortString = (Port == 80 || Port == 443 || Port == null) ? "" : $":{Port}";

            var WebsiteDomain = $"{Request.Scheme}://{HostString}{PortString}";
            return WebsiteDomain;
        }

        public async static Task SaveUploadedFile(IFormFile PostedFile, string Filename, AppSettingsCollection AppSettings)
        {
            var FilePhysicalPath = $"{AppSettings.UploadFolderPhysicalPath}{Filename}";
            using (var FS = new FileStream(FilePhysicalPath, FileMode.Create))
            {
                await PostedFile.CopyToAsync(FS);
            }
        }

        public static void SetLayoutViewModel<T>(ViewDataDictionary ViewData, T ViewModel, string Key)
        {
            ViewData[Key] = ViewModel;
        }
        #endregion
    }
}