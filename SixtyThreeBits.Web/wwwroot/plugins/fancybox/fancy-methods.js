var fancyBox = {
    src: null,
    selector: null,
    hash: false,

    baseClass: '',
    slideClass: '',

    infobar: true,
    buttons: null, // 'zoom','share','slideShow','fullScreen','thumbs','download','close'
	idleTime: false,
	
    smallBtn: null,
	customSmallBtnTpl: '<button type="button" data-fancybox-close="" class="close-popup-btn"><svg xmlns="http://www.w3.org/2000/svg" version="1" viewBox="0 0 13.6 13.6"><polygon points="8.3,6.8 13.5,1.7 11.9,0.1 6.8,5.2 1.7,0.1 0.1,1.7 5.2,6.8 0.1,11.9 1.7,13.5 6.8,8.3 11.9,13.5 13.5,11.9"/></svg></button>', //custom close btn
	
	modal: false,
	
	width: null,
    height: null,

	// Open/close animation
	animationEffect: 'zoom', // 'fade', 'zoom', 'zoom-in-out'
    animationDuration: 366,
	
	// Transition effect between slides
    transitionEffect: 'slide', // 'fade', 'slide', 'circular', 'tube', 'zoom-in-out', 'rotate'
	transitionDuration: 366,

    loop: true,
	
	autoStart: false,
    speed: 3000,
    
	preload: 'auto',
    protect: false,

    clickContent: false,
    clickSlide: false,
    clickOutside: false,

    onInit: null,

    beforeLoad: null,
    afterLoad: null,

    beforeShow: null,
    afterShow: null,

    beforeClose: null,
    afterClose: null,

    onActivate: null,
    onDeactivate: null,

    init: function (options = {}) {
        fancyBox.src = options.src;
        fancyBox.selector = options.selector == undefined ? fancyBox.selector : options.selector;
        fancyBox.hash = options.hash == undefined ? fancyBox.hash : options.hash;

        fancyBox.baseClass = options.baseClass == undefined ? fancyBox.baseClass : options.baseClass;
        fancyBox.slideClass = options.slideClass == undefined ? fancyBox.slideClass : options.slideClass;

        fancyBox.infobar = options.infobar == undefined ? fancyBox.infobar : options.infobar;
        fancyBox.buttons = options.buttons == undefined ? fancyBox.buttons : options.buttons;
        fancyBox.smallBtn = options.smallBtn == undefined ? fancyBox.smallBtn : options.smallBtn;
		fancyBox.customSmallBtnTpl = options.customSmallBtnTpl == undefined ? fancyBox.customSmallBtnTpl : options.customSmallBtnTpl;

        fancyBox.width = options.width == undefined ? fancyBox.width : options.width;
        fancyBox.height = options.height == undefined ? fancyBox.height : options.height;

        fancyBox.loop = options.loop == undefined ? fancyBox.loop : options.loop;
        
		fancyBox.autoStart = options.autoStart == undefined ? fancyBox.autoStart : options.autoStart;
		fancyBox.speed = options.speed == undefined ? fancyBox.speed : options.speed;
		
        fancyBox.preload = options.preload == undefined ? fancyBox.preload : options.preload;
        fancyBox.protect = options.protect == undefined ? fancyBox.protect : options.protect;

		fancyBox.animationEffect = options.animationEffect == undefined ? fancyBox.animationEffect : options.animationEffect;
		fancyBox.animationDuration = options.animationDuration == undefined ? fancyBox.animationDuration : options.animationDuration;
		
        fancyBox.transitionEffect = options.transitionEffect == undefined ? fancyBox.transitionEffect : options.transitionEffect;
		fancyBox.transitionDuration = options.transitionDuration == undefined ? fancyBox.transitionDuration : options.transitionDuration;

        fancyBox.clickContent = options.clickContent == undefined ? fancyBox.clickContent : options.clickContent;
        fancyBox.clickSlide = options.clickSlide == undefined ? fancyBox.clickSlide : options.clickSlide;
        fancyBox.clickOutside = options.clickOutside == undefined ? fancyBox.clickOutside : options.clickOutside;

        fancyBox.modal = options.modal == undefined ? fancyBox.modal : options.modal;

        fancyBox.onInit = options.onInit == undefined ? fancyBox.onInit : options.onInit;

        fancyBox.beforeLoad = options.beforeLoad == undefined ? fancyBox.beforeLoad : options.beforeLoad;
        fancyBox.afterLoad = options.afterLoad == undefined ? fancyBox.afterLoad : options.afterLoad;

        fancyBox.beforeShow = options.beforeShow == undefined ? fancyBox.beforeShow : options.beforeShow;
        fancyBox.afterShow = options.afterShow == undefined ? fancyBox.afterShow : options.afterShow;

        fancyBox.beforeClose = options.beforeClose == undefined ? fancyBox.beforeClose : options.beforeClose;
        fancyBox.afterClose = options.afterClose == undefined ? fancyBox.afterClose : options.afterClose;

        fancyBox.onActivate = options.onActivate == undefined ? fancyBox.onActivate : options.onActivate;
        fancyBox.onDeactivate = options.onDeactivate == undefined ? fancyBox.onDeactivate : options.onDeactivate;

        return fancyBox;
    },

	showImagePopup: function () {
        $.fancybox.open({
            type: 'image',
            src: fancyBox.src,

            baseClass: fancyBox.baseClass,
            slideClass: fancyBox.slideClass,

            infobar: fancyBox.infobar,
            buttons: fancyBox.buttons || ['zoom', 'close'],
			idleTime: fancyBox.idleTime,

			animationEffect: fancyBox.animationEffect,
			animationDuration: fancyBox.animationDuration,
			
            clickContent: fancyBox.clickContent,
            clickSlide:  fancyBox.clickSlide,
            clickOutside:  fancyBox.clickOutside,

            backFocus: false,

            protect: fancyBox.protect,
            image: {
                preload: fancyBox.preload,
            }
        },
            {
                onInit: fancyBox.onInit,

                beforeShow: fancyBox.beforeShow,
                afterShow: fancyBox.afterShow,

                beforeClose: fancyBox.beforeClose,
                afterClose: fancyBox.afterClose,
            });
    },

    showImageGallery: function () {
        $(fancyBox.selector).fancybox({
            type: 'image',
            src: fancyBox.src,
            hash: fancyBox.hash,
            selector: fancyBox.selector,

            baseClass: fancyBox.baseClass,
            slideClass: fancyBox.slideClass,

            infobar: fancyBox.infobar,
            buttons: fancyBox.buttons || ['zoom', 'slideShow', 'thumbs', 'close'],
			idleTime: fancyBox.idleTime,
			
			animationEffect: fancyBox.animationEffect,
			animationDuration: fancyBox.animationDuration,
			
            transitionEffect: fancyBox.transitionEffect,
			transitionDuration: fancyBox.transitionDuration,
			
			loop: fancyBox.loop,
			
			autoStart: fancyBox.autoStart,
			speed: fancyBox.speed,
			
			clickContent: fancyBox.clickContent,
            clickSlide: fancyBox.clickSlide,
            clickOutside: fancyBox.clickOutside,

            backFocus: false,

            protect: fancyBox.protect,
            image: {
                preload: fancyBox.preload,
            },

            onInit: fancyBox.onInit,

            beforeShow: fancyBox.beforeShow,
            afterShow: fancyBox.afterShow,

            beforeClose: fancyBox.beforeClose,
            afterClose: fancyBox.afterClose,
        });
    },

    showVirtualGallery: function () {
        $.fancybox.open(fancyBox.src, {
            type: 'image',

            baseClass: fancyBox.baseClass,
            slideClass: fancyBox.slideClass,

            infobar: fancyBox.infobar,
            buttons: fancyBox.buttons || ['zoom', 'slideShow', 'close'],
			idleTime: fancyBox.idleTime,
			
            smallBtn: fancyBox.smallBtn,
			btnTpl : {
				smallBtn: fancyBox.customSmallBtnTpl
			},

            animationEffect: fancyBox.animationEffect,
			animationDuration: fancyBox.animationDuration,
			
            transitionEffect: fancyBox.transitionEffect,
			transitionDuration: fancyBox.transitionDuration,
			
			loop: fancyBox.loop,
			
			autoStart: fancyBox.autoStart,
			speed: fancyBox.speed,

            clickContent: fancyBox.clickContent,
            clickSlide: fancyBox.clickSlide,
            clickOutside: fancyBox.clickOutside,

            backFocus: false,

            protect: fancyBox.protect,
            image: {
                preload: fancyBox.preload,
            },

            onInit: fancyBox.onInit,

            beforeShow: fancyBox.beforeShow,
            afterShow: fancyBox.afterShow,

            beforeClose: fancyBox.beforeClose,
            afterClose: fancyBox.afterClose,
        });
    },

    showVideoPopup: function () {
        $.fancybox.open({
            src: fancyBox.src,

            baseClass: fancyBox.baseClass,
            slideClass: fancyBox.slideClass,

            infobar: fancyBox.infobar,
            buttons: fancyBox.buttons || ['close'],
			idleTime: fancyBox.idleTime,
			
            smallBtn: fancyBox.smallBtn,
			btnTpl : {
				smallBtn: fancyBox.customSmallBtnTpl
			},
			
			animationEffect: fancyBox.animationEffect,
			animationDuration: fancyBox.animationDuration,

            clickSlide: fancyBox.clickSlide,
            clickOutside: fancyBox.clickOutside,

            backFocus: false

        },
            {
                onInit: fancyBox.onInit,

                beforeLoad: fancyBox.beforeLoad,

                afterLoad: fancyBox.afterLoad || function (instance, current) {
                    current.$content.css({
                        overflow: 'visible'
                    });
                },
                onUpdate: fancyBox.onUpdate || function (instance, current) {
                    var width,
                        height,
                        ratio = 16 / 9,

                        video = current.$content;

                    if (video) {
                        video.hide();

                        width = current.$slide.width();
                        height = current.$slide.height() - 100;

                        if (height * ratio > width) {
                            height = width / ratio;
                        } else {
                            width = height * ratio;
                        }

                        video.css({
                            width: width,
                            height: height
                        }).show();

                    }
                },

                beforeShow: fancyBox.beforeShow,
                afterShow: fancyBox.afterShow,

                beforeClose: fancyBox.beforeClose,
                afterClose: fancyBox.afterClose,
            });
    },

    showVideoGallery: function () {
        $(fancyBox.selector).fancybox({
            src: fancyBox.src,
            hash: fancyBox.hash,
            selector: fancyBox.selector,

            baseClass: fancyBox.baseClass,
            slideClass: fancyBox.slideClass,

            infobar: fancyBox.infobar,
            buttons: fancyBox.buttons || ['close'],
			idleTime: fancyBox.idleTime,
			
			smallBtn: fancyBox.smallBtn,
			btnTpl : {
				smallBtn: fancyBox.customSmallBtnTpl
			},

            animationEffect: fancyBox.animationEffect,
			animationDuration: fancyBox.animationDuration,
			
            transitionEffect: fancyBox.transitionEffect,
			transitionDuration: fancyBox.transitionDuration,
			
			loop: fancyBox.loop,
			
			autoStart: fancyBox.autoStart,
			speed: fancyBox.speed,

            clickSlide: fancyBox.clickSlide,
            clickOutside: fancyBox.clickOutside,

            backFocus: false,

            onInit: fancyBox.onInit,

            afterLoad: fancyBox.afterLoad || function (instance, current) {
                current.$content.css({
                    overflow: 'visible'
                });
            },
            onUpdate: fancyBox.onUpdate || function (instance, current) {
                var width,
                    height,
                    ratio = 16 / 9,
                    video = current.$content;

                if (video) {
                    video.hide();

                    width = current.$slide.width();
                    height = current.$slide.height() - 100;

                    if (height * ratio > width) {
                        height = width / ratio;
                    } else {
                        width = height * ratio;
                    }

                    video.css({
                        width: width,
                        height: height
                    }).show();

                }
            },

            beforeShow: fancyBox.beforeShow,
            afterShow: fancyBox.afterShow,

            beforeClose: fancyBox.beforeClose,
            afterClose: fancyBox.afterClose,
        });
    },

    showInlinePopup: function () {
        $.fancybox.open({
            type: 'inline',
            src: fancyBox.src,

            baseClass: 'fancybox-inline-popup ' + fancyBox.baseClass,
            slideClass: fancyBox.slideClass,

            buttons: fancyBox.buttons || ['close'],
			idleTime: fancyBox.idleTime,
			
            smallBtn: fancyBox.smallBtn === null ? true : fancyBox.smallBtn,
			btnTpl : {
				smallBtn: fancyBox.customSmallBtnTpl
			},
			
			animationEffect: fancyBox.animationEffect,
			animationDuration: fancyBox.animationDuration,

            clickSlide: fancyBox.clickSlide,
            clickOutside: fancyBox.clickOutside,

            touch: false,
            backFocus: false,

            modal: fancyBox.modal
        },
            {
                onInit: fancyBox.onInit,

                beforeLoad: fancyBox.beforeLoad,
                afterLoad: fancyBox.afterLoad,

                beforeShow: fancyBox.beforeShow,
                afterShow: fancyBox.afterShow,

                beforeClose: fancyBox.beforeClose,
                afterClose: fancyBox.afterClose,
            });
    },

    showAjaxPopup: function () {
        $.fancybox.open({
            type: 'ajax',
            src: fancyBox.src,

            baseClass: 'fancybox-ajax-popup ' + fancyBox.baseClass,
            slideClass: fancyBox.slideClass,

            buttons: fancyBox.buttons || ['close'],
			idleTime: fancyBox.idleTime,
			
            smallBtn: fancyBox.smallBtn === null ? true : fancyBox.smallBtn,
			btnTpl : {
				smallBtn: fancyBox.customSmallBtnTpl
			},
			
			animationEffect: fancyBox.animationEffect,
			animationDuration: fancyBox.animationDuration,

            clickSlide: fancyBox.clickSlide,
            clickOutside: fancyBox.clickOutside,

            touch: false,
            backFocus: false,

            modal: fancyBox.modal
        },
            {
                onInit: fancyBox.onInit,

                beforeLoad: fancyBox.beforeLoad,
                afterLoad: fancyBox.afterLoad,

                beforeShow: fancyBox.beforeShow,
                afterShow: fancyBox.afterShow,

                beforeClose: fancyBox.beforeClose,
                afterClose: fancyBox.afterClose,
            });
    },
    
    showHtmlPopup: function () {
        $.fancybox.open({
            type: 'html',
            src: fancyBox.src,

            baseClass: 'fancybox-html-popup ' + fancyBox.baseClass,
            slideClass: fancyBox.slideClass,

            buttons: fancyBox.buttons || ['close'],
			idleTime: fancyBox.idleTime,
			
            smallBtn: fancyBox.smallBtn === null ? true : fancyBox.smallBtn,
			btnTpl : {
				smallBtn: fancyBox.customSmallBtnTpl
			},
			
			animationEffect: fancyBox.animationEffect,
			animationDuration: fancyBox.animationDuration,

            clickSlide: fancyBox.clickSlide,
            clickOutside: fancyBox.clickOutside,

            touch: false,
            backFocus: false,

            modal: fancyBox.modal
        },
            {
                onInit: fancyBox.onInit,

                beforeLoad: fancyBox.beforeLoad,
                afterLoad: fancyBox.afterLoad,

                beforeShow: fancyBox.beforeShow,
                afterShow: fancyBox.afterShow,

                beforeClose: fancyBox.beforeClose,
                afterClose: fancyBox.afterClose,
            });
    },
	
	showIframePopup: function () {
        $.fancybox.open({
            type: 'iframe',
            src: fancyBox.src,

            baseClass: 'fancybox-iframe-popup ' + fancyBox.baseClass,
            slideClass: fancyBox.slideClass,

            buttons: fancyBox.buttons || ['close'],
			idleTime: fancyBox.idleTime,
			
            smallBtn: fancyBox.smallBtn === null ? true : fancyBox.smallBtn,
			btnTpl : {
				smallBtn: fancyBox.customSmallBtnTpl
			},
			
			animationEffect: fancyBox.animationEffect,
			animationDuration: fancyBox.animationDuration,

            clickSlide: fancyBox.clickSlide,
            clickOutside: fancyBox.clickOutside,

            touch: false,
            backFocus: false,

            modal: fancyBox.modal,

            iframe: {
                css: {
                    width: fancyBox.width,
                    height: fancyBox.height
                },
                attr: {
                    scrolling: false
                }
            },
        },
            {
                onInit: fancyBox.onInit,

                beforeLoad: fancyBox.beforeLoad,
                afterLoad: fancyBox.afterLoad,

                beforeShow: fancyBox.beforeShow,
                afterShow: fancyBox.afterShow,

                beforeClose: fancyBox.beforeClose,
                afterClose: fancyBox.afterClose,
            });
    },
		
    closePopup: function () {
        $.fancybox.close();
    }
};