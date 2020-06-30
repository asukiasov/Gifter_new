$(function () {
    //--- scrollto nav
    if ($('.js-t63-scrollto-nav').length > 0) {
        $('.app-page').addClass('has-scroll-to-nav');
        $('.js-t63-scrollto-nav-item a').click(function () {
            var href = $(this).attr('href').slice(1);
            var section = $('.t63-section[data-id="' + href + '"]');
            var posTop = section.offset().top - 116;
            $('html,body').animate({ scrollTop: posTop }, 200);
            return false;
        });
    }

    //--- slider
    if ($('.js-t63-slider').length > 0) {
        $('.js-t63-slider').slick({
            dots: true
        });
    }

    if ($('.js-t63-testimonials-slider').length > 0) {
        $('.js-t63-testimonials-slider').slick({
            fade: true,
            arrows: false,
            dots: true,
            autoplay: true,
            autoplaySpeed: 5000,
            pauseOnHover: false,
            pauseOnFocus: false
        });
    }

    //--- faq
    $('.js-faq-item-head').click(function () {
        const container = $(this).closest('.js-faq-item');
        $(container).toggleClass('is-open');
        $(container).find('.js-faq-item-body').slideToggle(200);
    });


    //--- text-wrap
    if ($('.js-text-wrap table').length > 0) {
        $('.js-text-wrap table').each(function () {
            $(this).wrap('<div class="t63-table-container"></div>');
        });
    }
});