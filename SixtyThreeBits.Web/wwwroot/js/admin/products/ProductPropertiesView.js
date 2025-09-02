const model = {
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
            handle: '.js-product-image',
            onSort: function (e) {
                model.sortImages();
            },
        });
    },
    sortImages: function () {
        const sortIndexes = new Array();

        $('.js-product-image-item').each(function (index, item) {
            sortIndexes.push({ ID: $(item).attr('data-id'), SortIndex: index, });
        });

        $.ajax({
            method: 'POST',
            url: model.urlImageSort,
            data: { SortIndexes: sortIndexes },
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
            url: model.urlImageUpdate,
            data: { ProductImageID: productImageID, ProductImageAltText: productImageAltText },
            dataType: 'json',
            beforeSend: function () {
                saveButtton.showLoader();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    successErrorToast63Bits.showSuccessMessage();
                    saveButtton.attr('data-is-unsaved', false);
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
                saveButtton.hideLoader();
            }
        });
    },

    deleteImage: function (deleteButton) {
        const productImageID = deleteButton.closest('.js-product-image-item').attr('data-id');
        const productImageFilename = deleteButton.attr('data-filename');
        components63Bits.dialog.confirm({
            textConfirm: model.textConfirmDeleteImage,
            confirmButtonColor: components63Bits.dialog.buttonColors.red,
            resolve: function () {
                $.ajax({
                    method: 'POST',
                    url: model.urlImageDelete,
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
            model.templates.productImageProgressTemplate = Template7.compile($('#productImageProgressTemplate').html());
            model.templates.productImageTemplate = Template7.compile($('#productImageTemplate').html());
        },
        productImageProgressTemplate: null,
        productImageTemplate: null
    }
}


$(function () {
    new TinyMCE({ selector: '.js-apply-tinymce', width: '100%', height: 300 }).displaySimplified();

    model.templates.compile();
    model.initSortable();

    model.productImagesUploader = new FileUplaoder({
        inputElement: $('.js-product-images-uploader')[0],
        urlFileUplaod: model.urlImageUpload,
        isReportProgressIndividual: true,
        onStartCallback: function (e) {            
            const productImageProgressModel = {
                productImageFilename: e.filename,
                productImageFileUploadProgressPercent: 0
            }
            const productImageProgressItemHtml = model.templates.productImageProgressTemplate(productImageProgressModel);
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
                const productImageItemHtml = model.templates.productImageTemplate(productImageModel);
                productImageProgressItem.replaceWith(productImageItemHtml);                
            }            
        },
        onComplete: function () {            
            model.sortImages();
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
        model.productImagesUploader.upload();
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
        const isUnsaved = _this.attr('data-is-unsaved') === 'true';
        if (isUnsaved) {
            model.updateImage(_this);
        }
    });

    $('.js-product-images-container').on('keydown', '.js-product-image-alt-text-textbox', function (e) {
        if (e.keyCode === 9) {
            return; // Exclude Tab key
        }
        const saveBtn = $(this).closest('.js-product-image-item').find('.js-product-image-save-button');
        saveBtn.attr('data-is-unsaved', true);
    });

    $('.js-product-images-container').on('focusout', '.js-product-image-alt-text-textbox', function () {
        const _this = $(this);
        const saveBtn = _this.closest('.js-product-image-item').find('.js-product-image-save-button');
        const isUnsaved = saveBtn.attr('data-is-unsaved') === 'true';
        if (isUnsaved) {
            model.updateImage(saveBtn);
        }
    });

    $('.js-product-images-container').on('click', '.js-product-image-delete-button', function (e) {
        e.preventDefault();

        const _this = $(this);
        model.deleteImage(_this);
    });    
});