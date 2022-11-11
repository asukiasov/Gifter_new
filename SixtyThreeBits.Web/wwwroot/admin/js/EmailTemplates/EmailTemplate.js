const EmailTemplateModel = {
    EmailTemplatePlaceHoldersJson: null
};

$(function () {
    new TinyMCE({ Selector: '.js-apply-tinymce', Width: '100%', Height: 400, PlaceHolders: EmailTemplateModel.EmailTemplatePlaceHoldersJson }).DisplaySimplified();
});