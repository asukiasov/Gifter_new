//values for globals.textAbort and globals.textSuccess variables, are assigned in the end of  _Layout.cshtml file
const globals = {
    textError: null,
    textSuccess: null,
    constants: {
        nullValueFor: {
            int: -1,
            string: '',
            date:'1900-01-01'
        }
    },
    devexpress: {
        onGridCheckBoxColumnEditorInit: function (grid, editor, eventArgs) {
            if (grid.IsNewRowEditing()) {
                editor.SetValue(false);
            }
        },

        onGridEndCallback: function (grid, eventArgs) {
            if (grid.cpErrorMessage && grid.cpErrorMessage != '') {
                components63Bits.dialog.error(grid.cpErrorMessage);
                grid.cpErrorMessage = null;
            }
        },

        onTreeEndCallback: function (tree, eventArgs) {
            if (tree.cpErrorMessage && tree.cpErrorMessage != '') {
                components63Bits.dialog.error(tree.cpErrorMessage);
                tree.cpErrorMessage = null;
            }
        },

        onTreeCheckBoxColumnEditorInit: function (tree, editor, eventArgs) {
            const value = editor.GetValue(); // Can return NULL
            if (value) {
                editor.SetChecked(true);
            }
            else {
                editor.SetChecked(false);
            }
        },

        setGridFullHeight: function (grid, gridElement, heightCorrectionInPixels) {
            // Making sure that number is passed, if not heightCorrectionInPixels will be zero.
            heightCorrectionInPixels = heightCorrectionInPixels % 1 === 0 ? heightCorrectionInPixels : 0;
            const screenHeight = $(window).outerHeight();                                    
            const paddingBottom = 50;
            const offsetTop = $(gridElement).offset().top;
            const gridHeight = screenHeight - offsetTop - paddingBottom;
            grid.option('height', gridHeight);
            
        },

        getGridSortIndexes: function (keyFieldName, grid, e) {            
            const rows = grid.getVisibleRows();
            const sortIndexes = new Array();

            rows.forEach(function (item, index) {
                sortIndexes.push({
                    ID: item.data[keyFieldName],
                    SortIndex: index
                });
            });

            if (e && e.event != undefined) {
                const fromIndex = e.fromIndex;
                const toIndex = e.toIndex;
                const isMovingUp = fromIndex > toIndex;

                const step = isMovingUp ? 1 : - 1;
                sortIndexes[toIndex].SortIndex += step;
                let i = toIndex + step;
                while (i != fromIndex) {
                    sortIndexes[i].SortIndex += step;
                    i += step;
                }
                sortIndexes[fromIndex].SortIndex = toIndex;
            }

            return sortIndexes;
        },

        exportGridToExcel: function (grid, fileName) {
            preloader.show();
            const workbook = new ExcelJS.Workbook();
            const worksheet = workbook.addWorksheet('Sheet1');
            DevExpress.excelExporter.exportDataGrid({
                component: grid,
                worksheet,
                autoFilterEnabled: true,
            }).then(function() {
                preloader.hide();
                workbook.xlsx.writeBuffer().then((buffer) => {
                    saveAs(new Blob([buffer], { type: 'application/octet-stream' }), fileName);
                });
            });
        }
    },    
    selectors: {
        buttonAddNew: '.js-add-new-button',
        buttonSave: '.js-save-button'
    },
};