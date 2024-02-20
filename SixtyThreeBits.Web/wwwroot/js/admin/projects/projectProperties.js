$(function () {
    new TinyMCE({ selector: '.js-apply-tinymce', width: '100%', height: 250, }).displaySimplified();

    $('.js-slug-textbox').change(function () {
        $(this).val($(this).ToSlug());
    });    
});