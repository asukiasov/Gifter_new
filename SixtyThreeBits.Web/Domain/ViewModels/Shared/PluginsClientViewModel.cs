using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;

namespace SixtyThreeBits.Web.Domain.ViewModels.Shared
{
    public class PluginsClientViewModel
    {
        #region Properties        
        public bool Is63BitsAnalogClockEnabled { get; private set; }
        public bool Is63BitsComponentsEnabled { get; private set; }
        public bool Is63BitsFileUploaderEnabled { get; private set; }
        public bool Is63BitsFormsEnabled { get; private set; }
        public bool Is63BitsFontsEnabled { get; private set; }
        public bool Is63BitsSuccessErrorToastEnabled { get; private set; }
        public bool IsAdminThemeEnabled { get; private set; }        
        public bool IsBootstrapEnabled { get; private set; }
        public bool IsDevextremeEnabled { get; private set; }
        public bool IsDevextremeExportExcelLibrariesEnabled { get; private set; }
        public bool IsGoogleFontsEnabled { get; private set; }
        public bool IsFancyboxEnabled { get; private set; }
        public bool IsFlatPickrEnabled { get; private set; }
        public bool IsFontAwesomeEnabled { get; private set; }
        public bool IsJQueryEnabled { get; private set; }
        public bool IsJQueryAppearEnabled { get; private set; }
        public bool IsJQueryConfirmEnabled { get; private set; }
        public bool IsJQueryMaskedInputEnabled { get; private set; }
        public bool IsJQueryNestedSortableEnabled { get; private set; }
        public bool IsJQueryNumericInputEnabled { get; private set; }
        public bool IsJsClientEnabled { get; private set; }
        public bool IsJsZipEnabled { get; private set; }
        public bool IsJWPlayerEnabled { get; private set; }
        public bool IsMalihuScrollEnabled { get; private set; }
        public bool IsMetisMenuEnabled { get; private set; }
        public bool IsPageBuilderEnabled { get; private set; }
        public bool IsPageBuilderEditorEnabled { get; private set; }
        public bool IsPreloaderEnabled { get; private set; }
        public bool IsSlickSliderEnabled { get; private set; }
        public bool IsSortableJSEnabled { get; private set; }
        
        public bool IsTemplate7Enabled { get; private set; }
        public bool IsTinyMceEnabled { get; private set; }
        public bool IsUtilsEnabled { get; private set; }

        public string LanguageCultureCode { get; private set; }
        public bool ShouldIncludeLocalizationFile { get; private set; }

        public readonly string TextError = Resources.TextError;
        public readonly string TextSuccess = Resources.TextSuccess;
        #endregion

        #region Constructors
        public PluginsClientViewModel()
        {

        }

        public PluginsClientViewModel(string languageCultureCode)
        {
            LanguageCultureCode = languageCultureCode;
            ShouldIncludeLocalizationFile = languageCultureCode != Enums.Languages.ENGLISH;
        }
        #endregion

        #region Methods
        public PluginsClientViewModel Enable63BitsAnalogClock(bool value)
        {
            Is63BitsAnalogClockEnabled = value;
            return this;
        }

        public PluginsClientViewModel Enable63BitsComponents(bool value)
        {
            Is63BitsComponentsEnabled = value;
            return this;
        }

        public PluginsClientViewModel Enable63BitsFileUploader(bool value)
        {
            Is63BitsFileUploaderEnabled = value;
            return this;
        }

        public PluginsClientViewModel Enable63BitsForms(bool value)
        {
            Is63BitsFormsEnabled = value;
            return this;
        }

        public PluginsClientViewModel Enable63BitsFonts(bool value)
        {
            Is63BitsFontsEnabled = value;
            return this;
        }        

        public PluginsClientViewModel EnableAdminTheme(bool value)
        {
            IsAdminThemeEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableBootstrap(bool value)
        {
            IsBootstrapEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableDevextreme(bool value)
        {
            IsDevextremeEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableDevextremeExportExcelLibraries(bool value)
        {
            IsDevextremeExportExcelLibrariesEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableFancybox(bool value)
        {
            IsFancyboxEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableGoogleFonts(bool value)
        {
            IsGoogleFontsEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableFlatPickr(bool value)
        {
            IsFlatPickrEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableFontAwesome(bool value)
        {
            IsFontAwesomeEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableJQuery(bool value)
        {
            IsJQueryEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableJQueryAppear(bool value)
        {
            IsJQueryAppearEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableJQueryConfirm(bool value)
        {
            IsJQueryConfirmEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableJQueryMaskedInput(bool value)
        {
            IsJQueryMaskedInputEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableJQueryNestedSortable(bool value)
        {
            IsJQueryNestedSortableEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableJQueryNumericInput(bool value)
        {
            IsJQueryNumericInputEnabled = value;
            return this;
        }


        public PluginsClientViewModel EnableJsClient(bool value)
        {
            IsJsClientEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableJsZip(bool value)
        {
            IsJsZipEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableJWPlayer(bool value)
        {
            IsJWPlayerEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableMalihuScroll(bool value)
        {
            IsMalihuScrollEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableMetisMenu(bool value)
        {
            IsMetisMenuEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnablePageBuilder(bool value)
        {
            IsPageBuilderEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnablePageBuilderEditor(bool value)
        {
            IsPageBuilderEditorEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnablePreloader(bool value)
        {
            IsPreloaderEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableSlickSlider(bool value)
        {
            IsSlickSliderEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableSortableJS(bool value)
        {
            IsSortableJSEnabled = value;
            return this;
        }

        public PluginsClientViewModel Enable63BitsSuccessErrorToast(bool value)
        {
            Is63BitsSuccessErrorToastEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableTemplate7(bool value)
        {
            IsTemplate7Enabled = value;
            return this;
        }

        public PluginsClientViewModel EnableTinyMce(bool value)
        {
            IsTinyMceEnabled = value;
            return this;
        }

        public PluginsClientViewModel EnableUtils(bool value)
        {
            IsUtilsEnabled = value;
            return this;
        }
        #endregion
    }
}