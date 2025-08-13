using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class CultureChangeModel : ModelBase
    {
        #region Methods
        public void Change(string culture)
        {
            if (Utilities.SupportedLanguageStrings.Contains(culture))
            {
                CookieAssistance.Set(WebConstants.Cookies.AdminLanguageCultureCode, culture, DateTime.Now.AddMonths(12));
            }
        }
        #endregion
    }
}
