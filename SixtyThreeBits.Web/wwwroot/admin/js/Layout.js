$(function () {
    $('[data-trigger-resize]').click(function (e) {
        e.preventDefault();
        const Body = $('body');
        let IsSidebarCollapsed;

        if ($(this).hasClass('sidebar-toggle')) {
            if (Body.hasClass('aside-toggled')) {
                Body.removeClass('aside-toggled');
                IsSidebarCollapsed = false;
            }
            else {
                Body.addClass('aside-toggled');
                IsSidebarCollapsed = true;
            }
        }
        else {
            if (Body.hasClass('aside-collapsed')) {
                Body.removeClass('aside-collapsed');
                IsSidebarCollapsed = false;
            }
            else {
                Body.addClass('aside-collapsed');
                IsSidebarCollapsed = true;
            }
        }

        Utilities.SetCookie(LayoutModel.SidebarStatusCookieKey, IsSidebarCollapsed);
    });

    $(document).on('mouseenter', '.aside-collapsed ul.sidebar-nav > [data-menu-top-item]', function () {
        LayoutModel.Navigation.ToggleMenuItem($(this));
    });
});

const LayoutModel = {
    SidebarStatusCookieKey: null,
    Navigation: {
        ToggleMenuItem: function ($listItem) {

            LayoutModel.Navigation.RemoveFloatingNav();

            $sidebar = $('.sidebar');
            $win = $(window);

            var ul = $listItem.children('ul');

            if (!ul.length) return $();
            if ($listItem.hasClass('open')) {
                LayoutModel.Navigation.ToggleTouchItem($listItem);
                return $();
            }

            var $aside = $('.aside-container');
            var $asideInner = $('.aside-inner'); // for top offset calculation
            // float aside uses extra padding on aside
            var mar = parseInt($asideInner.css('padding-top'), 0) + parseInt($aside.css('padding-top'), 0);

            var subNav = ul.clone().appendTo($aside);

            LayoutModel.Navigation.ToggleTouchItem($listItem);

            var itemTop = ($listItem.position().top + mar) - $sidebar.scrollTop();
            var vwHeight = $win.height();

            subNav
              .addClass('nav-floating')
              .css({
                  position: 'absolute',
                  top: itemTop,
                  bottom: (subNav.outerHeight(true) + itemTop > vwHeight) ? 0 : 'auto'
              });

            subNav.on('mouseleave', function () {
                LayoutModel.Navigation.ToggleTouchItem($listItem);
                subNav.remove();
            });

            return subNav;
        },

        ToggleTouchItem: function ($element) {
            $element
              .siblings('li')
              .removeClass('open')
              .end()
              .toggleClass('open');
        },

        RemoveFloatingNav: function () {
            $('.sidebar-subnav.nav-floating').remove();
            $('.dropdown-backdrop').remove();
            $('.sidebar li.open').removeClass('open');
        }
    }
};