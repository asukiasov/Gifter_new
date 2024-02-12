const pageModel = {
    initExternalUrlInput: function () {
        const isExternalUrl = $('.js-is-external-url-checkbox').is(':checked');
        if (isExternalUrl) {
            $('.js-external-url-input').enableElement();
        }
        else {
            $('.js-external-url-input').disableElement();
        }
    }
};

$(function () {
    pageModel.initExternalUrlInput();

    $('.js-is-external-url-checkbox').change(function () {
        pageModel.initExternalUrlInput();
    });
});