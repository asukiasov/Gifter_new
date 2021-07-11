const ProductModel = {
    UrlImageUpload: null,
    UrlImageDelete: null,
    UrlImageSort: null,
    TextConfirmDeleteImage: null
}


$(function () {
    new TinyMCE({ Selector: '.js-apply-tinymce', Width: '100%', Height: 300 }).DisplaySimplified();

    $('.js-slug-textbox').change(function () {
        $(this).val($(this).ToSlug());
    });

    $('.js-save-button').click(function () {
        preloader.show();
    });

    $('.js-numeric-input').numericInput({ allowFloat: true });

    $('.js-show-upload-popup-button').click(function () {
        $('.js-upload-popup').modal('show');
    });

    $('.js-dropzone').dropzone({
        url: ProductModel.UrlImageUpload,
        uploadMultiple: true,
        parallelUploads: 30,
        maxFilesize: 8,
        success: function (file, response) {
            if (response.IsSuccess) {
                file.previewElement.classList.add('dz-success');
                setTimeout(function () {
                    $('.js-save-button').trigger('click');
                }, 2000);
            } else {
                file.previewElement.classList.add('dz-error');
            }

        },
        error: function (file, response) {
            file.previewElement.classList.add('dz-error');
        }
    });

    $('.js-sortable').sortable({
        items: '.js-drag-me',
        helper: 'clone',
        placeholder: 'sortable-placeholder',
        update: function () {
            var SortIndexes = new Array();

            $('.js-body').each(function (Index, Item) {
                SortIndexes.push({ ID: $(Item).attr('data-image-id'), SortIndex: Index, });
            });

            $.ajax({
                method: 'POST',
                url: ProductModel.UrlImageSort,
                data: { SortIndexes: SortIndexes },
                dataType: 'json',
                beforeSend: function () {
                    preloader.show();
                },
                success: function () {
                },
                complete: function () {
                    preloader.hide();
                }
            });
        }
    });

    $('.js-delete-image-button').click(function (e) {
        e.preventDefault();

        const _this = $(this);
        const ProductImageID = _this.attr('data-id');
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
                            _this.closest('.js-image-item').slideUp(function () {
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