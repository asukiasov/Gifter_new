var PageModel = {
    UrlFileManager: null
};

$(function () {
    $('.js-slug').change(function () {
        $(this).val($(this).ToSlug());
    });

    $('.js-save-button').click(function () {
        preloader.show();
    }); 
});