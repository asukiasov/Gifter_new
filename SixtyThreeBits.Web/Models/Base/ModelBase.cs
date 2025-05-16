using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Abstractions.Web;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Base
{
    public class ModelBase
    {
        #region Properties
        public string LanguageCultureCode { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string UrlCurrentPageWithDomain { get; set; }
        public string UrlCurrentPageWithoutDomain { get; set; }
        public string UrlPreviousPage { get; set; }
        public string WebsiteDomain { get; set; }
        public string WebsiteHttpPath => $"{WebsiteDomain}/";
        public string IP { get; set; }
        public bool IsHttps { get; set; }
        public bool IsAjaxRequest { get; set; }
        public RepositoryFactory RepositoriesFactory { get; set; }
        public AppSettingsCollection AppSettings { get; set; }
        public UtilityCollection Utilities { get; set; }
        public ISessionAssistance SessionAssistance { get; set; }
        public ICookieAssistance CookieAssistance { get; set; }
        public Controller Controller { get; set; }
        public IUrlHelper Url { get; set; }
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public IPageTitle PageTitle { get; set; }
        public ViewDataDictionary ViewData { get; set; }
        public IFileStorage FileStorage { get; set; }

        public Breadcrumbs Breadcrumbs { get; set; }
        public List<ProjectMenuViewItem> Tabs { get; set; } = [];

        public PluginsClientViewModel PluginsClient { get; set; }
        public UserDTO User { get; set; }
        public bool IsLoggedIn => User != null;
        public ValueWrapper<bool> IsSidebarCollapsed { get; set; }
        public FormViewModelBase Form { get; set; }
        public SystemPropertiesDTO SystemProperties { get; set; }
        public NotificationManager NotificationManager { get; set; }

        public readonly string CultureDefault = Enums.Languages.GEORGIAN;
        public readonly SuccessErrorToastPartialViewModel SuccessErrorPartialViewModel = new();
        #endregion

        #region Methods
        public string GetFilenameFromUploadedFile(IFormFile postedFile)
        {
            return postedFile?.FileName.ToAZ09Dash(guidInlcuded: true);
        }

        public IActionResult GetNotFoundWebsiteViewResult()
        {
            var viewModel = new NotFoundViewModel();
            viewModel.PluginsClient = PluginsClient;
            viewModel.UrlLogout = Url.RouteUrl(ControllerActionRouteNames.Admin.AuthController.Logout);

            return new ViewResult
            {
                ViewName = ViewNames.Website.Errors.NotFoundView,
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = viewModel
                },
                StatusCode = Enums.HttpStatusCodes.Status404NotFound
            };
        }

        public IActionResult GetNotFoundAdminViewResult()
        {
            var viewModel = new NotFoundViewModel();
            PluginsClient.EnableAdminTheme(true);
            viewModel.PluginsClient = PluginsClient;
            viewModel.UrlLogout = Url.RouteUrl(ControllerActionRouteNames.Admin.AuthController.Logout);

            return new ViewResult
            {
                ViewName = ViewNames.Admin.Errors.NotFoundView,
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = viewModel
                },
                StatusCode = Enums.HttpStatusCodes.Status404NotFound
            };
        }

        public string GetRouteByName(string routeName, object routeValues = null, string languageCultureCode = null)
        {
            if (string.IsNullOrWhiteSpace(languageCultureCode))
            {
                languageCultureCode = LanguageCultureCode;
            }

            var result = Url.RouteUrl(routeName, routeValues);
            if (languageCultureCode == Utilities.LanguageDefault.LanguageCultureCode)
            {
                result = $"{WebsiteHttpPath}{result.TrimStart('/')}";
            }
            else
            {
                result = $"{WebsiteHttpPath}{languageCultureCode}{result}";
            }
            return result;

        }

        public string GetUrlPages(string pageSlug, string languageCultureCode = null)
        {
            var url = GetRouteByName(routeName: ControllerActionRouteNames.Website.PagesController.Page, new { pageSlug = pageSlug }, languageCultureCode: languageCultureCode);
            return url;
        }

        public async Task SaveUploadedFile(IFormFile postedFile, string filename, string folderPath = null)
        {
            using (var MS = new MemoryStream())
            {
                await postedFile.CopyToAsync(MS);
                await FileStorage.SaveUploadedFile(MS, filename, folderPath);
            }
        }

        public async Task DeleteUploadedFile(string filename, string folderPath = null)
        {
            await FileStorage.DeleteFile(filename, folderPath);
        }

        #region Success/Error Toast
        public void InitSuccessErrorToastNotificationPartialViewModel()
        {            
            var errorMessage = SessionAssistance.Get<string>(WebConstants.Session.SuccessErrorMessage.Error);
            if (errorMessage != null)
            {
                SuccessErrorPartialViewModel.ShowError = true;
                SuccessErrorPartialViewModel.Message = errorMessage;
                SessionAssistance.Remove(WebConstants.Session.SuccessErrorMessage.Error);
            }
            else
            {
                var successMessage = SessionAssistance.Get<string>(WebConstants.Session.SuccessErrorMessage.Success);
                if (successMessage != null)
                {
                    SuccessErrorPartialViewModel.ShowSuccess = true;
                    SuccessErrorPartialViewModel.Message = successMessage;
                    SessionAssistance.Remove(WebConstants.Session.SuccessErrorMessage.Success);
                }
            }
        }

        public void ShowSuccessToastNotification(string message = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = Resources.TextSuccess;
            }
            SessionAssistance.Set(WebConstants.Session.SuccessErrorMessage.Success, message);
        }

        public void ShowErrorToastNotification(string message = null, bool shouldDisplayAfterPageReload = false)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = Resources.TextError;
            }

            if (shouldDisplayAfterPageReload)
            {
                SessionAssistance.Set(WebConstants.Session.SuccessErrorMessage.Error, message);
            }
            else
            {
                SuccessErrorPartialViewModel.ShowError = true;
                SuccessErrorPartialViewModel.Message = message;
            }
        }
        #endregion

        #endregion
    }
}
