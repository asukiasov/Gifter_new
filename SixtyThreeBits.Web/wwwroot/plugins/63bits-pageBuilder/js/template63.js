$(function () {
    //--- scrollto nav
    if ($('.js-t63-scrollto-nav').length > 0) {
        $('.js-t63-scrollto-nav').prependTo('.page');

        $('.js-t63-scrollto-nav-item a').click(function () {
            var href = $(this).attr('href').slice(1);
            var section = $('.t63-section[data-id="' + href + '"]');
            var posTop = section.position().top - parseInt($('.t63-section').css('padding-top'));
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
});

$(window).on('scroll', function () {
    if ($('.js-t63-scrollto-nav').length > 0) {
        if ($(window).scrollTop() > $('header').outerHeight()) {
            $('.js-t63-scrollto-nav > div').addClass('position-fixed');
        } else {
            $('.js-t63-scrollto-nav > div').removeClass('position-fixed');
        }
    }
});