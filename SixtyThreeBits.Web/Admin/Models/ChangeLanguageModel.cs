using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Web.Domain;
using System;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class ChangeLanguageModel : WebProjectModelBase
    {
        #region Methods
        public void ChangeLanguage(string culture)
        {
            if(Utilities.SupportedLanguageStrings.Contains(culture))
            {                
                CookieAssistance.Set(Constants.Cookies.AdminLanguageCultureCode, culture, DateTime.Now.AddMonths(12));
            }
        }
        #endregion
    }
}
