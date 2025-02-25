const productModel = {
    urlImageUpload: null,
    urlImageUpdate: null,
    urlImageDelete: null,
    urlImageSort: null,
    TextConfirmDeleteImage: null,

    productImagesUploader: null,
    initSortable: function () {
        const sortableElement = $('.js-product-images-container')[0];
        new Sortable(sortableElement, {
            animation: 150,
            fallbackOnBody: true,
            swapThreshold: 0.65,
            onSort: function (e) {
                productModel.sortImages();
            },
        });
    },
    sortImages: function () {
        const SortIndexes = new Array();

        $('.js-product-image-item').each(function (Index, Item) {
            SortIndexes.push({ ID: $(Item).attr('data-id'), SortIndex: Index, });
        });

        $.ajax({
            method: 'POST',
            url: productModel.urlImageSort,
            data: { SortIndexes: SortIndexes },
            dataType: 'json',            
            success: function () {
            }
        });
    },

    updateImage: function (saveButtton) {
        const parent = saveButtton.closest('.js-product-image-item');
        const productImageID = parent.attr('data-id');
        const productImageAltText = parent.find('.js-product-image-alt-text-textbox').val();

        $.ajax({
            method: 'POST',
            url: productModel.urlImageUpdate,
            data: { ProductImageID: productImageID, ProductImageAltText: productImageAltText },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    successErrorToast63Bits.showSuccessMessage();
                }
                else {
                    if (res.Data) {
                        if (res.Data.RedirectUrl) {
                            window.location = res.Data.RedirectUrl;
                        }
                        else {
                            components63Bits.dialog.error(res.Data);
                        }
                    }
                    else {
                        components63Bits.dialog.error();
                    }
                }
            },
            error: function () {
                components63Bits.dialog.error();
            },
            complete: function () {
                preloader.hide();
            }
        });
    },

    deleteImage: function (deleteButton) {
        const productImageID = deleteButton.closest('.js-product-image-item').attr('data-id');
        const productImageFilename = deleteButton.attr('data-filename');
        components63Bits.dialog.confirm({
            textConfirm: productModel.textConfirmDeleteImage,
            confirmButtonColor: components63Bits.dialog.buttonColors.red,
            resolve: function () {
                $.ajax({
                    method: 'POST',
                    url: productModel.urlImageDelete,
                    data: { ProductImageID: productImageID, ProductImageFilename: productImageFilename },
                    dataType: 'json',
                    beforeSend: function () {
                        preloader.show();
                    },
                    success: function (res) {
                        if (res.IsSuccess) {
                            deleteButton.closest('.js-product-image-item').slideUp(function () {
                                $(this).remove();
                            })
                        }
                        else {
                            components63Bits.dialog.error();
                        }
                    },
                    complete: function () {
                        preloader.hide();
                    }
                });
            }
        })
    },

    templates: {
        compile: function () {
            productModel.templates.productImageProgressTemplate = Template7.compile($('#productImageProgressTemplate').html());
            productModel.templates.productImageTemplate = Template7.compile($('#productImageTemplate').html());
        },
        productImageProgressTemplate: null,
        productImageTemplate: null
    }
}


$(function () {
    new TinyMCE({ selector: '.js-apply-tinymce', width: '100%', height: 300 }).displaySimplified();

    productModel.templates.compile();
    productModel.initSortable();

    productModel.productImagesUploader = new FileUplaoder({
        inputElement: $('.js-product-images-uploader')[0],
        urlFileUplaod: productModel.urlImageUpload,
        isReportProgressIndividual: true,
        onStartCallback: function (e) {            
            const productImageProgressModel = {
                productImageFilename: e.filename,
                productImageFileUploadProgressPercent: 0
            }
            const productImageProgressItemHtml = productModel.templates.productImageProgressTemplate(productImageProgressModel);
            $(productImageProgressItemHtml).insertBefore('.js-product-images-uploader-container');
        },
        onProgressCallback: function (e) {            
            const productImageProgressItem = $('.js-product-image-item[data-filename="' + e.filename + '"]');
            productImageProgressItem.find('.js-progress-bar').width(e.progressPercent + '%');                        
        },
        onFinishUploadCallback: function (e) {
            if (e.IsSuccess) {
                const productImageProgressItem = $('.js-product-image-item[data-filename="' + e.Data.ProductImageFilename + '"]');
                const productImageModel = {
                    ProductImageID: e.Data.ProductImageID,
                    ProductImageFileHttpPath: e.Data.ProductImageFileHttpPath
                };
                const productImageItemHtml = productModel.templates.productImageTemplate(productImageModel);
                productImageProgressItem.replaceWith(productImageItemHtml);                
            }            
        },
        onComplete: function () {            
            productModel.sortImages();
        }
    });
    
    $('.js-save-button').click(function () {
        preloader.show();
    });

    $('.js-numeric-input').numericInput({ allowFloat: true });

    $('.js-show-upload-popup-button').click(function () {
        $('.js-upload-popup').modal('show');
    });    

    $('.js-product-images-uploader').change(function () {
        productModel.productImagesUploader.upload();
    });

    $('.js-product-images-container').on('click', '.js-product-image', function (e) {
        const productImageFileHttpPath = $(this).attr('src');
        fancyBox.init({            
            src: productImageFileHttpPath 
        }).showImagePopup();    
    });

    $('.js-product-images-container').on('click', '.js-product-image-save-button', function (e) {
        e.preventDefault();

        const _this = $(this);
        productModel.updateImage(_this);
    });

    $('.js-product-images-container').on('click', '.js-product-image-delete-button', function (e) {
        e.preventDefault();

        const _this = $(this);
        productModel.deleteImage(_this);
    });
});