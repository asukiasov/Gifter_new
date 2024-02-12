const emailTemplateModel = {
    emailTemplatePlaceHoldersJson: null
};

$(function () {
    new TinyMCE({ selector: '.js-apply-tinymce', width: '100%', height: 400, placeHolders: emailTemplateModel.emailTemplatePlaceHoldersJson }).displaySimplified();
});