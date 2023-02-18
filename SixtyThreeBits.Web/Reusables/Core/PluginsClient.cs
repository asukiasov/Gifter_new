namespace SixtyThreeBits.Web.Reusables.Core
{
    public class PluginsClient
    {
        #region Properties
        public bool Is63BitsFormsEnabled { get; private set; }
        public bool Is63BitsComponentsEnabled { get; private set; }
        public bool Is63BitsFontsEnabled { get; private set; }
        public bool IsAngleEnabled { get; private set; }
        public bool IsBootstrapEnabled { get; private set; }
        public bool IsDevextremeEnabled { get; private set; }
        public bool IsGoogleFontsEnabled { get; private set; }
        public bool IsFancyboxEnabled { get; private set; }
        public bool IsFontAwesomeEnabled { get; private set; }
        public bool IsJQueryEnabled { get; private set; }
        public bool IsJQueryAppearEnabled { get; private set; }
        public bool IsJQueryConfirmEnabled { get; private set; }
        public bool IsJQueryMaskedInputEnabled { get; private set; }
        public bool IsJQueryNestedSortableEnabled { get; private set; }
        public bool IsJQueryNumericInputEnabled { get; private set; }
        public bool IsJQueryUICssEnabled { get; private set; }
        public bool IsJQueryUIJsEnabled { get; private set; }
        public bool IsJsClientEnabled { get; private set; }
        public bool IsJsZipEnabled { get; private set; }
        public bool IsJWPlayerEnabled { get; private set; }
        public bool IsMalihuScrollEnabled { get; private set; }
        public bool IsPageBuilderEnabled { get; private set; }
        public bool IsPageBuilderEditorEnabled { get; private set; }
        public bool IsPreloaderEnabled { get; private set; }
        public bool IsSelect2Enabled { get; private set; }
        public bool IsSlickSliderEnabled { get; private set; }
        public bool IsSuccessErrorMessageEnabled { get; private set; }
        public bool IsTemplate7Enabled { get; private set; }
        public bool IsTinyMceEnabled { get; private set; }
        public bool IsUtilsEnabled { get; private set; }
        #endregion

        #region Methods
        public PluginsClient Enable63BitsForms(bool Value)
        {
            Is63BitsFormsEnabled = Value;
            return this;
        }

        public PluginsClient Enable63BitsComponents(bool Value)
        {
            Is63BitsComponentsEnabled = Value;
            return this;
        }

        public PluginsClient Enable63BitsFonts(bool Value)
        {
            Is63BitsFontsEnabled = Value;
            return this;
        }

        public PluginsClient EnableAngle(bool Value)
        {
            IsAngleEnabled = Value;
            return this;
        }

        public PluginsClient EnableBootstrap(bool Value)
        {
            IsBootstrapEnabled = Value;
            return this;
        }

        public PluginsClient EnableDevextreme(bool Value)
        {
            IsDevextremeEnabled = Value;
            return this;
        }

        public PluginsClient EnableFancybox(bool Value)
        {
            IsFancyboxEnabled = Value;
            return this;
        }

        public PluginsClient EnableGoogleFonts(bool Value)
        {
            IsGoogleFontsEnabled = Value;
            return this;
        }

        public PluginsClient EnableFontAwesome(bool Value)
        {
            IsFontAwesomeEnabled = Value;
            return this;
        }

        public PluginsClient EnableJQuery(bool Value)
        {
            IsJQueryEnabled = Value;
            return this;
        }

        public PluginsClient EnableJQueryAppear(bool Value)
        {
            IsJQueryAppearEnabled = Value;
            return this;
        }

        public PluginsClient EnableJQueryUI(bool EnableJs, bool EnableCss = false)
        {
            IsJQueryUIJsEnabled = EnableJs;
            IsJQueryUICssEnabled = EnableCss;
            return this;
        }

        public PluginsClient EnableJQueryConfirm(bool Value)
        {
            IsJQueryConfirmEnabled = Value;
            return this;
        }

        public PluginsClient EnableJQueryMaskedInput(bool Value)
        {
            IsJQueryMaskedInputEnabled = Value;
            return this;
        }

        public PluginsClient EnableJQueryNestedSortable(bool Value)
        {
            IsJQueryNestedSortableEnabled = Value;
            return this;
        }

        public PluginsClient EnableJQueryNumericInput(bool Value)
        {
            IsJQueryNumericInputEnabled = Value;
            return this;
        }


        public PluginsClient EnableJsClient(bool Value)
        {
            IsJsClientEnabled = Value;
            return this;
        }

        public PluginsClient EnableJsZip(bool Value)
        {
            IsJsZipEnabled = Value;
            return this;
        }

        public PluginsClient EnableJWPlayer(bool Value)
        {
            IsJWPlayerEnabled = Value;
            return this;
        }

        public PluginsClient EnableMalihuScroll(bool Value)
        {
            IsMalihuScrollEnabled = Value;
            return this;
        }

        public PluginsClient EnablePageBuilder(bool Value)
        {
            IsPageBuilderEnabled = Value;
            return this;
        }

        public PluginsClient EnablePageBuilderEditor(bool Value)
        {
            IsPageBuilderEditorEnabled = Value;
            return this;
        }

        public PluginsClient EnablePreloader(bool Value)
        {
            IsPreloaderEnabled = Value;
            return this;
        }

        public PluginsClient EnableSelect2(bool Value)
        {
            IsSelect2Enabled = Value;
            return this;
        }

        public PluginsClient EnableSlickSlider(bool Value)
        {
            IsSlickSliderEnabled = Value;
            return this;
        }

        public PluginsClient EnableSuccessErrorMessage(bool Value)
        {
            IsSuccessErrorMessageEnabled = Value;
            return this;
        }

        public PluginsClient EnableTemplate7(bool Value)
        {
            IsTemplate7Enabled = Value;
            return this;
        }

        public PluginsClient EnableTinyMce(bool Value)
        {
            IsTinyMceEnabled = Value;
            return this;
        }

        public PluginsClient EnableUtils(bool Value)
        {
            IsUtilsEnabled = Value;
            return this;
        }
        #endregion
    }
}