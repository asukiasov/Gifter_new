const ProductModel = {
    UrlImageUpload: null,
    UrlImageDelete: null,
    UrlImageSort: null,
    TextConfirmDeleteImage: null,

    ProductImagesUploader: null,

    SortImages: function () {
        const SortIndexes = new Array();

        $('.js-product-image-item').each(function (Index, Item) {
            SortIndexes.push({ ID: $(Item).attr('data-id'), SortIndex: Index, });
        });

        $.ajax({
            method: 'POST',
            url: ProductModel.UrlImageSort,
            data: { SortIndexes: SortIndexes },
            dataType: 'json',            
            success: function () {
            }
        });
    },

    Templates: {
        Compile: function () {
            ProductModel.Templates.ProductImageProgressTemplate = Template7.compile($('#ProductImageProgressTemplate').html());
            ProductModel.Templates.ProductImageTemplate = Template7.compile($('#ProductImageTemplate').html());
        },
        ProductImageProgressTemplate: null,
        ProductImageTemplate: null
    }
}


$(function () {
    new TinyMCE({ Selector: '.js-apply-tinymce', Width: '100%', Height: 300 }).DisplaySimplified();

    ProductModel.Templates.Compile();

    ProductModel.ProductImagesUploader = new FileUplaoder({
        InputElement: $('.js-product-images-uploader')[0],
        UrlFileUplaod: ProductModel.UrlImageUpload,
        IsReportProgressIndividual: true,
        OnStartCallback: function (e) {            
            const ProductImageProgressModel = {
                ProductImageFilename: e.Filename,
                ProductImageFileUploadProgressPercent: 0
            }
            const ProductImageProgressItemHtml = ProductModel.Templates.ProductImageProgressTemplate(ProductImageProgressModel);
            $(ProductImageProgressItemHtml).insertBefore('.js-product-images-uploader-container');
        },
        OnProgressCallback: function (e) {
            const ProductImageProgressItem = $('.js-product-image-item[data-filename="' + e.Filename + '"]');
            ProductImageProgressItem.find('.js-progress-bar').width(e.ProgressPercent + '%');                        
        },
        OnFinishUploadCallback: function (e) {
            if (e.IsSuccess) {
                const ProductImageProgressItem = $('.js-product-image-item[data-filename="' + e.Data.ProductImageFilename + '"]');
                const ProductImageModel = {
                    ProductImageID: e.Data.ProductImageID,
                    ProductImageFileHttpPath: e.Data.ProductImageFileHttpPath
                };
                const ProductImageItemHtml = ProductModel.Templates.ProductImageTemplate(ProductImageModel);
                ProductImageProgressItem.replaceWith(ProductImageItemHtml);                
            }            
        },
        OnComplete: function () {            
            ProductModel.SortImages();
        }
    });
    
    $('.js-save-button').click(function () {
        preloader.show();
    });

    $('.js-numeric-input').numericInput({ allowFloat: true });

    $('.js-show-upload-popup-button').click(function () {
        $('.js-upload-popup').modal('show');
    });    

    $('.js-product-images-container').sortable({
        helper: 'clone',
        forceHelperSize: true,
        forcePlaceholderSize: true,
        placeholder: 'sortable-placeholder',
        cancel: '.js-product-images-uploader-container',
        update: function () {
            ProductModel.SortImages();
        }
    });

    

    $('.js-product-images-uploader').change(function () {
        ProductModel.ProductImagesUploader.Upload();
    });

    $('.js-product-images-container').on('click', '.js-product-image', function (e) {
        const ProductImageFileHttpPath = $(this).attr('src');
        FancyBox.Init({            
            src: ProductImageFileHttpPath 
        }).ShowImagePopup();    
    });

    $('.js-product-images-container').on('click', '.js-product-image-delete-button', function (e) {
        e.preventDefault();

        const _this = $(this);
        const ProductImageID = _this.closest('.js-product-image-item').attr('data-id');
        const ProductImageFilename = _this.attr('data-filename');

        Components63Bits.Dialog.Confirm({
            TextConfirm: ProductModel.TextConfirmDeleteImage,
            ConfirmButtonColor: Components63Bits.Dialog.ButtonColors.Red,
            Resolve: function () {
                $.ajax({
                    method: 'POST',
                    url: ProductModel.UrlImageDelete,
                    data: { ProductImageID: ProductImageID, ProductImageFilename: ProductImageFilename },
                    dataType: 'json',
                    beforeSend: function () {
                        preloader.show();
                    },
                    success: function (res) {
                        if (res.IsSuccess) {
                            _this.closest('.js-product-image-item').slideUp(function () {
                                $(this).remove();
                            })
                        }
                        else {
                            Components63Bits.Dialog.Error();
                        }
                    },
                    complete: function () {
                        preloader.hide();
                    }
                });
            }
        })
    });
});