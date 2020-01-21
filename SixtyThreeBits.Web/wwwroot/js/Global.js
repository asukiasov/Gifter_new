//TextAbort და TextSuccess ცვლადების მნიშნველობების გაფორმება ხდება _Layout.cshtml - ის ბოლოში
var Globals = {
    TextError: null,
    TextSuccess: null,
    Formats: {
        JQueryIUDate: 'M d, yy'
    },
    Devexpress: {
        OnGridCheckBoxColumnEditorInit: function (Grid, Editor, EventArgs) {
            if (Grid.IsNewRowEditing()) {
                Editor.SetValue(false);
            }
        },

        OnGridEndCallback: function (Grid, EventArgs) {
            if (Grid.cpErrorMessage && Grid.cpErrorMessage != '') {
                Components63Bits.Dialog.Error(Grid.cpErrorMessage);
                Grid.cpErrorMessage = null;
            }
        },

        OnTreeEndCallback: function (Tree, EventArgs) {
            if (Tree.cpErrorMessage && Tree.cpErrorMessage != '') {
                Components63Bits.Dialog.Error(Tree.cpErrorMessage);
                Tree.cpErrorMessage = null;
            }
        },

        OnTreeCheckBoxColumnEditorInit: function (Tree, Editor, EventArgs) {
            var Value = Editor.GetValue(); // Can return NULL
            if (Value) {
                Editor.SetChecked(true);
            }
            else {
                Editor.SetChecked(false);
            }
        },
        SetGridFullHeight: function (Grid, GridElement, HeightCorrectionInPixels) {
            // Making sure that number is passed, if not HeightCorrectionInPixels will be zero.
            HeightCorrectionInPixels = HeightCorrectionInPixels % 1 === 0 ? HeightCorrectionInPixels : 0;
            var ScreenHeight = $(window).outerHeight();
                        
            
            var PaddingBottom = 25;
            var OffsetTop = $(GridElement).offset().top;
            var GridHeight = ScreenHeight - OffsetTop - PaddingBottom;
            Grid.option('height', GridHeight);
            
        },
    },
    Common: {
        ProcessSelect2AjaxResultFromSimpleKeyValue: function (Result) {
            var Select2Object = { results: new Array() };

            if (Result && Result.Data) {
                $(Result.Data).each(function (Index, Item) {
                    Select2Object.results.push({ id: Item.Key, text: Item.Value });
                });
            }

            return Select2Object;
        },
        ClosePopupAndRefreshGrid: function (Grid) {
            FancyBox.ClosePopup();
            Grid.Refresh();
        }
    }
};