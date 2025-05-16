const model = {
    grid: null,
    urlExcelUpload: null,
    excelUploadModal: null,
    onGridInit: function (e) {
        model.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    },
    getDetailsButtonColumnCellHtml: function (element, cellInfo) {
        element.append('<a href=\'' + cellInfo.data.UrlProductsProperties + '\'><i class=\'fas fa-info-circle\'></i></a>')
    }
};

$(function () {
    model.excelUploadModal = components63Bits.modal.create('.js-upload-excel-file-modal');

    $(globals.selectors.buttonAddNew).click(function () {
        model.grid.addRow();
    });

    $('.js-upload-excel-button').click(function () {
        $('.js-custom-file-upload-clear-button').trigger('click');
        $('.js-excel-errors').empty();
        model.excelUploadModal.show();        
    });
    
    $('.js-upload-excel-file-button').click(function () {        
        const hasExcelFile = $('.js-excel-file-input').val().length;
        if (hasExcelFile) {
            preloader.show();

            utilities.getBase64FromInputFilePromise('.js-excel-file-input').then(function (result) {
                $.ajax({
                    method: 'POST',
                    url: model.urlExcelUpload,
                    data: { ExcelFileBytes: result.fileBase64, ExcelFilename: result.filename },
                    dataType: 'json',
                    beforeSend: function () {
                        $('.js-excel-errors').empty();
                    },
                    success: function (res) {                        
                        if (res.IsSuccess) {
                            model.grid.refresh();
                            $('.js-upload-excel-file-modal').modal('hide');
                        }
                        else {
                            if (res.Data && res.Data.HasExcelErrors) {
                                const ErrorsHtml = validation.templates.errorsListTemplate(res.Data.ExcelErrors);
                                $('.js-excel-errors').html(ErrorsHtml);
                            }
                            else {
                                components63Bits.dialog.error(res.Data);
                            }
                        }
                    },
                    error: function () {
                        components63Bits.dialog.error();
                    },
                    complete: function () {
                        preloader.hide();
                        $('.js-custom-file-upload-clear-button').trigger('click');
                    }
                });

            }).catch(function (e) {
                alert(e)
                preloader.hide();
                components63Bits.dialog.error();
            });
        }
        else {
            $('.js-excel-file-input').closest('.form-group').shakeElement();
        }
    }); 

    $('.js-export-excel-button').click(function () {
        globals.devexpress.exportGridToExcel(model.grid, 'Products.xlsx');
    });
});
