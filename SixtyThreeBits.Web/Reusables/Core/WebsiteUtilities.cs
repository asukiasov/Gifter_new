using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace SixtyThreeBits.Web.Reusables.Core
{
    public class WebsiteUtilities
    {                 
        public static WebProjectModelBase GetWebProjectModelBaseFromController(object Controller)
        {
            dynamic C = Controller;
            var Model = C.Model as WebProjectModelBase;
            return Model;
        }        

        public static void SetLayoutViewModel<T>(ViewDataDictionary ViewData, T ViewModel, string Key)
        {
            ViewData[Key] = ViewModel;            
        }

        public static T GetLayoutViewModel<T>(ViewDataDictionary ViewData, string Key = null)
        {
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
    }    
}