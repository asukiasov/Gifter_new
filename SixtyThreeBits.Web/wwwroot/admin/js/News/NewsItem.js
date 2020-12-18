$(function () {
    TinyMCE.Init({
        Selector: '.js-apply-tinymce'
    }).Display();

    $('.js-slug-textbox').change(function () {
        $(this).val($(this).ToSlug());
    });

    $('.js-save-button').click(function () {
        preloader.show();
    });
});