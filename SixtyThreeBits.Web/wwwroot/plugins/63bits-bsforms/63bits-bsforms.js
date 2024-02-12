const validation = {
    errorsJson: null,
    init: function (options) {
        if (typeof (options.errorsJson) == 'object') {
            validation.errorsJson = options.errorsJson;
        }
        else if (typeof (options.errorsJson) == 'string') {
            validation.errorsJson = JSON.parse(options.errorsJson);
        }
        else {
            validation.errorsJson = null;
        }

        return validation;
    },
    showErrors: function () {
        $(validation.errorsJson).each(function (index, item) {
            const selector = item.Key;
            $(selector).addClass('is-invalid');
            $(selector).parent().find('.invalid-feedback').text(item.Value);
        });
        $('.is-invalid').first().scrollToElement();
    },
    getErrorMessageFromvalidationObject: function (validationObject) {
        let errorMessage = null;
        if (validationObject && validationObject.Errors && validationObject.Errors.length > 0) {
            errorMessage = validationObject.Errors[0].Message;
        }

        return errorMessage;
    },
    hideErrors: function () {
        $('.is-invalid').removeClass('is-invalid');
    },
    templates: {
        compile: function () {
            validation.templates.errorsListTemplate = Template7.compile(validation.templates.errorsListTemplate);
        },
        errorsListTemplate: `<ul class="list-group js-errors-list">
{{#each this}}
<li class="list-group-item js-validation-error-item">
    <label class="d-flex justify-content-start align-middle">
        <input type="checkbox"class="me-2 js-validation-error-checkbox" />
        <span class="text-danger">{{this}}</div>
    </div>
</li>
{{/each}}
</ul>
`
    }
};

$(function () {
    //https://idangero.us/template7/
    if (window.Template7) {
        validation.templates.compile();
        $(document).on('click', '.js-validation-error-checkbox', function (e) {
            if ($(this).is(':checked')) {
                $(this).next().addClass('strike');
            }
            else {
                $(this).next().removeClass('strike');
            }
        });
    }

    $('.js-generate-slug-from-title').click(function (e) {
        e.preventDefault();
        const titleInputSelector = $(this).data('title-input-selector');
        const slugInputSelector = $(this).data('slug-input-selectpr');
        const slug = $(titleInputSelector).toSlug();
        $(slugInputSelector).val(slug);
    });
    $('.js-apply-slug-input-behaviour').focusout(function () {
        const slugInput = $(this);
        slugInput.val(slugInput.toSlug());
    })

    $('.js-password-show-hide-toggle-button').click(function (e) {
        e.preventDefault();
        const isHidden = $(this).attr('data-is-hidden') == 'true';
        $(this).find('i').hideElement();
        if (isHidden) {
            $(this).closest('.form-group').find('input').attr('type', 'text');
            $(this).find('i:eq(1)').showElement();
            $(this).attr('data-is-hidden', 'false');
        }
        else {
            $(this).closest('.form-group').find('input').attr('type', 'password');            
            $(this).find('i:eq(0)').showElement();
            $(this).attr('data-is-hidden', 'true');
        }
    })

    $('.js-custom-file-upload input').change(function () {
        const container = $(this).closest('.js-custom-file-upload');
        const currentAttachment = container.find('.js-custom-file-upload-attachment');
        const newAttachment = container.find('.js-custom-file-upload-new-attachment');
        const clearButton = container.find('.js-custom-file-upload-clear-button');

        currentAttachment.hideElement();
        newAttachment.text($(this).val())
        newAttachment.showElement();
        clearButton.showElement();
    });
    $('.js-custom-file-upload-clear-button').click(function () {
        const container = $(this).closest('.js-custom-file-upload');
        const currentAttachment = container.find('.js-custom-file-upload-attachment');
        const newAttachment = container.find('.js-custom-file-upload-new-attachment');
        const clearButton = container.find('.js-custom-file-upload-clear-button');

        currentAttachment.showElement();
        newAttachment.text('');
        newAttachment.hideElement();
        clearButton.hideElement();

        container.find('input').val(null);
    });
    $('.js-custom-file-upload .js-custom-file-upload-attachment[data-type="image"]').click(function (e) {
        e.preventDefault();
        fancyBox.init({
            src: $(this).attr('href')
        }).showImagePopup();
    });
    $('.js-custom-file-upload-delete-button').click(function () {
        const _this = $(this)
        const container = _this.closest('.js-custom-file-upload');
        const currentAttachment = container.find('.js-custom-file-upload-attachment');
        const deleteButton = container.find('.js-custom-file-upload-delete-button');
        const textConfirm = _this.attr('data-text-confirm');
        const urlDelete = _this.attr('data-url');
        const hash = _this.attr('data-hash');

        components63Bits.dialog.confirm({
            textConfirm: textConfirm,
            confirmButtonColor: components63Bits.dialog.buttonColors.red,
            resolve: function () {
                $.ajax({
                    type: 'POST',
                    url: urlDelete,
                    data: { Hash: hash },
                    dataType: 'json',
                    beforeSend: function () {
                        preloader.show();
                    },
                    success: function (res) {
                        if (res.IsSuccess) {
                            deleteButton.remove();
                            currentAttachment.remove();
                        }
                    },
                    complete: function () {
                        preloader.hide();
                    }
                });
            }
        });        
    });   

    $('.checkbox-list input').change(function () {
        $(this).parent().toggleClass('active');
    });
});