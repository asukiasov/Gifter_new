//TextAbort და TextSuccess ცვლადების მნიშნველობების გაფორმება ხდება _Layout.cshtml - ის ბოლოში
const Globals = {
    TextError: null,
    TextSuccess: null,
    Constants: {
        NullValueFor: {
            Int: -1,
            String: '',
            Date:'1900-01-01'
        }
    },
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
            const ScreenHeight = $(window).outerHeight();
                        
            
            const PaddingBottom = 25;
            const OffsetTop = $(GridElement).offset().top;
            const GridHeight = ScreenHeight - OffsetTop - PaddingBottom;
            Grid.option('height', GridHeight);
            
        },

        GetGridSortIndexes: function (e, KeyFieldName) {
            const Grid = e.component;
            const Rows = Grid.getVisibleRows();

            var SortIndexes = new Array();

            Rows.forEach(function (Item, Index) {
                SortIndexes.push({
                    ID: Item.data[KeyFieldName],
                    SortIndex: Index
                });
            });

            if (e.event != undefined) {
                const FromIndex = e.fromIndex;
                const ToIndex = e.toIndex;
                const IsMovingUp = FromIndex > ToIndex;

                const Step = IsMovingUp ? 1 : - 1;
                SortIndexes[ToIndex].SortIndex += Step;
                let i = ToIndex + Step;
                while (i != FromIndex) {
                    SortIndexes[i].SortIndex += Step;
                    i += Step;
                }
                SortIndexes[FromIndex].SortIndex = ToIndex;
            }

            return SortIndexes;
        }
    },
    Common: {
        ProcessSelect2AjaxResultFromSimpleKeyValue: function (Result) {
            const Select2Object = { results: new Array() };

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