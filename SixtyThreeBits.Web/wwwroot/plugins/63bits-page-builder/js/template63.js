const t63Model = {
    tabs: {
        show: function (navItem) {
            var container = $(navItem).closest('.js-t63-page-section');
            var id = $(navItem).attr('href');

            container.find('.js-t63-tab-nav-item a').removeClass('active');
            container.find('.js-t63-tab-content-item').removeClass('active').removeClass('show');

            $(navItem).addClass('active');
            $(id).addClass('show').addClass('active');
        },
        init: function () {
            $('.js-t63-tab-nav-item:first-child a').each(function (index, item) {
                t63Model.tabs.show($(item));
            });

            $('.js-t63-tab-nav-item a').click( function (e) {
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
        $('.js-t63-page-section[data-is-fullheight="true"] .js-t63-slide-content').css({ 'min-height': height });
        $('.js-t63-page-section[data-is-fullheight="true"]').removeClass('t63-invisible');
    }
}

$(function () {
    //--- scrollto nav
    t63Model.scrollToNav.init();
    
    //--- img & slider section height
    t63Model.setSlideFullHeight();
    

    //--- slider
    if ($('.js-t63-multimedia-slider').length > 0) {
        $('.js-t63-multimedia-slider').each(function (index, item) {
            const id = $(item).closest('.js-t63-page-section').attr('data-id');

            const autoplayEnapled = $(item).attr('data-slider-autoplay') == 'true';
            const autoplaySpeed = +$(item).attr('data-slider-autoplay-speed');

            let autoplay = false;

            if (autoplayEnapled) {
                autoplay = {
                    delay: autoplaySpeed || 400,
                };
            }

            new Swiper('[data-id="'+id+'"] .js-t63-multimedia-slider', {
                loop: true,
                autoplay: autoplay,
                pagination: {
                    el: '.js-t63-slider-pagination',
                },
                navigation: {
                    nextEl: '.js-t63-slider-btn-next',
                    prevEl: '.js-t63-slider-btn-prev',
                },
            });

        });
    }

    if ($('.js-t63-testimonials-slider').length > 0) {
        new Swiper('.js-t63-testimonials-slider', {
            loop: true,
            autoHeight: true,
            pagination: {
                el: '.js-t63-slider-pagination',
                clickable: true
            },
        });
    }

    //--- accordion
    $('.js-t63-accordion-item-head').click(function () {
        const container = $(this).closest('.js-t63-accordion-items-row');
        const item = $(this).closest('.js-t63-accordion-item');
        if ($(item).hasClass('is-open')) {
            $(item).removeClass('is-open');
            $(item).find('.js-t63-accordion-item-body').slideUp(200);
        } else {
            container.find('.js-t63-accordion-item.is-open .js-t63-accordion-item-body').slideUp(200);
            container.find('.js-t63-accordion-item.is-open').removeClass('is-open');            

            $(item).addClass('is-open');
            $(item).find('.js-t63-accordion-item-body').slideDown(200);
        }
    });


    //--- text-wrap
    if ($('.js-t63-text-wrap table').length > 0) {
        $('.js-t63-text-wrap table').each(function () {
            $(this).wrap('<div class="t63-table-container"></div>');
        });
    }

    //--- tabs
    t63Model.tabs.init();

    //--- flip-cards
    $('.js-t63-flip-cards-grid-item[data-clickable="true"]').click(function () {
        $(this).toggleClass('flip-card-item--rotate');
    });

    //--- tooltip
    $('[data-bs-toggle="tooltip"]').tooltip();
    $('a[data-bs-toggle="tooltip"]').click(function (e) {
        e.preventDefault();
    });

    //--- jwPlayer
    $('.js-t63-jwPlayer-wrap').each(function (index, item) {
        if ($(item).children('.jwplayer').length == 0) {
            new VideoPlayer({
                selector: $(item).children('div'),
                file: $(item).attr('data-video-url'),
                image: $(item).attr('data-cover-url')
            }).Init();
        }
    });
});

$(window).on('load resize', function () {
    t63Model.setSlideFullHeight();
});