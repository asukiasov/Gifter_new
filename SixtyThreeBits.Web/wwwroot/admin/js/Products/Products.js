const ProductsModel = {
    ProductsGrid: null,
    UrlExcelUpload: null,
    OnProductsGridInit: function (s) {
        ProductsModel.ProductsGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(ProductsModel.ProductsGrid, s.element[0]);
    },
    GetDetailsButtonColumnCellHtml: function (Element, CellInfo) {
        Element.append('<a href=\'' + CellInfo.data.UrlProductsProperties + '\'><i class=\'fas fa-info-circle\'></i></a>')
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        ProductsModel.ProductsGrid.addRow();
    });

    $('.js-upload-excel-button').click(function () {
        $('.js-custom-file-upload .js-clear-button').trigger('click');
        $('.js-excel-errors').empty();
        $('.js-upload-excel-file-modal').modal({ show: true, backdrop: 'static' });
    });
    
    $('.js-upload-excel-file-button').click(function () {        
        const HasExcelFile = $('.js-excel-file-input').val().length;
        if (HasExcelFile) {
            preloader.show();

            Utilities.GetBase64FromInputFilePromise('.js-excel-file-input').then(function (Result) {
                $.ajax({
                    method: 'POST',
                    url: ProductsModel.UrlExcelUpload,
                    data: { ExcelFileBytes: Result.FileBase64, ExcelFilename: Result.Filename },
                    dataType: 'json',
                    beforeSend: function () {
                        $('.js-excel-errors').empty();
                    },
                    success: function (res) {                        
                        if (res.IsSuccess) {
                            ProductsModel.ProductsGrid.refresh();
                            $('.js-upload-excel-file-modal').modal('hide');
                        }
                        else {
                            if (res.Data && res.Data.HasExcelErrors) {
                                const ErrorsHtml = Validation.Templates.ErrorsListTemplate(res.Data.ExcelErrors);
                                $('.js-excel-errors').html(ErrorsHtml);
                            }
                            else {
                                Components63Bits.Dialog.Error(res.Data);
                            }
                        }
                    },
                    error: function () {
                        Components63Bits.Dialog.Error();
                    },
                    complete: function () {
                        preloader.hide();
                        $('.js-custom-file-upload .js-clear-button').trigger('click');
                    }
                });

            }).catch(function () {
                preloader.hide();
                Components63Bits.Dialog.Error();
            });
        }
        else {
            $('.js-excel-file-input').closest('.form-group').Shake();
        }
    }); 
});
