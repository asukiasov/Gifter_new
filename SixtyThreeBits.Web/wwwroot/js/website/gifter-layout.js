const layoutModel = {
    mediaCarousel: {
        init: function () {
            const selector = '.js-media-carousel';

            if ($(selector).length > 0){
                new Swiper(selector, {
                    slidesPerView: 'auto',
                    spaceBetween: 24,
                    loop: false,
                    watchSlidesProgress: true,
                    scrollbar: {
                        el: '.js-swiper-scrollbar',
                        draggable: true,
                    },

                    breakpoints: {
                        768: {
                            slidesPerView: 2,
                        },

                        992: {
                            slidesPerView: 3,
                        },
                    }
                });
            }
        }
    },

    datepicker: {
        localKa: {
            days: ['კვირა', 'ორშაბათი', 'სამშაბათი', 'ოთხშაბათი', 'ხუთშაბათი', 'პარასკევი', 'შაბათი'],
            daysShort: ['კვი', 'ორშ', 'სამ', 'ოთხ', 'ხუთ', 'პარ', 'შაბ'],
            daysMin: ['კვ', 'ორ', 'სა', 'ოთ', 'ხუ', 'პა', 'შა'],
            months: ['იანვარი', 'თებერვალი', 'მარტი', 'აპრილი', 'მაისი', 'ივნისი', 'ივლისი', 'აგვისტო', 'სექტემბერი', 'ოქტომბერი', 'ნოემბერი', 'დეკემბერი'],
            monthsShort: ['იან', 'თებ', 'მარ', 'აპრ', 'მაი', 'ივნ', 'ივლ', 'აგვ', 'სექ', 'ოქტ', 'ნოე', 'დეკ'],
            today: 'დღეს',
            clear: 'გასუთავება',
            dateFormat: 'dd/MM/yyyy',
            timeFormat: 'hh:mm aa',
            firstDay: 1
        },
    },

    animateNumbers: function () {
        $('.js-animate-number').each(function () {
            $(this).prop('Counter', 0)
                .animate(
                    {
                        Counter: $(this).attr('data-counter'),
                    },
                    {
                        duration: 2000,
                        easing: "swing",
                        step: function (now) {
                            now = Number(Math.ceil(now)).toLocaleString('en');
                            $(this).text(now).removeClass('js-animate-number');
                        },
                    }
                );
        });
    },

    isInViewport: function (element) {
        const rect = element.getBoundingClientRect();
        return (
            rect.top >= 0 &&
            rect.left >= 0 &&
            rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
            rect.right <= (window.innerWidth || document.documentElement.clientWidth)
        );
    },
};

$(function(){
    //---
    if($('.js-svg-icon').length > 0){
        SVGInline();
    }


    //--- hamburger
	$('.js-app-hamburger').click(function () {
		$(this).closest('header').toggleClass('nav-is-open');
    });

    //--- nav dropdown
    $('.js-app-nav [data-bs-toggle="dropdown"]').click(function (e) {
        if($(window).outerWidth() < 1200){
            $(this).parent().toggleClass('dropdown-is-open');
            $(this).next().slideToggle(100)
        }
    });

    //--- accordion
    $('.js-accordion-item-head,.js-accordion-item-toggle-btn').click(function () {
        const container = $(this).closest('.js-accordion-items-row');
        const item = $(this).closest('.js-accordion-item');
        if ($(item).hasClass('accordion-is-open')) {
            $(item).removeClass('accordion-is-open');
            $(item).find('.js-accordion-item-body').slideUp(200);
        } else {
            container.find('.accordion-is-open .js-accordion-item-body').slideUp(200);
            container.find('.accordion-is-open').removeClass('accordion-is-open');

            $(item).addClass('accordion-is-open');
            $(item).find('.js-accordion-item-body').slideDown(200);
        }
    });


    //--- img upload
    $('.js-img-uploader-input').on('change', function (event) {
        const container = $(this).closest('.js-img-uploader');

        const file = event.target.files[0];

        const reader = new FileReader();

        reader.onload = function(e) {
            const base64String = e.target.result;

            const html = `<img src="${base64String}" alt="ID"/>`;
            container.find('div').append(html);

            container.find('label').addClass('d-none');
            container.find('div').removeClass('d-none');
        };

        reader.onerror = function() {
            console.error('Error reading file:', file.name);
        };

        reader.readAsDataURL(file);
    });

    //--- img remove
    $('.js-img-uploader-clear-btn').click(function (){
        const container = $(this).closest('.js-img-uploader');

        container.find('.js-img-uploader-input').val('');
        container.find('label').removeClass('d-none');
        container.find('div').addClass('d-none').children('img').remove();
    });
});

$(window).on('load scroll', function () {
    const scrollTop = $(window).scrollTop();

    if (scrollTop > 10) {
        $('html').addClass('scrolled');
    } else{
        $('html').removeClass('scrolled');
    }
});