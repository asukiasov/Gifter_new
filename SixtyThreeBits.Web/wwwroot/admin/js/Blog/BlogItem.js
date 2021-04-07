$(function () {
    TinyMCE.Init({
        Selector: '.js-apply-tinymce'
    }).Display();

    $('.js-slugfy-button').click(function () {
        var Slug = $('.js-blog-post-title-textbox').ToSlug();
        $('.js-blog-post-slug-textbox').val(Slug);
    })

    $('.js-blog-post-slug-textbox').change(function () {
        $(this).val($(this).ToSlug());
    });

    $('.js-save-button').click(function () {
        preloader.show();
    });
});