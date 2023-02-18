using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Reusables.Core
{

    public class LayoutViewModelBase
    {
        #region Properties
        public IPageTitle PageTitle { get; set; }
        public SuccessErrorPartialViewModel SuccessErrorPartialViewModel { get; set; }
        public bool IsSuccessErrorPartialViewModelinitialized => SuccessErrorPartialViewModel?.IsInitialized == true;
        public List<ProjectMenuItem> Menu { get; set; }
        public bool HasMenu => Menu?.Count > 0;        
        public Breadcrumbs Breadcrumbs { get; set; }
        public bool ShowBreadCrumbs { get; set; }
        public List<ProjectMenuItem> Tabs { get; set; }
        public bool HasTabs => Tabs?.Count > 0;
        public string TabsLayoutViewName { get; set; } = ViewNames.Admin.Shared.Layout;
        public string UrlLogout { get; set; }
        public PluginsClient PluginsClient { get; set; }        
        public readonly string TextError = Resources.TextError;
        public readonly string TextSuccess = Resources.TextSuccess;
        #endregion
    }
            
    [TypeFilter(typeof(BeforeWebProjectControllerLoaded), Order = 0)]
    public class WebProjectController<T> :  Controller
    {
        #region Properties
        public T Model { get; set; }
        #endregion        
    }

    public class WebProjectModelBase 
    {
        #region Properties
        public string Culture { get; set; }
        public readonly string CultureDefault = Enums.Languages.GEORGIAN;
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string UrlCurrentPageWithDomain { get; set; }
        public string UrlCurrentPageWithoutDomain { get; set; }
        public string WebsiteDomain { get; set; }
        public string WebsiteHttpPath => $"{WebsiteDomain}/";
        public string IP { get; set; }
        public bool IsHttps { get; set; }
        public DataAccessFactory DataAccessFactory { get; set; }
        public AppSettingsCollection AppSettings { get; set; }
        public UtilityCollection Utilities { get; set; }
        public ISessionAssistance SessionAssistance { get; set; }
        public ICookieAssistance CookieAssistance { get; set; }
        public IUrlHelper Url { get; set; }
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public IPageTitle PageTitle { get; set; }
        public ViewDataDictionary ViewData { get; set; }
        
        public Breadcrumbs Breadcrumbs { get; set; }
        public List<ProjectMenuItem> Tabs { get; set; } = new List<ProjectMenuItem>();

        public PluginsClient PluginsClient { get; set; }
        public readonly SuccessErrorPartialViewModel SuccessErrorPartialViewModel = new SuccessErrorPartialViewModel();                
        public User User { get; set; }
        public bool IsLoggedIn => User != null;        
        public ValueReference<bool> IsSidebarCollapsed { get; set; }
        public FormViewModelBase Form { get; set; }
        public SystemProperties SystemProperties { get; set; }
        #endregion

        #region Methods
        public string GetFileManagerUrl(string FolderPhysicalPath, string FolderVirtualPath, bool AllowSelectMultiple = false, bool RestrictToImagesOnly = false, string OnSelectedFilesChooseClientCallback = null)
        {
            var SB = new StringBuilder();
            SB.Append(Url.RouteUrl(ControllerActionRouteNames.Admin.FileManager.Index, new { FolderVirtualPathHash = FolderVirtualPath.AESEncryptString(), FolderPhysicalPathHash = FolderPhysicalPath.AESEncryptString() }));
            if (AllowSelectMultiple || RestrictToImagesOnly || !string.IsNullOrWhiteSpace(OnSelectedFilesChooseClientCallback))
            {
                SB.Append("?");
                if (AllowSelectMultiple)
                {
                    SB.Append($"{nameof(AllowSelectMultiple)}=true&");
                }
                if (RestrictToImagesOnly)
                {
                    SB.Append("AllowedExtensions=.jpg,.jpeg,.png,.svg&");
                }
                if (!string.IsNullOrWhiteSpace(OnSelectedFilesChooseClientCallback))
                {
                    SB.Append($"{nameof(OnSelectedFilesChooseClientCallback)}={OnSelectedFilesChooseClientCallback}&");
                }
            }
            return SB.ToString().TrimEnd('&');
            /*
             /admin/filemanager/

            /admin/filemanager/?AllowSelectMultiple=true

            /admin/filemanager/?AllowedExtensions=.jpg,.jpeg,.png,.svg

            /admin/filemanager/?AllowSelectMultiple=true&AllowedExtensions=.jpg,.jpeg,.png,.svg
            */
        }

        public string GetFilenameFromUploadedFile(IFormFile PostedFile)
        {
            return PostedFile?.FileName.ToAZ09Dash(GuidInlcuded: true);
        }

        public IActionResult GetNotFoundViewResult()
        {
            return new ViewResult { ViewName = ViewNames.Admin.Shared.NotFound };
        }

        public IActionResult GetNotFoundAdminViewResult()
        {
            return new ViewResult { ViewName = ViewNames.Admin.Shared.NotFound };
        }

        public string GetRouteByName(string RouteName, object RouteValues = null, bool GetFullPath = false)
        {            
            var Result = Url.RouteUrl(RouteName, RouteValues);
            if (Culture != Constants.Languages.ENGLISH)
            {
                Result = $"{WebsiteHttpPath}{Result.TrimStart('/')}";                
            }
            else
            {
                Result = $"{WebsiteHttpPath}{Culture}{Result}";
            }
            return Result;

        }

        public async Task SaveUploadedFile(IFormFile PostedFile, string Filename, string FolderPhysicalPath = null)
        {
            if (string.IsNullOrWhiteSpace(FolderPhysicalPath))
            {
                FolderPhysicalPath = AppSettings.UploadFolderPhysicalPath;
            }

            using (var Stream = new FileStream($"{FolderPhysicalPath}{Filename}", FileMode.Create))
            {
                await PostedFile.CopyToAsync(Stream);
            }
        }
        
        #region SuccessError
        public void InitSuccessErrorPartialViewModel()
        {
            var ErrorMessage = SessionAssistance.Get<string>(Constants.Session.SuccessErrorMessage.Error);
            if (ErrorMessage != null)
            {
                SuccessErrorPartialViewModel.IsInitialized = true;
                SuccessErrorPartialViewModel.ShowError = true;
                SuccessErrorPartialViewModel.Message = ErrorMessage;
                SessionAssistance.Remove(Constants.Session.SuccessErrorMessage.Error);
            }
            else
            {
                var SuccessMessage = SessionAssistance.Get<string>(Constants.Session.SuccessErrorMessage.Success);
                if (SuccessMessage != null)
                {
                    SuccessErrorPartialViewModel.IsInitialized = true;
                    SuccessErrorPartialViewModel.ShowSuccess = true;
                    SuccessErrorPartialViewModel.Message = SuccessMessage;
                    SessionAssistance.Remove(Constants.Session.SuccessErrorMessage.Success);
                }
            }
        }

        public void PrepareSuccessErrorForJavascript()
        {
            SuccessErrorPartialViewModel.IsInitialized = true;
        }

        public void ShowSuccess(string Message = null)
        {
            if (string.IsNullOrWhiteSpace(Message))
            {
                Message = Resources.TextSuccess;
            }
            SessionAssistance.Set(Constants.Session.SuccessErrorMessage.Success, Message);
        }

        public void ShowError(string Message = null, bool UseSession = false)
        {
            if (string.IsNullOrWhiteSpace(Message))
            {
                Message = Resources.TextError;
            }

            if (UseSession)
            {
                SessionAssistance.Set(Constants.Session.SuccessErrorMessage.Error, Message);
            }
            else
            {
                SuccessErrorPartialViewModel.IsInitialized = true;
                SuccessErrorPartialViewModel.ShowError = true;
                SuccessErrorPartialViewModel.Message = Message;
            }
        }        
        #endregion
        #endregion
    }

    public class FormViewModelBase
    {        
        #region Properties        
        public List<SimpleKeyValue<string, string>> Errors { get; set; }
        public string ErrorMessage => Errors == null ? null : string.Join("<br />", Errors.Select(Item => Item.Value));
        public bool HasErrors => Errors?.Count > 0;
        public bool IsValid => !HasErrors;
        public string ErrorsJson => Errors.ToJson();        
        public bool IsSaved { get; set; }        
        #endregion

        #region Methods
        public void AddError(string ErrorKey,string ErrorMessage)
        {
            if (Errors == null)
            {
                Errors = new List<SimpleKeyValue<string, string>>();
            }

            Errors.Add(new SimpleKeyValue<string, string> { Key = ErrorKey, Value = ErrorMessage });
        }

        public void AddError(string ErrorMessage)
        {
            AddError(ErrorKey: null, ErrorMessage: ErrorMessage);
        }
        #endregion
    }    
}
