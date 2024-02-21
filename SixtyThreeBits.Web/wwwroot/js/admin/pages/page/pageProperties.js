const pagePropertiesModel = {
    textPageShortDescriptionAndImageInfo: null,
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
    pagePropertiesModel.initExternalUrlInput();

    $('.js-is-external-url-checkbox').change(function () {
        pagePropertiesModel.initExternalUrlInput();
    });

    $('.js-short-description-and-image-info').click(function (e) {
        e.preventDefault();
        components63Bits.dialog.alert({ textAlert: pagePropertiesModel.textPageShortDescriptionAndImageInfo });
    });
});