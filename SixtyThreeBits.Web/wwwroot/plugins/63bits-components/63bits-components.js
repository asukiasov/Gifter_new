const components63Bits = {
    textError: null,
    textSuccess: null,

    dialog: {
        buttonColors: {
            blue: 'btn-blue',
            red: 'btn-danger',
            green: 'btn-success',
            orange: 'btn-warning'
        },
        sizes: {
            xSmall: 'xsmall',
            small: 'small',
            medium: 'medium',
            large: 'large',
            xLarge: 'xlarge'
        },
        alert: function (options) {
            const title = options.title ? options.title : '';
            const textAlert = options.textAlert ? options.textAlert : '';
            const buttons = options.buttons;

            $.alert({
                title: title,
                content: textAlert,
                buttons: buttons
            });
        },
        confirm: function (options) {
            const title = options.title ? options.title : '';
            const textConfirm = options.textConfirm ? options.textConfirm : '';
            const resolve = options.resolve ? options.resolve : null;
            const confirmButtonColor = options.confirmButtonColor ? options.confirmButtonColor : components63Bits.dialog.buttonColors.blue;

            $.confirm({
                title: title,
                content: textConfirm,
                type: 'yellow',
                animation: 'scale',
                closeAnimation: 'scale',
                scrollToPreviousElement:true,
                buttons: {
                    'Yes': {
                        btnClass: confirmButtonColor,
                        action: function () {
                            if (resolve) {
                                resolve();
                            }
                        }
                    },
                    'No': function () {

                    }
                }
            });
        },
        error: function (errorMessage) {
            $.alert({
                title: 'Error',
                content: errorMessage ? errorMessage : components63Bits.textError,
                icon: 'fas fa-exclamation-triangle',
                closeIcon: true,
                type: 'red',
                animation: 'scale',
                closeAnimation: 'scale',
                animateFromElement: false
            });
        },
        prompt: function (options) {
            const title = options.title
            const label = options.label ? options.label : '';
            const inputPlaceHolder = options.inputPlaceHolder ? options.inputPlaceHolder : '';
            const buttonColor = options.buttonColor ? options.buttonColor : components63Bits.dialog.buttonColors.blue;
            const resolve = options.resolve;

            $.confirm({
                title: title,
                content: '<div class="form-group"><label>' + label + '</label><input type="text" placeholder="' + inputPlaceHolder +'" class="form-control js-63bits-components-prompt" /></div>',
                buttons: {
                    OK: {
                        text: 'OK',
                        btnClass: buttonColor,
                        action: function () {
                            const inputValue = this.$content.find('.js-63bits-components-prompt').val();
                            if (resolve) {
                                resolve(inputValue);
                            }
                        }
                    },
                    Cancel: function () {
                        
                    },
                }                
            });
        },
        success: function (options) {

            const successTitle = options ? options.successTitle : null;
            const successMessage = options ? options.successMessage : null;
            const size = options ? (options.size ? options.size : components63Bits.dialog.sizes.small) : components63Bits.dialog.sizes.small;

            $.alert({
                title: successTitle,
                content: successMessage ? successMessage : components63Bits.textSuccess,
                icon: 'fal fa-shield-check fa-lg',
                closeIcon: true,
                type: 'green',
                animation: 'scale',
                closeAnimation: 'scale',
                columnClass: size,
                animateFromElement: false
            });
        },
        warning: function (warningMessage) {
            $.alert({
                title: 'Warning',
                content: warningMessage,
                icon: 'fas fa-exclamation-triangle fa-lg',
                closeIcon: true,
                type: 'orange',
                animation: 'scale',
                closeAnimation: 'scale',
                animateFromElement: false
            });
        },
    },
    select2: {
        initSimple: function (selector, searchable) {
            if (selector) {
                $(selector).select2({
                    minimumResultsForSearch: Searchable ? 1 : -1,                    
                    theme: 'bootstrap4',
                    placeholder: '...',
                    language: {
                        noResults: function () {
                            return ' ';
                        }
                    },
                });
            }
        },
        initSimpleWithData: function (options) {
            const selector = options.Selector;
            const data = options.Data;
            const keyFieldName = options.KeyFieldName;
            const valueFieldName = options.ValueFieldName;
            const placeHolder = options.PlaceHolder ? options.PlaceHolder : '...';
            const allowClear = options.AllowClear ? true : false;

            if (selector) {
                var select2Data = Components63Bits.Select2.ConvertToSelect2DataArray({ data: data, keyFieldName: keyFieldName, valueFieldName: valueFieldName });

                if (select2Data.length > 0) {
                    $(selector).select2({                        
                        theme: 'bootstrap4',
                        placeholder: placeHolder,
                        data: select2Data,
                        allowClear: allowClear,
                        language: {
                            noResults: function () {
                                return ' ';
                            }
                        }
                    });
                }
            }
        },
        convertToSelect2DataArray: function (options) {
            const data = options.data;
            const keyFieldName = options.keyFieldName;
            const valueFieldName = options.valueFieldName;
            const select2DataArray = new Array();

            if (data) {
                select2DataArray.push({ id: '', text: '...' });
                $(data).each(function (index, item) {
                    select2DataArray.push({
                        id: item[keyFieldName],
                        text: item[valueFieldName]
                    });
                });
            }

            return select2DataArray;
        },
        clearData: function (options) {
            const selector = options.Selector;
            if (selector) {
                $(selector).empty();
            }
        },
        clearSelection: function (options) {
            const selector = options.selector;
            if (selector) {
                $(selector).val(null).trigger('change');
            }
        },
        showDropDown: function (options) {
            const selector = options.selector;
            if (selector) {
                $(selector).select2('open');
            }
        },
        hideDropDown: function (options) {
            const selector = options.selector;
            if (selector) {
                $(selector).select2('close');
            }
        },
        processSelect2AjaxResultFromSimpleKeyValue: function (result) {
            const select2Object = { results: new Array() };

            if (result && result.Data) {
                $(result.Data).each(function (index, item) {
                    select2Object.results.push({ id: item.Key, text: item.Value });
                });
            }

            return select2Object;
        },
        setSelectedValue: function (options) {
            const selector = options.selector;
            const selectedValue = options.selectedValue;
            if (selector) {
                $(selector).val(selectedValue).trigger('change');
            }
        },
        updateData: function (options) {
            this.clearData(options);
            this.initSimpleWithData(options);
        },
        destroy: function (options) {
            const selector = options.selector;

            const className = $(selector).attr('class');
            if (className && className.indexOf('select2') > -1) {
                $(selector).select2('destroy');
            }
        }
    },
    modal: {
        create: function (selector) {
            const modalElement = document.querySelector(selector);
            const modal = new bootstrap.Modal(modalElement, {
                backdrop: 'static',
                focus: true
            });
            return modal;
        }
    }
};