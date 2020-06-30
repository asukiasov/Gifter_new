var PageModel = {
    InitSlugExternalUrl: function () {
        if ($('.js-is-external-url-checkbox').is(':checked')) {
            $('.js-slug-textbox').Hide();
            $('.js-text-slug').Hide();
            $('.js-text-link').Show();
            $('.js-external-url-textbox').Show();
        }
        else {
            $('.js-slug-textbox').Show();
            $('.js-text-slug').Show();
            $('.js-text-link').Hide();
            $('.js-external-url-textbox').Hide();
        }
    }
};

$(function () {
    PageModel.InitSlugExternalUrl();

    $('.js-is-external-url-checkbox').change(function () {
        PageModel.InitSlugExternalUrl();
    });


    $('.js-slug').change(function () {
        $(this).val($(this).ToSlug());
    });

    $('.js-save-button').click(function () {
        preloader.show();
    }); 
});