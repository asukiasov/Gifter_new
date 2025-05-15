using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain.Utilities;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Domain.Libraries
{
    public class CustomCultureProvider : RequestCultureProvider
    {
        #region Properties
        readonly UtilityCollection _utilities;
        #endregion

        #region Constructors
        public CustomCultureProvider(UtilityCollection utilities)
        {
            _utilities = utilities;
        }
        #endregion

        #region Methods
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext context)
        {
            string culture;
            var path = context.Request.Path.ToString() ?? string.Empty;
            if (path.StartsWith("/admin/") || path == "/admin")
            {
                var languageCultureCode = context.Request.Cookies[WebConstants.Cookies.AdminLanguageCultureCode]?.ToString();
                var language = _utilities.GetSupportedLanguageOrDefault(languageCultureCode);
                culture = language.LanguageCultureCode;
            }
            else
            {
                culture = context.Request.RouteValues[WebConstants.RouteValues.Culture]?.ToString() ?? _utilities.LanguageDefault.LanguageCultureCode;
            }

            await Task.Yield();
            return new ProviderCultureResult(culture);
        }
        #endregion
    }
}
