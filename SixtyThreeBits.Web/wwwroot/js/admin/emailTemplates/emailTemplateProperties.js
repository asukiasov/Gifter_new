const model = {
    emailTemplatePlaceHoldersJson: null
};

$(function () {
    new TinyMCE({ selector: '.js-apply-tinymce', width: '100%', height: 400, placeHolders: model.emailTemplatePlaceHoldersJson }).displaySimplified();
});