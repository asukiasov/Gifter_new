var FancyBox = {
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

    Init: function (options = {}) {
        FancyBox.src = options.src;
        FancyBox.selector = options.selector == undefined ? FancyBox.selector : options.selector;
        FancyBox.hash = options.hash == undefined ? FancyBox.hash : options.hash;

        FancyBox.baseClass = options.baseClass == undefined ? FancyBox.baseClass : options.baseClass;
        FancyBox.slideClass = options.slideClass == undefined ? FancyBox.slideClass : options.slideClass;

        FancyBox.infobar = options.infobar == undefined ? FancyBox.infobar : options.infobar;
        FancyBox.buttons = options.buttons == undefined ? FancyBox.buttons : options.buttons;
        FancyBox.smallBtn = options.smallBtn == undefined ? FancyBox.smallBtn : options.smallBtn;
		FancyBox.customSmallBtnTpl = options.customSmallBtnTpl == undefined ? FancyBox.customSmallBtnTpl : options.customSmallBtnTpl;

        FancyBox.width = options.width == undefined ? FancyBox.width : options.width;
        FancyBox.height = options.height == undefined ? FancyBox.height : options.height;

        FancyBox.loop = options.loop == undefined ? FancyBox.loop : options.loop;
        
		FancyBox.autoStart = options.autoStart == undefined ? FancyBox.autoStart : options.autoStart;
		FancyBox.speed = options.speed == undefined ? FancyBox.speed : options.speed;
		
        FancyBox.preload = options.preload == undefined ? FancyBox.preload : options.preload;
        FancyBox.protect = options.protect == undefined ? FancyBox.protect : options.protect;

		FancyBox.animationEffect = options.animationEffect == undefined ? FancyBox.animationEffect : options.animationEffect;
		FancyBox.animationDuration = options.animationDuration == undefined ? FancyBox.animationDuration : options.animationDuration;
		
        FancyBox.transitionEffect = options.transitionEffect == undefined ? FancyBox.transitionEffect : options.transitionEffect;
		FancyBox.transitionDuration = options.transitionDuration == undefined ? FancyBox.transitionDuration : options.transitionDuration;

        FancyBox.clickContent = options.clickContent == undefined ? FancyBox.clickContent : options.clickContent;
        FancyBox.clickSlide = options.clickSlide == undefined ? FancyBox.clickSlide : options.clickSlide;
        FancyBox.clickOutside = options.clickOutside == undefined ? FancyBox.clickOutside : options.clickOutside;

        FancyBox.modal = options.modal == undefined ? FancyBox.modal : options.modal;

        FancyBox.onInit = options.onInit == undefined ? FancyBox.onInit : options.onInit;

        FancyBox.beforeLoad = options.beforeLoad == undefined ? FancyBox.beforeLoad : options.beforeLoad;
        FancyBox.afterLoad = options.afterLoad == undefined ? FancyBox.afterLoad : options.afterLoad;

        FancyBox.beforeShow = options.beforeShow == undefined ? FancyBox.beforeShow : options.beforeShow;
        FancyBox.afterShow = options.afterShow == undefined ? FancyBox.afterShow : options.afterShow;

        FancyBox.beforeClose = options.beforeClose == undefined ? FancyBox.beforeClose : options.beforeClose;
        FancyBox.afterClose = options.afterClose == undefined ? FancyBox.afterClose : options.afterClose;

        FancyBox.onActivate = options.onActivate == undefined ? FancyBox.onActivate : options.onActivate;
        FancyBox.onDeactivate = options.onDeactivate == undefined ? FancyBox.onDeactivate : options.onDeactivate;

        return FancyBox;
    },

	ShowImagePopup: function () {
        $.fancybox.open({
            type: 'image',
            src: FancyBox.src,

            baseClass: FancyBox.baseClass,
            slideClass: FancyBox.slideClass,

            infobar: FancyBox.infobar,
            buttons: FancyBox.buttons || ['zoom', 'close'],
			idleTime: FancyBox.idleTime,

			animationEffect: FancyBox.animationEffect,
			animationDuration: FancyBox.animationDuration,
			
            clickContent: FancyBox.clickContent,
            clickSlide:  FancyBox.clickSlide,
            clickOutside:  FancyBox.clickOutside,

            backFocus: false,

            protect: FancyBox.protect,
            image: {
                preload: FancyBox.preload,
            }
        },
            {
                onInit: FancyBox.onInit,

                beforeShow: FancyBox.beforeShow,
                afterShow: FancyBox.afterShow,

                beforeClose: FancyBox.beforeClose,
                afterClose: FancyBox.afterClose,
            });
    },

    ShowImageGallery: function () {
        $(FancyBox.selector).fancybox({
            type: 'image',
            src: FancyBox.src,
            hash: FancyBox.hash,
            selector: FancyBox.selector,

            baseClass: FancyBox.baseClass,
            slideClass: FancyBox.slideClass,

            infobar: FancyBox.infobar,
            buttons: FancyBox.buttons || ['zoom', 'slideShow', 'thumbs', 'close'],
			idleTime: FancyBox.idleTime,
			
			animationEffect: FancyBox.animationEffect,
			animationDuration: FancyBox.animationDuration,
			
            transitionEffect: FancyBox.transitionEffect,
			transitionDuration: FancyBox.transitionDuration,
			
			loop: FancyBox.loop,
			
			autoStart: FancyBox.autoStart,
			speed: FancyBox.speed,
			
			clickContent: FancyBox.clickContent,
            clickSlide: FancyBox.clickSlide,
            clickOutside: FancyBox.clickOutside,

            backFocus: false,

            protect: FancyBox.protect,
            image: {
                preload: FancyBox.preload,
            },

            onInit: FancyBox.onInit,

            beforeShow: FancyBox.beforeShow,
            afterShow: FancyBox.afterShow,

            beforeClose: FancyBox.beforeClose,
            afterClose: FancyBox.afterClose,
        });
    },

    ShowVirtualGallery: function () {
        $.fancybox.open(FancyBox.src, {
            type: 'image',

            baseClass: FancyBox.baseClass,
            slideClass: FancyBox.slideClass,

            infobar: FancyBox.infobar,
            buttons: FancyBox.buttons || ['zoom', 'slideShow', 'close'],
			idleTime: FancyBox.idleTime,
			
            smallBtn: FancyBox.smallBtn,
			btnTpl : {
				smallBtn: FancyBox.customSmallBtnTpl
			},

            animationEffect: FancyBox.animationEffect,
			animationDuration: FancyBox.animationDuration,
			
            transitionEffect: FancyBox.transitionEffect,
			transitionDuration: FancyBox.transitionDuration,
			
			loop: FancyBox.loop,
			
			autoStart: FancyBox.autoStart,
			speed: FancyBox.speed,

            clickContent: FancyBox.clickContent,
            clickSlide: FancyBox.clickSlide,
            clickOutside: FancyBox.clickOutside,

            backFocus: false,

            protect: FancyBox.protect,
            image: {
                preload: FancyBox.preload,
            },

            onInit: FancyBox.onInit,

            beforeShow: FancyBox.beforeShow,
            afterShow: FancyBox.afterShow,

            beforeClose: FancyBox.beforeClose,
            afterClose: FancyBox.afterClose,
        });
    },

    ShowVideoPopup: function () {
        $.fancybox.open({
            src: FancyBox.src,

            baseClass: FancyBox.baseClass,
            slideClass: FancyBox.slideClass,

            infobar: FancyBox.infobar,
            buttons: FancyBox.buttons || ['close'],
			idleTime: FancyBox.idleTime,
			
            smallBtn: FancyBox.smallBtn,
			btnTpl : {
				smallBtn: FancyBox.customSmallBtnTpl
			},
			
			animationEffect: FancyBox.animationEffect,
			animationDuration: FancyBox.animationDuration,

            clickSlide: FancyBox.clickSlide,
            clickOutside: FancyBox.clickOutside,

            backFocus: false

        },
            {
                onInit: FancyBox.onInit,

                beforeLoad: FancyBox.beforeLoad,

                afterLoad: FancyBox.afterLoad || function (instance, current) {
                    current.$content.css({
                        overflow: 'visible'
                    });
                },
                onUpdate: FancyBox.onUpdate || function (instance, current) {
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

                beforeShow: FancyBox.beforeShow,
                afterShow: FancyBox.afterShow,

                beforeClose: FancyBox.beforeClose,
                afterClose: FancyBox.afterClose,
            });
    },

    ShowVideoGallery: function () {
        $(FancyBox.selector).fancybox({
            src: FancyBox.src,
            hash: FancyBox.hash,
            selector: FancyBox.selector,

            baseClass: FancyBox.baseClass,
            slideClass: FancyBox.slideClass,

            infobar: FancyBox.infobar,
            buttons: FancyBox.buttons || ['close'],
			idleTime: FancyBox.idleTime,
			
			smallBtn: FancyBox.smallBtn,
			btnTpl : {
				smallBtn: FancyBox.customSmallBtnTpl
			},

            animationEffect: FancyBox.animationEffect,
			animationDuration: FancyBox.animationDuration,
			
            transitionEffect: FancyBox.transitionEffect,
			transitionDuration: FancyBox.transitionDuration,
			
			loop: FancyBox.loop,
			
			autoStart: FancyBox.autoStart,
			speed: FancyBox.speed,

            clickSlide: FancyBox.clickSlide,
            clickOutside: FancyBox.clickOutside,

            backFocus: false,

            onInit: FancyBox.onInit,

            afterLoad: FancyBox.afterLoad || function (instance, current) {
                current.$content.css({
                    overflow: 'visible'
                });
            },
            onUpdate: FancyBox.onUpdate || function (instance, current) {
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

            beforeShow: FancyBox.beforeShow,
            afterShow: FancyBox.afterShow,

            beforeClose: FancyBox.beforeClose,
            afterClose: FancyBox.afterClose,
        });
    },

    ShowInlinePopup: function () {
        $.fancybox.open({
            type: 'inline',
            src: FancyBox.src,

            baseClass: 'fancybox-inline-popup ' + FancyBox.baseClass,
            slideClass: FancyBox.slideClass,

            buttons: FancyBox.buttons || ['close'],
			idleTime: FancyBox.idleTime,
			
            smallBtn: FancyBox.smallBtn === null ? true : FancyBox.smallBtn,
			btnTpl : {
				smallBtn: FancyBox.customSmallBtnTpl
			},
			
			animationEffect: FancyBox.animationEffect,
			animationDuration: FancyBox.animationDuration,

            clickSlide: FancyBox.clickSlide,
            clickOutside: FancyBox.clickOutside,

            touch: false,
            backFocus: false,

            modal: FancyBox.modal
        },
            {
                onInit: FancyBox.onInit,

                beforeLoad: FancyBox.beforeLoad,
                afterLoad: FancyBox.afterLoad,

                beforeShow: FancyBox.beforeShow,
                afterShow: FancyBox.afterShow,

                beforeClose: FancyBox.beforeClose,
                afterClose: FancyBox.afterClose,
            });
    },

    ShowAjaxPopup: function () {
        $.fancybox.open({
            type: 'ajax',
            src: FancyBox.src,

            baseClass: 'fancybox-ajax-popup ' + FancyBox.baseClass,
            slideClass: FancyBox.slideClass,

            buttons: FancyBox.buttons || ['close'],
			idleTime: FancyBox.idleTime,
			
            smallBtn: FancyBox.smallBtn === null ? true : FancyBox.smallBtn,
			btnTpl : {
				smallBtn: FancyBox.customSmallBtnTpl
			},
			
			animationEffect: FancyBox.animationEffect,
			animationDuration: FancyBox.animationDuration,

            clickSlide: FancyBox.clickSlide,
            clickOutside: FancyBox.clickOutside,

            touch: false,
            backFocus: false,

            modal: FancyBox.modal
        },
            {
                onInit: FancyBox.onInit,

                beforeLoad: FancyBox.beforeLoad,
                afterLoad: FancyBox.afterLoad,

                beforeShow: FancyBox.beforeShow,
                afterShow: FancyBox.afterShow,

                beforeClose: FancyBox.beforeClose,
                afterClose: FancyBox.afterClose,
            });
    },
    
    ShowHtmlPopup: function () {
        $.fancybox.open({
            type: 'html',
            src: FancyBox.src,

            baseClass: 'fancybox-html-popup ' + FancyBox.baseClass,
            slideClass: FancyBox.slideClass,

            buttons: FancyBox.buttons || ['close'],
			idleTime: FancyBox.idleTime,
			
            smallBtn: FancyBox.smallBtn === null ? true : FancyBox.smallBtn,
			btnTpl : {
				smallBtn: FancyBox.customSmallBtnTpl
			},
			
			animationEffect: FancyBox.animationEffect,
			animationDuration: FancyBox.animationDuration,

            clickSlide: FancyBox.clickSlide,
            clickOutside: FancyBox.clickOutside,

            touch: false,
            backFocus: false,

            modal: FancyBox.modal
        },
            {
                onInit: FancyBox.onInit,

                beforeLoad: FancyBox.beforeLoad,
                afterLoad: FancyBox.afterLoad,

                beforeShow: FancyBox.beforeShow,
                afterShow: FancyBox.afterShow,

                beforeClose: FancyBox.beforeClose,
                afterClose: FancyBox.afterClose,
            });
    },
	
	ShowIframePopup: function () {
        $.fancybox.open({
            type: 'iframe',
            src: FancyBox.src,

            baseClass: 'fancybox-iframe-popup ' + FancyBox.baseClass,
            slideClass: FancyBox.slideClass,

            buttons: FancyBox.buttons || ['close'],
			idleTime: FancyBox.idleTime,
			
            smallBtn: FancyBox.smallBtn === null ? true : FancyBox.smallBtn,
			btnTpl : {
				smallBtn: FancyBox.customSmallBtnTpl
			},
			
			animationEffect: FancyBox.animationEffect,
			animationDuration: FancyBox.animationDuration,

            clickSlide: FancyBox.clickSlide,
            clickOutside: FancyBox.clickOutside,

            touch: false,
            backFocus: false,

            modal: FancyBox.modal,

            iframe: {
                css: {
                    width: FancyBox.width,
                    height: FancyBox.height
                },
                attr: {
                    scrolling: false
                }
            },
        },
            {
                onInit: FancyBox.onInit,

                beforeLoad: FancyBox.beforeLoad,
                afterLoad: FancyBox.afterLoad,

                beforeShow: FancyBox.beforeShow,
                afterShow: FancyBox.afterShow,

                beforeClose: FancyBox.beforeClose,
                afterClose: FancyBox.afterClose,
            });
    },
	
	
    ClosePopup: function () {
        $.fancybox.close();
    }
};