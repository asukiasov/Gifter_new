using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public interface IDevExtremeGridModel<T> where T : class
    {
        #region Properties
        DataGridBuilder<T> Render(IHtmlHelper Html); 
        #endregion
    }    

    public interface IDevExtremeTreeModel<T> where T : class
    {
        #region Properties
        TreeListBuilder<T> Render(IHtmlHelper Html);
        #endregion
    }

    public class DevExtremeGridViewModelBase
    {
        #region Properties        
        public bool AllowAdd { get; set; }
        public bool AllowUpdate { get; set; }
        public bool AllowDelete { get; set; }

        public string UrlLoad { get; set; }
        public object LoadParams { get; set; }
        public string UrlAddNew { get; set; }
        public string UrlUpdate { get; set; }        
        public string UrlDelete { get; set; }        

        public string BeforeSendJSFunction { get; set; }        

        public bool IsError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public string ErrorMessage { get; set; }
        #endregion        

        #region Methods
        public DataGridBuilder<T> GetGridWithStartupValues<T>(IHtmlHelper Html, string KeyFieldName)
        {
            return Html.DevExtreme().DataGrid<T>()
            .Width("100%")
            .ShowBorders(true)
            .ShowRowLines(true)
            .FocusedRowEnabled(true)
            .FocusedRowIndex(0)
            .Scrolling(Options =>
            {
                Options.Mode(GridScrollingMode.Standard);
                Options.ShowScrollbar(ShowScrollbarMode.Always);
            })
            .OnToolbarPreparing("function(e){ e.toolbarOptions.visible = false;  }")
            .FilterRow(Options =>
            {
                Options.Visible(true);
                Options.ApplyFilter(GridApplyFilterMode.Auto);
                Options.ShowAllText(Resources.TextAllDevexpressGridFilterRaw);
            })
            .DataSource(Options => {
                var OptionsResult = Options.RemoteController();
                OptionsResult.Key(KeyFieldName);
                OptionsResult.LoadUrl(UrlLoad);                
                OptionsResult.InsertUrl(UrlAddNew);
                OptionsResult.UpdateUrl(UrlUpdate);
                OptionsResult.DeleteUrl(UrlDelete);

                if (!string.IsNullOrWhiteSpace(BeforeSendJSFunction))
                {
                    OptionsResult.OnBeforeSend(BeforeSendJSFunction);
                }
                if (LoadParams != null)
                {
                    OptionsResult.LoadParams(LoadParams);                    
                }

                return OptionsResult;    
            })
            .Editing(Options =>
            {
                Options.Mode(GridEditMode.Row);
                //Options.Mode(GridEditMode.Cell);
                Options.AllowAdding(AllowAdd);
                Options.AllowUpdating(AllowUpdate);
                Options.AllowDeleting(AllowDelete);
                Options.Texts(OptionsTexts =>
                {
                    OptionsTexts.ConfirmDeleteMessage(Resources.TextConfirmDelete);
                    
                });

            })            
            .Pager(Options =>
            {
                Options.AllowedPageSizes(new[] { 30, 50, 100 });
                Options.ShowInfo(true);
                Options.ShowPageSizeSelector(true);
                Options.Visible(true);
            })
            .Paging(Options =>
            {
                Options.Enabled(true);
                Options.PageSize(30);
            })            
            .Columns(Columns =>
            {                
                if (AllowAdd || AllowUpdate || AllowDelete)
                {
                    var Width = 60;
                    //var Width = 30;
                    Columns.Add().Type(GridCommandColumnType.Buttons).Alignment(HorizontalAlignment.Center).Width(Width).Visible(AllowDelete).Buttons(b =>
                    {                           
                        b.Add().Name(GridColumnButtonName.Edit).Icon("fas fa-pencil-alt").Text(Resources.TextUpdate);
                        b.Add().Name(GridColumnButtonName.Delete).Icon("fas fa-trash-alt").Text(Resources.TextDelete);
                        b.Add().Name(GridColumnButtonName.Save).Icon("fas fa-check").Text(Resources.TextSave);
                        b.Add().Name(GridColumnButtonName.Cancel).Icon("fas fa-minus-circle").Text(Resources.TextCancel);
                    });
                }
            });
        }

        public TreeListBuilder<T> GetTreeWithStartupValues<T>(IHtmlHelper Html, string KeyFieldName, string ParentFieldName)
        {
            return Html.DevExtreme().TreeList<T>()
            .KeyExpr(KeyFieldName)
            .ParentIdExpr(ParentFieldName)
            .Width("100%")
            .ShowBorders(true)
            .ShowRowLines(true)
            .FocusedRowEnabled(true)
            .FocusedRowIndex(0)
            .AutoExpandAll(true)
            .RootValue(null)
            .Scrolling(Options =>
            {
                Options.Mode(TreeListScrollingMode.Standard);
                Options.ShowScrollbar(ShowScrollbarMode.Always);
            })
            .FilterRow(Options =>
            {
                Options.Visible(true);
                Options.ApplyFilter(GridApplyFilterMode.Auto);
                Options.ShowAllText(Resources.TextAllDevexpressGridFilterRaw);
            })            
            .DataSource(Options =>
                Options.RemoteController()
                .LoadUrl(UrlLoad)
                .InsertUrl(UrlAddNew)
                .UpdateUrl(UrlUpdate)
                .DeleteUrl(UrlDelete)
                .Key(KeyFieldName)
            )
            .Editing(Options =>
            {                
                Options.Mode(GridEditMode.Cell);
                Options.AllowAdding(AllowAdd);
                Options.AllowUpdating(AllowUpdate);
                Options.AllowDeleting(AllowDelete);                
                Options.Texts(OptionsTexts =>
                {
                    OptionsTexts.ConfirmDeleteMessage(Resources.TextConfirmDelete);
                });

            })
            .Pager(Options =>
            {
                Options.AllowedPageSizes(new[] { 30, 50, 100 });
                Options.ShowInfo(true);
                Options.ShowPageSizeSelector(true);
                Options.Visible(true);
            })
            .Paging(Options =>
            {
                Options.Enabled(true);
                Options.PageSize(30);
            })            
            .Columns(Columns =>
            {
                if (AllowAdd || AllowUpdate || AllowDelete)
                {
                    var Width = (AllowDelete && !AllowAdd && !AllowUpdate) ? 30 : 60;
                    Columns.Add().Alignment(HorizontalAlignment.Center).Type(TreeListCommandColumnType.Buttons).Width(Width).Buttons(b =>
                    {                        
                        b.Add().Name(TreeListColumnButtonName.Add).Icon("fas fa-plus").Text(Resources.TextAdd);
                        b.Add().Name(TreeListColumnButtonName.Edit).Icon("fas fa-pencil-alt").Text(Resources.TextUpdate);
                        b.Add().Name(TreeListColumnButtonName.Delete).Icon("fas fa-trash-alt").Text(Resources.TextDelete);
                        b.Add().Name(TreeListColumnButtonName.Save).Icon("fas fa-check").Text(Resources.TextSave);
                        b.Add().Name(TreeListColumnButtonName.Cancel).Icon("fas fa-minus-circle").Text(Resources.TextCancel);
                    });
                }
            });
        }

        public void InitTextboxColumn<T>(DataGridColumnBuilder<T> Column, bool IsRequired = false, bool ShouldValidateEmailFormat = false, int? MaxLength = null)
        {
            Column.ValidationRules(Options =>
            {
                if (IsRequired)
                {
                    Options.AddRequired().Message(Resources.ValidationRequired).Trim(true);
                }
                if (ShouldValidateEmailFormat)
                {
                    Options.AddEmail().Message(Resources.ValidationEmailFormatInvalid);
                }
                if (MaxLength > 0)
                {
                    Options.AddStringLength().Min(1).Max(MaxLength.Value).Message(string.Format(Resources.ValidationTextMaxLength, MaxLength));
                }
            });
        }

        public void InitLookupColumn<T1,T2,T3>(DataGridColumnBuilder<T1> Column, IEnumerable<SimpleKeyValue<T2, T3>> Data, bool IsRequired = false)
        {
            Column.Lookup(Options =>
            {
                Options.DataSource(d => d.Array().Data(Data).Key(nameof(SimpleKeyValue<T2, T3>.Key))).ValueExpr(nameof(SimpleKeyValue<T2, T3>.Key)).DisplayExpr(nameof(SimpleKeyValue<T2, T3>.Value));
            });

            if (IsRequired)
            {
                Column.ValidationRules(Options =>
                {
                    Options.AddRequired().Message(Resources.ValidationRequired).Trim(true);
                });
            }
        }

        public void InitCheckboxColumn<T>(DataGridColumnBuilder<T> Column)
        {            
            Column.TrueText(Resources.TextYes);
            Column.FalseText(Resources.TextNo);
        }

        public void InitDateColumn<T>(DataGridColumnBuilder<T> Column, bool FormatDateTime = false)
        {
            if (FormatDateTime)
            {
                Column.Format(Constants.Formats.DateTime);
            }
            else
            {
                Column.Format(Constants.Formats.Date);                
            }            
        }
        #endregion
    }
    
    public class LayoutViewModelBase
    {
        #region Properties
        public IPageTitle PageTitle { get; set; }
        public SuccessErrorPartialViewModel SuccessErrorPartialViewModel { get; set; }
        public bool IsSuccessErrorPartialViewModelinitialized => SuccessErrorPartialViewModel?.IsInitialized == true;
        public List<ProjectMenuItem> Menu { get; set; }
        public bool HasMenu => Menu?.Count > 0;        
        public Breadcrumbs Breadcrumbs { get; set; }
        public bool HasBreadcrumbs => Breadcrumbs != null;
        public List<ProjectMenuItem> Tabs { get; set; }
        public bool HasTabs => Tabs?.Count > 0;
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
        public string UrlCurrentPage { get; set; }
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
            string Protocol = GetFullPath ? (IsHttps ? Constants.Protocols.HTTPS : Constants.Protocols.HTTP) : null;
            RouteName = string.IsNullOrWhiteSpace(Culture) || Culture == CultureDefault ? RouteName : $"{RouteName}Culture";
            var UrlResult = Url.RouteUrl(RouteName, RouteValues, Protocol);            
            return UrlResult;
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
        public string ErrorsJson => Errors.ToJSON();        
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
