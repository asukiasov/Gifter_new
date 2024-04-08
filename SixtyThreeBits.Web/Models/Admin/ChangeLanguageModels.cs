using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
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
