var Components63Bits = {
    Dialog: {
        ButtonColors: {
            Blue: 'btn-blue',
            Red: 'btn-danger',
            Green: 'btn-success',
            Orange: 'btn-warning'
        },
        Alert: function (Options) {
            var Title = Options.Title ? Options.Title : '';
            var TextAlert = Options.TextAlert ? Options.TextAlert : '';
            var Buttons = Options.Buttons;

            $.alert({
                title: Title,
                content: TextAlert,
                buttons: Buttons
            });
        },
        Confirm: function (Options) {
            var Title = Options.Title ? Options.Title : '';
            var TextConfirm = Options.TextConfirm ? Options.TextConfirm : '';
            var Resolve = Options.Resolve ? Options.Resolve : null;
            var ConfirmButtonColor = Options.ConfirmButtonColor ? Options.ConfirmButtonColor : Components63Bits.Dialog.ButtonColors.Blue;

            $.confirm({
                title: Title,
                content: TextConfirm,
                type: 'yellow',
                animation: 'scale',
                closeAnimation: 'scale',
                scrollToPreviousElement:true,
                buttons: {
                    'Yes': {
                        btnClass: ConfirmButtonColor,
                        action: function () {
                            if (Resolve) {
                                Resolve();
                            }
                        }
                    },
                    'No': function () {

                    }
                }
            });
        },
        Error: function (ErrorMessage) {
            $.alert({
                title: 'Error',
                content: ErrorMessage ? ErrorMessage : Globals.TextError,
                icon: 'fas fa-exclamation-triangle',
                closeIcon: true,
                type: 'red',
                animation: 'scale',
                closeAnimation: 'scale',
                animateFromElement: false
            });
        },
        Prompt: function (Options) {

            const Title = Options.Title
            const Label = Options.Label ? Options.Label : '';
            const InputPlaceHolder = Options.InputPlaceHolder ? Options.InputPlaceHolder : '';
            const ButtonColor = Options.ButtonColor ? Options.ButtonColor : Components63Bits.Dialog.ButtonColors.Blue;
            const Resolve = Options.Resolve;

            $.confirm({
                title: Title,
                content: '<div class="form-group"><label>' + Label + '</label><input type="text" placeholder="' + InputPlaceHolder +'" class="form-control js-63bits-components-prompt" /></div>',
                buttons: {
                    OK: {
                        text: 'OK',
                        btnClass: ButtonColor,
                        action: function () {
                            const InputValue = this.$content.find('.js-63bits-components-prompt').val();
                            if (Resolve) {
                                Resolve(InputValue);
                            }
                        }
                    },
                    Cancel: function () {
                        
                    },
                }                
            });
        }
    },
    Select2: {
        InitSimple: function (Selector, Searchable) {
            if (Selector) {
                $(Selector).select2({
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
        InitSimpleWithData: function (Options) {
            var Selector = Options.Selector;
            var Data = Options.Data;
            var KeyFieldName = Options.KeyFieldName;
            var ValueFieldName = Options.ValueFieldName;
            var PlaceHolder = Options.PlaceHolder ? Options.PlaceHolder : '...';
            var AllowClear = Options.AllowClear ? true : false;

            if (Selector) {
                var Select2Data = Components63Bits.Select2.ConvertToSelect2DataArray({ Data: Data, KeyFieldName: KeyFieldName, ValueFieldName: ValueFieldName });

                if (Select2Data.length > 0) {
                    $(Selector).select2({                        
                        theme: 'bootstrap4',
                        placeholder: PlaceHolder,
                        data: Select2Data,
                        allowClear: AllowClear,
                        language: {
                            noResults: function () {
                                return ' ';
                            }
                        }
                    });
                }
            }
        },
        ConvertToSelect2DataArray: function (Options) {
            var Data = Options.Data;
            var KeyFieldName = Options.KeyFieldName;
            var ValueFieldName = Options.ValueFieldName;
            var Select2DataArray = new Array();

            if (Data) {
                Select2DataArray.push({ id: '', text: '...' });
                $(Data).each(function (Index, Item) {
                    Select2DataArray.push({
                        id: Item[KeyFieldName],
                        text: Item[ValueFieldName]
                    });
                });
            }

            return Select2DataArray;
        },
        ClearData: function (Options) {
            var Selector = Options.Selector;
            if (Selector) {
                $(Selector).empty();
            }
        },
        ClearSelection: function (Options) {
            var Selector = Options.Selector;
            if (Selector) {
                $(Selector).val(null).trigger('change');
            }
        },
        SetSelectedValue: function (Options) {
            var Selector = Options.Selector;
            var SelectedValue = Options.SelectedValue;
            if (Selector) {
                $(Selector).val(SelectedValue).trigger('change');
            }
        },
        UpdateData: function (Options) {
            this.ClearData(Options);
            this.InitSimpleWithData(Options);
        },
        Destroy: function (Options) {
            var Selector = Options.Selector;

            var ClassName = $(Selector).attr('class');
            if (ClassName && ClassName.indexOf('select2') > -1) {
                $(Selector).select2('destroy');
            }
        }
    }
};