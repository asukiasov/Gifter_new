using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Abstractions.Web;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Shared
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
        public RepositoryFactory RepositoriesFactory { get; set; }
        public AppSettingsCollection AppSettings { get; set; }
        public UtilityCollection Utilities { get; set; }
        public ISessionAssistance SessionAssistance { get; set; }
        public ICookieAssistance CookieAssistance { get; set; }
        public IUrlHelper Url { get; set; }
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public IPageTitle PageTitle { get; set; }
        public ViewDataDictionary ViewData { get; set; }
        public IFileStorage FileStorage { get; set; }

        public Breadcrumbs Breadcrumbs { get; set; }
        public List<ProjectMenuItem> Tabs { get; set; } = [];

        public PluginsClient PluginsClient { get; set; }
        public UserDTO User { get; set; }
        public bool IsLoggedIn => User != null;
        public ValueWrapper<bool> IsSidebarCollapsed { get; set; }
        public FormViewModelBase Form { get; set; }
        public SystemPropertiesDTO SystemProperties { get; set; }

        public readonly string CultureDefault = Enums.Languages.GEORGIAN;
        public readonly SuccessErrorPartialViewModel SuccessErrorPartialViewModel = new();
        #endregion

        #region Methods
        public string GetFilenameFromUploadedFile(IFormFile postedFile)
        {
            return postedFile?.FileName.ToAZ09Dash(guidInlcuded: true);
        }

        public IActionResult GetNotFoundWebsiteViewResult()
        {
            var viewModel = new NotFoundViewModel();
            PluginsClient.EnableAdminTheme(true);
            viewModel.PluginsClient = PluginsClient;
            viewModel.UrlLogout = Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Logout);

            return new ViewResult
            {
                ViewName = ViewNames.Website.NotFound.Page,
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = viewModel
                }
            };
        }

        public IActionResult GetNotFoundAdminViewResult()
        {
            var viewModel = new NotFoundViewModel();
            PluginsClient.EnableAdminTheme(true);
            viewModel.PluginsClient = PluginsClient;

            return new ViewResult
            {
                ViewName = ViewNames.Admin.Shared.NotFound,
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = viewModel
                }
            };
        }

        public string GetRouteByName(string routeName, object routeValues = null)
        {
            var Result = Url.RouteUrl(routeName, routeValues);
            if (LanguageCultureCode != Constants.Languages.ENGLISH)
            {
                Result = $"{WebsiteHttpPath}{Result.TrimStart('/')}";
            }
            else
            {
                Result = $"{WebsiteHttpPath}{LanguageCultureCode}{Result}";
            }
            return Result;

        }

        public async Task SaveUploadedFile(IFormFile postedFile, string filename, string folderPath)
        {
            using (var MS = new MemoryStream())
            {
                await postedFile.CopyToAsync(MS);
                await FileStorage.SaveUploadedFile(MS, filename, folderPath);
            }
        }

        public async Task DeleteUploadedFile(string filename, string folderPath)
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
