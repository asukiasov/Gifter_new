const LayoutModel = {
    Animations: {
        cssAnimations: {
            selector: '.js-animate:not(.animated)',
            init: function (_this, index) {
                var animationClass = _this.data('animation') ? _this.data('animation') : 'fadeIn';
                var animationDelay = _this.data('delay') ? _this.data('delay') : 150;
                setTimeout(function () {
                    _this.addClass('animated ' + animationClass);
                }, index * animationDelay);
            }
        },
        appear: {
            init: function (selector, func) {
                $(selector).appear();

                if ($().appear) {

                    $(selector).filter(':appeared').each(function (index, item) {
                        func($(item), index);
                    });

                    $('body').on('appear', selector, function (e, $affected) {
                        $($affected).each(function (index, item) {
                            func($(item), index);
                        });
                    });
                }

            }
        },
    },
};

$(function () {


    //--- animations
    LayoutModel.Animations.appear.init(LayoutModel.Animations.cssAnimations.selector, LayoutModel.Animations.cssAnimations.init);
});