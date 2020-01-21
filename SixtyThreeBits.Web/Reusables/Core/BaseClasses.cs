using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public interface IDevexpressGridModel<T> where T : class
    {
        #region Properties
        DataGridBuilder<T> InitGrid(IHtmlHelper Html); 
        #endregion
    }

    public class DevexpressTypesBase
    {
        #region Properties        
        public bool AllowAdd { get; set; }
        public bool AllowUpdate { get; set; }
        public bool AllowDelete { get; set; }

        public string UrlAddNew { get; set; }
        public string UrlUpdate { get; set; }
        public string UrlNodeDragDrop { get; set; }
        public string UrlDelete { get; set; }
        public string UrlList { get; set; }
        public string UrlCustomAction { get; set; }

        public bool IsError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public string ErrorMessage { get; set; }
        #endregion        
    }

    public class DevexpressGridViewModelBase : DevexpressTypesBase
    {
        public DataGridBuilder<T> GetGridWithStartupValues<T>(IHtmlHelper Html, string KeyFieldName)
        {
            return Html.DevExtreme().DataGrid<T>()            
            .Width("100%")
            .ShowBorders(true)
            .ShowRowLines(true)
            .FocusedRowEnabled(true)
            .FilterRow(Options =>
            {
                Options.Visible(true);
                Options.ApplyFilter(GridApplyFilterMode.Auto);
            })
            .DataSource(Options =>
                Options.RemoteController()
                .LoadUrl(UrlList)
                .InsertUrl(UrlAddNew)
                .UpdateUrl(UrlUpdate)
                .DeleteUrl(UrlDelete)
                .Key(KeyFieldName)
            )
            .Editing(Options => {
                Options.Mode(GridEditMode.Row);
                Options.AllowAdding(AllowAdd);
                Options.AllowUpdating(AllowUpdate);
                Options.AllowDeleting(AllowDelete);                

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
            .Scrolling(Options =>
            {
                Options.Mode(GridScrollingMode.Standard);
                Options.ShowScrollbar(ShowScrollbarMode.Always);
            })
            .Columns(Columns =>
            {
                if (AllowAdd || AllowUpdate || AllowDelete)
                {
                    var Width = (AllowDelete && !AllowAdd && !AllowUpdate) ? 40 : 80;
                    Columns.Add().Type(GridCommandColumnType.Buttons).Width(Width).Buttons(b =>
                    {
                        b.Add().Name(GridColumnButtonName.Edit).Icon("fas fa-pencil-alt");
                        b.Add().Name(GridColumnButtonName.Delete).Icon("fas fa-trash-alt");
                        b.Add().Name(GridColumnButtonName.Save).Icon("fas fa-check");
                        b.Add().Name(GridColumnButtonName.Cancel).Icon("fas fa-minus-circle");
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

        public void InitLookupColumn<T>(DataGridColumnBuilder<T> Column, IEnumerable<SimpleKeyValue<int?,string>> Data, bool IsRequired = false)
        {
            Column.Lookup(Options =>
            {
                Options.DataSource(d => d.Array().Data(Data).Key(nameof(SimpleKeyValue<int?, string>.Key))).ValueExpr(nameof(SimpleKeyValue<int?, string>.Key)).DisplayExpr(nameof(SimpleKeyValue<int?, string>.Value));
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
    }

    public class LayoutViewModelBase
    {
        #region Properties
        public string PageTitle { get; set; }
        public SuccessErrorPartialViewModel SuccessErrorViewModel { get; set; }
        public bool IsSuccessErrorPartialInitiated => SuccessErrorViewModel != null;
        public List<ProjectMenuItem> Menu { get; set; }
        public bool HasMenu => Menu?.Count > 0;        
        public Breadcrumbs Breadcrumbs { get; set; }
        public bool HasBreadcrumbs => Breadcrumbs != null;
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
        bool _IsDevextremeEnabled;
        bool _IsGoogleFontsEnabled;
        bool _IsFancyboxEnabled;
        bool _IsFontAwesomeEnabled;
        bool _IsJQueryEnabled;
        bool _IsJQueryConfirmEnabled;
        bool _IsPreloaderEnabled;
        bool _IsSelect2Enabled;
        bool _IsSuccessErrorMessageEnabled;
        bool _IsUtilsEnabled;

        public bool Is63BitsFormsEnabled => _Is63BitsFormsEnabled;
        public bool Is63BitsComponentsEnabled => _Is63BitsComponentsEnabled;
        public bool Is63BitsFontsEnabled => _Is63BitsFontsEnabled;
        public bool IsAngleEnabled => _IsAngleEnabled;
        public bool IsBootstrapEnabled => _IsBootstrapEnabled;
        public bool IsDevextremeEnabled => _IsDevextremeEnabled;
        public bool IsGoogleFontsEnabled => _IsGoogleFontsEnabled;
        public bool IsFancyboxEnabled => _IsFancyboxEnabled;
        public bool IsFontAwesomeEnabled => _IsFontAwesomeEnabled;
        public bool IsJQueryEnabled => _IsJQueryEnabled;
        public bool IsJQueryConfirmEnabled => _IsJQueryConfirmEnabled;
        public bool IsPreloaderEnabled => _IsPreloaderEnabled;
        public bool IsSelect2Enabled => _IsSelect2Enabled;
        public bool IsSuccessErrorMessageEnabled => _IsSuccessErrorMessageEnabled;
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

        public PluginClient EnableJQueryConfirm(bool Value)
        {
            _IsJQueryConfirmEnabled = Value;
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

        public PluginClient EnableUtils(bool Value)
        {
            _IsUtilsEnabled = Value;
            return this;
        }        
        #endregion
    }

    [BeforeWebProjectControllerLoaded(Order = 0)]
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
        public DataAccessFactory DataAccessFactory { get; set; }
        public AppSettingsModel AppSettings { get; set; }
        public ISessionAssistance SessionAssistance { get; set; }
        public ICookieAssistance CookieAssistance { get; set; }
        public IUrlHelper Url { get; set; }
        public PluginClient PluginClient { get; set; }
        public string Language { get; set; }
        public User User { get; set; }
        public bool IsLoggedIn => User != null;
        #endregion

        #region Methods
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
        #endregion
    }
}
