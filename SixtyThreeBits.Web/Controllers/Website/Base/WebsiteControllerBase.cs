using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Controllers.Base;
using SixtyThreeBits.Web.Filters.Website;

namespace SixtyThreeBits.Web.Controllers.Website.Base
{
    [TypeFilter(typeof(BeforeWebsitePageLoad), Order = 1)]
    public class WebsiteControllerBase<T> : ControllerBase<T> where T : new ()
    {
        #region Properties
        public const string RouteValueLanguageCode = $"{{Culture:regex(^({RouteLanguageCodes})$)}}";
        public const string RouteLanguageCodes = $"{Enums.Languages.GEORGIAN}|{Enums.Languages.ENGLISH}"; 
        #endregion
    }
}
