using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain.Libraries;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SixtyThreeBits.Web.Domain
{
    public class WebUtilities
    {
        #region Methods
        public static bool IsAjaxRequest(HttpRequest request)
        {
            var Header = request?.Headers["X-Requested-With"].ToString();
            return Header == "XMLHttpRequest";
        }

        public static string GetClientIP(HttpRequest request)
        {
            return request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public static T GetModelFromController<T>(object controller) where T : class
        {
            dynamic C = controller;
            var Model = C.Model as T;
            return Model;
        }

        public static T GetLayoutViewModel<T>(ViewDataDictionary viewData, string key)
        {
            return (T)viewData[key];
        }

        public static List<DevExtremeGridFilterItem> GetDevExtremeGridFilterValues(string filterString)
        {
            var filters = string.IsNullOrWhiteSpace(filterString) ? new List<DevExtremeGridFilterItem>() :
            Regex.Matches(filterString, @"\[\""(?<key>\w+)\"",\""(?<operator>[^\""]+)\"",(\"")?(?<value>[^\""|\]]+)(\"")?\]").OfType<Match>()

            .Select(Item => new DevExtremeGridFilterItem
            {
                FieldName = Item.Groups["key"].Value,
                Operator = Item.Groups["operator"].Value,
                Value = Item.Groups["value"].Value,
            }).ToList() ?? new List<DevExtremeGridFilterItem>();

            return filters;
        }

        public static List<DevExtremeGridSortItem> GetDevExtremeGridSortValues(string sortString)
        {
            var sortValues = string.IsNullOrWhiteSpace(sortString) ? new List<DevExtremeGridSortItem>() :
            //[{"selector":"CaseID","desc":false}]
            Regex.Matches(sortString, @"\{\""selector\"":\""(?<key>\w+)\"",\""desc\"":(?<value>\w+)\}")
            .OfType<Match>()
            .Select(item => new DevExtremeGridSortItem
            {
                FieldName = item.Groups["key"].Value,
                IsDescending = item.Groups["value"].Value == "true",
            }).ToList() ?? new List<DevExtremeGridSortItem>();

            return sortValues;
        }

        public static string GetWebsiteDomain(HttpRequest request)
        {
            var port = request.Host.Port;
            var hostString = request.Host.Host.TrimEnd(':');
            var portString = port == 80 || port == 443 || port == null ? "" : $":{port}";

            var websiteDomain = $"{request.Scheme}://{hostString}{portString}";
            return websiteDomain;
        }

        public static void SetLayoutViewModel<T>(ViewDataDictionary viewData, T viewModel, string key)
        {
            viewData[key] = viewModel;
        }
        #endregion
    }
}