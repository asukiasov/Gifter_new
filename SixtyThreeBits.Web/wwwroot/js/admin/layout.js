const layoutModel = {
    scrollPosition: null,
    topbar: null,
    sidebarStatusCookieKey: null,
    resizeTimeout: null,
    initTopbarSticky: function () {
        layoutModel.scrollPosition = window.scrollY;
        layoutModel.topbar = document.getElementById('page-topbar');
    },
    initSideMenu: function () {
        new MetisMenu('.js-side-menu')
    },

    setMainContentMinHeight: function () {
        const sidebarHeight = $('.js-app-sidebar-menu-wrapper').offset().top + $('.js-app-sidebar-menu-wrapper').outerHeight();

        if ($('body').attr('data-sidebar-size') == 'sm' && sidebarHeight > $(window).outerHeight()) {
            $('.js-app-main-content').css({ 'min-height': sidebarHeight });
        } else {
            $('.js-app-main-content').removeAttr('style');
        }
    },
};
$(function () {
    layoutModel.initSideMenu();
    layoutModel.initTopbarSticky();

    $('.js-side-menu-collapse-button').click(function () {
        $('body').attr('class', 'sidebar-enable');
        $('body').attr('data-sidebar-size', 'sm');
        const IsSidebarCollapsed = true;
        utilities.setCookie(layoutModel.sidebarStatusCookieKey, IsSidebarCollapsed);

        layoutModel.setMainContentMinHeight();
    });
    $('.js-side-menu-expand-button').click(function () {
        if ($(window).width() > 991) {
            $('body').removeAttr('class');
            $('body').attr('data-sidebar-size', 'lg');
            const IsSidebarCollapsed = false;
            utilities.setCookie(layoutModel.sidebarStatusCookieKey, IsSidebarCollapsed);
        }
        else {
            $('body').toggleClass('sidebar-enable');
        }

        layoutModel.setMainContentMinHeight();
    });        

    $('.js-show-preloader').click(function () { preloader.show(); });
});

window.addEventListener('scroll', function () {
    layoutModel.scrollPosition = window.scrollY;
    if (layoutModel.scrollPosition >= 30) {
        layoutModel.topbar.classList.add('sticky');
    } else {
        layoutModel.topbar.classList.remove('sticky');
    }
});

$(window).on('load resize', function () {
    if (layoutModel.resizeTimeout) {
        layoutModel.resizeTimeout = null;
    }

    layoutModel.resizeTimeout = setTimeout(function () {
        layoutModel.setMainContentMinHeight();
    }, 200);
});