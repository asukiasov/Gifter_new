using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using System;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class AdminLayoutViewModel : LayoutViewModelBase
    {
        #region Properties                
        public string ProjectName { get; set; }
        public string UserFullname { get; set; }
        public string UserEmail { get; set; }
        public ValueWrapper<bool> IsSidebarCollapsed { get; set; }
        public string UrlRelogin { get; set; }

        public Language LanguageActive { get; set; }
        public List<Language> Languages { get; set; }

        public readonly string TextBackToWebsite = Resources.TextBackToWebsite;
        public readonly string TextRelogin = Resources.TextRelogin;
        public readonly string TextLogout = Resources.TextLogout;

        public readonly string SidebarStatusCookieKey = Constants.Cookies.IsAdminSideBarCollapsed;
        public readonly string HeaderSectionName = Constants.ViewSections.HeaderSection;
        public readonly string FooterSectionName = Constants.ViewSections.FooterSection;
        #endregion

        #region Nested Classes
        public class Language
        {
            #region Properties
            public string LanguageCultureCode { get; set; }
            public string LanguageName { get; set; }
            public bool IsActive { get; set; }
            public string UrlChangeLanguage { get; set; }
            #endregion

            #region Methods
            public override string ToString()
            {
                return $"{LanguageCultureCode} - {LanguageName} - {UrlChangeLanguage}";
            }
            #endregion
        }
        #endregion
    }

    public class UserLayoutViewModel : LayoutViewModelBase
    {
        #region Properties        
        public string UserFullname { get; set; }
        public string UserDateCreated { get; set; }
        public bool UserIsActive { get; set; }
        public string RoleName { get; set; }        


        public readonly string HeaderSectionName = Constants.ViewSections.HeaderSection;
        public readonly string FooterSectionName = Constants.ViewSections.FooterSection;
        #endregion
    }
}