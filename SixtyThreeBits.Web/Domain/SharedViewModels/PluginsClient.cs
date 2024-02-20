using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;

namespace SixtyThreeBits.Web.Domain.SharedViewModels
{
    public class PluginsClient
    {
        #region Properties        
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
        public PluginsClient()
        {

        }

        public PluginsClient(string languageCultureCode)
        {
            LanguageCultureCode = languageCultureCode;
            ShouldIncludeLocalizationFile = languageCultureCode != Enums.Languages.ENGLISH;
        }
        #endregion

        #region Methods
        public PluginsClient Enable63BitsComponents(bool value)
        {
            Is63BitsComponentsEnabled = value;
            return this;
        }

        public PluginsClient Enable63BitsFileUploader(bool value)
        {
            Is63BitsFileUploaderEnabled = value;
            return this;
        }

        public PluginsClient Enable63BitsForms(bool value)
        {
            Is63BitsFormsEnabled = value;
            return this;
        }

        public PluginsClient Enable63BitsFonts(bool value)
        {
            Is63BitsFontsEnabled = value;
            return this;
        }

        public PluginsClient EnableAdminTheme(bool value)
        {
            IsAdminThemeEnabled = value;
            return this;
        }

        public PluginsClient EnableBootstrap(bool value)
        {
            IsBootstrapEnabled = value;
            return this;
        }

        public PluginsClient EnableDevextreme(bool value)
        {
            IsDevextremeEnabled = value;
            return this;
        }

        public PluginsClient EnableDevextremeExportExcelLibraries(bool value)
        {
            IsDevextremeExportExcelLibrariesEnabled = value;
            return this;
        }

        public PluginsClient EnableFancybox(bool value)
        {
            IsFancyboxEnabled = value;
            return this;
        }

        public PluginsClient EnableGoogleFonts(bool value)
        {
            IsGoogleFontsEnabled = value;
            return this;
        }

        public PluginsClient EnableFlatPickr(bool value)
        {
            IsFlatPickrEnabled = value;
            return this;
        }

        public PluginsClient EnableFontAwesome(bool value)
        {
            IsFontAwesomeEnabled = value;
            return this;
        }

        public PluginsClient EnableJQuery(bool value)
        {
            IsJQueryEnabled = value;
            return this;
        }

        public PluginsClient EnableJQueryAppear(bool value)
        {
            IsJQueryAppearEnabled = value;
            return this;
        }

        public PluginsClient EnableJQueryConfirm(bool value)
        {
            IsJQueryConfirmEnabled = value;
            return this;
        }

        public PluginsClient EnableJQueryMaskedInput(bool value)
        {
            IsJQueryMaskedInputEnabled = value;
            return this;
        }

        public PluginsClient EnableJQueryNestedSortable(bool value)
        {
            IsJQueryNestedSortableEnabled = value;
            return this;
        }

        public PluginsClient EnableJQueryNumericInput(bool value)
        {
            IsJQueryNumericInputEnabled = value;
            return this;
        }


        public PluginsClient EnableJsClient(bool value)
        {
            IsJsClientEnabled = value;
            return this;
        }

        public PluginsClient EnableJsZip(bool value)
        {
            IsJsZipEnabled = value;
            return this;
        }

        public PluginsClient EnableJWPlayer(bool value)
        {
            IsJWPlayerEnabled = value;
            return this;
        }

        public PluginsClient EnableMalihuScroll(bool value)
        {
            IsMalihuScrollEnabled = value;
            return this;
        }

        public PluginsClient EnableMetisMenu(bool value)
        {
            IsMetisMenuEnabled = value;
            return this;
        }

        public PluginsClient EnablePageBuilder(bool value)
        {
            IsPageBuilderEnabled = value;
            return this;
        }

        public PluginsClient EnablePageBuilderEditor(bool value)
        {
            IsPageBuilderEditorEnabled = value;
            return this;
        }

        public PluginsClient EnablePreloader(bool value)
        {
            IsPreloaderEnabled = value;
            return this;
        }

        public PluginsClient EnableSlickSlider(bool value)
        {
            IsSlickSliderEnabled = value;
            return this;
        }

        public PluginsClient EnableSortableJS(bool value)
        {
            IsSortableJSEnabled = value;
            return this;
        }

        public PluginsClient Enable63BitsSuccessErrorToast(bool value)
        {
            Is63BitsSuccessErrorToastEnabled = value;
            return this;
        }

        public PluginsClient EnableTemplate7(bool value)
        {
            IsTemplate7Enabled = value;
            return this;
        }

        public PluginsClient EnableTinyMce(bool value)
        {
            IsTinyMceEnabled = value;
            return this;
        }

        public PluginsClient EnableUtils(bool value)
        {
            IsUtilsEnabled = value;
            return this;
        }
        #endregion
    }
}