$(function () {
    new TinyMCE({ Selector: '.js-apply-tinymce' }).DisplaySimplified();
        
    $('.js-save-button').click(function () {
        preloader.show();
    });
});
