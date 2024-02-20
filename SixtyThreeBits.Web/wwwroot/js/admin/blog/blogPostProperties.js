$(function () {
    $('.js-date-picker').flatpickr({
        allowInput: false,
        dateFormat: 'M d, Y'
    });

    new TinyMCE({ selector: '.js-apply-tinymce', width: '100%', height: 250 }).displaySimplified();    
});