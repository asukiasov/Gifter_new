using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Models.Shared;
using System;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class ChangeLanguageModel : ModelBase
    {
        #region Methods
        public void ChangeLanguage(string culture)
        {
            if (Utilities.SupportedLanguageStrings.Contains(culture))
            {
                CookieAssistance.Set(WebConstants.Cookies.AdminLanguageCultureCode, culture, DateTime.Now.AddMonths(12));
            }
        }
        #endregion
    }
}
