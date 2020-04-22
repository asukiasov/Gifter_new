using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Reusables;
using SixtyThreeBits.Web.Reusables.Core;
using System;
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
                    var Width = 30;
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

    public class DevExtremeGridFilterItem
    {
        #region Properties
        public string FieldName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; } 
        #endregion
    }

    public class DevExtremeGridSortItem
    {
        #region Properties
        public string FieldName { get; set; }
        public bool IsDescending { get; set; }
        #endregion
    }

    public class LayoutViewModelBase
    {
        #region Properties
        public StringBuilder PageTitle { get; set; }
        public SuccessErrorPartialViewModel SuccessErrorPartialViewModel { get; set; }
        public bool IsSuccessErrorPartialViewModelinitialized => SuccessErrorPartialViewModel?.IsInitialized == true;
        public List<ProjectMenuItem> Menu { get; set; }
        public bool HasMenu => Menu?.Count > 0;        
        public Breadcrumbs Breadcrumbs { get; set; }
        public bool HasBreadcrumbs => Breadcrumbs != null;
        public List<ProjectMenuItem> Tabs { get; set; }
        public bool HasTabs => Tabs?.Count > 0;
        public string UrlLogout { get; set; }
        public PluginClient PluginClient { get; set; }
        #endregion
    }

    public class PluginClient
    {
        #region Properties
        bool _Is63BitsFormsEnabled;
        bool _Is63BitsComponentsEnabled;
        bool _Is63BitsFontsEnabled;
        bool _IsAngleEnabled;
        bool _IsBootstrapEnabled;
        bool _IsDataTablesEnabled;
        bool _IsDevextremeEnabled;
        bool _IsGoogleFontsEnabled;
        bool _IsFancyboxEnabled;
        bool _IsFontAwesomeEnabled;
        bool _IsJQueryEnabled;
        bool _IsJQueryUIEnabled;
        bool _IsJQueryConfirmEnabled;
        bool _IsJQueryNestedSortableEnabled;
        bool _IsJsZipEnabled;
        bool _IsPreloaderEnabled;
        bool _IsSelect2Enabled;
        bool _IsSuccessErrorMessageEnabled;
        bool _IsTemplate7Enabled;
        bool _IsUtilsEnabled;

        public bool Is63BitsFormsEnabled => _Is63BitsFormsEnabled;
        public bool Is63BitsComponentsEnabled => _Is63BitsComponentsEnabled;
        public bool Is63BitsFontsEnabled => _Is63BitsFontsEnabled;
        public bool IsAngleEnabled => _IsAngleEnabled;
        public bool IsBootstrapEnabled => _IsBootstrapEnabled;
        public bool IsDataTablesEnabled => _IsDataTablesEnabled;
        public bool IsDevextremeEnabled => _IsDevextremeEnabled;
        public bool IsGoogleFontsEnabled => _IsGoogleFontsEnabled;
        public bool IsFancyboxEnabled => _IsFancyboxEnabled;
        public bool IsFontAwesomeEnabled => _IsFontAwesomeEnabled;
        public bool IsJQueryEnabled => _IsJQueryEnabled;
        public bool IsJQueryConfirmEnabled => _IsJQueryConfirmEnabled;
        public bool IsJQueryNestedSortableEnabled => _IsJQueryNestedSortableEnabled;
        public bool IsJQueryUIEnabled => _IsJQueryEnabled;
        public bool IsJsZipEnabled => _IsJsZipEnabled;
        public bool IsPreloaderEnabled => _IsPreloaderEnabled;
        public bool IsSelect2Enabled => _IsSelect2Enabled;
        public bool IsSuccessErrorMessageEnabled => _IsSuccessErrorMessageEnabled;
        public bool IsTemplate7Enabled => _IsTemplate7Enabled;
        public bool IsUtilsEnabled => _IsUtilsEnabled;
        #endregion

        #region Methods
        public PluginClient Enable63BitsForms(bool Value)
        {
            _Is63BitsFormsEnabled = Value;
            return this;
        }

        public PluginClient Enable63BitsComponents(bool Value)
        {
            _Is63BitsComponentsEnabled = Value;
            return this;
        }

        public PluginClient Enable63BitsFonts(bool Value)
        {
            _Is63BitsFontsEnabled = Value;
            return this;
        }

        public PluginClient EnableAngle(bool Value)
        {
            _IsAngleEnabled = Value;
            return this;
        }

        public PluginClient EnableBootstrap(bool Value)
        {
            _IsBootstrapEnabled = Value;
            return this;
        }

        public PluginClient EnableDataTables(bool Value)
        {
            _IsDataTablesEnabled = Value;
            return this;
        }

        public PluginClient EnableDevextreme(bool Value)
        {
            _IsDevextremeEnabled = Value;
            return this;
        }

        public PluginClient EnableFancybox(bool Value)
        {
            _IsFancyboxEnabled = Value;
            return this;
        }

        public PluginClient EnableGoogleFonts(bool Value)
        {
            _IsGoogleFontsEnabled = Value;
            return this;
        }

        public PluginClient EnableFontAwesome(bool Value)
        {
            _IsFontAwesomeEnabled = Value;
            return this;
        }

        public PluginClient EnableJQuery(bool Value)
        {
            _IsJQueryEnabled = Value;
            return this;
        }

        public PluginClient EnableJQueryUI(bool Value)
        {
            _IsJQueryUIEnabled = Value;
            return this;
        }

        public PluginClient EnableJQueryConfirm(bool Value)
        {
            _IsJQueryConfirmEnabled = Value;
            return this;
        }

        public PluginClient EnableJQueryNestedSortable(bool Value)
        {
            _IsJQueryNestedSortableEnabled = Value;
            return this;
        }

        public PluginClient EnableJsZip(bool Value)
        {
            _IsJsZipEnabled = Value;
            return this;
        }

        public PluginClient EnablePreloader(bool Value)
        {
            _IsPreloaderEnabled = Value;
            return this;
        }

        public PluginClient EnableSelect2(bool Value)
        {
            _IsSelect2Enabled = Value;
            return this;
        }

        public PluginClient EnableSuccessErrorMessage(bool Value)
        {
            _IsSuccessErrorMessageEnabled = Value;
            return this;
        }

        public PluginClient EnableTemplate7(bool Value)
        {
            _IsTemplate7Enabled = Value;
            return this;
        }

        public PluginClient EnableUtils(bool Value)
        {
            _IsUtilsEnabled = Value;
            return this;
        }        
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
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string UrlCurrentPage { get; set; }
        public string WebsiteDomain { get; set; }
        public string WebsiteHttpPath => $"{WebsiteDomain}/";
        public DataAccessFactory DataAccessFactory { get; set; }
        public AppSettingsCollection AppSettings { get; set; }
        public UtilityCollection Utilities { get; set; }
        public ISessionAssistance SessionAssistance { get; set; }
        public ICookieAssistance CookieAssistance { get; set; }
        public IUrlHelper Url { get; set; }
        public ViewDataDictionary ViewData { get; set; }


        public StringBuilder PageTitle { get; } = new StringBuilder();
        public Breadcrumbs Breadcrumbs { get; set; }
        public List<ProjectMenuItem> Tabs { get; set; } = new List<ProjectMenuItem>();

        public PluginClient PluginClient { get; set; }
        public SuccessErrorPartialViewModel SuccessErrorPartialViewModel { get; set; } = new SuccessErrorPartialViewModel();
        public string Language { get; set; }
        public User User { get; set; }
        public bool IsLoggedIn => User != null;        
        public ValueReference<bool> IsSidebarCollapsed { get; set; }
        public FormViewModelBase Form { get; set; }
        #endregion

        #region Methods
        public string GetFileManagerUrl(IUrlHelper Url, string FolderPhysicalPath, string FolderVirtualPath, bool AllowSelectMultiple = false, bool RestrictToImagesOnly = false, string OnSelectedFilesChooseClientCallback = null)
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

        public string GetFilenameFromUploadedFile(IFormFile PostedFile)
        {
            return PostedFile?.FileName.ToAZ09Dash(GuidInlcuded: true);
        }

        public string GetRouteByName(string RouteName, object RouteValues = null, bool GetFullPath = false, string Protocol = Constants.Protocols.HTTP)
        {                        
            if (GetFullPath)
            {
                return Url.RouteUrl((string.IsNullOrWhiteSpace(Language) || Language == Constants.Languages.GEORGIAN) ? RouteName : $"{RouteName}Culture", RouteValues, Protocol);
            }
            else
            {
                return Url.RouteUrl((string.IsNullOrWhiteSpace(Language) || Language == Constants.Languages.GEORGIAN) ? RouteName : $"{RouteName}Culture", RouteValues);
            }
        }

        public string GetWebsiteDomain(HttpRequest Request)
        {            
            var Port = Request.Host.Port;
            var HostString = Request.Host.Host.TrimEnd(':');            
            var PortString = (Port == 80 || Port == 443 || Port == null) ? "" : $":{Port}";            

            var WebsiteDomain = $"{Request.Scheme}://{HostString}{PortString}";
            return WebsiteDomain;
        }

        public string LogRequest(HttpRequest Request, string LogFilePhysicalPath = null)
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

        public async Task SaveUploadedFile(IFormFile PostedFile, string Filename, string FolderPath = null)
        {
            using (var Stream = new FileStream($"{AppSettings.UploadFolderPhysicalPath}{FolderPath}{Filename}", FileMode.Create))
            {
                await PostedFile.CopyToAsync(Stream);
            }
        }

        public void SetPageTitle(string PageTitle)
        {
            this.PageTitle.Clear().Append(PageTitle);
            if (Breadcrumbs != null && Breadcrumbs.HasItems)
            {
                Breadcrumbs.Items[Breadcrumbs.ItemsCount - 1].Caption = PageTitle;
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
        public string ErrorMessage => string.Join("<br />", Errors?.Select(Item => Item.Value));
        public bool HasErrors => Errors?.Count > 0;
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
