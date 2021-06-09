const t63Model = {
    tabs: {
        show: function (navItem) {
            var container = $(navItem).closest('.js-page-section');
            var id = $(navItem).attr('href');

            container.find('.js-tab-nav-item a').removeClass('active');
            container.find('.js-tab-content-item').removeClass('active').removeClass('show');

            $(navItem).addClass('active');
            $(id).addClass('show').addClass('active');
        },
        init: function () {
            $('.js-tab-nav-item:first-child a').each(function (index, item) {
                t63Model.tabs.show($(item));
            });

            $('.js-tab-nav-item a').click( function (e) {
                t63Model.tabs.show($(this));
                return false;
            });
        }
    },
    scrollToNav: {
        init: function () {
            if ($('.js-t63-scrollto-nav').length > 0) {
                $('.app-page').addClass('has-scroll-to-nav');
                $('.js-t63-scrollto-nav-item a').click(function () {
                    var href = $(this).attr('href').slice(1);
                    var posTop = $('.t63-section[data-id="' + href + '"]').offset().top - $('.js-t63-scrollto-nav').outerHeight();
                    $('html,body').animate({ scrollTop: posTop }, 200);
                    return false;
                });

                $(window).on('load scroll', function () {
                    if ($(window).scrollTop() > $('.app-header').outerHeight()) {
                        $('.js-t63-scrollto-nav').addClass('position-fixed');
                    } else {
                        $('.js-t63-scrollto-nav').removeClass('position-fixed');
                    }
                });

            }
        }
    },
    setSlideFullHeight: function () {
        var height = $(window).height() - $('.app-page').offset().top - ($('.js-t63-scrollto-nav').height() || 0);
        $('.js-page-section[data-is-fullheight="true"] .js-slide-content').css({ 'min-height': height });
        $('.js-page-section[data-is-fullheight="true"]').removeClass('t63-invisible');
    }
}

$(function () {
    //--- scrollto nav
    t63Model.scrollToNav.init();
    
    //--- img & slider section height
    t63Model.setSlideFullHeight();
    

    //--- slider
    if ($('.js-t63-slider').length > 0) {
        $('.js-t63-slider').each(function (index, item) {
            const autoplay = $(item).attr('data-slider-autoplay') == 'true';
            const autoplaySpeed = $(item).attr('data-slider-autoplay-speed');
            $(item).slick({
                dots: true,
                adaptiveHeight: true,
                pauseOnFocus: false,
                pauseOnHover: false,
                autoplay: autoplay,
                autoplaySpeed: autoplaySpeed,
            });
        })
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

    //--- accordion
    $('.js-accordion-item-head').click(function () {
        const container = $(this).closest('.js-accordion-items-row');
        const item = $(this).closest('.js-accordion-item');
        if ($(item).hasClass('is-open')) {
            $(item).removeClass('is-open');
            $(item).find('.js-accordion-item-body').slideUp(200);
        } else {
            container.find('.js-accordion-item.is-open .js-accordion-item-body').slideUp(200);
            container.find('.js-accordion-item.is-open').removeClass('is-open');            

            $(item).addClass('is-open');
            $(item).find('.js-accordion-item-body').slideDown(200);
        }
    });


    //--- text-wrap
    if ($('.js-text-wrap table').length > 0) {
        $('.js-text-wrap table').each(function () {
            $(this).wrap('<div class="t63-table-container"></div>');
        });
    }

    //--- tabs
    t63Model.tabs.init();

    //--- flip-cards
    $('.js-flip-cards-grid-item[data-clickable="true"]').click(function () {
        $(this).toggleClass('flip-card-item--rotate');
    });

    //--- tooltip
    $('[data-toggle="tooltip"]').tooltip();
    $('a[data-toggle="tooltip"]').click(function (e) {
        e.preventDefault();
    });

    //--- jwPlayer
    $('.js-jwPlayer-wrap').each(function (index, item) {
        if ($(item).children('.jwplayer').length == 0) {
            new VideoPlayer({
                Selector: $(item).children('div'),
                File: $(item).attr('data-video-url'),
                Image: $(item).attr('data-cover-url')
            }).Init();
        }
    });
});

$(window).on('load resize', function () {
    t63Model.setSlideFullHeight();
});