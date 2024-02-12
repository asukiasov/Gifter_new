$(function () {
    //--- filters slider
    const filtersSlider = new Swiper('.js-filters-slider', {
        slidesPerView: "auto",
        spaceBetween: 0,
        navigation: {
            prevEl: '.js-filters-slider-prev-btn',
            nextEl: '.js-filters-slider-next-btn',
        },
    });

    //--- virtual select
    VirtualSelect.init({
        ele: '.js-combo',
        hideClearButton: true,
        optionHeight: '30px'
    });
});