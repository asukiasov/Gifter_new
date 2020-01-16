using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public partial class LocalUtilities
    {
        #region Methods
        public static void AddBreadCrumbsItem<T>(ViewDataDictionary ViewData, string Caption, string NavigateUrl = null) where T : LayoutViewModelBase
        {
            var Model = GetLayoutViewModel<T>(ViewData, Constants.ViewData.LayoutViewModel);
            if (Model != null && Model.Breadcrumbs != null)
            {
                Model.Breadcrumbs.AddItem(new Breadcrumbs.BreadCrumbItem
                {
                    Caption = Caption,
                    NavigateUrl = NavigateUrl,
                    IsLastItem = true
                });
            }
            SetLayoutViewModel(ViewData: ViewData, ViewModel: Model, Key: Constants.ViewData.LayoutViewModel);
        }

        public static string GetCurrentPageUrl(HttpRequest Request)
        {
            var Url = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request);            
            return $"{Url.Split('?')[0].TrimEnd('/')}/";
        }

        public static string GetFileManagerUrl(IUrlHelper Url, string FolderPhysicalPath, string FolderVirtualPath, bool AllowSelectMultiple = false,bool RestrictToImagesOnly = false, string OnSelectedFilesChooseClientCallback = null)
        {
            var SB = new System.Text.StringBuilder();
            SB.Append(Url.RouteUrl(ControllerActionRouteNames.Admin.FileManager.Index, new { FolderVirtualPathHash = FolderVirtualPath.EncryptWeb(), FolderPhysicalPathHash = FolderPhysicalPath.EncryptWeb() }));
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

        public static T GetLayoutViewModel<T>(ViewDataDictionary ViewData, string Key)
        {
            return (T)ViewData[Key];
        }

        public static string GetWebsiteDomain(HttpRequest Request)
        {
            var Port = Request.Host.Port;
            var PortString = Port < 1000 ? "" : $":{Port}";

            var WebsiteDomain = $"{Request.Scheme}://{Request.Host.Host}{PortString}";
            return WebsiteDomain;
        }

        public static WebProjectModelBase GetWebProjectModelBaseFromController(object Controller)
        {
            dynamic C = Controller;
            var Model = C.Model as WebProjectModelBase;
            return Model;
        }

        public static string LogRequest(HttpRequest Request, string LogFilePhysicalPath = null)
        {
            var SB = new System.Text.StringBuilder();
            SB.Append($"QueryString:{Environment.NewLine}");
            foreach (var Key in Request.Query.Keys)
            {
                SB.Append($"{Key}: {Request.Query[Key]}{Environment.NewLine}");
            }
            SB.Append($"{Environment.NewLine}{Environment.NewLine}Form:{Environment.NewLine}");
            foreach (var Key in Request.Form.Keys)
            {
                SB.Append($"{Key}: {Request.Form[Key]}{Environment.NewLine}");
            }
            var RequestString = SB.ToString();
            if (!string.IsNullOrWhiteSpace(LogFilePhysicalPath))
            {
                RequestString.LogString(LogFilePhysicalPath);
            }
            return RequestString;
        }

        public static void RemoveBreadCrumbsItem<T>(ViewDataDictionary ViewData, int? Index = null) where T : LayoutViewModelBase
        {
            var Model = GetLayoutViewModel<T>(ViewData, Constants.ViewData.LayoutViewModel);

            if (Model?.Breadcrumbs?.Items?.Count > 0)
            {
                Index = Index ?? Model.Breadcrumbs.Items.Count - 1;
                Model.Breadcrumbs.DeleteItem(Index.Value);
            }
            SetLayoutViewModel(ViewData: ViewData, ViewModel: Model, Key: Constants.ViewData.LayoutViewModel);
        }

        public static void SetPageTitle<T>(ViewDataDictionary ViewData, string PageTitle, bool UpdateLastBreadcrumbItem = false) where T : LayoutViewModelBase
        {
            var Model = GetLayoutViewModel<T>(ViewData, Constants.ViewData.LayoutViewModel);
            if (Model != null)
            {
                Model.PageTitle = PageTitle;
                if (UpdateLastBreadcrumbItem && Model.HasBreadcrumbs)
                {
                    Model.Breadcrumbs.Items[Model.Breadcrumbs.ItemsCount - 1].Caption = PageTitle;
                }
            }
        }

        public static void SetLayoutViewModel<T>(ViewDataDictionary ViewData, T ViewModel, string Key)
        {
            ViewData[Key] = ViewModel;            
        }

        public static void UpdateBreadCrumbsItem<T>(ViewDataDictionary ViewData, string Caption, bool RemovePrevious = false, int? Index = null, string NavigateUrl = null) where T : LayoutViewModelBase
        {
            var Model = GetLayoutViewModel<T>(ViewData, Constants.ViewData.LayoutViewModel);
            if (Model?.Breadcrumbs?.Items?.Count > 0)
            {
                Index = Index?? Model.Breadcrumbs.Items.Count - 1;
                
                Model.Breadcrumbs.UpdateItem(new Breadcrumbs.BreadCrumbItem
                {
                    Caption = Caption,
                    NavigateUrl = NavigateUrl
                }, Index.Value);
                if (RemovePrevious)
                {
                    Model.Breadcrumbs.DeleteItem(Index.Value-1);
                }
            }
            SetLayoutViewModel(ViewData: ViewData, ViewModel: Model, Key: Constants.ViewData.LayoutViewModel);
        }
        #endregion
    }    
}