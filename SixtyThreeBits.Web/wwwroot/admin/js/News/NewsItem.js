var NewsItemModel = {
    UrlFileManager: null
};

$(function () {
    new TinyMCE({
        Selector: '.js-apply-tinymce', Width: '100%', Height: 250,
        FileManagerPath: NewsItemModel.UrlFileManager
    }).DisplaySimplified();

    $('.js-slug-textbox').change(function () {
        $(this).val($(this).ToSlug());
    });

    $('.js-save-button').click(function () {
        preloader.show();
    });
});