/*
External plugins

HTML:
    <div class="mceNonEditable selector" data-plagin-container>
        <div></div>
    </div>

JS:
    $('.selector > div').plgunName();
 */

var PageBuilderModel = {
    urlSave: null,
    textError: null,
    language: null,

    currentViewSelector: '.js-editor-page',

    guid: {
        generateRandomString: function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        },
        s4: function () {
            return PageBuilderModel.guid.generateRandomString().toLowerCase();
        },
        s8: function () {
            return (
                PageBuilderModel.guid.generateRandomString() + "-" +
                PageBuilderModel.guid.generateRandomString()
            ).toLowerCase();
        },
        s15: function () {
            return (
                PageBuilderModel.guid.generateRandomString() + "-" +
                PageBuilderModel.guid.generateRandomString() + "-" +
                PageBuilderModel.guid.generateRandomString().substr(0, 3) + "-" +
                PageBuilderModel.guid.generateRandomString()
            ).toLowerCase();
        }
    },

    builder: {
        selector: '.js-builder',
        closeBtnSelector: '.js-close-builder',

        show: function () {
            $(PageBuilderModel.builder.selector).removeClass('closed');
        },
        hide: function () {
            $(PageBuilderModel.builder.selector).addClass('closed');
        },

        pageItems: {
            selector: '.js-page-items',

            sortable: function () {
                $(PageBuilderModel.builder.pageItems.selector + ' ul').sortable({
                    revert: true,
                    handle: '.js-builder-item-handle',
                    placeholder: 'ui-sortable-placeholder',
                    update: function (event, ui) {

                        if (ui.item.hasClass('ui-draggable')) {
                            PageBuilderModel.builder.toggleCloseBtnByEmtyState();

                            //remove draggable selectors
                            ui.item.removeClass('ui-draggable');
                            ui.item.find('.handle').removeClass('ui-draggable-handle');

                            //add section to page
                            PageBuilderModel.sections.addTopage(ui.item, ui.item.index(), PageBuilderModel.settings);

                            //hide new items container
                            PageBuilderModel.builder.newItems.hide();
                        }
                        else {
                            //sort sections
                            PageBuilderModel.sections.sort(ui.item, ui.item.index());

                            //scrollto nav items
                            PageBuilderModel.scrollToNavigation.init();
                        }

                        PageBuilderModel.builder.addIndexToSections();
                    },
                });
                $(PageBuilderModel.builder.selector + ' ul').disableSelection();
            },

            remove: function (index) {
                $(PageBuilderModel.builder.pageItems.selector + ' [data-index="' + index + '"]').remove();
                $(PageBuilderModel.currentViewSelector + ' .js-page-section[data-index="' + index + '"]').remove();
                PageBuilderModel.builder.addIndexToSections();
                PageBuilderModel.builder.toggleCloseBtnByEmtyState();
                if (!$(PageBuilderModel.builder.pageItems.selector + ' li').length && $(PageBuilderModel.builder.newItems.selector + '.hidden').length > 0) {
                    $(PageBuilderModel.builder.pageItems.selector).addClass('hidden');
                }
            },

            toggleContainerByEmptyState: function () {
                if ($(PageBuilderModel.builder.pageItems.selector + ' li').length > 0) {
                    $(PageBuilderModel.builder.pageItems.selector).removeClass('hidden');
                } else {
                    $(PageBuilderModel.builder.pageItems.selector).addClass('hidden');
                }
            }
        },

        newItems: {
            selector: '.js-new-items',

            draggable: function () {
                $(PageBuilderModel.builder.newItems.selector + ' li').draggable({
                    connectToSortable: PageBuilderModel.builder.pageItems.selector + ' ul',
                    helper: 'clone',
                    revert: 'invalid',
                    handle: '.handle',
                    placeholder: 'ui-sortable-placeholder'
                });
                $(PageBuilderModel.builder.selector + ' li').disableSelection();
            },

            show: function () {
                $(PageBuilderModel.builder.pageItems.selector).removeClass('hidden');
                $(PageBuilderModel.builder.newItems.selector).removeClass('hidden');
                $(PageBuilderModel.builder.pageItems.selector).addClass('droppable');
                PageBuilderModel.builder.toggleCloseBtnByEmtyState();
            },
            hide: function () {
                $(PageBuilderModel.builder.newItems.selector).addClass('hidden');
                $(PageBuilderModel.builder.pageItems.selector).removeClass('droppable');
                PageBuilderModel.builder.toggleCloseBtnByEmtyState();
                PageBuilderModel.builder.pageItems.toggleContainerByEmptyState();
            }
        },

        addIndexToSections: function () {
            $('.js-page-items li,' + PageBuilderModel.sections.selector).each(function (i, item) {
                $(item).attr('data-index', $(item).index());
            });
        },

        generateItemsByPageSections: function () {
            $(PageBuilderModel.builder.pageItems.selector + ' ul').html('');

            $(PageBuilderModel.sections.selector).each(function (index, item) {
                var sectionType = $(item).attr('data-type');
                var sectionName = $(item).attr('data-section');
                var builderItem = $(PageBuilderModel.builder.newItems.selector + ' [data-item="' + sectionName + '"][data-type="' + sectionType + '"]').clone();

                $(PageBuilderModel.builder.pageItems.selector + ' ul').append(builderItem);

                var builderItemName = $(PageBuilderModel.sections.selector).eq($(builderItem).index()).find('.js-display-name-input').val();
                if ($.trim(builderItemName) != '') {
                    $(builderItem).find('.js-builder-item-name').text(builderItemName);
                }
            });
        },

        toggleCloseBtnByEmtyState: function () {
            if ($(PageBuilderModel.builder.pageItems.selector + ' li').length > 0 && $(PageBuilderModel.builder.newItems.selector + '.hidden').length > 0) {
                $(PageBuilderModel.builder.closeBtnSelector).removeClass('hidden');
            }
            else {

                $(PageBuilderModel.builder.closeBtnSelector).addClass('hidden');
            }
        },

        init: function () {
            //
            PageBuilderModel.builder.generateItemsByPageSections();
            PageBuilderModel.builder.addIndexToSections();
            PageBuilderModel.builder.toggleCloseBtnByEmtyState();
            PageBuilderModel.builder.pageItems.toggleContainerByEmptyState();

            //enable sort and drag
            PageBuilderModel.builder.pageItems.sortable();
            PageBuilderModel.builder.newItems.draggable();

            //toggle builder
            $(PageBuilderModel.builder.pageItems.selector).hover(function () {
                PageBuilderModel.builder.show();
            });
            $(PageBuilderModel.builder.closeBtnSelector).click(function () {
                PageBuilderModel.builder.hide();
            });

            //toggle new items container
            $('.js-show-new-items-container').click(function () {
                PageBuilderModel.builder.newItems.show();
            });
            $('.js-close-new-items-container').click(function () {
                PageBuilderModel.builder.newItems.hide();
            });

            //scrollTo section
            $('.js-page-items').on('click', '.js-builder-item-name', function () {
                var index = $(this).closest('.js-builder-item').attr('data-index');
                var posTop = $('.js-page-section[data-index="' + index + '"]').position().top - parseInt($(PageBuilderModel.currentViewSelector).css('padding-top'));
                $('html,body').animate({ scrollTop: posTop }, 200);
            });

            //delete builder item and page section 
            $('.js-page-items').on('click', '.js-delete-section', function () {
                var index = $(this).closest('.js-builder-item').attr('data-index');
                PageBuilderModel.builder.pageItems.remove(index);
            });
        }
    },

    settings: {
        defaults: {
            name: '',
            id: null,
            index: null,
            type: 1,
            backgroundColor: false,
            additionalClassNames: null,
            contentSizes: function (args = {}) {
                var xxs = {
                    name: 'xxs',
                    exists: args['xxs'] ? args['xxs'].exists : false,
                    value: 'xxs'
                };
                var xs = {
                    name: 'xs',
                    exists: args['xs'] ? args['xs'].exists : true,
                    value: 'xs'
                };
                var sm = {
                    name: 'sm',
                    exists: args['sm'] ? args['sm'].exists : true,
                    value: 'sm'
                };
                var md = {
                    name: 'md',
                    exists: args['md'] ? args['md'].exists : true,
                    value: 'md'
                };
                var lg = {
                    name: 'lg',
                    exists: args['lg'] ? args['lg'].exists : true,
                    value: 'lg'
                };
                var fluid = {
                    name: '100%',
                    exists: args['fluid'] ? args['fluid'].exists : true,
                    value: 'fluid'
                };

                return [xxs, xs, sm, md, lg, fluid];
            },
            contentSizeSelected: 'md',
            contentAlignment: null,
            animations: null,

            displayName: '',
            isScrollToNavItem: false,

            hasCardToggler: false,
            hasReverseToggler: false,
            hasFullScreenToggler: false,

            isCard: false,
            isReversed: false,
            isFullScreen: false,
            isFullHeight: false,

            verticalSpacing: function (args = {}) {
                var none = {
                    name: '0',
                    exists: args['none'] ? args['none'].exists : true,
                    value: 'none'
                };
                var xs = {
                    name: 'xs',
                    exists: args['xs'] ? args['xs'].exists : true,
                    value: 'xs'
                };
                var sm = {
                    name: 'sm',
                    exists: args['sm'] ? args['sm'].exists : true,
                    value: 'sm'
                };
                var md = {
                    name: 'md',
                    exists: args['md'] ? args['md'].exists : true,
                    value: 'md'
                };
                var lg = {
                    name: 'lg',
                    exists: args['lg'] ? args['lg'].exists : true,
                    value: 'lg'
                };
                var xl = {
                    name: 'xl',
                    exists: args['xl'] ? args['xl'].exists : true,
                    value: 'xl'
                };

                return [none, xs, sm, md, lg, xl];
            },
            verticalSpacingSelected: 'md',

            components: null
        },

        components: {
            title: {
                isActive: true,
                isFullfeatured: false,
                html: '<h2>Lorem Ipsum dolor sit Amet!</h2>'
            },

            text: {
                isActive: true,
                isFullfeatured: false,
                html: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum a tempor dolor, ac faucibus ligula. Fusce sit amet magna at elit porttitor fringilla. Praesent lorem purus, lobortis at iaculis non, tincidunt sit amet metus. Quisque ac augue in eros lobortis egestas et in ipsum. Curabitur dapibus ex orci, id semper magna vulputate nec. Curabitur lacinia cursus lobortis. Nullam dolor velit, rutrum at mi non, efficitur ullamcorper leo. Suspendisse non ipsum consectetur, ornare urna vel, lobortis mi. Sed vel nibh nec lacus fringilla varius. Nunc sed bibendum dolor.',
            },

            textPlain: {
                isActive: true,
                additionalClassNames: null,
                html: 'plain text'
            },

            button: {
                isActive: 'true',
                name: 'see more',
                href: '#',
                //target: '_self'
                isTargetBlank: false
            },

            image: {
                isActive: true,
                hasControls: false,
                url: '/plugins/63bits-pageBuilder/images/demo/slide1.jpg'
            },

            icon: {
                isActive: true,
                isImage: false,
                name: 'fal fa-folders',
                url: '',
            },

            fontAwesomeIcon: {
                isActive: true,
                name: 'fal fa-folders'
            },

            video: {
                isActive: true,
                iframe: null,
                url: 'https://www.youtube.com/embed/cVFzblT5VPE?rel=0'
            },

            jwPlayer: {
                isActive: true,

                videoUrl: '/plugins/63bits-pageBuilder/video/demo.mp4',
                coverUrl: '/plugins/63bits-pageBuilder/video/cover.jpg',
                markersUrl: null,
                subTitlesUrl: null,

                isPrivate: false,
                privateUrl: null
            },

            multimedia: {
                isActive: true,
                isImage: true,
                image: {
                    url: '/plugins/63bits-pageBuilder/images/demo/slide1.jpg'
                },
                video: {
                    controls: true,
                    muted: false,
                    loop: false,
                    autoplay: false,
                    url: '/plugins/63bits-pageBuilder/video/demo.mp4',
                }
            },

            iframe: {
                isActive: true,
                url: ''
            },

            pdfViewerIframe: {
                isActive: true,
                url: '',
                downloadEnabled: false
            },

            attachements: [
                { name: '', url: '' }
            ],

            imgGridItem: {
                columnClassesNames: [
                    'col-sm-6 col-lg-3',
                    'col-sm-6 col-lg-4',
                    'col-sm-6'
                ],
                size: 1, //1, 2, 3

                url: null,
                isTargetBlank: false,

                title: {
                    isActive: false,
                    value: 'Name of the block'
                },
                text: {
                    isActive: false,
                    value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer auctor erat'
                },
                button: {
                    isActive: null,
                    name: null,
                    href: null,
                    isTargetBlank: null
                },
                image: {
                    url: '/plugins/63bits-pageBuilder/images/demo/grid_img.jpg'
                },
            },

            articlesGridItem: function () {
                return {
                    components: {
                        title: PageBuilderModel.settings.components.title,
                        text: PageBuilderModel.settings.components.text,
                        button: PageBuilderModel.settings.components.button,
                        image: {
                            ...PageBuilderModel.settings.components.image,
                            hasControls: true
                        }
                    }
                };
            },

            articlesListItem: function () {
                return {
                    components: {
                        title: PageBuilderModel.settings.components.title,
                        text: PageBuilderModel.settings.components.text,
                        button: PageBuilderModel.settings.components.button,
                        image: {
                            ...PageBuilderModel.settings.components.image,
                            hasControls: true
                        }
                    }
                };
            },

            sliderItem: function () {
                return {
                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true,
                            html: '<h3>Lorem Ipsum dolor sit Amet!</h3>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            html: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum a tempor dolor, ac faucibus ligula.'
                        },
                        button: PageBuilderModel.settings.components.button,
                        multimedia: {
                            ...PageBuilderModel.settings.components.multimedia,
                            video: {
                                ...PageBuilderModel.settings.components.multimedia.video,
                                controls: false,
                                muted: true,
                                loop: true,
                                autoplay: true,
                            }
                        }
                    }
                }
            },

            accordionItem: function () {
                return {
                    components: {
                        title: PageBuilderModel.settings.components.title,
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isFullfeatured: true
                        },
                        fontAwesomeIcon: {
                            ...PageBuilderModel.settings.components.fontAwesomeIcon,
                            isActive: false,
                        }
                    }
                }
            },

            tabItem: function () {
                return {
                    id: PageBuilderModel.guid.s15(),
                    name: 'Tab Name',
                    isActive: true,
                    components: {
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isFullfeatured: true,
                        },
                        fontAwesomeIcon: {
                            isActive: false,
                            name: 'far fa-folder'
                        }
                    }
                };
            },

            testimonialsItem: function () {
                return {
                    components: {
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            html: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.Vestibulum a tempor dolor, ac faucibus ligula.Fusce sit amet magna at elit porttitor fringilla.Praesent lorem purus.'
                        },
                        image: {
                            ...PageBuilderModel.settings.components.image,
                            url: '/plugins/63bits-pageBuilder/images/demo/no_avatar.jpg',
                            hasControls: true
                        },
                        author: {
                            ...PageBuilderModel.settings.components.textPlain,
                            additionalClassNames: 'author-wrap js-author-wrap',
                            html: 'Name Surname'
                        },
                        description: {
                            ...PageBuilderModel.settings.components.textPlain,
                            additionalClassNames: 'description-wrap js-description-wrap',
                            html: 'Position'
                        }
                    }
                };
            },

            servicesGridItem: function () {
                return {
                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            html: '<h3>Title</h3>'
                        },
                        subTitle: {
                            ...PageBuilderModel.settings.components.textPlain,
                            isActive: false,
                            html: '$9.99'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            html: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy.'
                        },
                        button: PageBuilderModel.settings.components.button,
                        icon: PageBuilderModel.settings.components.icon,
                        //image: {
                        //    ...PageBuilderModel.settings.components.image,
                        //    url: '/plugins/63bits-pageBuilder/images/demo/services_item.svg',
                        //    hasControls: true
                        //}
                    }
                };
            },

            mediaObjectsGridItem: function () {
                return {
                    columnClassesNames: [
                        'col-sm-6 col-lg-3',
                        'col-sm-6 col-lg-4',
                        'col-sm-6'
                    ],
                    size: 2, //1, 2, 3

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            html: '<h4>Lorem ipsum dolor sit amet</h4>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            html: '<p>Consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.</p>'
                        },
                        icon: PageBuilderModel.settings.components.icon
                    }
                };
            },

            packagesGridItem: function () {
                return {
                    columnClassesNames: [
                        'col-sm-6 col-lg-3',
                        'col-sm-6 col-lg-4',
                        'col-sm-6'
                    ],
                    size: 2, //1, 2, 3

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            html: '<h4>Package Name</h4>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            html: '<p>package option 1</p><p>package option 2</p><p>package option 3</p>'
                        },
                        icon: PageBuilderModel.settings.components.icon
                    }
                };
            },

            flipCardsGridItem: function () {
                return {
                    hasIcon: true,
                    hasImage: false,
                    columnClassesNames: [
                        'col-sm-6 col-lg-3',
                        'col-sm-6 col-lg-4',
                        'col-sm-6'
                    ],
                    size: 2, //1, 2, 3

                    rotateOnClick: false,

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            html: '<h3>Title</h3>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            html: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer auctor erat'
                        },
                        icon: PageBuilderModel.settings.components.icon,
                        image: {
                            ...PageBuilderModel.settings.components.image,
                            hasControls: true,
                            url: '/plugins/63bits-pageBuilder/images/demo/grid_img.jpg'
                        }
                    }
                };
            },

            videoGridItem: function () {
                return {
                    columnClassesNames: [
                        'col-sm-6 col-lg-3',
                        'col-sm-6 col-lg-4',
                        'col-sm-6'
                    ],
                    size: 3, //1, 2, 3

                    components: {
                        video: PageBuilderModel.settings.components.video,
                    },
                };
            },

            jwPlayerGridItem: function () {
                return {
                    columnClassesNames: [
                        'col-sm-6 col-lg-3',
                        'col-sm-6 col-lg-4',
                        'col-sm-6'
                    ],
                    size: 3, //1, 2, 3

                    components: {
                        jwPlayer: PageBuilderModel.settings.components.jwPlayer,
                    },
                };
            },

            postCardsGridItem: function () {
                return {
                    columnClassesNames: [
                        'col-sm-6 col-lg-3',
                        'col-sm-6 col-lg-4',
                        'col-sm-6'
                    ],
                    size: 2, //1, 2, 3

                    components: {
                        title: PageBuilderModel.settings.components.title,
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            html: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum a tempor dolor, ac faucibus ligula.'
                        },
                        image: {
                            ...PageBuilderModel.settings.components.image,
                            hasControls: true
                        }
                    }
                };
            },

            booksGridItem: function () {
                return {
                    columnClassesNames: [
                        'col-sm-6 col-lg-3',
                        'col-sm-6 col-lg-4',
                        'col-sm-6'
                    ],
                    size: 2, //1, 2, 3

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            html: 'Book Title'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            html: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.'
                        },
                        button: PageBuilderModel.settings.components.button,
                        image: {
                            ...PageBuilderModel.settings.components.image,
                            hasControls: true,
                            url: '/plugins/63bits-pageBuilder/images/demo/book_cover.jpg'
                        }
                    }
                };
            },

            peopleGridItem: function () {
                return {
                    columnClassesNames: [
                        'col-sm-6 col-lg-3',
                        'col-sm-6 col-lg-4',
                        'col-sm-6'
                    ],
                    size: 1, //1, 2, 3

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            html: 'Name Surname'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            html: 'Position'
                        },
                        image: {
                            ...PageBuilderModel.settings.components.image,
                            hasControls: true
                        }
                    }
                };
            },

            cardsGridItem: function () {
                return {
                    columnClassesNames: [
                        'col-sm-6 col-lg-3',
                        'col-sm-6 col-lg-4',
                        'col-sm-6'
                    ],
                    size: 2, //1, 2, 3

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            html: '<h3>Card title</h3>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            html: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.'
                        },
                        icon: PageBuilderModel.settings.components.icon
                    }
                };
            },
        },

        sections: {
            slide: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'slide',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    contentAlignment: 'center', //topLeft,topCenter,topRight  centerLeft,center,centerRight  bottomLeft,bottomCenter,bottomRight

                    hasFullScreenToggler: true,
                    isFullScreen: false,
                    isFullHeight: false,

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true,
                            html: '<h3>Lorem Ipsum dolor sit Amet!</h3>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            html: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum a tempor dolor, ac faucibus ligula.'
                        },
                        button: PageBuilderModel.settings.components.button,
                        image: PageBuilderModel.settings.components.image
                    },
                }
            },

            slider: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'slider',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    contentAlignment: 'center', //topLeft,topCenter,topRight  centerLeft,center,centerRight  bottomLeft,bottomCenter,bottomRight

                    hasFullScreenToggler: true,
                    isFullScreen: false,
                    isFullHeight: false,

                    slider: {
                        autoplay: false,
                        autoplaySpeed: 4000
                    },

                    items: [
                        { ...PageBuilderModel.settings.components.sliderItem() },
                        {
                            components: {
                                ...PageBuilderModel.settings.components.sliderItem().components,
                                multimedia: {
                                    ...PageBuilderModel.settings.components.sliderItem().components.multimedia,
                                    isImage: false
                                }
                            }
                        }
                    ]
                }
            },

            article: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'article',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),
                    hasCardToggler: true,

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isFullfeatured: true
                        }
                    }
                };
            },

            article2Col: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'article2Col',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true
                        },
                        text: PageBuilderModel.settings.components.text,
                        button: PageBuilderModel.settings.components.button,

                        title2: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true
                        },
                        text2: PageBuilderModel.settings.components.text,
                        button2: PageBuilderModel.settings.components.button,
                    },
                }
            },

            articleText2Col: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'articleText2Col',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true
                        },

                        text: PageBuilderModel.settings.components.text,
                        button: PageBuilderModel.settings.components.button,

                        text2: PageBuilderModel.settings.components.text,
                        button2: PageBuilderModel.settings.components.button,
                    },
                }
            },

            article2ColWithImg: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'article2ColWithImg',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true
                        },
                        text: PageBuilderModel.settings.components.text,
                        button: PageBuilderModel.settings.components.button,
                        image: PageBuilderModel.settings.components.image,
                    },
                }
            },

            article2ColWithVideo: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'article2ColWithVideo',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true
                        },
                        text: PageBuilderModel.settings.components.text,
                        button: PageBuilderModel.settings.components.button,
                        video: PageBuilderModel.settings.components.video
                    },
                }
            },

            article2ColWithJwPlayer: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'article2ColWithJwPlayer',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true
                        },
                        text: PageBuilderModel.settings.components.text,
                        button: PageBuilderModel.settings.components.button,
                        jwPlayer: PageBuilderModel.settings.components.jwPlayer
                    },
                }
            },

            articleWithFiles: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'articleWithFiles',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isFullfeatured: true
                        },
                        attachements: null
                    },
                }
            },

            articleWithVideo: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'articleWithVideo',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isFullfeatured: true
                        },
                        video: PageBuilderModel.settings.components.video,
                    },
                }
            },

            video: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'video',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        video: PageBuilderModel.settings.components.video,
                    },
                }
            },

            jwPlayer: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'jwPlayer',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        jwPlayer: PageBuilderModel.settings.components.jwPlayer,
                    },
                }
            },

            iframe: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'iframe',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true
                        },
                        iframe: PageBuilderModel.settings.components.iframe,
                    },
                }
            },

            html: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'html',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        text: {
                            isActive: true,
                            isFullfeatured: true,
                            html: 'enter html'
                        },
                    },
                }
            },

            imgGrid: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'imgGrid',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes({
                        xs: { exists: false }
                    }),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true
                        },
                        button: PageBuilderModel.settings.components.button
                    },

                    items: [
                        {
                            ...PageBuilderModel.settings.components.imgGridItem,
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.imgGridItem.columnClassesNames[0]
                        }
                    ]
                }
            },

            articlesGrid: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'articlesGrid',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes({
                        xs: { exists: false }
                    }),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isActive: false,
                            isFullfeatured: true,
                            html: '<h2>Lorem Ipsum</h2>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isActive: false,
                            isFullfeatured: true,
                            html: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry.'
                        }
                    },

                    items: [
                        { ...PageBuilderModel.settings.components.articlesGridItem() }
                    ]
                };
            },

            articlesList: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'articlesList',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes({
                        xs: { exists: false }
                    }),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isActive: false,
                            isFullfeatured: true,
                            html: '<h2>Lorem Ipsum</h2>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isActive: false,
                            isFullfeatured: true,
                            html: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry.'
                        }
                    },

                    items: [
                        { ...PageBuilderModel.settings.components.articlesListItem() }
                    ]
                };
            },

            accordion: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'accordion',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isActive: false,
                            isFullfeatured: true,
                            html: '<h2>Accordion</h2>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isActive: false,
                            isFullfeatured: true,
                            html: 'How can we help?'
                        }
                    },

                    items: [
                        { ...PageBuilderModel.settings.components.accordionItem() }
                    ]
                }
            },

            tabs: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'tabs',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isActive: false,
                            isFullfeatured: true,
                            html: '<h2>Tabs</h2>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isActive: false,
                            isFullfeatured: true,
                            html: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry.'
                        }
                    },

                    items: [
                        {
                            ...PageBuilderModel.settings.components.tabItem(),
                            name: 'Tab Name 1'
                        },
                        {
                            ...PageBuilderModel.settings.components.tabItem(),
                            name: 'Tab Name 2'
                        }
                    ]
                }
            },

            testimonials: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'testimonials',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),
                    contentSizeSelected: 'sm',

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true,
                            html: '<h2>What People Say About Us</h2>',
                        },
                    },

                    items: [
                        { ...PageBuilderModel.settings.components.testimonialsItem() },
                        { ...PageBuilderModel.settings.components.testimonialsItem() }
                    ]
                };
            },

            services: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'services',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),
                    contentSizeSelected: 'md',

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isActive: false,
                            isFullfeatured: true,
                            html: '<h2>Services</h2>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isActive: false,
                            isFullfeatured: true,
                            html: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry.'
                        }
                    },

                    items: [
                        { ...PageBuilderModel.settings.components.servicesGridItem() }
                    ]
                };
            },

            pageHead: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'pageHead',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        title: PageBuilderModel.settings.components.title,
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            html: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh'
                        },
                        button: PageBuilderModel.settings.components.button,
                        image: {
                            ...PageBuilderModel.settings.components.image,
                            url: '/plugins/63bits-pageBuilder/images/demo/car.png'
                        }
                    }
                };
            },

            mediaObject: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'mediaObject',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    hasCardToggler: true,
                    isCard: true,

                    components: {
                        title: PageBuilderModel.settings.components.title,
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isFullfeatured: true
                        },
                        icon: PageBuilderModel.settings.components.icon
                    }
                };
            },

            mediaObjectsGrid: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'mediaObjectsGrid',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes({
                        xs: { exists: false }
                    }),

                    contentAlignment: 'centerLeft', //centerLeft,center,centerRight

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isActive: false,
                            isFullfeatured: true,
                            html: '<h2>Lorem Ipsum</h2>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isActive: false,
                            isFullfeatured: true,
                            html: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry.'
                        }
                    },

                    items: [
                        {
                            ...PageBuilderModel.settings.components.mediaObjectsGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.mediaObjectsGridItem().columnClassesNames[1]
                        }
                    ]
                };
            },

            card2Col: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'card2Col',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true
                        },
                        text: PageBuilderModel.settings.components.text
                    },
                }
            },

            flipCardsGrid: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'flipCardsGrid',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes({
                        xs: { exists: false }
                    }),

                    contentAlignment: 'centerLeft', //centerLeft,center,centerRight

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isActive: false,
                            isFullfeatured: true,
                            html: '<h2>Lorem Ipsum</h2>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isActive: false,
                            isFullfeatured: true,
                            html: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry.'
                        }
                    },

                    items: [
                        {
                            ...PageBuilderModel.settings.components.flipCardsGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.flipCardsGridItem().columnClassesNames[1]
                        }
                    ]
                };
            },

            packagesGrid: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'packagesGrid',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes({
                        xs: { exists: false }
                    }),

                    contentAlignment: 'centerLeft', //centerLeft,center,centerRight

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isActive: false,
                            isFullfeatured: true,
                            html: '<h2>Lorem Ipsum</h2>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isActive: false,
                            isFullfeatured: true,
                            html: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry.'
                        }
                    },

                    items: [
                        {
                            ...PageBuilderModel.settings.components.packagesGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.packagesGridItem().columnClassesNames[1]
                        }
                    ]
                };
            },

            quote: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'quote',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isFullfeatured: true,
                            html:
                                `<blockquote>
                                    <h3 style="text-align: center;">Learning programs are comprised of a mix of online courses, online discussions, webinars</h3>
                                    <footer><p style="text-align: center;">Name Surname</p></footer>
                                </blockquote>`
                        },
                        fontAwesomeIcon: {
                            ...PageBuilderModel.settings.components.fontAwesomeIcon,
                            isActive: false,
                            name: 'fas fa-quote-left'
                        }
                    }
                };
            },

            article2colWithBgImgAndImg: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'article2colWithBgImgAndImg',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isFullfeatured: true
                        },
                        text: PageBuilderModel.settings.components.text,
                        bgImage: {
                            ...PageBuilderModel.settings.components.image,
                            url: '/plugins/63bits-pageBuilder/images/demo/signing_of_declaration.jpg'
                        },
                        image: {
                            ...PageBuilderModel.settings.components.image,
                            url: '/plugins/63bits-pageBuilder/images/demo/john_adams.jpg'
                        }
                    },
                }
            },

            pdfViewer: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'pdfViewer',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        pdfViewerIframe: PageBuilderModel.settings.components.pdfViewerIframe
                    },
                }
            },

            videoGrid: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'videoGrid',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes({
                        xs: { exists: false }
                    }),

                    items: [
                        {
                            ...PageBuilderModel.settings.components.videoGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.videoGridItem().columnClassesNames[2]
                        }
                    ]
                }
            },

            jwPlayerGrid: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'jwPlayerGrid',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes({
                        xs: { exists: false }
                    }),

                    items: [
                        {
                            ...PageBuilderModel.settings.components.jwPlayerGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.jwPlayerGridItem().columnClassesNames[2]
                        }
                    ]
                }
            },

            postCardsGrid: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'postCardsGrid',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes({
                        xs: { exists: false }
                    }),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isActive: false,
                            isFullfeatured: true,
                            html: '<h2>Lorem Ipsum</h2>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isActive: false,
                            isFullfeatured: true,
                            html: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry.'
                        }
                    },

                    contentAlignment: 'centerLeft', //centerLeft,center,centerRight

                    items: [
                        {
                            ...PageBuilderModel.settings.components.postCardsGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.packagesGridItem().columnClassesNames[1]
                        }
                    ]
                };
            },

            booksGrid: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'booksGrid',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes({
                        xs: { exists: false }
                    }),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isActive: false,
                            isFullfeatured: true,
                            html: '<h2>Lorem Ipsum</h2>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isActive: false,
                            isFullfeatured: true,
                            html: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry.'
                        }
                    },

                    items: [
                        {
                            ...PageBuilderModel.settings.components.booksGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.booksGridItem().columnClassesNames[1]
                        }
                    ]
                };
            },

            peopleGrid: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'peopleGrid',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes({
                        xs: { exists: false }
                    }),

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isActive: false,
                            isFullfeatured: true,
                            html: '<h2>Lorem Ipsum</h2>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isActive: false,
                            isFullfeatured: true,
                            html: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry.'
                        }
                    },

                    contentAlignment: 'centerLeft', //centerLeft,center,centerRight

                    items: [
                        {
                            ...PageBuilderModel.settings.components.peopleGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.peopleGridItem().columnClassesNames[0]
                        }
                    ]
                };
            },

            cardsGrid: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'cardsGrid',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes({
                        xs: { exists: false }
                    }),

                    contentAlignment: 'centerLeft', //centerLeft,center,centerRight

                    components: {
                        title: {
                            ...PageBuilderModel.settings.components.title,
                            isActive: false,
                            isFullfeatured: true,
                            html: '<h2>Lorem Ipsum</h2>'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            isActive: false,
                            isFullfeatured: true,
                            html: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry.'
                        }
                    },

                    items: [
                        {
                            ...PageBuilderModel.settings.components.cardsGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.cardsGridItem().columnClassesNames[1]
                        }
                    ]
                };
            },
        },
    },

    data: {
        default: function () {
            var sections = PageBuilderModel.settings.sections;
            return [
                {
                    ...sections.slide()
                },
                {
                    ...sections.slider()
                },
                {
                    ...sections.article()
                },
                {
                    ...sections.article2Col()
                },
                {
                    ...sections.articleText2Col()
                },
                {
                    ...sections.article2ColWithImg(),

                    //for demo
                    components: {
                        ...sections.article2ColWithImg().components,
                        image: {
                            ...sections.article2ColWithImg().components.image,
                            url: '/plugins/63bits-pageBuilder/images/demo/01.jpg'
                        },
                    },
                },
                {
                    ...sections.article2ColWithImg(),
                    type: 2,
                    additionalClassNames: 'has-sm-img',

                    //for demo
                    components: {
                        ...sections.article2ColWithImg().components,
                        image: {
                            ...sections.article2ColWithImg().components.image,
                            url: '/plugins/63bits-pageBuilder/images/demo/03.png'
                        },
                    },
                },
                /*
                {
                    ...sections.article2ColWithImg(),
                    type: 3,
                    additionalClassNames: 'has-sm-img',

                    //for demo
                    components: {
                        ...sections.article2ColWithImg().components,
                        image: {
                            ...sections.article2ColWithImg().components.image,
                            url: '/plugins/63bits-pageBuilder/images/demo/03.png'
                        },
                    },
                },
                {
                    ...sections.article2ColWithImg(),
                    type: 4,
                    additionalClassNames: 'flex-row-reverse has-sm-img',

                    //for demo
                    components: {
                        ...sections.article2ColWithImg().components,
                        image: {
                            ...sections.article2ColWithImg().components.image,
                            url: '/plugins/63bits-pageBuilder/images/demo/04.png'
                        },
                    },
                },*/
                {
                    ...sections.article2ColWithVideo(),
                },
                {
                    ...sections.article2ColWithJwPlayer(),
                },
                {
                    ...sections.articleWithFiles()
                },
                {
                    ...sections.articleWithVideo()
                },
                {
                    ...sections.video()
                },
                {
                    ...sections.jwPlayer()
                },
                {
                    ...sections.iframe()
                },
                {
                    ...sections.html()
                },
                {
                    ...sections.imgGrid()
                },
                {
                    ...sections.articlesGrid()
                },
                {
                    ...sections.articlesList()
                },
                {
                    ...sections.accordion()
                },
                {
                    ...sections.tabs()
                },
                {
                    ...sections.testimonials()
                },
                {
                    ...sections.services()
                },
                {
                    ...sections.pageHead()
                },
                {
                    ...sections.mediaObject()
                },
                {
                    ...sections.mediaObjectsGrid()
                },
                {
                    ...sections.card2Col()
                },
                {
                    ...sections.packagesGrid()
                },
                {
                    ...sections.flipCardsGrid()
                },
                {
                    ...sections.quote()
                },
                {
                    ...sections.article2colWithBgImgAndImg()
                },
                {
                    ...sections.pdfViewer()
                },
                {
                    ...sections.videoGrid()
                },
                {
                    ...sections.jwPlayerGrid()
                },
                {
                    ...sections.postCardsGrid()
                },
                {
                    ...sections.booksGrid()
                },
                {
                    ...sections.peopleGrid()
                },
                {
                    ...sections.cardsGrid()
                },
            ];
        },

        fromServer: null,

        getFromPage: function () {
            var components = {
                title: function (container) {
                    return {
                        isActive: $(container).find('.js-title-visibility-toggler input:checked').length > 0,
                        isFullfeatured: $(container).find('.js-title.js-tinymce-full').length > 0,
                        html: $(container).find('.js-title').html().replace(/"/g, '&quot;'),
                    };
                },
                text: function (container) {
                    return {
                        isActive: $(container).find('.js-text-visibility-toggler input:checked').length > 0,
                        isFullfeatured: $(container).find('.js-tinymce-full').length > 0,
                        html: $(container).find('.js-text-wrap').html().replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/>/g, '&gt;'),
                    };
                },
                textPlain: function (container) {
                    return {
                        isActive: $(container).find('.js-plain-text-visibility-toggler input:checked').length > 0,
                        additionalClassNames: container.attr('data-additional-classes') || null,
                        html: $(container).find('.js-plain-text').html().replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/>/g, '&gt;'),
                    };
                },
                button: function (container) {
                    return {
                        isActive: $(container).find('.js-btn-visibility-toggler input:checked').length > 0,
                        name: $(container).find('.js-btn').text().replace(/"/g, '&quot;'),
                        href: $(container).find('.js-btn').attr('href'),
                        isTargetBlank: $(container).find('.js-btn-target-checkbox:checked').length > 0
                    };
                },
                icon: function (container) {
                    return {
                        isActive: $(container).find('.js-icon-visibility-toggler input:checked').length > 0,
                        isImage: $(container).find('.js-icon-type-checkbox:checked').length > 0,
                        name: $(container).find('.js-font-icon').attr('data-icon-classes'),
                        url: $(container).find('.js-img-icon').attr('data-icon-url'),
                    };
                },
                fontAwesomeIcon: function (container) {
                    return {
                        isActive: $(container).find('.js-fontawesome-visibility-toggler input:checked').length > 0,
                        name: $(container).find('.js-fa-icon').attr('data-fontawesome-icon')
                    };
                },
                image: function (container) {
                    return {
                        isActive: $(container).find('.js-image-visibility-toggler').length > 0 ? $(container).find('.js-image-visibility-toggler input').is(':checked') : true,
                        url: $(container).find('.js-img').attr('data-img-src')
                    };
                },
                video: function (container) {
                    return {
                        url: $(container).find('.js-iframe-container iframe').attr('src')
                    };
                },
                jwPlayer: function (container) {
                    return {
                        videoUrl: $(container).find('.js-jwPlayer-wrap').attr('data-video-url'),
                        coverUrl: $(container).find('.js-jwPlayer-wrap').attr('data-cover-url'),
                        markersUrl: $(container).find('.js-jwPlayer-wrap').attr('data-markers-url'),
                        subTitlesUrl: $(container).find('.js-jwPlayer-wrap').attr('data-subtitles-url'),

                        isPrivate: false,
                        privateUrl: null
                    };
                },
                multimedia: function (container) {
                    return {
                        isActive: true,
                        isImage: $(container).find('.js-multimedia-container').attr('data-multimedia-type') == 'image',
                        image: {
                            url: $(container).find('.js-multimedia-image').attr('data-image-src')
                        },
                        video: {
                            controls: $(container).find('.js-multimedia-video').attr('data-controls') == 'true',
                            muted: $(container).find('.js-multimedia-video').attr('data-muted') == 'true',
                            loop: $(container).find('.js-multimedia-video').attr('data-loop') == 'true',
                            autoplay: $(container).find('.js-multimedia-video').attr('data-autoplay') == 'true',
                            url: $(container).find('.js-multimedia-video').attr('data-video-src')
                        }
                    };
                },
                iframe: function (container) {
                    return {
                        url: $(container).find('.js-iframe-container iframe').attr('src')
                    };
                },
                pdfViewerIframe: function (container) {
                    return {
                        isActive: true,
                        url: $(container).find('.js-pdfViewerIframe-container iframe').attr('data-pdfviewer-url'),
                        urlPdf: $(container).find('.js-pdfViewerIframe-container iframe').attr('data-pdf-url'),
                        downloadEnabled: $(container).find('.js-pdfViewerIframe-state-toggler input:checked').length > 0
                    };
                },
            };

            var data = {
                sections: [],
                pageScripts: {
                    headerSection: [],
                    footerSection: []
                }
            };

            //--- sections
            $(PageBuilderModel.sections.selector).each(function () {
                var section = $(this);
                var sectionName = section.attr('data-section');

                var leftCol = section.find('.js-left-col');
                var rightCol = section.find('.js-right-col');

                var model = {
                    name: section.attr('data-section'),
                    id: section.attr('data-id'),
                    index: section.attr('data-index'),
                    type: section.attr('data-type'),
                    backgroundColor: +section.find('.js-background-color-toggler input:checked').length > 0,
                    additionalClassNames: section.attr('data-additional-classes') || null,
                    contentSizes: PageBuilderModel.settings.sections[sectionName]().contentSizes,
                    contentSizeSelected: section.attr('data-content-size') || null,
                    animations: section.find('.js-section-animation-combo').val() == 'false' ? false : section.find('.js-section-animation-combo').val(),

                    displayName: section.attr('data-display-name'),
                    isScrollToNavItem: section.attr('data-isScrollToNavItem') === 'true',

                    isCard: section.attr('data-is-card') === 'true',
                    isReversed: section.attr('data-reverse') === 'true',
                    isFullScreen: section.attr('data-is-fullscreen') === 'true',
                    isFullHeight: section.attr('data-is-fullheight') === 'true',

                    verticalSpacingSelected: section.attr('data-spacing-v') || 'md',
                };

                if (sectionName == PageBuilderModel.settings.sections.slide().name) {
                    model.contentAlignment = section.find('[data-align-content]').attr('data-align-content');

                    model.components = {
                        title: components.title(section),
                        text: components.text(section),
                        button: components.button(section),
                        image: components.image(section)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.slider().name) {

                    model.slider = {
                        //autoplay: section.find('.js-t63-slider').attr('data-slider-autoplay') == 'true',
                        //autoplaySpeed: section.find('.js-t63-slider').attr('data-slider-autoplay-speed'),

                        autoplay: section.find('.js-slider-autoplay-toggler input:checked').length > 0,
                        autoplaySpeed: $.trim(section.find('.js-slider-autoplay-speed-input').val()) || 4000
                    }

                    model.contentAlignment = section.find('[data-align-content]').attr('data-align-content');
                    model.items = [];

                    section.find('.js-t63-slider > div').each(function () {
                        var slide = $(this);
                        var item = {
                            components: {
                                title: components.title(slide),
                                text: components.text(slide),
                                button: components.button(slide),
                                multimedia: components.multimedia(slide)
                            }
                        }
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.article().name) {
                    model.components = {
                        title: components.title(section),
                        text: components.text(section)
                    };
                }

                if (sectionName == PageBuilderModel.settings.sections.article2Col().name) {
                    model.components = {
                        title: components.title(leftCol),
                        text: components.text(leftCol),
                        button: components.button(leftCol),

                        title2: components.title(rightCol),
                        text2: components.text(rightCol),
                        button2: components.button(rightCol)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.articleText2Col().name) {
                    model.components = {
                        title: components.title(section),
                        text: components.text(leftCol),
                        button: components.button(leftCol),

                        text2: components.text(rightCol),
                        button2: components.button(rightCol)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.article2ColWithImg().name) {
                    model.components = {
                        title: components.title(section),
                        text: components.text(section),
                        button: components.button(section),
                        image: components.image(section)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.article2ColWithVideo().name) {
                    model.components = {
                        title: components.title(section),
                        text: components.text(section),
                        button: components.button(section),
                        video: components.video(section)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.article2ColWithJwPlayer().name) {
                    model.components = {
                        title: components.title(section),
                        text: components.text(section),
                        button: components.button(section),
                        jwPlayer: components.jwPlayer(section)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.articleWithFiles().name) {
                    model.components = {
                        title: components.title(section),
                        text: components.text(section),
                        attachements: []
                    }

                    section.find('.js-files-container li').each(function () {
                        var file = {
                            name: $(this).find('a').text(),
                            url: $(this).find('a').attr('href'),
                        }
                        model.components.attachements.push(file);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.articleWithVideo().name) {
                    model.components = {
                        title: components.title(section),
                        text: components.text(section),
                        video: components.video(section)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.video().name) {
                    model.components = {
                        video: components.video(section)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.jwPlayer().name) {
                    model.components = {
                        jwPlayer: components.jwPlayer(section)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.iframe().name) {
                    model.components = {
                        title: components.title(section),
                        iframe: components.iframe(section)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.imgGrid().name) {
                    model.components = {
                        title: components.title(section),
                        button: components.button(section)
                    }

                    model.items = [];

                    section.find('.js-img-grid-item').each(function () {
                        var gridItem = $(this);
                        var item = {
                            id: gridItem.attr('data-id'),
                            size: gridItem.attr('data-size'),
                            columnClassesNames: PageBuilderModel.settings.components.imgGridItem.columnClassesNames[+gridItem.attr('data-size') - 1],
                            url: $(gridItem).attr('href') || null,
                            isTargetBlank: $(gridItem).find('.js-target-checkbox:checked').length > 0,
                            title: {
                                isActive: $(gridItem).find('.js-title-toggler input:checked').length > 0,
                                value: $(gridItem).find('.js-title').html().replace(/"/g, '&quot;'),
                            },
                            text: {
                                isActive: $(gridItem).find('.js-text-toggler input:checked').length > 0,
                                value: $(gridItem).find('.js-text').html().replace(/"/g, '&quot;'),
                            },
                            button: {
                                isActive: null,
                                name: null,
                                href: null,
                                isTargetBlank: null
                            },
                            image: components.image(gridItem)
                        };
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.articlesGrid().name) {
                    model.components = {
                        title: components.title(section.find('.js-section-title-container')),
                        text: components.text(section.find('.js-section-text-container'))
                    };

                    model.items = [];

                    section.find('.js-articles-grid-item').each(function () {
                        var gridItem = $(this);
                        var item = {
                            components: {
                                title: components.title(gridItem),
                                text: components.text(gridItem),
                                button: components.button(gridItem),
                                image: {
                                    ...components.image(gridItem),
                                    hasControls: PageBuilderModel.settings.components.articlesGridItem().components.image.hasControls
                                }
                            }
                        }
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.articlesList().name) {
                    model.components = {
                        title: components.title(section.find('.js-section-title-container')),
                        text: components.text(section.find('.js-section-text-container'))
                    };

                    model.items = [];

                    section.find('.js-articles-list-item').each(function () {
                        var listItem = $(this);
                        var item = {
                            components: {
                                title: components.title(listItem),
                                text: components.text(listItem),
                                button: components.button(listItem),
                                image: {
                                    ...components.image(listItem),
                                    hasControls: PageBuilderModel.settings.components.articlesListItem().components.image.hasControls
                                }
                            }
                        };
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.accordion().name) {
                    model.components = {
                        title: components.title(section.find('.js-section-title-container')),
                        text: components.text(section.find('.js-section-text-container'))
                    };

                    model.items = [];

                    section.find('.js-t63-accordion-item').each(function () {
                        var gridItem = $(this);
                        var item = {
                            components: {
                                title: components.title(gridItem),
                                text: components.text(gridItem),
                                fontAwesomeIcon: components.fontAwesomeIcon(section)
                            }
                        };
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.tabs().name) {
                    model.components = {
                        title: components.title(section.find('.js-section-title-container')),
                        text: components.text(section.find('.js-section-text-container'))
                    };

                    model.items = [];

                    section.find('.js-tab-nav-item').each(function () {
                        var tabNavItem = $(this);
                        var id = tabNavItem.children('a').attr('href').slice(1);
                        var tabContentItem = $('.js-tab-content-item[id="' + id + '"]');

                        var item = {
                            id: id,
                            isActive: $(tabNavItem).find('.js-tab-visibility-toggler input:checked').length > 0,
                            name: tabNavItem.find('.js-tab-item-name').html().replace(/"/g, '&quot;'),
                            components: {
                                text: components.text(tabContentItem),
                                fontAwesomeIcon: {
                                    isActive: $(tabNavItem).find('.js-faIcon-toggler input:checked').length > 0,
                                    name: $(tabNavItem).find('.js-tab-item-icon-wrap i').attr('class')
                                }
                            }
                        };
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.html().name) {
                    model.components = {
                        text: {
                            isActive: true,
                            isFullfeatured: false,
                            html: section.find('.js-plain-html-input').val().replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
                        },
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.testimonials().name) {
                    model.components = {
                        title: components.title(section)
                    };

                    model.items = [];

                    section.find('.js-t63-testimonials-slider > div').each(function () {
                        var slide = $(this);
                        var item = {
                            components: {
                                text: components.text(slide),
                                image: {
                                    ...components.image(slide),
                                    hasControls: PageBuilderModel.settings.components.testimonialsItem().components.image.hasControls
                                },
                                author: components.textPlain(slide.find('.js-author-wrap')),
                                description: components.textPlain(slide.find('.js-description-wrap'))
                            }
                        };

                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.services().name) {
                    model.components = {
                        title: components.title(section.find('.js-section-title-container')),
                        text: components.text(section.find('.js-section-text-container'))
                    };

                    model.items = [];

                    section.find('.js-t63-services-grid-item').each(function () {
                        var gridItem = $(this);
                        var item = {
                            components: {
                                title: components.title(gridItem),
                                subTitle: components.textPlain(gridItem),
                                text: components.text(gridItem),
                                button: components.button(gridItem),
                                icon: components.icon(gridItem),
                                image: null
                                //image: {
                                //    ...components.image(gridItem),
                                //    hasControls: PageBuilderModel.settings.components.servicesGridItem().components.image.hasControls
                                //}
                            }
                        };

                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.pageHead().name) {
                    model.components = {
                        title: components.title(section),
                        text: components.text(section),
                        button: components.button(section),

                        image: components.image(section)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.mediaObject().name) {
                    model.components = {
                        title: components.title(section),
                        text: components.text(section),
                        icon: components.icon(section)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.mediaObjectsGrid().name) {
                    model.components = {
                        title: components.title(section.find('.js-section-title-container')),
                        text: components.text(section.find('.js-section-text-container'))
                    };

                    model.contentAlignment = section.find('[data-align-content]').attr('data-align-content');
                    model.items = [];

                    section.find('.js-media-objects-grid-item').each(function () {
                        var gridItem = $(this);
                        var item = {
                            id: gridItem.attr('data-id'),
                            size: gridItem.attr('data-size'),
                            columnClassesNames: PageBuilderModel.settings.components.packagesGridItem().columnClassesNames[+gridItem.attr('data-size') - 1],
                            components: {
                                title: components.title(gridItem),
                                text: components.text(gridItem),
                                icon: components.icon(gridItem)
                            }
                        }
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.card2Col().name) {
                    model.components = {
                        title: components.title(section),
                        text: components.text(section)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.packagesGrid().name) {
                    model.components = {
                        title: components.title(section.find('.js-section-title-container')),
                        text: components.text(section.find('.js-section-text-container'))
                    };

                    model.contentAlignment = section.find('[data-align-content]').attr('data-align-content');
                    model.items = [];

                    section.find('.js-packages-grid-item').each(function () {
                        var gridItem = $(this);
                        var item = {
                            id: gridItem.attr('data-id'),
                            size: gridItem.attr('data-size'),
                            columnClassesNames: PageBuilderModel.settings.components.packagesGridItem().columnClassesNames[+gridItem.attr('data-size') - 1],
                            components: {
                                title: components.title(gridItem),
                                text: components.text(gridItem),
                                icon: components.icon(gridItem)
                            }
                        }
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.quote().name) {
                    model.components = {
                        text: components.text(section),
                        fontAwesomeIcon: components.fontAwesomeIcon(section)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.flipCardsGrid().name) {
                    model.components = {
                        title: components.title(section.find('.js-section-title-container')),
                        text: components.text(section.find('.js-section-text-container'))
                    };

                    model.contentAlignment = section.find('[data-align-content]').attr('data-align-content');
                    model.items = [];

                    section.find('.js-flip-cards-grid-item').each(function () {
                        var gridItem = $(this);

                        var item = {
                            id: gridItem.attr('data-id'),
                            hasIcon: $(gridItem).find('.js-media-container').attr('data-media-type') === 'icon',
                            hasImage: $(gridItem).find('.js-media-container').attr('data-media-type') === 'image',
                            size: gridItem.attr('data-size'),
                            columnClassesNames: PageBuilderModel.settings.components.flipCardsGridItem().columnClassesNames[+gridItem.attr('data-size') - 1],

                            clickOnRotate: $(gridItem).attr('data-clickable') || false,

                            components: {
                                title: components.title(gridItem),
                                text: components.text(gridItem),
                                icon: components.icon(gridItem),
                                image: {
                                    ...components.image(gridItem),
                                    hasControls: PageBuilderModel.settings.components.flipCardsGridItem().components.image.hasControls
                                }
                            }
                        };
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.article2colWithBgImgAndImg().name) {
                    model.components = {
                        title: components.title(section),
                        text: components.text(section),
                        bgImage: components.image(leftCol),
                        image: components.image(rightCol)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.pdfViewer().name) {
                    model.components = {
                        pdfViewerIframe: components.pdfViewerIframe(section)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.videoGrid().name) {
                    model.items = [];

                    section.find('.js-video-grid-item').each(function () {
                        var gridItem = $(this);
                        var item = {
                            id: gridItem.attr('data-id'),
                            size: gridItem.attr('data-size'),
                            columnClassesNames: PageBuilderModel.settings.components.videoGridItem().columnClassesNames[+gridItem.attr('data-size') - 1],

                            components: {
                                video: components.video(gridItem)
                            }
                        };
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.jwPlayerGrid().name) {
                    model.items = [];

                    section.find('.js-jwp-grid-item').each(function () {
                        var gridItem = $(this);
                        var item = {
                            id: gridItem.attr('data-id'),
                            size: gridItem.attr('data-size'),
                            columnClassesNames: PageBuilderModel.settings.components.jwPlayerGridItem().columnClassesNames[+gridItem.attr('data-size') - 1],

                            components: {
                                jwPlayer: components.jwPlayer(gridItem)
                            }
                        };
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.postCardsGrid().name) {
                    model.components = {
                        title: components.title(section.find('.js-section-title-container')),
                        text: components.text(section.find('.js-section-text-container'))
                    };

                    model.contentAlignment = section.find('[data-align-content]').attr('data-align-content');
                    model.items = [];

                    section.find('.js-post-cards-grid-item').each(function () {
                        var gridItem = $(this);
                        var item = {
                            id: gridItem.attr('data-id'),
                            size: gridItem.attr('data-size'),
                            columnClassesNames: PageBuilderModel.settings.components.postCardsGridItem().columnClassesNames[+gridItem.attr('data-size') - 1],
                            components: {
                                title: components.title(gridItem),
                                text: components.text(gridItem),
                                image: {
                                    ...components.image(gridItem),
                                    hasControls: PageBuilderModel.settings.components.postCardsGridItem().components.image.hasControls
                                }
                            }
                        }
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.booksGrid().name) {
                    model.components = {
                        title: components.title(section.find('.js-section-title-container')),
                        text: components.text(section.find('.js-section-text-container'))
                    };

                    model.contentAlignment = section.find('[data-align-content]').attr('data-align-content');
                    model.items = [];

                    section.find('.js-books-grid-item').each(function () {
                        var gridItem = $(this);
                        var item = {
                            id: gridItem.attr('data-id'),
                            size: gridItem.attr('data-size'),
                            columnClassesNames: PageBuilderModel.settings.components.booksGridItem().columnClassesNames[+gridItem.attr('data-size') - 1],
                            components: {
                                title: components.title(gridItem),
                                text: components.text(gridItem),
                                image: {
                                    ...components.image(gridItem),
                                    hasControls: PageBuilderModel.settings.components.booksGridItem().components.image.hasControls
                                },
                                button: components.button(gridItem),
                            }
                        }
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.peopleGrid().name) {
                    model.components = {
                        title: components.title(section.find('.js-section-title-container')),
                        text: components.text(section.find('.js-section-text-container'))
                    };

                    model.contentAlignment = section.find('[data-align-content]').attr('data-align-content');
                    model.items = [];

                    section.find('.js-people-grid-item').each(function () {
                        var gridItem = $(this);
                        var item = {
                            id: gridItem.attr('data-id'),
                            size: gridItem.attr('data-size'),
                            columnClassesNames: PageBuilderModel.settings.components.peopleGridItem().columnClassesNames[+gridItem.attr('data-size') - 1],
                            components: {
                                title: components.title(gridItem),
                                text: components.text(gridItem),
                                image: {
                                    ...components.image(gridItem),
                                    hasControls: PageBuilderModel.settings.components.peopleGridItem().components.image.hasControls
                                }
                            }
                        }
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.cardsGrid().name) {
                    model.components = {
                        title: components.title(section.find('.js-section-title-container')),
                        text: components.text(section.find('.js-section-text-container'))
                    };

                    model.contentAlignment = section.find('[data-align-content]').attr('data-align-content');
                    model.items = [];

                    section.find('.js-cards-grid-item').each(function () {
                        var gridItem = $(this);
                        var item = {
                            id: gridItem.attr('data-id'),
                            size: gridItem.attr('data-size'),
                            columnClassesNames: PageBuilderModel.settings.components.cardsGridItem().columnClassesNames[+gridItem.attr('data-size') - 1],
                            components: {
                                title: components.title(gridItem),
                                text: components.text(gridItem),
                                icon: components.icon(gridItem)
                            }
                        }
                        model.items.push(item);
                    });
                }


                data.sections.push(model);
            });

            //--- page scripts
            $('.js-css-files-container li').each(function (inedx, item) {
                var model = {
                    url: $(item).attr('data-url'),
                    name: $(item).attr('data-name'),
                    extension: $(item).attr('data-extension')
                }
                data.pageScripts.headerSection.push(model);
            });
            $('.js-js-files-container li').each(function (inedx, item) {
                var model = {
                    url: $(item).attr('data-url'),
                    name: $(item).attr('data-name'),
                    extension: $(item).attr('data-extension')
                }
                data.pageScripts.footerSection.push(model);
            });

            return JSON.stringify(data);
        }
    },

    scrollToNavigation: {
        selector: '.js-t63-scrollto-nav',

        container: {
            template:
                `<div class="t63-section t63-scrollto-nav js-t63-scrollto-nav">
                    <div class="container"><ul class="js-t63-scrollto-nav-wrap"></ul></div>
                </div>`,

            getHtml: function () {
                return PageBuilderModel.scrollToNavigation.container.template();
            }
        },

        items: {
            template: '<li class="js-t63-scrollto-nav-item"><a href="#{{hash}}">{{name}}</a></li>',

            getHtml: function (model) {
                return PageBuilderModel.scrollToNavigation.items.template(model);
            },

            generateByPageSections: function () {
                $(PageBuilderModel.currentViewSelector + ' [data-isScrolltoNavItem="true"]').each(function (index, item) {
                    //
                    var model = {
                        name: $(item).attr('data-display-name'),
                        hash: $(item).attr('data-id')
                    }
                    $(PageBuilderModel.scrollToNavigation.selector + ' .js-t63-scrollto-nav-wrap').append(PageBuilderModel.scrollToNavigation.items.getHtml(model));
                });
            },

            addClickEvent: function () {
                $('.js-t63-scrollto-nav-item a').click(function () {
                    var href = $(this).attr('href').slice(1);
                    var section = $(PageBuilderModel.currentViewSelector).find('[data-id="' + href + '"]');
                    var posTop = section.position().top - parseInt($(PageBuilderModel.currentViewSelector).css('padding-top'));
                    $('html,body').animate({ scrollTop: posTop }, 200);
                    return false;
                });
            },

            remove: function () {
                $(PageBuilderModel.scrollToNavigation.selector + ' .js-t63-scrollto-nav-wrap').html('');
            }
        },

        addToPage: function () {
            $(PageBuilderModel.currentViewSelector).append(PageBuilderModel.scrollToNavigation.container.getHtml());
        },

        remove: function () {
            $(PageBuilderModel.scrollToNavigation.selector).remove();
        },

        init: function () {
            if ($(PageBuilderModel.currentViewSelector + ' [data-isScrolltoNavItem="true"]').length > 0) {
                if ($(PageBuilderModel.scrollToNavigation.selector).length == 0) {
                    PageBuilderModel.scrollToNavigation.addToPage();
                    $(PageBuilderModel.currentViewSelector).addClass('has-scrollto-nav');
                }
                else {
                    PageBuilderModel.scrollToNavigation.items.remove();
                }
                PageBuilderModel.scrollToNavigation.items.generateByPageSections();
                PageBuilderModel.scrollToNavigation.items.addClickEvent();
            }
            else {
                if ($(PageBuilderModel.scrollToNavigation.selector).length > 0) {
                    PageBuilderModel.scrollToNavigation.remove();
                    $(PageBuilderModel.currentViewSelector).removeClass('has-scrollto-nav');
                }
            }
        }
    },

    sections: {
        selector: '.js-page-section',

        //-- components
        components: {
            title: {
                partial:
                    `<div class="section-row title-container js-section--container{{#unless components.title.isActive}} component-disabled{{/unless}}{{#if animations}} t63-invisible js-animate{{/if}}" data-container="title" data-animation="{{animations}}">
                        <div class="title js-title js-tinymce {{#if components.title.isFullfeatured}}full-featured js-tinymce-full{{else}}js-tinymce-basic{{/if}}">{{components.title.html}}</div>
                        {{titleControlsHepler components.title}}
                    </div>`,

                template:
                    `<div class="section-row title-container js-section--container{{#unless title.isActive}} component-disabled{{/unless}}{{#if animations}} t63-invisible js-animate{{/if}}" data-container="title" data-animation="{{animations}}">
                        <div class="title js-title js-tinymce {{#if title.isFullfeatured}}full-featured js-tinymce-full{{else}}js-tinymce-basic{{/if}}">{{title.html}}</div>
                        {{titleControlsHepler title}}
                    </div>`,

                helper: function (title, actionButtons, animations) {
                    return PageBuilderModel.sections.components.title.template({ title, actionButtons, animations });
                }
            },

            text: {
                partial:
                    `<div class="section-row text-container js-section--container{{#unless components.text.isActive}} component-disabled{{/unless}}{{#if animations}} t63-invisible js-animate{{/if}}" data-container="text" data-animation="{{animations}}">
                        <div class="text-wrap js-text-wrap js-tinymce {{#if components.text.isFullfeatured}}js-tinymce-full{{else}}js-tinymce-basic{{/if}}">{{components.text.html}}</div>
                        {{textControlsHepler components.text}}
                    </div>`,

                template:
                    `<div class="section-row text-container js-section--container{{#unless text.isActive}} component-disabled{{/unless}}{{/unless}}{{#if animations}} t63-invisible js-animate{{/if}}" data-container="text" data-animation="{{animations}}">
                        <div class="text-wrap js-text-wrap js-tinymce  {{#if text.isFullfeatured}}js-tinymce-full{{else}}js-tinymce-basic{{/if}}">{{text.html}}</div>
                        {{textControlsHepler text}}
                    </div>`,

                helper: function (text, actionButtons, animations) {
                    return PageBuilderModel.sections.components.text.template({ text, actionButtons, animations });
                }
            },

            textPlain: {
                partial:
                    `<div class="section-row plain-text-container js-section--container {{components.textPlain.additionalClassNames}}{{#unless components.textPlain.isActive}} component-disabled{{/unless}}{{#if animations}} t63-invisible js-animate{{/if}}" data-container="textPlain" data-additional-classes="{{components.textPlain.additionalClassNames}}" data-animation="{{animations}}">
						<div class="text js-plain-text">{{components.textPlain.html}}</div>
                        {{textPlainControlsHepler components.textPlain}}
					</div>`,

                template:
                    `<div class="section-row plain-text-container js-section--container {{textPlain.additionalClassNames}}{{#unless textPlain.isActive}} component-disabled{{/unless}}{{#if animations}} t63-invisible js-animate{{/if}}" data-container="textPlain" data-additional-classes="{{textPlain.additionalClassNames}}" data-animation="{{animations}}">
                        <div class="text js-plain-text">{{textPlain.html}}</div>
                        {{textPlainControlsHepler textPlain}}
                    </div>`,

                helper: function (textPlain, actionButtons, animations) {
                    return PageBuilderModel.sections.components.textPlain.template({ textPlain, actionButtons, animations });
                }
            },

            button: {
                partial:
                    `<div class="section-row btn-container js-section--container{{#unless components.button.isActive}} component-disabled{{/unless}}{{#if animations}} t63-invisible js-animate{{/if}}" data-container="button" data-animation="{{animations}}">
                        <a href="{{components.button.href}}" class="btn btn-primary js-btn"{{#if components.button.isTargetBlank}} target="_blank"{{/if}}>{{components.button.name}}</a>
                        {{actionButtonsHepler actionButtons.button}}
						{{buttonControlsHepler components.button}}
                    </div>`,

                template:
                    `<div class="section-row btn-container js-section--container{{#unless button.isActive}} component-disabled{{/unless}}{{#if animations}} t63-invisible js-animate{{/if}}" data-container="button" data-animation="{{animations}}">
                        <a href="{{button.href}}" class="btn btn-primary js-btn"{{#if button.isTargetBlank}} target="_blank"{{/if}}>{{button.name}}</a>
                        {{actionButtonsHepler actionButtons}}
						{{buttonControlsHepler button}}
                    </div>`,

                helper: function (button, actionButtons, animations) {
                    return PageBuilderModel.sections.components.button.template({ button, actionButtons, animations });
                }
            },

            icon: {
                partial:
                    `<div class="section-row t63-icon-container js-section--container{{#unless components.icon.isActive}} component-disabled{{/unless}}{{#if animations}} t63-invisible js-animate{{/if}}" data-container="icon" data-icon-type="{{#if components.icon.isImage}}image{{else}}font{{/if}}" data-animation="{{animations}}">
                        <span class="t63-icon-wrap">
                            <i class="{{components.icon.name}} js-font-icon" data-icon-classes="{{components.icon.name}}"></i>
                            <img class="js-img-icon" src="{{components.icon.url}}" data-icon-url="{{components.icon.url}}" alt="">
                        </span>
                        {{actionButtonsHepler actionButtons.icon }}
                        {{iconControlsHepler components.icon}}
                    </div>`,

                template:
                    `<div class="section-row t63-icon-container js-section--container{{#unless icon.isActive}} component-disabled{{/unless}}{{#if animations}} t63-invisible js-animate{{/if}}" data-container="icon" data-icon-type="{{#if icon.isImage}}image{{else}}font{{/if}}" data-animation="{{animations}}">
                        <span class="t63-icon-wrap">
                            <i class="{{icon.name}} js-font-icon" data-icon-classes="{{icon.name}}"></i>
                            <img class="js-img-icon" src="{{icon.url}}" data-icon-url="{{icon.url}}" alt="">
                        </span>
                        {{actionButtonsHepler actionButtons }}
                        {{iconControlsHepler icon}}
                    </div>`,

                helper: function (icon, actionButtons, animations) {
                    return PageBuilderModel.sections.components.icon.template({ icon, actionButtons, animations });
                }
            },

            fontAwesomeIcon: {
                partial:
                    `<div class="section-row fontawesome-icon-container js-section--container{{#unless components.fontAwesomeIcon.isActive}} component-disabled{{/unless}}" data-container="fontAwesomeIcon">
                        <span class="t63-icon-wrap">
                            <i class="{{components.fontAwesomeIcon.name}} js-fa-icon" data-fontawesome-icon="{{components.fontAwesomeIcon.name}}"></i>
                        </span>
                        {{actionButtonsHepler actionButtons.fontAwesomeIcon }}
                        {{fontAwesomeIconControlsHepler components.fontAwesomeIcon}}
                    </div>`,

                template:
                    `<div class="section-row fontawesome-icon-container js-section--container{{#unless fontAwesomeIcon.isActive}} component-disabled{{/unless}}" data-container="fontAwesomeIcon">
                        <span class="t63-icon-wrap">
                            <i class="{{fontAwesomeIcon.name}} js-fa-icon" data-fontawesome-icon="{{fontAwesomeIcon.name}}"></i>
                        </span>
                        {{actionButtonsHepler actionButtons }}
                        {{fontAwesomeIconControlsHepler fontAwesomeIcon}}
                    </div>`,

                helper: function (fontAwesomeIcon, actionButtons) {
                    return PageBuilderModel.sections.components.fontAwesomeIcon.template({ fontAwesomeIcon, actionButtons });
                }
            },

            image: {
                partial:
                    `<div class="img-container js-section--container{{#unless components.image.isActive}} component-disabled{{/unless}}{{#if animations}} t63-invisible js-animate{{/if}}" data-container="image" data-animation="{{animations}}">
                        <figure class="embed-responsive js-img" data-img-src="{{components.image.url}}">
                            <img class="embed-responsive-item" src="{{components.image.url}}" alt="" />
                        </figure>
                        {{actionButtonsHepler actionButtons.image }}
                        {{#if components.image.hasControls}}
                            {{imageControlsHepler components.image}}
                        {{/if}}
                    </div>`,

                template:
                    `<div class="img-container js-section--container{{#unless image.isActive}} component-disabled{{/unless}}{{#if animations}} t63-invisible js-animate{{/if}}" data-container="image" data-animation="{{animations}}">
                        <figure class="embed-responsive js-img" data-img-src="{{image.url}}"><img class="embed-responsive-item" src="{{image.url}}" alt="" /></figure>
                        {{actionButtonsHepler actionButtons}}
                        {{#if image.hasControls}}
                            {{imageControlsHepler image}}
                        {{/if}}
                    </div>`,

                helper: function (image, actionButtons, animations) {
                    return PageBuilderModel.sections.components.image.template({ image, actionButtons, animations });
                }
            },

            bgImage: {
                partial:
                    `<div class="img-container js-section--container{{#unless components.image.isActive}} component-disabled{{/unless}}{{#if animations}} t63-invisible js-animate{{/if}}" data-container="image" data-animation="{{animations}}">
                        <span class="bg-img js-img" style="background-image: url({{components.image.url}})" data-img-src="{{components.image.url}}"></span>
                        {{actionButtonsHepler actionButtons.image }}
                        {{#if components.image.hasControls}}
                            {{imageControlsHepler components.image}}
                        {{/if}}
                    </div>`,

                template:
                    `<div class="img-container js-section--container{{#unless image.isActive}} component-disabled{{/unless}}{{#if animations}} t63-invisible js-animate{{/if}}" data-container="image" data-animation="{{animations}}">
                        <span class="bg-img js-img" style="background-image: url({{image.url}})" data-img-src="{{image.url}}"></span>
                        {{actionButtonsHepler actionButtons}}
                        {{#if image.hasControls}}
                            {{imageControlsHepler image}}
                        {{/if}}
                    </div>`,

                helper: function (image, actionButtons, animations) {
                    return PageBuilderModel.sections.components.bgImage.template({ image, actionButtons, animations });
                }
            },

            video: {
                partial:
                    `<div class="section-row video-container js-section--container{{#if animations}} t63-invisible js-animate{{/if}}" data-container="video" data-animation="{{animations}}">
                        <div class="embed-responsive embed-responsive-16by9 js-iframe-container">
                            <iframe class="embed-responsive-item" src="{{components.video.url}}" frameborder="0" allow="encrypted-media" allowfullscreen=""></iframe>
                        </div>
                        {{actionButtonsHepler actionButtons.video }}
						{{videoControlsHepler components.video}}
                    </div>`,

                template:
                    `<div class="section-row video-container js-section--container{{#if animations}} t63-invisible js-animate{{/if}}" data-container="video" data-animation="{{animations}}">
                        <div class="embed-responsive embed-responsive-16by9 js-iframe-container">
                            <iframe class="embed-responsive-item" src="{{video.url}}" frameborder="0" allow="encrypted-media" allowfullscreen=""></iframe>
                        </div>
                        {{actionButtonsHepler actionButtons }}
						{{videoControlsHepler video}}
                    </div>`,

                helper: function (video, actionButtons, animations) {
                    return PageBuilderModel.sections.components.video.template({ video, actionButtons, animations });
                }
            },

            //html5Video: {
            //    partial:
            //        `<div class="section-row html5-video-container js-section--container{{#if animations}} t63-invisible js-animate{{/if}}" data-container="html5video" data-animation="{{animations}}">
            //            <div class="embed-responsive">
            //                <video class="embed-responsive-item" controls="false" muted autoplay="true">
            //                    <source src="{{components.video.url}}" type="video/mp4">
            //                </video>
            //            </div>
            //            {{actionButtonsHepler actionButtons.html5Video}}
            //        </div>`,

            //    template:
            //        `<div class="section-row html5-video-container js-section--container{{#if animations}} t63-invisible js-animate{{/if}}" data-container="html5video" data-animation="{{animations}}">
            //            <div class="embed-responsive">
            //                <video class="embed-responsive-item" width="300" height="150" controls="false" muted autoplay="true">
            //                    <source src="{{video.url}}" type="video/mp4">
            //                </video>
            //            </div>
            //            {{actionButtonsHepler actionButtons }}
            //        </div>`,

            //    helper: function (html5Video, actionButtons, animations) {
            //        return PageBuilderModel.sections.components.html5Video.template({ html5Video, actionButtons, animations });
            //    }
            //},

            jwPlayer: {
                partial:
                    `<div class="section-row jwplayer-container js-section--container{{#if animations}} t63-invisible js-animate{{/if}}" data-container="jwPlayer" data-animation="{{animations}}">
                        <div class="js-jwPlayer-wrap" data-video-url="{{components.jwPlayer.videoUrl}}" data-cover-url="{{components.jwPlayer.coverUrl}}" data-markers-url="{{components.jwPlayer.markersUrl}}" data-subtitles-url="{{components.jwPlayer.subTitlesUrl}}" data-player-id="{{id}}">
                            <div>JW Player.</div>
                        </div>
                        {{actionButtonsHepler actionButtons.jwPlayer }}
                        {{jwPlayerControlsHepler components.jwPlayer}}
                    </div>`,

                template:
                    `<div class="section-row jwplayer-container js-section--container{{#if animations}} t63-invisible js-animate{{/if}}" data-container="jwPlayer" data-animation="{{animations}}">
                        <div class="js-jwPlayer-wrap" data-video-url="{{jwPlayer.videoUrl}}" data-cover-url="{{jwPlayer.coverUrl}}" data-markers-url="{{jwPlayer.markersUrl}}" data-subtitles-url="{{jwPlayer.subTitlesUrl}}" data-player-id="{{id}}">
                            <div>JW Player.</div>
                        </div>
                        {{actionButtonsHepler actionButtons }}
                        {{jwPlayerControlsHepler jwPlayer}}
                    </div>`,

                helper: function (jwPlayer, actionButtons, animations) {
                    return PageBuilderModel.sections.components.jwPlayer.template({ jwPlayer, actionButtons, animations });
                }
            },

            iframe: {
                partial:
                    `<div class="section-row iframe-container js-section--container{{#if animations}} t63-invisible js-animate{{/if}}" data-container="iframe" data-animation="{{animations}}">
                        <div class="embed-responsive js-iframe-container">
                            <iframe class="embed-responsive-item" src="{{components.iframe.url}}" frameborder="0" allow="encrypted-media" allowfullscreen=""></iframe>
                        </div>
                        {{actionButtonsHepler actionButtons.iframe }}
						{{iframeControlsHepler components.iframe}}
                    </div>`,
            },

            pdfViewerIframe: {
                partial:
                    `<div class="section-row pdfViewerIframe-container js-section--container{{#if animations}} t63-invisible js-animate{{/if}}" data-container="pdfViewerIframe" data-animation="{{animations}}">
                        <div class="embed-responsive js-pdfViewerIframe-container">
                            <iframe class="embed-responsive-item" src="{{components.pdfViewerIframe.url}}" data-pdf-url="{{components.pdfViewerIframe.urlPdf}}" data-pdfviewer-url="{{components.pdfViewerIframe.url}}" frameborder="0" allow="encrypted-media" allowfullscreen=""></iframe>
                        </div>
                        {{actionButtonsHepler actionButtons.pdfViewerIframe }}
						{{pdfViewerIframeControlsHepler components.pdfViewerIframe}}
                    </div>`,
            },

            attachements: {
                containerPartial:
                    `<div class="section-row files-container js-files-container">
						<ul class="file-list">
							{{> "filesPartial"}}
						</ul>
					</div>`,

                files: {
                    partial:
                        `{{#each components.attachements}}
						<li class="file-item js-section--container{{#if ../animations}} t63-invisible js-animate{{/if}}" data-container="file" data-animation="{{../animations}}">
							<img class="icon" src="/plugins/63bits-pageBuilder/images/icons/file.svg" alt="file icon">
							<div class="name">
								<a class="link js-file-name" href="{{url}}" download>{{name}}</a>
								{{> "fileControlsPartial"}}
							</div>
							{{actionButtonsHepler ../actionButtons.file}}
						</li>
						{{/each}}`,

                    template: '{{> filesPartial}}',

                    getHtml: function (model) {
                        return PageBuilderModel.sections.components.attachements.files.template(model);
                    }
                }
            },

            multimedia: {
                template:
                    `<div class="t63-multimedia-container js-multimedia-container js-section--container{{#if animations}} t63-invisible js-animate{{/if}}" data-container="multimedia" data-animation="{{animations}}" data-multimedia-type="{{#if multimedia.isImage}}image{{else}}video{{/if}}">
                        <figure class="embed-responsive js-multimedia-image-container">
                            <img class="embed-responsive-item js-multimedia-image" src="{{multimedia.image.url}}" alt="" data-image-src="{{multimedia.image.url}}" />
                        </figure>
                        <div class="embed-responsive js-multimedia-video-container">
                            <video class="embed-responsive-item js-multimedia-video"{{#if multimedia.video.controls}} controls{{/if}}{{#if multimedia.video.muted}} muted{{/if}}{{#if multimedia.video.loop}} loop{{/if}}{{#if multimedia.video.autoplay}} autoplay{{/if}} data-video-src="{{multimedia.video.url}}" data-controls="{{multimedia.video.controls}}" data-muted="{{multimedia.video.muted}}" data-autoplay="{{multimedia.video.autoplay}}" data-loop="{{multimedia.video.loop}}">
                                <source src="{{multimedia.video.url}}" type="video/mp4">
                            </video>
                        </div>
                        {{actionButtonsHepler actionButtons }}
                    </div>`,

                helper: function (multimedia, actionButtons, animations) {
                    return PageBuilderModel.sections.components.multimedia.template({ multimedia, actionButtons, animations });
                }
            },
        },

        //-- sections
        slide: {
            template:
                `<div class="t63-section img-section js-page-section{{#if isFullHeight}} t63-invisible{{/if}}" data-section="slide" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}" data-animation="false" data-children-animation="false" data-is-fullscreen="{{isFullScreen}}" data-is-fullheight="{{isFullHeight}}">
                    <div class="container t63-padding-v">
                        <div class="t63-slide-container">
					        {{> "bgImagePartial"}}
					        <div class="container slide-content js-slide-content" data-align-content="{{contentAlignment}}">
						        <section>
							        {{> "titlePartial"}}
                                    {{> "textPartial"}}
							        {{> "buttonPartial"}}
						        </section>
					        </div>
                        </div>
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</div>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.slide.template(model);
            }
        },

        slider: {
            template:
                `<div class="t63-section slider-section js-page-section{{#if isFullHeight}} t63-invisible{{/if}}" data-section="slider" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}" data-animation="false" data-children-animation="false" data-is-fullscreen="{{isFullScreen}}" data-is-fullheight="{{isFullHeight}}">
                    <div class="container t63-padding-v">
                        <div class="t63-slider js-t63-slider js-t63-slider-container" data-slider-autoplay="{{slider?.autoplay}}" data-slider-autoplay-speed="{{slider?.autoplaySpeed}}">
                            {{> "sliderItemsPartial"}}
                        </div>
                    </div>
                    {{> "sliderItemsControlsPartial"}}
					{{> "sectionEditorContainerPartial"}}
				</div>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.slider.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
                    <div>
                        {{multimediaHepler components.multimedia ../actionButtons.multimedia}}
                        <div class="container slide-content js-slide-content" data-align-content="{{contentAlignment}}">
                            <section>
                                {{titleHepler components.title ../actionButtons.title}}
                                {{textHepler components.text ../actionButtons.text}}
                                {{buttonHepler components.button ../actionButtons.button}}
                            </section>
                        </div>
                    </div>
                    {{/each}}`,

                template: `{{> "sliderItemsPartial"}}`,

                getHtml: function (model) {
                    return PageBuilderModel.sections.slider.items.template(model);
                }
            }
        },

        article: {
            template:
                `<section class="t63-section js-page-section" data-section="article" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-additional-classes="{{additionalClassNames}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}" data-is-card="{{isCard}}">
					<div class="container t63-padding-v">
					<article class="t63-article {{additionalClassNames}}">
						{{> "titlePartial"}}
						{{> "textPartial"}}
					</article>
					</div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.article.template(model);
            }
        },

        article2Col: {
            template:
                `<section class="t63-section js-page-section" data-section="article2Col" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
                    <div class="container">
					<div class="row text2col">
						<div class="col-lg-6 js-left-col">
							<article class="t63-article">
								{{titleHepler components.title false animations}}
								{{textHepler components.text false animations}}
								{{buttonHepler components.button actionButtons.button animations}}
							</article>
						</div>
						<div class="col-lg-6 js-right-col">
							<article class="t63-article">
								{{titleHepler components.title2 false animations}}
								{{textHepler components.text2 false animations}}
								{{buttonHepler components.button2 actionButtons.button animations}}
							</article>
						</div>
					</div>
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.article2Col.template(model);
            }
        },

        articleText2Col: {
            template:
                `<section class="t63-section js-page-section" data-section="articleText2Col" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<article class="container t63-article">
						{{> "titlePartial"}}
						<div class="row text2col">
							<div class="col-lg-6 js-left-col">
								{{textHepler components.text false animations}}
                                {{buttonHepler components.button actionButtons.button animations}}
							</div>
							<div class="col-lg-6 js-right-col">
								{{textHepler components.text2 false animations}}
                                {{buttonHepler components.button2 actionButtons.button animations}}
							</div>
						</div>

					</article>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.articleText2Col.template(model);
            }
        },

        article2ColWithImg: {
            template:
                `<section class="t63-section js-page-section {{additionalClassNames}}" data-section="article2ColWithImg" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-additional-classes="{{additionalClassNames}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}" data-reverse="{{isReversed}}">
                    <div class="container d-lg-flex align-items-stretch {{additionalClassNames}}">
					    <div class="col-lg-6 d-lg-flex align-items-center justify-content-end text-col{{#if animations}} t63-invisible js-animate{{/if}}" data-animation="{{animations}}">
						    <div class="t63-article">
							    {{> "titlePartial"}}
							    {{> "textPartial"}}
							    {{> "buttonPartial"}}
						    </div>
					    </div>
					    <div class="col-lg-6 img-col{{#if animations}} t63-invisible js-animate{{/if}}" data-animation="{{animations}}">
						    {{> "imagePartial"}}
					    </div>
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.article2ColWithImg.template(model);
            }
        },

        article2ColWithVideo: {
            template:
                `<section class="t63-section js-page-section {{additionalClassNames}}" data-section="article2ColWithVideo" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-additional-classes="{{additionalClassNames}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}" data-reverse="{{isReversed}}">
                    <div class="container d-lg-flex align-items-stretch {{additionalClassNames}}">
					    <div class="col-lg-6 d-lg-flex align-items-center justify-content-end text-col{{#if animations}} t63-invisible js-animate{{/if}}" data-animation="{{animations}}">
						    <div class="t63-article">
							    {{> "titlePartial"}}
							    {{> "textPartial"}}
							    {{> "buttonPartial"}}
						    </div>
					    </div>
					    <div class="col-lg-6 video-col{{#if animations}} t63-invisible js-animate{{/if}}" data-animation="{{animations}}">
						    {{> "videoPartial"}}
					    </div>
                    </div>
                    <div class="container citation-container">
					    {{> "citationPartial"}}
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.article2ColWithVideo.template(model);
            }
        },

        article2ColWithJwPlayer: {
            template:
                `<section class="t63-section js-page-section {{additionalClassNames}}" data-section="article2ColWithJwPlayer" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-additional-classes="{{additionalClassNames}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}" data-reverse="{{isReversed}}">
                    <div class="container d-lg-flex align-items-stretch {{additionalClassNames}}">
					    <div class="col-lg-6 d-lg-flex align-items-center justify-content-end text-col{{#if animations}} t63-invisible js-animate{{/if}}" data-animation="{{animations}}">
						    <div class="t63-article">
							    {{> "titlePartial"}}
							    {{> "textPartial"}}
							    {{> "buttonPartial"}}
						    </div>
					    </div>
					    <div class="col-lg-6 video-col{{#if animations}} t63-invisible js-animate{{/if}}" data-animation="{{animations}}">
						    {{> "jwPlayerPartial"}}
					    </div>
                    </div>
                    <div class="container citation-container">
					    {{> "citationPartial"}}
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.article2ColWithJwPlayer.template(model);
            }
        },

        articleWithFiles: {
            template:
                `<section class="t63-section js-page-section" data-section="articleWithFiles" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<article class="container t63-article">
						{{> "titlePartial"}}
						{{> "textPartial"}}
						{{> "attachemetsContainerPartial"}}
						<div class="btn-row choose-files-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-choose-file-btn">Attach Files</button>
							</div>
						</div>
					</article>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.articleWithFiles.template(model);
            },
        },

        articleWithVideo: {
            template:
                `<section class="t63-section js-page-section" data-section="articleWithVideo" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<article class="container t63-article">
						{{> "titlePartial"}}
						{{> "videoPartial"}}
                        {{> "textPartial"}}
					</article>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.articleWithVideo.template(model);
            }
        },

        video: {
            template:
                `<section class="t63-section js-page-section" data-section="video" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container">
                        {{> "videoPartial"}}
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.video.template(model);
            }
        },

        videoGrid: {
            template:
                `<section class="t63-section t63-video-grid-section js-page-section" data-section="videoGrid" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v">
						<div class="row js-video-grid-row">
                            {{> "videoGridItemsPartial"}}
                        </div>
						<div class="btn-row grid-add-cols-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-video-grid-col-btn" data-size="1" data-cols="col-sm-6 col-lg-3">Small</button>
								<button class="btn editor-btn js-add-video-grid-col-btn" data-size="2" data-cols="col-sm-6 col-lg-4">Medium</button>
								<button class="btn editor-btn js-add-video-grid-col-btn" data-size="3" data-cols="col-sm-6">Large</button>
							</div>
						</div>
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.videoGrid.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
					<div class="{{columnClassesNames}}">
						<div class="video-grid-item js-section--container js-video-grid-item{{#if ../../animations}} t63-invisible js-animate{{/if}}" data-container="videoGridItem" data-id="{{id}}" data-size="{{size}}" data-animation="{{../../animations}}" data-children-animation="false">
                            {{videoHepler components.video ../../actionButtons.video}}
                            {{actionButtonsHepler ../actionButtons.videoGridItem}}
							{{> "videoGridItemControlsPartial"}}
						</div>
					</div>
					{{/each}}`,

                template: '{{> "videoGridItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.videoGrid.items.template(model);
                }
            },

            sortable: function (container) {
                if (container || $('.js-video-grid-row').length > 0) {
                    container = container || '.js-video-grid-row';
                    $(container).sortable({
                        placeholder: 'grid-item-placeholder',
                        start: function (event, ui) {
                            var className = $(ui.item).attr('class').replace('ui-sortable-handle', '');
                            $('.grid-item-placeholder').addClass(className);
                        }
                    });
                    $(container).disableSelection();
                }
            }
        },

        jwPlayer: {
            template:
                `<section class="t63-section js-page-section" data-section="jwPlayer" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v{{#if animations}} t63-invisible js-animate{{/if}}" data-animation="{{animations}}">
                        {{> "jwPlayerPartial"}}
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.jwPlayer.template(model);
            }
        },

        jwPlayerGrid: {
            template:
                `<section class="t63-section t63-jwp-grid-section js-page-section" data-section="jwPlayerGrid" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v">
						<div class="row js-jwp-grid-row">
                            {{> "jwPlayerGridItemsPartial"}}
                        </div>
						<div class="btn-row grid-add-cols-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-jwp-grid-col-btn" data-size="1" data-cols="col-sm-6 col-lg-3">Small</button>
								<button class="btn editor-btn js-add-jwp-grid-col-btn" data-size="2" data-cols="col-sm-6 col-lg-4">Medium</button>
								<button class="btn editor-btn js-add-jwp-grid-col-btn" data-size="3" data-cols="col-sm-6">Large</button>
							</div>
						</div>
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.jwPlayerGrid.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
					<div class="{{columnClassesNames}}">
						<div class="jwp-grid-item js-section--container js-jwp-grid-item{{#if ../../animations}} t63-invisible js-animate{{/if}}" data-container="jwPlayerGridItem" data-id="{{id}}" data-size="{{size}}" data-animation="{{../../animations}}" data-children-animation="false">
                            {{jwPlayerHepler components.jwPlayer ../../actionButtons.jwPlayer}}
                            {{actionButtonsHepler ../actionButtons.jwPlayerGridItem}}
							{{> "jwPlayerGridItemControlsPartial"}}
						</div>
					</div>
					{{/each}}`,

                template: '{{> "jwPlayerGridItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.jwPlayerGrid.items.template(model);
                }
            },

            sortable: function (container) {
                if (container || $('.js-jwp-grid-row').length > 0) {
                    container = container || '.js-jwp-grid-row';
                    $(container).sortable({
                        placeholder: 'grid-item-placeholder',
                        start: function (event, ui) {
                            var className = $(ui.item).attr('class').replace('ui-sortable-handle', '');
                            $('.grid-item-placeholder').addClass(className);
                        }
                    });
                    $(container).disableSelection();
                }
            }
        },

        imgGrid: {
            template:
                `<section class="t63-section t63-img-grid-section js-page-section" data-section="imgGrid" data-id="{{id}}" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v">
						{{> "titlePartial"}}
						<div class="row js-img-grid-row">
                            {{> "imgGridItemsPartial"}}
                        </div>
                        {{> "buttonPartial"}}
						<div class="btn-row grid-add-cols-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-grid-col-btn" data-size="1" data-cols="col-sm-6 col-lg-3">Small</button>
								<button class="btn editor-btn js-add-grid-col-btn" data-size="2" data-cols="col-sm-6 col-lg-4">Medium</button>
								<button class="btn editor-btn js-add-grid-col-btn" data-size="3" data-cols="col-sm-6">Large</button>
							</div>
						</div>
					</div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.imgGrid.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
					<div class="{{columnClassesNames}}">
						<a class="grid-item js-section--container js-img-grid-item{{#if ../animations}} t63-invisible js-animate{{/if}}" data-container="imgGridItem" data-id="{{id}}" data-size="{{size}}" data-animation="{{../animations}}" data-children-animation="false"{{#if url}} href="{{url}}"{{/if}}{{#if isTargetBlank}} target="_blank"{{/if}}>
							<div class="img-container js-section--container" data-container="image">
								<span class="bg-img js-img" style="background-image: url({{image.url}})" data-img-src="{{image.url}}"></span>
								{{actionButtonsHepler ../actionButtons.image}}
							</div>
							<h3 class="grid-item-title js-title{{#unless title.isActive}} component-disabled{{/unless}}">{{title.value}}</h3>

							<div class="overlay{{#js_if "!this.title.isActive && !this.text.isActive && !this.button.isActive"}} component-disabled{{/js_if}}">
								<div>
									<span class="grid-item-title js-title{{#unless title.isActive}} component-disabled{{/unless}}">{{title.value}}</span>
									<p class="grid-item-text js-text{{#unless text.isActive}} component-disabled{{/unless}}">{{text.value}}</p>
								</div>
							</div>
							{{actionButtonsHepler ../actionButtons.imgGridItem}}
							{{> "imgGridItemControlsPartial"}}
						</a>
					</div>
					{{/each}}`,

                template: '{{> "imgGridItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.imgGrid.items.template(model);
                }
            },

            sortable: function (container) {
                if (container || $('.js-img-grid-row').length > 0) {
                    container = container || '.js-img-grid-row';
                    $(container).sortable({
                        placeholder: 'grid-item-placeholder',
                        start: function (event, ui) {
                            var className = $(ui.item).attr('class').replace('ui-sortable-handle', '');
                            $('.grid-item-placeholder').addClass(className);
                        }
                    });
                    $(container).disableSelection();
                }
            }
        },

        articlesGrid: {
            template:
                `<section class="t63-section t63-articles-grid-section js-page-section" data-section="articlesGrid" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v">
                        <div class="t63-section-title-container js-section-title-container">{{> "titlePartial"}}</div>
                        <div class="t63-section-text-container js-section-text-container">{{> "textPartial"}}</div>
						<div class="row js-articles-grid-row">
							{{> "articlesGridItemsPartial"}}
						</div>
						<div class="btn-row add-new-grid-item-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-articles-grid-item-btn">New item</button>
							</div>
						</div>
					</div>
					{{> "sectionEditorContainerPartial"}}
                </section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.articlesGrid.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
					<div class="col-sm-6 col-lg-4">
                        <div class="grid-item js-articles-grid-item js-section--container{{#if ../animations}} t63-invisible js-animate{{/if}}" data-container="articlesGridItem" data-animation="{{../animations}}" data-children-animation="false">
                            {{bgImageHepler components.image ../actionButtons.image}}
                            <div class="content">
                                {{titleHepler components.title}}
								{{textHepler components.text}}
								{{buttonHepler components.button ../actionButtons.button}}
                            </div>
							{{actionButtonsHepler ../actionButtons.articlesGridItem}}
                        </div>
                    </div>
					{{/each}}`,

                template: '{{> "articlesGridItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.articlesGrid.items.template(model);
                }
            }
        },

        articlesList: {
            template:
                `<section class="t63-section t63-articles-list-section js-page-section" data-section="articlesList" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v">
                        <div class="t63-section-title-container js-section-title-container">{{> "titlePartial"}}</div>
                        <div class="t63-section-text-container js-section-text-container">{{> "textPartial"}}</div>
						<div class="js-articles-list-container">
							{{> "articlesListItemsPartial"}}
						</div>
						<div class="btn-row add-new-list-item-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-articles-list-item-btn">New item</button>
							</div>
						</div>
					</div>
					{{> "sectionEditorContainerPartial"}}
                </section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.articlesList.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
                    <div class="list-item js-articles-list-item js-section--container{{#if ../animations}} t63-invisible js-animate{{/if}}" data-container="articlesListItem" data-animation="{{../animations}}" data-children-animation="false">
                        {{bgImageHepler components.image ../actionButtons.image}}
                        <div class="content">
                            {{titleHepler components.title}}
							{{textHepler components.text}}
							{{buttonHepler components.button ../actionButtons.button}}
                        </div>
						{{actionButtonsHepler ../actionButtons.articlesListItem}}
                    </div>
					{{/each}}`,

                template: '{{> "articlesListItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.articlesList.items.template(model);
                }
            }
        },

        packagesGrid: {
            template:
                `<section class="t63-section t63-packages-grid-section js-page-section" data-section="packagesGrid" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v">
                        <div class="t63-section-title-container js-section-title-container">{{> "titlePartial"}}</div>
                        <div class="t63-section-text-container js-section-text-container">{{> "textPartial"}}</div>
						<div class="row js-packages-grid-row" data-align-content="{{contentAlignment}}">
							{{> "packagesGridItemsPartial"}}
						</div>
						<div class="btn-row add-new-grid-item-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-packages-grid-item-btn" data-size="1" data-cols="col-sm-6 col-lg-3">Small</button>
								<button class="btn editor-btn js-add-packages-grid-item-btn" data-size="2" data-cols="col-sm-6 col-lg-4">Medium</button>
								<button class="btn editor-btn js-add-packages-grid-item-btn" data-size="3" data-cols="col-sm-6">Large</button>
							</div>
						</div>
					</div>
					{{> "sectionEditorContainerPartial"}}
                </section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.packagesGrid.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
					<div class="{{columnClassesNames}}">
                        <div class="packages-grid-item js-packages-grid-item js-section--container has-circled-icon{{#if ../../animations}} t63-invisible js-animate{{/if}}" data-container="packagesGridItem" data-id="{{id}}" data-size="{{size}}" data-media-link="true" data-animation="{{../../animations}}" data-children-animation="false">
                            {{iconHepler components.icon ../actionButtons.icon}}
                            {{titleHepler components.title false false}}
							{{textHepler components.text false false}}
							{{actionButtonsHepler ../actionButtons.packagesGridItem}}
                            {{> "packagesGridItemControlsPartial"}}
                        </div>
                    </div>
					{{/each}}`,

                template: '{{> "packagesGridItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.packagesGrid.items.template(model);
                }
            }
        },

        accordion: {
            template:
                `<section class="t63-section t63-accordion-section js-page-section" data-section="accordion" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
                    <div class="container t63-padding-v">
                        <div class="t63-section-title-container js-section-title-container">{{> "titlePartial"}}</div>
                        <div class="t63-section-text-container js-section-text-container">{{> "textPartial"}}</div>
                        <div class="content-wrap">
                            <div class="accordion-container">
                                <div class="js-t63-accordion-items-row">
					                {{> "accordionItemsPartial"}}
						        </div>
					            <div class="btn-row add-accordion-item-btn-wrap js-additional-action-buttons">
							        <div class="justify-content-center">
								        <button class="btn editor-btn js-add-accordion-item-btn">New item</button>
							        </div>
						        </div>
						    </div>
                        </div>
                    </div>
                    {{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.accordion.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
					<article class="accordion-item js-section--container js-t63-accordion-item{{#if ../animations}} t63-invisible js-animate{{/if}}" data-container="accordionItem" data-animation="{{../animations}}" data-children-animation="false">
                        <i class="handle js-additional-action-buttons"><img src="/plugins/63bits-pageBuilder/images/icons/drug.svg" alt="drag icon"></i>
						<div class="accordion-item-head js-t63-accordion-item-head">
                            {{fontAwesomeIconHepler components.fontAwesomeIcon ../actionButtons.fontAwesomeIcon}}
                            {{titleHepler components.title}}
                        </div>

						<div class="accordion-item-body js-t63-accordion-item-body">
						    {{textHepler components.text}}
						</div>
                        {{actionButtonsHepler ../actionButtons.accordionItem}}
					</article>
					{{/each}}`,

                template: '{{> "accordionItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.accordion.items.template(model);
                }
            },

            sortable: function (container) {
                if (container || $('.js-t63-accordion-items-row').length > 0) {
                    container = container || '.js-t63-accordion-items-row';

                    $(container).sortable({
                        handle: '.handle',
                        placeholder: 'accordion-item-placeholder',
                        start: function (event, ui) {
                            var className = $(ui.item).attr('class').replace('ui-sortable-handle', '');
                            $('.accordion-item-placeholder').addClass(className);
                        }
                    });
                }
            }
        },

        tabs: {
            template:
                `<section class="t63-section js-page-section" data-section="tabs" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
                    <div class="container t63-padding-v tabs-container{{#if animations}} t63-invisible js-animate{{/if}}" data-animation="{{animations}}" data-children-animation="false">
                        <div class="t63-section-title-container js-section-title-container">{{> "titlePartial"}}</div>
                        <div class="t63-section-text-container js-section-text-container">{{> "textPartial"}}</div>
					    <ul class="nav tab-nav js-tab-nav">
                            {{> "tabNavItemsPartial"}}
                        </ul>
                        <div class="tab-content js-tab-content">
                            {{> "tabContentItemsPartial"}}
                        </div>
					    <div class="btn-row add-accordion-item-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-tab-item-btn">New item</button>
							</div>
						</div>
                    </div>
                    {{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.tabs.template(model);
            },

            navItems: {
                partial:
                    `{{#each items}}
					<li class="js-section--container js-tab-nav-item" data-container="tabItem">
                        <a href="#{{id}}">
                            <span class="t63-icon-wrap js-tab-item-icon-wrap{{#unless components.fontAwesomeIcon.isActive}} component-disabled{{/unless}}">
                                <i class="{{components.fontAwesomeIcon.name}}"></i>
                            </span>
                            <span class="js-tab-item-name">{{name}}</span>
                        </a>
                        {{actionButtonsHepler ../actionButtons.tabItem}}
                        {{> "tabItemControlsPartial"}}
					</li>
					{{/each}}`,

                template: '{{> "tabNavItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.tabs.navItems.template(model);
                }
            },

            contentItems: {
                partial:
                    `{{#each items}}
					<div class="tab-pane fade js-tab-content-item" id="{{id}}">
                        {{textHepler components.text}}
                    </div>
					{{/each}}`,

                template: '{{> "tabContentItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.tabs.contentItems.template(model);
                }
            }
        },

        html: {
            template:
                `<section class="t63-section js-page-section mt-0" data-section="html" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v">
                        <div class="section-row text-container js-section--container" data-container="text">
                            <textarea class="form-control js-plain-html-input" rows="10">{{components.text.html}}</textarea>
                            <div class="plain-html-wrap js-plain-html-wrap"><pre class="js-text-wrap"></pre></div>
                        </div>
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.html.template(model);
            }
        },

        testimonials: {
            template:
                `<div class="t63-section testimonials-section js-page-section" data-section="testimonials" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
                    <div class="container t63-padding-v">
					    {{> "titlePartial"}}
					
					    <div class="t63-testimonials-slider js-t63-testimonials-slider js-t63-slider-container{{#if animations}} t63-invisible js-animate{{/if}}" data-animation="{{animations}}" data-children-animation="false">
                            {{> "testimonialsItemsPartial"}}
                        </div>
				    </div>
                    {{> "sliderItemsControlsPartial"}}
					{{> "sectionEditorContainerPartial"}}
				</div>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.testimonials.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
                    <div>
						<article class="t63-testimonial-item">
							<div class="avatar-wrap">
                                {{bgImageHepler components.image ../actionButtons.image}}
                            </div>
							{{textHepler components.text}}
							{{textPlainHelper components.author}}
                            {{textPlainHelper components.description}}
						</article>
					</div>
                    {{/each}}`,

                template: `{{> "testimonialsItemsPartial"}}`,

                getHtml: function (model) {
                    return PageBuilderModel.sections.testimonials.items.template(model);
                }
            }
        },

        services: {
            template:
                `<section class="t63-section t63-services-section js-page-section" data-section="services" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v">
                        <div class="t63-section-title-container js-section-title-container">{{> "titlePartial"}}</div>
                        <div class="t63-section-text-container js-section-text-container">{{> "textPartial"}}</div>
						<div class="row js-t63-services-grid-row">
							{{> "servicesGridItemsPartial"}}
						</div>
						<div class="btn-row add-new-grid-item-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-services-grid-item-btn">New item</button>
							</div>
						</div>
					</div>
					{{> "sectionEditorContainerPartial"}}
                </section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.services.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
					<div class="col-sm-6 col-lg-4">
                        <div class="grid-item js-t63-services-grid-item js-section--container{{#if ../animations}} t63-invisible js-animate{{/if}}" data-container="servicesGridItem" data-animation="{{../animations}}" data-children-animation="false">
                            {{iconHepler components.icon ../actionButtons.icon}}
                            <div class="content">
                                {{titleHepler components.title}}
                                {{textPlainHelper components.subTitle}}
								{{textHepler components.text}}
								{{buttonHepler components.button ../actionButtons.button}}
                            </div>
							{{actionButtonsHepler ../actionButtons.servicesGridItem}}
                        </div>
                    </div>
					{{/each}}`,

                template: '{{> "servicesGridItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.services.items.template(model);
                }
            }
        },

        pageHead: {
            template:
                `<div class="t63-section t63-page-head js-page-section" data-section="pageHead" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
                    <div class="container">
                        <div class="content">
                            <div class="info-wrap">
                                {{> "titlePartial"}}
                                {{> "textPartial"}}
                                {{> "buttonPartial"}}
                            </div>
						    {{> "imagePartial"}}
                        </div>
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</div>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.pageHead.template(model);
            }
        },

        mediaObject: {
            template:
                `<section class="t63-section t63-media-object-section js-page-section" data-section="mediaObject" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-additional-classes="{{additionalClassNames}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-is-card="{{isCard}}">
                    <div class="container t63-padding-v">
					    <article class="has-circled-icon{{#if animations}} t63-invisible js-animate{{/if}}" data-animation="{{animations}}">
                            {{> "iconPartial"}}
                            <div class="text-col">
                                {{> "titlePartial"}}
						        {{> "textPartial"}}
                            </div>
					    </article>
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.mediaObject.template(model);
            }
        },

        mediaObjectsGrid: {
            template:
                `<section class="t63-section t63-media-objects-grid-section js-page-section" data-section="mediaObjectsGrid" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v">
                        <div class="t63-section-title-container js-section-title-container">{{> "titlePartial"}}</div>
                        <div class="t63-section-text-container js-section-text-container">{{> "textPartial"}}</div>
						<div class="row js-media-objects-grid-row" data-align-content="{{contentAlignment}}">
							{{> "mediaObjectsGridItemsPartial"}}
						</div>
						<div class="btn-row add-new-grid-item-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-media-objects-grid-col-btn" data-size="1" data-cols="col-sm-6 col-lg-3">Small</button>
								<button class="btn editor-btn js-add-media-objects-grid-col-btn" data-size="2" data-cols="col-sm-6 col-lg-4">Medium</button>
								<button class="btn editor-btn js-add-media-objects-grid-col-btn" data-size="3" data-cols="col-sm-6">Large</button>
							</div>
						</div>
					</div>
					{{> "sectionEditorContainerPartial"}}
                </section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.mediaObjectsGrid.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
					<div class="{{columnClassesNames}}">
                        <div class="t63-media-objects-grid-item js-media-objects-grid-item js-section--container has-rounded-icon{{#if ../../animations}} t63-invisible js-animate{{/if}}" data-container="mediaObjectsGridItem" data-id="{{id}}" data-size="{{size}}" data-animation="{{../../animations}}" data-children-animation="false">
                            {{iconHepler components.icon ../actionButtons.icon}}
                            <div class="info-wrap">
                                {{titleHepler components.title false false}}
							    {{textHepler components.text false false}}
                            </div>
							{{actionButtonsHepler ../actionButtons.mediaObjectsGridItem}}
                            {{> "mediaObjectsGridItemControlsPartial"}}
                        </div>
                    </div>
					{{/each}}`,

                template: '{{> "mediaObjectsGridItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.mediaObjectsGrid.items.template(model);
                }
            }
        },

        card2Col: {
            template:
                `<section class="t63-section t63-card2col-section js-page-section {{additionalClassNames}}" data-section="card2Col" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-additional-classes="{{additionalClassNames}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}" data-reverse="{{isReversed}}">
                    <article class="container">
					    <div class="card-col{{#if animations}} t63-invisible js-animate{{/if}}" data-animation="{{animations}}">
						    {{> "titlePartial"}}
					    </div>
					    <div class="card-col{{#if animations}} t63-invisible js-animate{{/if}}" data-animation="{{animations}}">
						    {{> "textPartial"}}
					    </div>
                    </article>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.card2Col.template(model);
            }
        },

        flipCardsGrid: {
            template:
                `<section class="t63-section t63-flip-cards-section js-page-section" data-section="flipCardsGrid" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v">
                        <div class="t63-section-title-container js-section-title-container">{{> "titlePartial"}}</div>
                        <div class="t63-section-text-container js-section-text-container">{{> "textPartial"}}</div>
						<div class="row js-flip-cards-grid-row" data-align-content="{{contentAlignment}}">
							{{> "flipCardsGridItemsPartial"}}
						</div>
                        <div class="btn-row add-new-grid-item-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-flip-cards-grid-item-btn" data-size="1" data-cols="col-sm-6 col-lg-3">Small</button>
								<button class="btn editor-btn js-add-flip-cards-grid-item-btn" data-size="2" data-cols="col-sm-6 col-lg-4">Medium</button>
								<button class="btn editor-btn js-add-flip-cards-grid-item-btn" data-size="3" data-cols="col-sm-6">Large</button>
							</div>
						</div>
					</div>
					{{> "sectionEditorContainerPartial"}}
                </section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.flipCardsGrid.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
					<div class="{{columnClassesNames}}">
                        <div class="flip-card-item js-flip-cards-grid-item js-section--container has-circled-icon{{#if ../../../animations}} t63-invisible js-animate{{/if}}" data-container="flipCardsGridItem" data-id="{{id}}" data-size="{{size}}" data-clickable="{{rotateOnClick}}" data-animation="{{../../../animations}}" data-children-animation="false">
                            <div class="flip-wrapper">
                                <div class="flip-front">
                                    <div class="front-contents">
                                        <div class="media-container js-media-container" data-media-type="{{#if hasIcon}}icon{{else}}image{{/if}}" data-icon-type="faIcon">
                                            {{iconHepler components.icon ../actionButtons.icon}}
                                            {{imageHepler components.image ../actionButtons.image}}
                                        </div>                                        
                                        {{titleHepler components.title}}
                                    </div>
                                </div>
                                <div class="flip-back">
                                    <div class="back-contents">
                                        {{textHepler components.text}}
                                    </div>
                                </div>
                            </div>
                            {{actionButtonsHepler ../actionButtons.flipCardsGridItem}}
                            {{> "flipCardsGridItemControlsPartial"}}
                        </div>
                    </div>
					{{/each}}`,

                template: '{{> "flipCardsGridItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.flipCardsGrid.items.template(model);
                },
            }
        },

        quote: {
            template:
                `<section class="t63-section t63-quote-section js-page-section" data-section="quote" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-additional-classes="{{additionalClassNames}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
                    <div class="container t63-padding-v">
                        <article {{#if animations}}class="t63-invisible js-animate"{{/if}} data-animation="{{animations}}">
                            {{> "fontAwesomeIconPartial"}}
                            {{> "textPartial"}}
                        </article>
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.quote.template(model);
            }
        },

        article2colWithBgImgAndImg: {
            template:
                `<section class="t63-section t63-article2colWithBgImgAndImg-section js-page-section {{additionalClassNames}}" data-section="article2colWithBgImgAndImg" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-additional-classes="{{additionalClassNames}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
                    <article class="container t63-padding-v">
                        {{> "titlePartial"}}
                        <div class="row">
					        <div class="col-lg-7 left-col js-left-col{{#if animations}} t63-invisible js-animate{{/if}}" data-animation="{{animations}}" data-children-animation="false">
						        <div class="content">
                                    {{bgImageHepler components.bgImage ../actionButtons.image}}
							        {{> "textPartial"}}
						        </div>
					        </div>
					        <div class="col-lg-5 right-col js-right-col{{#if animations}} t63-invisible js-animate{{/if}}" data-animation="{{animations}}" data-children-animation="false">
						        {{imageHepler components.image ../actionButtons.image}}
					        </div>
					    </div>
                    </article>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.article2colWithBgImgAndImg.template(model);
            }
        },

        iframe: {
            template:
                `<section class="t63-section js-page-section" data-section="iframe" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-article">
                        {{> "titlePartial"}}
                        {{> "iframePartial"}}
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.iframe.template(model);
            }
        },

        pdfViewer: {
            template:
                `<section class="t63-section js-page-section" data-section="pdfViewer" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-article">
                        {{> "pdfViewerIframePartial"}}
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.pdfViewer.template(model);
            }
        },

        postCardsGrid: {
            template:
                `<section class="t63-section t63-post-cards-grid-section js-page-section" data-section="postCardsGrid" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v">
                        <div class="t63-section-title-container js-section-title-container">{{> "titlePartial"}}</div>
                        <div class="t63-section-text-container js-section-text-container">{{> "textPartial"}}</div>
						<div class="row js-post-cards-grid-row" data-align-content="{{contentAlignment}}">
							{{> "postCardsGridItemsPartial"}}
						</div>
						<div class="btn-row add-new-grid-item-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-post-cards-grid-col-btn" data-size="1" data-cols="col-sm-6 col-lg-3">Small</button>
								<button class="btn editor-btn js-add-post-cards-grid-col-btn" data-size="2" data-cols="col-sm-6 col-lg-4">Medium</button>
								<button class="btn editor-btn js-add-post-cards-grid-col-btn" data-size="3" data-cols="col-sm-6">Large</button>
							</div>
						</div>
					</div>
					{{> "sectionEditorContainerPartial"}}
                </section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.postCardsGrid.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
					<div class="{{columnClassesNames}}">
                        <div class="card t63-post-cards-grid-item js-post-cards-grid-item js-section--container{{#if ../animations}} t63-invisible js-animate{{/if}}" data-container="postCardsGridItem" data-id="{{id}}" data-size="{{size}}" data-media-link="true" data-animation="{{../animations}}" data-children-animation="false">
                            {{imageHepler components.image ../actionButtons.image}}
                            <div class="card-body">
                                {{titleHepler components.title}}
								{{textHepler components.text}}
                            </div>
							{{actionButtonsHepler ../actionButtons.postCardsGridItem}}
                            {{> "postCardsGridItemControlsPartial"}}
                        </div>
                    </div>
					{{/each}}`,

                template: '{{> "postCardsGridItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.postCardsGrid.items.template(model);
                }
            }
        },

        booksGrid: {
            template:
                `<section class="t63-section t63-books-grid-section js-page-section" data-section="booksGrid" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v">
                        <div class="t63-section-title-container js-section-title-container">{{> "titlePartial"}}</div>
                        <div class="t63-section-text-container js-section-text-container">{{> "textPartial"}}</div>
						<div class="row js-books-grid-row">
							{{> "booksGridItemsPartial"}}
						</div>
						<div class="btn-row add-new-grid-item-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-books-grid-col-btn" data-size="1" data-cols="col-sm-6 col-lg-3">Small</button>
								<button class="btn editor-btn js-add-books-grid-col-btn" data-size="2" data-cols="col-sm-6 col-lg-4">Medium</button>
								<button class="btn editor-btn js-add-books-grid-col-btn" data-size="3" data-cols="col-sm-6">Large</button>
							</div>
						</div>
					</div>
					{{> "sectionEditorContainerPartial"}}
                </section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.booksGrid.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
					<div class="{{columnClassesNames}}">
                        <div class="grid-item js-books-grid-item js-section--container{{#if ../animations}} t63-invisible js-animate{{/if}}" data-container="booksGridItem" data-id="{{id}}" data-size="{{size}}" data-animation="{{../animations}}" data-children-animation="false">
                            {{imageHepler components.image ../actionButtons.image}}
                            <div class="content">
                                {{titleHepler components.title}}
								{{textHepler components.text}}
								{{buttonHepler components.button ../actionButtons.button}}
                            </div>
							{{actionButtonsHepler ../actionButtons.booksGridItem}}
                            {{> "booksGridItemControlsPartial"}}
                        </div>
                    </div>
					{{/each}}`,

                template: '{{> "booksGridItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.booksGrid.items.template(model);
                }
            }
        },

        peopleGrid: {
            template:
                `<section class="t63-section t63-people-grid-section js-page-section" data-section="peopleGrid" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v">
                        <div class="t63-section-title-container js-section-title-container">{{> "titlePartial"}}</div>
                        <div class="t63-section-text-container js-section-text-container">{{> "textPartial"}}</div>
						<div class="row js-people-grid-row" data-align-content="{{contentAlignment}}">
							{{> "peopleGridItemsPartial"}}
						</div>
						<div class="btn-row add-new-grid-item-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-people-grid-col-btn" data-size="1" data-cols="col-sm-6 col-lg-3">Small</button>
								<button class="btn editor-btn js-add-people-grid-col-btn" data-size="2" data-cols="col-sm-6 col-lg-4">Medium</button>
								<button class="btn editor-btn js-add-people-grid-col-btn" data-size="3" data-cols="col-sm-6">Large</button>
							</div>
						</div>
					</div>
					{{> "sectionEditorContainerPartial"}}
                </section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.peopleGrid.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
					<div class="{{columnClassesNames}}">
                        <div class="t63-people-grid-item js-people-grid-item js-section--container{{#if ../animations}} t63-invisible js-animate{{/if}}" data-container="peopleGridItem" data-id="{{id}}" data-size="{{size}}" data-media-link="true" data-animation="{{../animations}}" data-children-animation="false">
                            {{imageHepler components.image ../actionButtons.image}}
                            <div class="info-wrap">
                                {{titleHepler components.title}}
								{{textHepler components.text}}
                            </div>
							{{actionButtonsHepler ../actionButtons.peopleGridItem}}
                            {{> "peopleGridItemControlsPartial"}}
                        </div>
                    </div>
					{{/each}}`,

                template: '{{> "peopleGridItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.peopleGrid.items.template(model);
                }
            }
        },

        cardsGrid: {
            template:
                `<section class="t63-section t63-cards-grid-section js-page-section" data-section="cardsGrid" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}" data-spacing-v="{{verticalSpacingSelected}}" data-background-color="{{#if backgroundColor}}true{{else}}false{{/if}}">
					<div class="container t63-padding-v">
                        <div class="t63-section-title-container js-section-title-container">{{> "titlePartial"}}</div>
                        <div class="t63-section-text-container js-section-text-container">{{> "textPartial"}}</div>
						<div class="row js-cards-grid-row" data-align-content="{{contentAlignment}}">
							{{> "cardsGridItemsPartial"}}
						</div>
						<div class="btn-row add-new-grid-item-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-cards-grid-col-btn" data-size="1" data-cols="col-sm-6 col-lg-3">Small</button>
								<button class="btn editor-btn js-add-cards-grid-col-btn" data-size="2" data-cols="col-sm-6 col-lg-4">Medium</button>
								<button class="btn editor-btn js-add-cards-grid-col-btn" data-size="3" data-cols="col-sm-6">Large</button>
							</div>
						</div>
					</div>
					{{> "sectionEditorContainerPartial"}}
                </section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.cardsGrid.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
					<div class="{{columnClassesNames}}">
                        <div class="cards-grid-item js-cards-grid-item js-section--container{{#if ../../animations}} t63-invisible js-animate{{/if}}" data-container="cardsGridItem" data-id="{{id}}" data-size="{{size}}" data-media-link="true" data-animation="{{../../animations}}" data-children-animation="false">
                            <div class="head">
                                {{iconHepler components.icon ../actionButtons.icon}}
                                {{titleHepler components.title false false}}
                            </div>
							{{textHepler components.text false false}}
							{{actionButtonsHepler ../actionButtons.cardsGridItem}}
                            {{> "cardsGridItemControlsPartial"}}
                        </div>
                    </div>
					{{/each}}`,

                template: '{{> "cardsGridItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.cardsGrid.items.template(model);
                }
            }
        },

        //-- functions
        addTopage: function (item, index) {
            var sectionHtml, currentSection;

            if (item) {
                var sectionName = item.data('item') || null;
                var sectionType = item.data('type') || 1;
                var sectionDisplayName = item.find('.js-builder-item-name').text();

                var model = PageBuilderModel.data.default().find(function (item) {
                    return item.name === sectionName && item.type == sectionType;
                });

                model.id = PageBuilderModel.guid.s8();
                model.displayName = sectionDisplayName;
                model.actionButtons = PageBuilderModel.editors.actionButtons.settings;

                sectionHtml = PageBuilderModel.sections[sectionName].getHtml(model);

                //add and sort sections on builder item drag/drop
                if (index > 0) {
                    $(sectionHtml).insertAfter($(PageBuilderModel.currentViewSelector + ' ' + PageBuilderModel.sections.selector).eq(index - 1));
                } else {
                    $(PageBuilderModel.currentViewSelector).prepend(sectionHtml);
                }

                currentSection = $(PageBuilderModel.sections.selector).eq(index);
            } else {
                if (PageBuilderModel.data.fromServer.sections) {
                    //console.log(PageBuilderModel.data.fromServer.sections)
                    PageBuilderModel.data.fromServer.sections.forEach(function (item) {
                        item.actionButtons = PageBuilderModel.editors.actionButtons.settings;
                        item.hasCardToggler = PageBuilderModel.settings.sections[item.name]().hasCardToggler;
                        item.hasReverseToggler = PageBuilderModel.settings.sections[item.name]().hasReverseToggler;
                        item.hasFullScreenToggler = PageBuilderModel.settings.sections[item.name]().hasFullScreenToggler;
                        if (item.components && item.components.title) {
                            item.components.title.isFullfeatured = PageBuilderModel.settings.sections[item.name]().components.title.isFullfeatured;
                        }
                        if (item.name != 'slide' || item.name != 'slider') {
                            if (!item.verticalSpacing) {
                                item.verticalSpacing = PageBuilderModel.settings.defaults.verticalSpacing();
                            }
                            if (!item.verticalSpacingSelected) {
                                item.verticalSpacingSelected = PageBuilderModel.settings.defaults.verticalSpacingSelected;
                            }
                        }
                        sectionHtml = PageBuilderModel.sections[item.name].getHtml(item).replace(/&quot;/g, '"').replace(/&lt;/g, '<').replace(/&gt;/g, '>');
                        $(PageBuilderModel.currentViewSelector).append(sectionHtml);
                    });
                    currentSection = $(PageBuilderModel.sections.selector);
                }

                //append scripts
                //if (PageBuilderModel.data.fromServer.pageScripts?.footerSection) {
                //    PageBuilderModel.pageScripts.appendFooterSectionToBody();
                //}
            }

            //init plugins
            PageBuilderModel.editors.title.tinymce.init(currentSection);
            PageBuilderModel.editors.text.tinymce.init(currentSection);

            PageBuilderModel.sections.imgGrid.sortable();
            PageBuilderModel.sections.accordion.sortable();

            PageBuilderModel.editors.sliderItem.init(currentSection);

            $('.js-tab-nav-item:first-child a').each(function (index, item) {
                PageBuilderModel.plugins.tabs.show($(item));
            });

            $('.js-jwPlayer-wrap').each(function (index, item) {
                if ($(item).children('.jwplayer').length == 0) {
                    new VideoPlayer({
                        Selector: $(item).children('div'),
                        File: $(item).attr('data-video-url'),
                        Image: $(item).attr('data-cover-url')
                    }).Init();
                }
            });
        },

        sort: function (item, index) {
            var sortIndex = index;
            var sectionIndex = item.attr('data-index');
            var sectionsSelector = PageBuilderModel.currentViewSelector + ' ' + PageBuilderModel.sections.selector;
            if (sectionIndex > sortIndex) {
                $(sectionsSelector + '[data-index="' + sectionIndex + '"]').insertBefore($(sectionsSelector).eq(sortIndex));
            }
            else {
                $(sectionsSelector + '[data-index="' + sectionIndex + '"]').insertAfter($(sectionsSelector).eq(sortIndex));
            }
        },

        init: function () {
            PageBuilderModel.sections.addTopage();
        }
    },

    editors: {
        actionButtons: {
            settings: {
                section: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: true
                },
                button: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: null
                },
                icon: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: null
                },
                fontAwesomeIcon: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: null
                },
                image: {
                    editBtn: true,
                    doneBtn: null,
                    removeBtn: null
                },
                video: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: null,
                    visibilityBtn: null,
                },
                jwPlayer: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: null,
                    visibilityBtn: null,
                },
                multimedia: {
                    editBtn: true,
                    doneBtn: null,
                    removeBtn: null
                },
                iframe: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: null,
                    visibilityBtn: null,
                },
                file: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: true
                },
                imgGridItem: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: true
                },
                articlesGridItem: {
                    editBtn: null,
                    doneBtn: null,
                    removeBtn: true,
                },
                articlesListItem: {
                    editBtn: null,
                    doneBtn: null,
                    removeBtn: true,
                },
                accordionItem: {
                    editBtn: null,
                    doneBtn: null,
                    removeBtn: true,
                },
                tabItem: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: true,
                },
                servicesGridItem: {
                    editBtn: null,
                    doneBtn: null,
                    removeBtn: true,
                },
                packagesGridItem: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: true,
                },
                mediaObjectsGridItem: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: true,
                },
                flipCardsGridItem: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: true,
                },
                pdfViewerIframe: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: null,
                    visibilityBtn: null,
                },
                videoGridItem: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: true
                },
                jwPlayerGridItem: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: true
                },
                postCardsGridItem: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: true,
                },
                booksGridItem: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: true,
                },
                peopleGridItem: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: true,
                },
                cardsGridItem: {
                    editBtn: true,
                    doneBtn: true,
                    removeBtn: true,
                },
            },

            template:
                `<div class="action-buttons js-action-buttons">
                    {{#if editBtn}}
                        <button class="editor-btn-rounded action-btn component-edit-btn js-edit-btn js-togglable-buttons" data-placement="left" data-toggle="tooltip" data-placement="left" title="{{editBtn.tooltip}}"></button>
                    {{/if}}
                    {{#if doneBtn}}
                        <button class="editor-btn-rounded action-btn component-done-btn js-done-btn js-togglable-buttons hidden" data-toggle="tooltip" data-placement="left" title="{{doneBtn.tooltip}}"></button>
                    {{/if}}
                    {{#if removeBtn}}
                        <button class="editor-btn-rounded action-btn component-remove-btn js-remove-btn" data-toggle="tooltip" data-placement="left" title="{{removeBtn.tooltip}}"></button>
                    {{/if}}
                </div>`,

            helper: function (model) {
                return PageBuilderModel.editors.actionButtons.template(model)
            },

            toggleVisibility: function (container) {
                container.children('.js-action-buttons').find('.js-togglable-buttons').toggleClass('hidden');
            },

            remove: function (container) {
                container.find('.js-action-buttons').remove();
            }
        },

        //--- editors & events
        section: {
            controls: {
                partial:
                    `<div class="section-row section-editor-container js-section--container" data-container="section">
						{{actionButtonsHepler actionButtons.section}}
						<div class="controls popover js-section-controls hidden">
							{{#js_if "this.name === 'slide' || this.name === 'slider' || this.name === 'packagesGrid' || this.name === 'flipCardsGrid' || this.name === 'postCardsGrid' || this.name === 'booksGrid' || this.name === 'cardsGrid'"}}
								{{> "alignmentWrapPartial"}}
							{{/js_if}}
							<div class="d-flex align-items-center justify-content-between">
								<label class="form-label">Section name</label>
								<label class="toggler toggler-sm js-toggler">
									<span>ScrollTo Nav</span>
									<input type="checkbox" {{#if isScrollToNavItem}}checked{{/if}}>
									<i></i>
								</label>
							</div>
							<div class="form-group">
								<input type="text" class="form-control js-display-name-input" value="{{displayName}}">
							</div>
                            {{#if contentSizes}}
                            <div class="form-group">
                                <label class="form-label">Content size</label>
                                <div class="custom-input-group">
                                    {{#each contentSizes}}
                                        {{#if exists}}
                                            <label>
                                                <input class="js-size-input" type="radio" name="size-{{../../id}}" value="{{value}}" {{#js_if "this.value === ../../contentSizeSelected"}}checked{{/js_if}}>
                                                <span>{{name}}</span>
                                            </label>
                                        {{/if}}
                                    {{/each}}
                                </div>
                            </div>
							{{/if}}
                            <div class="form-group">
                                <label class="form-label">Vertical Spacing</label>
                                <div class="custom-input-group">
                                    {{#each verticalSpacing}}
                                        {{#if exists}}
                                            <label>
                                                <input class="js-vertical-spacing-input" type="radio" name="vertical-spacing-{{../../id}}" value="{{value}}" {{#js_if "this.value === ../../verticalSpacingSelected"}}checked{{/js_if}}>
                                                <span>{{name}}</span>
                                            </label>
                                        {{/if}}
                                    {{/each}}
                                </div>
                            </div>
                            {{#if hasFullScreenToggler}}
                            <div class="form-group">
								<label class="form-label">FullScreen</label>
								
							    <div class="d-flex align-items-center justify-content-between">
								    <label class="toggler toggler-sm js-fullscreen-toggler">
									    <input type="checkbox" {{#if isFullScreen}}checked{{/if}}>
									    <i></i>
                                        <span>isFullScreen</span>
								    </label>
								    <label class="toggler toggler-sm js-fullheight-toggler{{#unless isFullScreen}} disabled{{/unless}}">
									    <input type="checkbox" {{#if isFullHeight}}checked{{/if}}>
									    <i></i>
                                        <span>Full height</span>
								    </label>
							    </div>
							</div>
							{{/if}}
                            {{#if hasCardToggler}}
                            <div class="form-group">
								<label class="form-label">Card</label>
								<label class="toggler toggler-sm js-card-toggler">
									<input type="checkbox" {{#if isCard}}checked{{/if}}>
									<i></i>
                                    <span>isCard</span>
								</label>
							</div>
							{{/if}}
                            {{#js_if "this.name === 'article2ColWithImg' || this.name === 'article2ColWithVideo' || this.name === 'article2ColWithJwPlayer' || this.name === 'card2Col'"}}
                            <div class="form-group">
								<label class="form-label">Reverse content</label>
								<label class="toggler toggler-sm js-content-reverse-toggler">
									<input type="checkbox" {{#if isReversed}}checked{{/if}}>
									<i></i>
								</label>
							</div>
							{{/js_if}}
                            {{#js_if "this.name !== 'slide' && this.name !== 'slider'"}}
                            <div class="form-group">
								<label class="form-label">Backgroun color</label>
								<label class="toggler toggler-sm js-background-color-toggler">
									<input type="checkbox" {{#if backgroundColor}}checked{{/if}}>
									<i></i>
                                    <span>Dark</span>
								</label>
							</div>
                            <div class="form-group">
							    <label class="form-label">Animations</label>
								<select class="form-control js-section-animation-combo">
                                    <option value="false" {{#js_if "this.animations === 'false'"}}selected{{/js_if}}>None</option>
                                    <option value="fadeIn" {{#js_if "this.animations === 'fadeIn'"}}selected{{/js_if}}>FadeIn</option>
                                    <option value="fadeInUp" {{#js_if "this.animations === 'fadeInUp'"}}selected{{/js_if}}>FadeIn Up</option>
                                    <option value="fadeInDown" {{#js_if "this.animations === 'fadeInDown'"}}selected{{/js_if}}>FadeIn Down</option>
                                    <option value="fadeInLeft" {{#js_if "this.animations === 'fadeInLeft'"}}selected{{/js_if}}>FadeIn Left</option>
                                    <option value="fadeInRight" {{#js_if "this.animations === 'fadeInRight'"}}selected{{/js_if}}>FadeIn Right</option>
                                </select>
							</div>
							{{/js_if}}
                            {{#js_if "this.name === 'slider'"}}
                            <div class="form-group">
								<label class="form-label">Slider options</label>
                                <div class="d-flex align-items-center justify-content-between">
								    <label class="toggler toggler-sm mb-0 js-slider-autoplay-toggler">
									    <input type="checkbox" {{#if slider?.autoplay}}checked{{/if}}>
									    <i></i>
                                        <span>autoplay</span>
								    </label>
							        <div class="d-flex align-items-center" title="Autoplay Speed in milliseconds">
                                        <span class="mr-1" style="font-size: 11px; line-height: 15px;">Speed (ms)</span>
                                        <input style="width: 52px" type="text" class="form-control pl-1 pr-1 js-slider-autoplay-speed-input" value="{{slider?.autoplaySpeed}}">
                                    </div>
							    </div>
							</div>
							{{/js_if}}
						</div>
					</div>`,

                alignmentWrapPartial:
                    `<div class="content-alignment-wrap">
						<span class="form-label">Alignment</span>
						<div>
							<label>
								<input class="js-alignment-input" type="radio" name="alignment-{{id}}" value="topLeft" {{#js_if "this.contentAlignment === 'topLeft' "}}checked{{/js_if}}>
								<span><img src="/plugins/63bits-pageBuilder/images/icons/alignment/1_1.svg" /></span>
							</label>
							<label>
								<input class="js-alignment-input" type="radio" name="alignment-{{id}}" value="topCenter" {{#js_if "this.contentAlignment === 'topCenter' "}}checked{{/js_if}}>
								<span><img src="/plugins/63bits-pageBuilder/images/icons/alignment/1_2.svg" /></span>
							</label>
							<label>
								<input class="js-alignment-input" type="radio" name="alignment-{{id}}" value="topRight" {{#js_if "this.contentAlignment === 'topRight' "}}checked{{/js_if}}>
								<span><img src="/plugins/63bits-pageBuilder/images/icons/alignment/1_3.svg" /></span>
							</label>
							<label>
								<input class="js-alignment-input" type="radio" name="alignment-{{id}}" value="centerLeft" {{#js_if "this.contentAlignment === 'centerLeft' "}}checked{{/js_if}}>
								<span><img src="/plugins/63bits-pageBuilder/images/icons/alignment/2_1.svg" /></span>
							</label>
							<label>
								<input class="js-alignment-input" type="radio" name="alignment-{{id}}" value="center" {{#js_if "this.contentAlignment === 'center' "}}checked{{/js_if}}>
								<span><img src="/plugins/63bits-pageBuilder/images/icons/alignment/2_2.svg" /></span>
							</label>
							<label>
								<input class="js-alignment-input" type="radio" name="alignment-{{id}}" value="centerRight" {{#js_if "this.contentAlignment === 'centerRight' "}}checked{{/js_if}}>
								<span><img src="/plugins/63bits-pageBuilder/images/icons/alignment/2_3.svg" /></span>
							</label>
							<label>
								<input class="js-alignment-input" type="radio" name="alignment-{{id}}" value="bottomLeft" {{#js_if "this.contentAlignment === 'bottomLeft' "}}checked{{/js_if}}>
								<span><img src="/plugins/63bits-pageBuilder/images/icons/alignment/3_1.svg" /></span>
							</label>
							<label>
								<input class="js-alignment-input" type="radio" name="alignment-{{id}}" value="bottomCenter" {{#js_if "this.contentAlignment === 'bottomCenter' "}}checked{{/js_if}}>
								<span><img src="/plugins/63bits-pageBuilder/images/icons/alignment/3_2.svg" /></span>
							</label>
							<label>
								<input class="js-alignment-input" type="radio" name="alignment-{{id}}" value="bottomRight" {{#js_if "this.contentAlignment === 'bottomRight' "}}checked{{/js_if}}>
								<span><img src="/plugins/63bits-pageBuilder/images/icons/alignment/3_3.svg" /></span>
							</label>
						</div>
					</div>`,
            },

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var section = container.closest(PageBuilderModel.sections.selector);
                var displayName = $.trim(container.find('.js-display-name-input').val()) || 'Nav Item';
                var isScrollToNavItem = container.find('.js-toggler input:checked').length > 0;
                var itemSize = container.find('.js-size-input:checked').val();
                var verticalSpacing = container.find('.js-vertical-spacing-input:checked').val();
                var backgoundColor = container.find('.js-background-color-toggler input:checked').length > 0;
                var isCard = container.find('.js-card-toggler input:checked').length > 0;
                var isReversed = container.find('.js-content-reverse-toggler input:checked').length > 0;
                var isFullScreen = container.find('.js-fullscreen-toggler input:checked').length > 0;
                var isFullHeight = isFullScreen && container.find('.js-fullheight-toggler input:checked').length > 0;

                section.attr('data-display-name', displayName);
                $(PageBuilderModel.builder.pageItems.selector + ' li').eq(section.index()).find('.js-builder-item-name').text(displayName);

                section.attr('data-isScrolltoNavItem', isScrollToNavItem);
                PageBuilderModel.scrollToNavigation.init();

                section.find('[data-align-content]').attr('data-align-content', section.find('.js-alignment-input:checked').val());

                section.attr('data-content-size', itemSize);

                section.attr('data-spacing-v', verticalSpacing);

                section.attr('data-background-color', backgoundColor);

                section.attr('data-is-card', isCard);

                section.attr('data-reverse', isReversed);

                section.attr('data-is-fullscreen', isFullScreen);

                section.attr('data-is-fullheight', isFullHeight);

                //slider options
                var sliderAutoplay = container.find('.js-slider-autoplay-toggler input:checked').length > 0;
                var sliderAutoplaySpeed = $.trim(container.find('.js-slider-autoplay-speed-input').val()) || 4000;

                section.find('.js-t63-slider-container').attr('data-slider-autoplay', sliderAutoplay);
                section.find('.js-t63-slider-container').attr('data-slider-autoplay-speed', sliderAutoplaySpeed);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);

                //
                PageBuilderModel.editors.hide(container);
            },

            initAnimations: function (section) {
                var animation = section.find('.js-section-animation-combo').val();
                var animatedItem = section.find('[data-animation]');
                if (animation != 'false') {
                    animatedItem.attr('data-animation', animation);
                    animatedItem.addClass('t63-invisible').addClass('js-animate');
                } else {
                    animatedItem.attr('data-animation', false);
                    animatedItem.removeClass('t63-invisible').removeClass('js-animate');
                }

                section.find('[data-children-animation="false"] [data-animation]').removeClass('t63-invisible').removeClass('js-animate').removeAttr('data-animation');
            },

            remove: function (container) {
                var index = container.closest(PageBuilderModel.sections.selector).attr('data-index');
                PageBuilderModel.builder.pageItems.remove(index);
                PageBuilderModel.scrollToNavigation.init();
            }
        },

        title: {
            helper: function (model) {
                return (
                    `<div class="controls text-controls js-section-controls">
						<div>
                            <label class="toggler toggler-sm js-component-visibility-toggler js-title-visibility-toggler">
                                <span>Is Active</span>
                                <input type="checkbox" ${model.isActive ? 'checked' : ''}>
                                <i></i>
                            </label>
                        </div>
					</div>`
                )
            },

            tinymce: {
                init: function (section) {
                    var optionsBasic = {
                        toolbar: 'alignleft aligncenter alignright alignjustify | italic | link | removeformat | disableTooltip | code',
                        plugins: 'paste autoresize link code help wordcount emoticons charmap',
                        block_formats: null,
                        fontsize_formats: null,
                        paste_word_valid_elements: 'i,em',
                        forced_root_block: ''
                    };

                    var optionsFullFeatured = {
                        toolbar: 'formatselect | alignleft aligncenter alignright alignjustify | italic | link | removeformat | disableTooltip | code',
                        plugins: 'paste autoresize link code help wordcount emoticons charmap',
                        block_formats: 'Header 1=h1;Header 2=h2;Header 3=h3;Header 4=h4;Header 5=h5;Header 6=h6',
                        paste_word_valid_elements: 'i,em',
                        forced_root_block: ''
                    };

                    if ($(section).find('.js-tinymce-basic.js-title').length > 0) {
                        PageBuilderModel.plugins.tinymce.init($(section).find('.js-tinymce-basic.js-title'), optionsBasic);
                    }

                    if ($(section).find('.js-tinymce-full.js-title').length > 0) {
                        PageBuilderModel.plugins.tinymce.init($(section).find('.js-tinymce-full.js-title'), optionsFullFeatured);
                    }
                }
            }
        },

        text: {
            helper: function (model) {
                return (
                    `<div class="controls text-controls js-section-controls">
						<div>
                            <label class="toggler toggler-sm js-component-visibility-toggler js-text-visibility-toggler">
                                <span>Is Active</span>
                                <input type="checkbox" ${model.isActive ? 'checked' : ''}>
                                <i></i>
                            </label>
                        </div>
					</div>`
                );
            },

            tinymce: {
                addImage: function (files) {
                    if (files) {
                        var urlDownload = files[0].urlDownload;
                        PageBuilderModel.plugins.fileManager.tinymce.callback(urlDownload, { alt: '' });
                    }
                    PageBuilderModel.plugins.fileManager.hide();

                    PageBuilderModel.plugins.fileManager.tinymce.isEnabled = false;
                    PageBuilderModel.plugins.fileManager.tinymce.callback = null;
                },
                addMedia: function (files) {
                    if (files) {
                        var urlDownload = files[0].urlDownload;
                        PageBuilderModel.plugins.fileManager.tinymce.callback(urlDownload, { source2: '', poster: '' });
                    }
                    PageBuilderModel.plugins.fileManager.hide();

                    PageBuilderModel.plugins.fileManager.tinymce.isEnabled = false;
                    PageBuilderModel.plugins.fileManager.tinymce.callback = null;
                },
                init: function (section) {
                    var optionsBasic = {
                        toolbar: 'blockquote | alignleft aligncenter alignright alignjustify | bullist numlist | removeformat | disableTooltip | code',
                        plugins: 'paste autoresize link hr lists advlist code help wordcount emoticons charmap visualblocks searchreplace',
                        //block_formats: 'Header 1=h1;Header 2=h2;Header 3=h3;Header 4=h4;Header 5=h5;Header 6=h6; Paragraph=p',
                        block_formats: null,
                        paste_word_valid_elements: 'b,strong,i,em,ul,li,ol,p,br,sub,sup',
                        forced_root_block: ''
                    };

                    var optionsFullFeatured = {
                        toolbar: [
                            'formatselect | bullist numlist outdent indent | link | hr | image media | table | code',
                            'fontsizeselect | forecolor backcolor | bold italic underline strikethrough superscript subscript | blockquote | alignleft aligncenter alignright alignjustify | removeformat | disableTooltip'
                        ],
                        plugins: 'paste autoresize image link media table hr lists advlist code help wordcount emoticons charmap visualblocks searchreplace',
                        block_formats: 'Header 1=h1;Header 2=h2;Header 3=h3;Header 4=h4;Header 5=h5;Header 6=h6; Paragraph=p',
                        paste_word_valid_elements: 'b,strong,i,em,ul,li,ol,p,br,sub,sup',
                        forced_root_block: '',

                        image_advtab: true,

                        file_picker_types: 'image media',
                        file_picker_callback: function (callback, value, meta) {
                            PageBuilderModel.plugins.fileManager.tinymce.isEnabled = true;
                            PageBuilderModel.plugins.fileManager.tinymce.callback = callback;
                            PageBuilderModel.plugins.fileManager.tinymce.type = meta.filetype;

                            if (meta.filetype == 'media') {
                                PageBuilderModel.plugins.fileManager.show(PageBuilderModel.plugins.fileManager.getSingleFileUrl());
                            } else {
                                PageBuilderModel.plugins.fileManager.show(PageBuilderModel.plugins.fileManager.getSingleImageUrl());
                            }
                        },

                        toolbar_sticky: true
                    };

                    if ($(section).find('.js-tinymce-basic.js-text-wrap').length > 0) {
                        PageBuilderModel.plugins.tinymce.init($(section).find('.js-tinymce-basic.js-text-wrap'), optionsBasic);
                    }

                    if ($(section).find('.js-tinymce-full.js-text-wrap').length > 0) {
                        PageBuilderModel.plugins.tinymce.init($(section).find('.js-tinymce-full.js-text-wrap'), optionsFullFeatured);
                    }
                }
            }
        },

        textPlain: {
            helper: function (model) {
                return (
                    `<div class="controls plain-text-controls js-section-controls">
                        <input class="plain-text-input js-plain-text-input" type="text" value="${model.html}">
                        <div class="toggler-wrap">
                            <div>
                                <label class="toggler toggler-sm js-component-visibility-toggler js-plain-text-visibility-toggler">
                                    <span>Is Active</span>
                                    <input type="checkbox" ${model.isActive ? 'checked' : ''}>
                                    <i></i>
                                </label>
                            </div>
                        </div>
                    </div>`
                );
            },
            done: function (container) {
                var value = container.find('.js-plain-text-input').val();
                container.find('.js-plain-text').html(value);
            }
        },

        button: {
            helper: function (model) {
                return (
                    `<div class="controls popover js-section-controls hidden">
                        <div class="d-flex justify-content-end">
                            <label class="toggler toggler-sm js-btn-visibility-toggler">
                                <span>Is Active</span>
                                <input type="checkbox" ${model.isActive ? 'checked' : ''}>
                                <i></i>
                            </label>
                        </div>
                        <div class="form-group">
                            <input type="text" class="form-control js-btn-name-input" placeholder="Button name" value="${model.name}">
                        </div>
                        <div class="form-group">
                            <input type="text" class="form-control js-btn-url-input" placeholder="http://example.com" value="${model.href}">
                        </div>
                        <div class="d-flex btn-target-checkbox-wrap">
                            <label>
                                <input class="js-btn-target-checkbox" type="checkbox" ${model.isTargetBlank ? 'checked' : ''}>
                                <span>Open in new window</span>
                            </label>
                        </div>
                    </div>`
                );
            },

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var isActive = container.find('.js-btn-visibility-toggler input:checked').length > 0;
                var btnName = container.find('.js-btn-name-input').val();
                var btnUrl = container.find('.js-btn-url-input').val();
                var isTargetBlank = container.find('.js-btn-target-checkbox').is(':checked');

                if (isActive) {
                    container.removeClass('component-disabled');
                }
                else {
                    container.addClass('component-disabled');
                }
                container.children('.js-btn').text(btnName);
                container.children('.js-btn').attr('href', btnUrl);

                if (isTargetBlank) {
                    container.children('.js-btn').attr('target', '_blank');
                }
                else {
                    container.children('.js-btn').removeAttr('target');
                }

                //
                PageBuilderModel.editors.hide(container);
            }
        },

        icon: {
            helper: function (model) {
                return (
                    `<div class="controls popover js-section-controls hidden">
                        <div class="d-flex justify-content-end">
                            <label class="toggler toggler-sm js-icon-visibility-toggler">
                                <span>Is Active</span>
                                <input type="checkbox" ${model.isActive ? 'checked' : ''}>
                                <i></i>
                            </label>
                        </div>
                        <div class="form-group">
                            <input type="text" class="form-control js-icon-classes-input ${model.isImage ? 'disabled' : ''}" placeholder="icon name" value="${model.name}">
                        </div>
                        <div class="form-group d-flex align-items-center justify-content-between">
                            <label class="custom-control custom-checkbox">
                                <input class="custom-control-input js-icon-type-checkbox" type="checkbox" ${model.isImage ? 'checked' : ''}>
                                <span class="custom-control-label">Image Icon</span>
                            </label>
                            <button class="btn btn-xs btn-primary js-icon-upload-btn ${model.isImage ? '' : 'disabled'}">Upload icon</button>
                        </div>
                    </div>`
                );
            },

            fileManager: {
                onSelectedFilesChooseClientCallback: function (files) {
                    var container = PageBuilderModel.editors.icon.container;

                    if (files) {
                        var urlDownload = files[0].urlDownload;

                        container.find('.js-img-icon').attr({ 'src': urlDownload, 'data-icon-url': urlDownload });
                    }

                    PageBuilderModel.plugins.fileManager.hide();
                    PageBuilderModel.editors.icon.done(container)
                }
            },

            container: '',

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);

                PageBuilderModel.editors.icon.container = container;

                $('.js-icon-type-checkbox').change(function () {
                    if ($(this).is(':checked')) {
                        container.find('.js-icon-classes-input').addClass('disabled');
                        container.find('.js-icon-upload-btn').removeClass('disabled');
                    } else {
                        container.find('.js-icon-classes-input').removeClass('disabled');
                        container.find('.js-icon-upload-btn').addClass('disabled');
                    }
                });

                container.find('.js-icon-upload-btn').off('click');
                container.find('.js-icon-upload-btn').click(function () {
                    PageBuilderModel.plugins.fileManager.show(PageBuilderModel.plugins.fileManager.getIconUrl());
                });
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var isActive = container.find('.js-icon-visibility-toggler input:checked').length > 0;
                var isImage = container.find('.js-icon-type-checkbox').is(':checked');
                var iconName = container.find('.js-icon-classes-input').val();

                if (isActive) {
                    container.removeClass('component-disabled');
                }
                else {
                    container.addClass('component-disabled');
                }

                if (isImage) {
                    container.attr('data-icon-type', 'image');
                }
                else {
                    container.attr('data-icon-type', 'font');
                }

                if (iconName.indexOf('class="') > -1) {
                    iconName = iconName.slice(iconName.indexOf('"') + 1, iconName.lastIndexOf('"'))
                }

                container.find('.js-font-icon').removeAttr('class').addClass(iconName + ' js-font-icon').attr('data-icon-classes', iconName);

                //
                PageBuilderModel.editors.hide(container);
            }
        },

        fontAwesomeIcon: {
            helper: function (model) {
                return (
                    `<div class="controls popover js-section-controls hidden">
                        <div class="d-flex justify-content-end">
                            <label class="toggler toggler-sm js-fontawesome-visibility-toggler">
                                <span>Is Active</span>
                                <input type="checkbox" ${model.isActive ? 'checked' : ''}>
                                <i></i>
                            </label>
                        </div>
                        <div class="form-group">
                            <input type="text" class="form-control js-fontawesome-icon-name-input" placeholder="icon name" value="${model.name}">
                        </div>
                    </div>`
                );
            },

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var isActive = container.find('.js-fontawesome-visibility-toggler input:checked').length > 0;
                var iconName = container.find('.js-fontawesome-icon-name-input').val();

                if (isActive) {
                    container.removeClass('component-disabled');
                }
                else {
                    container.addClass('component-disabled');
                }

                if (iconName.indexOf('class="') > -1) {
                    iconName = iconName.slice(iconName.indexOf('"') + 1, iconName.lastIndexOf('"'))
                }

                container.find('.js-fa-icon').removeAttr('class').addClass(iconName + ' js-fa-icon').attr('data-fontawesome-icon', iconName);

                //
                PageBuilderModel.editors.hide(container);
            }
        },

        image: {
            helper: function (model) {
                return (
                    `<div class="controls image-controls js-section-controls">
						<div>
                            <label class="toggler toggler-sm js-component-visibility-toggler js-image-visibility-toggler">
                                <span>Is Active</span>
                                <input type="checkbox" ${model.isActive ? 'checked' : ''}>
                                <i></i>
                            </label>
                        </div>
					</div>`
                );
            },

            container: '',
            edit: function (container) {
                PageBuilderModel.plugins.fileManager.file.isImage = true;
                PageBuilderModel.editors.image.container = container;
                PageBuilderModel.plugins.fileManager.show(PageBuilderModel.plugins.fileManager.getSingleImageUrl());
            },
            add: function (files) {
                if (files) {
                    var urlDownload = files[0].urlDownload;
                    var imgWrap = PageBuilderModel.editors.image.container.children('.js-img');

                    imgWrap.attr('data-img-src', urlDownload);

                    if (imgWrap.find('img').length > 0) {
                        imgWrap.find('img').attr('src', urlDownload);
                    } else {
                        imgWrap.css({ 'background-image': 'url(\'' + urlDownload + '\')' });
                    }

                    var slider = PageBuilderModel.editors.image.container.closest('.js-t63-slider-container');

                    if ($(slider).length > 0) {
                        $(slider).closest('.js-page-section').find('.js-slider-settings-container li.active .js-go-to-slide-btn').css({ 'background-image': 'url(\'' + urlDownload + '\')' });
                    }
                }
                PageBuilderModel.plugins.fileManager.hide();
            }
        },

        video: {
            helper: function (model) {
                return (
                    `<div class="controls popover js-section-controls hidden">
                        <div class="form-group">
                            <input class="form-control js-video-input" placeholder="Video url" />
                        </div>
                    </div>`
                )
            },

            getInfo: function (url) {
                url.match(/(player.|www.)?(vimeo\.com|youtu(be\.com|\.be|be\.googleapis\.com))\/(video\/|embed\/|watch\?v=|v\/)?([A-Za-z0-9._%-]*)(\&\S+)?/);

                var type;

                if (RegExp.$2.indexOf('youtu') > -1) {
                    type = 'youtube';
                } else if (RegExp.$2.indexOf('vimeo') > -1) {
                    type = 'vimeo';
                }

                return {
                    type: type,
                    id: RegExp.$5
                };
            },

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);

                var url = container.find('.js-iframe-container iframe').attr('src');
                if (url != '') {
                    container.find('.js-video-input').val(url);
                }
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var url = $.trim(container.find('.js-video-input').val());

                if (url != '') {

                    var newUrl, params = '';

                    var type = PageBuilderModel.editors.video.getInfo(url).type;
                    var id = PageBuilderModel.editors.video.getInfo(url).id;

                    if (type == 'youtube') {
                        newUrl = 'https://www.youtube.com/embed/' + id + params;
                    }
                    else {
                        newUrl = 'https://player.vimeo.com/video/' + id;
                    }

                    container.find('.js-iframe-container iframe').attr('src', newUrl);
                }

                //
                PageBuilderModel.editors.hide(container);
            }
        },

        jwPlayer: {
            helper: function (model) {
                return (
                    `<div class="controls popover js-section-controls hidden">
                        <div class="form-group">
                            <label class="form-label">Video url</label>
                            <input class="form-control js-jwPlayer-video-url-input" placeholder="Video url" />
                        </div>
                        <div class="form-group">
                            <label class="form-label">Cover url</label>
                            <input class="form-control js-jwPlayer-cover-url-input" placeholder="Cover url" />
                        </div>
                    </div>`
                )
            },

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);

                var jwPlayer = container.children('.js-jwPlayer-wrap');
                var videoUrl = jwPlayer.attr('data-video-url');
                var coverUrl = jwPlayer.attr('data-cover-url');

                if (videoUrl != '') {
                    container.find('.js-jwPlayer-video-url-input').val(videoUrl);
                }
                container.find('.js-jwPlayer-cover-url-input').val(coverUrl);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var videoUrl = $.trim(container.find('.js-jwPlayer-video-url-input').val());
                var coverUrl = $.trim(container.find('.js-jwPlayer-cover-url-input').val());

                var jwPlayer = container.children('.js-jwPlayer-wrap');

                jwPlayer.attr('data-cover-url', coverUrl);

                if (videoUrl != '') {
                    jwPlayer.attr('data-video-url', videoUrl);

                    new VideoPlayer({
                        Selector: jwPlayer.children(),
                        File: videoUrl,
                        Image: coverUrl
                    }).Init();

                    //container.find('.js-jwPlayer-video-url-input').val('');
                }

                //
                PageBuilderModel.editors.hide(container);
            }

            //--- filemanager
            //fileManager: {
            //    onSelectedFilesChooseClientCallback: function (files) {
            //        PageBuilderModel.editors.jwPlayer.add(files);
            //    }
            //},

            //container: '',
            //edit: function (container) {
            //    PageBuilderModel.editors.jwPlayer.container = container;
            //    PageBuilderModel.plugins.fileManager.show(PageBuilderModel.plugins.fileManager.getJwPlayerUrl());
            //},
            //add: function (files) {
            //    console.log(files)
            //    if (files) {
            //        var urlDownload = (escape(files[0].urlDownload));
            //        var jwPlayer = PageBuilderModel.editors.jwPlayer.container.children('.js-jwPlayer-wrap');

            //        jwPlayer.attr('data-video-url', urlDownload);

            //        new VideoPlayer({
            //            Selector: jwPlayer.children(),
            //            File: urlDownload
            //        }).Init();
            //    }
            //    PageBuilderModel.plugins.fileManager.hide();
            //}
        },

        multimedia: {
            fileManager: {
                onSelectedFilesChooseClientCallback: function (files) {
                    var container = PageBuilderModel.editors.multimedia.container;

                    if (files) {
                        var urlDownload = files[0].urlDownload;
                        var fileExtension = urlDownload.slice(urlDownload.lastIndexOf('.') + 1);

                        var image = container.find('.js-multimedia-image');
                        var video = container.find('.js-multimedia-video');

                        if (fileExtension == 'mp4') {
                            video.attr('data-video-src', urlDownload);
                            video.children('source').attr('src', urlDownload);

                            var videoHtml = container.find('.js-multimedia-video-container').html()
                            container.find('.js-multimedia-video-container').html(videoHtml)
                        } else {
                            image.attr({ 'src': urlDownload, 'data-image-src': urlDownload });
                        }

                        var multimediaType = fileExtension == 'mp4' ? 'video' : 'image';
                        container.attr('data-multimedia-type', multimediaType);

                        var slider = container.closest('.js-t63-slider-container');
                        if ($(slider).length > 0) {

                            $(slider).closest('.js-page-section').find('.js-slider-settings-container li.active .js-multimedia-thumbnail-wrap').attr('data-multimedia-type', multimediaType);

                            if (fileExtension == 'mp4') {
                                $(slider).closest('.js-page-section').find('.js-slider-settings-container li.active .js-multimedia-video-thumbnail-wrap source').attr('src', urlDownload);

                                var videoThumbHtml = $(slider).closest('.js-page-section').find('.js-slider-settings-container li.active .js-multimedia-video-thumbnail-wrap').html()
                                $(slider).closest('.js-page-section').find('.js-slider-settings-container li.active .js-multimedia-video-thumbnail-wrap').html(videoThumbHtml)
                            } else {
                                $(slider).closest('.js-page-section').find('.js-slider-settings-container li.active .js-multimedia-image-thumbnail-wrap img').attr('src', urlDownload);
                            }
                        }
                    }

                    PageBuilderModel.plugins.fileManager.hide();
                }
            },

            container: '',

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);

                PageBuilderModel.editors.multimedia.container = container;

                PageBuilderModel.plugins.fileManager.show(PageBuilderModel.plugins.fileManager.getMultimediaFilesUrl());
            },
            add: function (files) {
                if (files) {
                    var urlDownload = files[0].urlDownload;
                    var container = PageBuilderModel.editors.multimedia.container;
                    var image = container.find('.js-multimedia-image');
                    var video = container.find('.js-multimedia-video');

                    if (fileExtension == 'mp4') {
                        video.attr('data-video-src', urlDownload);
                        video.children('source').attr('src', urlDownload);
                    } else {
                        image.attr({ 'src': urlDownload, 'data-img-src': urlDownload });
                    }

                    var slider = container.closest('.js-t63-slider-container');
                    if ($(slider).length > 0) {

                        if (fileExtension == 'mp4') {
                            $(slider).closest('.js-page-section').find('.js-slider-settings-container li.active .js-multimedia-video-thumbnail-wrap source').attr('src', urlDownload);
                        } else {
                            $(slider).closest('.js-page-section').find('.js-slider-settings-container li.active .js-multimedia-image-thumbnail-wrap img').attr('src', urlDownload);
                        }
                    }

                    //var slider = PageBuilderModel.editors.image.container.closest('.js-t63-slider-container');

                    //if ($(slider).length > 0) {
                    //    $(slider).closest('.js-page-section').find('.js-slider-settings-container li.active .js-go-to-slide-btn').css({ 'background-image': 'url(\'' + urlDownload + '\')' });
                    //}
                }
                PageBuilderModel.plugins.fileManager.hide();
            }
        },

        iframe: {
            helper: function (model) {
                return (
                    `<div class="controls popover js-section-controls hidden">
                        <div class="form-group">
                            <input class="form-control js-iframe-input" placeholder="Url" />
                        </div>
                    </div>`
                )
            },

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);

                var url = container.find('.js-iframe-container iframe').attr('src');
                if (url != '') {
                    container.find('.js-iframe-input').val(url);
                }
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var url = $.trim(container.find('.js-iframe-input').val());

                if (url != '') {
                    container.find('.js-iframe-container iframe').attr('src', url);
                }

                //
                PageBuilderModel.editors.hide(container);
            }
        },

        file: {
            partial:
                `<div class="controls js-section-controls hidden">
                    <input type="text" class="form-control js-file-name-input" placeholder="File Name" value="">
                </div>`,

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                //
                container.find('.js-file-name-input').val(container.find('.js-file-name').text());
                container.find('.js-file-name').addClass('hidden');
                //
                PageBuilderModel.editors.show(container);
                container.find('.js-file-name-input').focus();
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                //
                container.find('.js-file-name').text(container.find('.js-file-name-input').val());
                container.find('.js-file-name').removeClass('hidden');
                //
                PageBuilderModel.editors.hide(container);
            },

            currentSection: $('.js-page-section'),
            choose: function (section) {
                PageBuilderModel.plugins.fileManager.file.isImage = false;

                PageBuilderModel.editors.file.currentSection = section;

                PageBuilderModel.plugins.fileManager.show(PageBuilderModel.plugins.fileManager.getMultipleFilesUrl());
            },
            add: function (files) {
                if (files) {

                    var model = {
                        components: {
                            attachements: []
                        },
                        actionButtons: PageBuilderModel.editors.actionButtons.settings
                    };

                    var urlDownload, filename;

                    for (var i = 0; i < files.length; i++) {

                        urlDownload = files[i].urlDownload;
                        filename = files[i].name.split('.').slice(0, -1).join('.');

                        model.components.attachements[i] = {
                            url: urlDownload,
                            name: filename
                        };
                    }


                    $('.js-files-container > ul').append(PageBuilderModel.sections.components.attachements.files.getHtml(model));
                }
                //
                PageBuilderModel.plugins.fileManager.hide();
            },

            remove: function (container) {
                container.remove();
            }
        },

        imgGridItem: {
            partial:
                `<div class="controls popover js-section-controls hidden">
                    <div class="form-group">
                        <label class="form-label">Item size</label>
                        <div class="custom-input-group">
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="1" {{#js_if "this.size == 1"}}checked{{/js_if}}>
                                <span>Small</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="2" {{#js_if "this.size == 2"}}checked{{/js_if}}>
                                <span>Medium</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="3" {{#js_if "this.size == 3"}}checked{{/js_if}}>
                                <span>Large</span>
                            </label>
                        </div>
                    </div>
                    
                    <div class="d-flex align-items-center justify-content-between">
                        <label class="form-label">Title</label>
                        <label class="toggler toggler-sm js-title-toggler">
                            <span>Is Active</span>
                            <input type="checkbox" {{#if title.isActive}}checked{{/if}}>
                            <i></i>
                        </label>
                    </div>
                    <div class="form-group">
                        <input type="text" class="form-control js-title-input" placeholder="title" value="{{title.value}}">
                    </div>
                    
                    <div class="d-flex align-items-center justify-content-between">
                        <label class="form-label">Text</label>
                        <label class="toggler toggler-sm js-text-toggler">
                            <span>Is Active</span>
                            <input type="checkbox" {{#if text.isActive}}checked{{/if}}>
                            <i></i>
                        </label>
                    </div>
                    <div class="form-group">
                        <textarea class="form-control js-text-input" rows="3" maxlength="160" placeholder="Text">{{text.value}}</textarea>
                    </div>
                    
                    <div class="form-group">
                        <label class="form-label">Url</label>
                        <input type="text" class="form-control js-url-input" placeholder="http://example.com" value="{{url}}">
                    </div>
                        <div class="d-flex btn-target-checkbox-wrap">
                            <label>
                                <input class="js-target-checkbox" type="checkbox" {{#if isTargetBlank}}checked{{/if}}>
                                <span>Open in new window</span>
                            </label>
                        </div>
                </div>`,

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var itemSize = +container.find('.js-size-input:checked').val();

                var url = container.find('.js-url-input').val();
                var isTargetBlank = container.find('.js-target-checkbox:checked').length > 0;

                var title = {
                    isActive: container.find('.js-title-toggler input:checked').length > 0,
                    value: container.find('.js-title-input').val()
                };
                var text = {
                    isActive: container.find('.js-text-toggler input:checked').length > 0,
                    value: container.find('.js-text-input').val()
                };

                //
                container.find('.js-title').html(title.value);
                container.find('.js-text').html(text.value);
                //container.find('.js-btn').html(button.name).attr('href', button.url);

                if (title.isActive) {
                    container.find('.js-title').removeClass('component-disabled');
                }
                else {
                    container.find('.js-title').addClass('component-disabled');
                }

                if (text.isActive) {
                    container.find('.js-text').removeClass('component-disabled');
                }
                else {
                    container.find('.js-text').addClass('component-disabled');
                }

                if (title.isActive || text.isActive) {
                    container.find('.overlay').removeClass('component-disabled');
                }
                else {
                    container.find('.overlay').addClass('component-disabled');
                }


                if (url) {
                    container.attr('href', url);
                }
                else {
                    container.removeAttr('href');
                }

                if (isTargetBlank) {
                    container.attr('target', '_blank');
                }
                else {
                    container.removeAttr('target');
                }

                container.attr('data-size', itemSize);
                container.parent().removeAttr('class').addClass(PageBuilderModel.settings.components.imgGridItem.columnClassesNames[itemSize - 1]);

                //
                PageBuilderModel.editors.hide(container);
            },

            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemSize = +_this.attr('data-size');
                var itemsRow = section.find('.js-img-grid-row');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.imgGridItem,
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.imgGridItem.columnClassesNames[itemSize - 1],
                            size: itemSize,
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsRow.append(PageBuilderModel.sections.imgGrid.items.getHtml(model));

                PageBuilderModel.sections.imgGrid.sortable(itemsRow);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                container.parent().remove();
            }
        },

        articlesGridItem: {
            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemsRow = section.find('.js-articles-grid-row');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.articlesGridItem()
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsRow.append(PageBuilderModel.sections.articlesGrid.items.getHtml(model));

                PageBuilderModel.editors.title.tinymce.init(itemsRow);
                PageBuilderModel.editors.text.tinymce.init(itemsRow);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                container.parent().remove();
            }
        },

        articlesListItem: {
            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemsContainer = section.find('.js-articles-list-container');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.articlesListItem()
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsContainer.append(PageBuilderModel.sections.articlesList.items.getHtml(model));

                PageBuilderModel.editors.title.tinymce.init(itemsContainer);
                PageBuilderModel.editors.text.tinymce.init(itemsContainer);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                container.parent().remove();
            }
        },

        sliderItem: {
            controls: {
                containerPartial:
                    `<div class="t63-slider-settings-container js-slider-settings-container">
                            <div>
                                <div class="items-wrap">
                                    <ul>
                                        {{> "sliderItemsControlsItemPartial"}}
                                    </ul>
                                </div>
                                <button class="editor-btn-rounded add-slide-btn js-add-slide-btn"></button>
                            </div>
                        </div>`,

                items: {
                    partial:
                        `{{#each items}}
                            <li>
                                {{#if components.image}}
                                <button class="go-to-slide-btn js-go-to-slide-btn" style="background-image: url({{components.image.url}})"></button>
                                {{/if}}
                                {{#if components.multimedia}}
                                    <div class="multimedia-thumbnail-wrap js-multimedia-thumbnail-wrap" data-multimedia-type="{{#if components.multimedia.isImage}}image{{else}}video{{/if}}">
                                        <figure class="embed-responsive js-multimedia-image-thumbnail-wrap">
                                            <img class="embed-responsive-item" src="{{components.multimedia.image.url}}" alt="" />
                                        </figure>
                                        <div class="embed-responsive js-multimedia-video-thumbnail-wrap">
                                            <video class="embed-responsive-item">
                                                <source src="{{components.multimedia.video.url}}" type="video/mp4">
                                            </video>
                                        </div>
                                        <button class="go-to-slide-btn js-go-to-slide-btn"></button>
                                    </div>
                                {{/if}}
                                <button class="remove-slide-btn js-remove-slide-btn"></button>
                                <i class="handle ui-draggable-handle"></i>
                            </li>
                        {{/each}}`,

                    template: '{{> "sliderItemsControlsItemPartial"}}',

                    getHtml: function (model) {
                        return PageBuilderModel.editors.sliderItem.controls.items.template(model);
                    },
                }
            },

            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var sectionName = section.data('section');
                var slider = section.find('.js-t63-slider-container');
                var settingItemsContainer = section.find('.js-slider-settings-container ul');

                var model = {
                    items: [
                        { ...PageBuilderModel.settings.components[sectionName + 'Item']() }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                slider.append(PageBuilderModel.sections[sectionName].items.getHtml(model));

                settingItemsContainer.append(PageBuilderModel.editors.sliderItem.controls.items.getHtml(model));

                PageBuilderModel.editors.sliderItem.activeFirstSlide(section);
                PageBuilderModel.editors.sliderItem.sortable(section);

                var newSlide = slider.children('div:last-child');
                PageBuilderModel.editors.title.tinymce.init(newSlide);
                PageBuilderModel.editors.text.tinymce.init(newSlide);
            },

            remove: function (_this) {
                var section = _this.closest('.js-page-section');

                section.find('.js-t63-slider-container > div').eq(_this.parent().index()).remove();
                _this.parent().remove();

                PageBuilderModel.editors.sliderItem.activeFirstSlide(section);
                PageBuilderModel.editors.sliderItem.sortable(section);
            },

            goToSlide: function (_this) {
                var section = _this.closest('.js-page-section');
                var slide = section.find('.js-t63-slider-container > div');
                var controlsItem = section.find('.js-slider-settings-container li');

                slide.removeClass('active');
                controlsItem.removeClass('active');

                _this.closest('li').addClass('active');
                slide.eq(_this.closest('li').index()).addClass('active');
            },

            activeFirstSlide: function (section) {
                if ($(section).find('.js-t63-slider-container > div.active').length == 0) {
                    $(section).find('.js-t63-slider-container > div:first-child, .js-slider-settings-container li:first-child').addClass('active');
                }
            },

            addIndexes: function (section) {
                $('.js-slider-settings-container li').each(function (index, item) {
                    $(item).attr('data-index', index);
                });
            },

            sortable: function (section) {
                PageBuilderModel.editors.sliderItem.addIndexes(section);

                var container = section ? section.find('.js-slider-settings-container ul') : $('.js-slider-settings-container ul');

                $(container).sortable({
                    handle: '.handle',
                    placeholder: 'slider-settngs-item-placeholder',
                    update: function (event, ui) {
                        var oldIndex = +$(ui.item).attr('data-index');
                        var newIndex = $(ui.item).index();

                        var slide = section.find('.js-t63-slider-container > div');

                        if (newIndex > oldIndex) {
                            slide.eq(oldIndex).insertAfter(slide.eq(newIndex));
                        }
                        else {
                            slide.eq(oldIndex).insertBefore(slide.eq(newIndex));
                        }
                        PageBuilderModel.editors.sliderItem.addIndexes(section);
                    },
                });
                $(container).disableSelection();
            },

            init: function (section) {
                PageBuilderModel.editors.sliderItem.activeFirstSlide(section);
                PageBuilderModel.editors.sliderItem.sortable(section);

                //
                if (section) {
                    section.on('click', '.js-add-slide-btn', function () {
                        PageBuilderModel.editors.sliderItem.add($(this));
                    });
                    section.on('click', '.js-remove-slide-btn', function () {
                        PageBuilderModel.editors.sliderItem.remove($(this));
                    });
                    section.on('click', '.js-go-to-slide-btn', function () {
                        PageBuilderModel.editors.sliderItem.goToSlide($(this));
                    });
                }
            },
        },

        accordionItem: {
            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemsRow = section.find('.js-t63-accordion-items-row');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.accordionItem()
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsRow.append(PageBuilderModel.sections.accordion.items.getHtml(model));

                PageBuilderModel.editors.title.tinymce.init(itemsRow);
                PageBuilderModel.editors.text.tinymce.init(itemsRow);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                container.remove();
            }
        },

        tabItem: {
            partial:
                `<div class="controls popover js-section-controls hidden">
                    <div class="d-flex align-items-center justify-content-between">
                        <label class="form-label">tab</label>
                        <label class="toggler toggler-sm js-tab-visibility-toggler">
                            <span>Is Active</span>
                            <input type="checkbox" {{#if isActive}}checked{{/if}}>
                            <i></i>
                        </label>
                    </div>
                    <div class="form-group">
                        <input type="text" class="form-control js-name-input" placeholder="Tab name" value="{{name}}">
                    </div>

                    <div class="d-flex align-items-center justify-content-between">
                        <label class="form-label">FontAwesome</label>
                        <label class="toggler toggler-sm js-faIcon-toggler">
                            <span>Is Active {{components.fontAwesomeIcon.isActive}}</span>
                            <input type="checkbox" {{#if components.fontAwesomeIcon.isActive}}checked{{/if}}>
                            <i></i>
                        </label>
                    </div>
                    <div class="form-group">
                        <input type="text" class="form-control js-faIcon-name-input" placeholder="faIcon name" value="{{components.fontAwesomeIcon.name}}">
                    </div>
                </div>`,

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var name = container.find('.js-name-input').val();
                var isActive = container.find('.js-tab-visibility-toggler input').is(':checked');

                var fontAwesomeIcon = {
                    isActive: container.find('.js-faIcon-toggler input:checked').length > 0,
                    name: container.find('.js-faIcon-name-input').val()
                };

                //
                container.find('.js-tab-item-name').html(name);

                if (fontAwesomeIcon.name.indexOf('class="') > -1) {
                    fontAwesomeIcon.name = fontAwesomeIcon.name.slice(fontAwesomeIcon.name.indexOf('"') + 1, fontAwesomeIcon.name.lastIndexOf('"'))
                }
                container.find('.js-tab-item-icon-wrap i').removeAttr('class').addClass(fontAwesomeIcon.name);

                if (isActive) {
                    container.removeClass('component-disabled');
                } else {
                    container.addClass('component-disabled');
                }

                if (fontAwesomeIcon.isActive) {
                    container.find('.js-tab-item-icon-wrap').removeClass('component-disabled');
                }
                else {
                    container.find('.js-tab-item-icon-wrap').addClass('component-disabled');
                }

                //
                PageBuilderModel.editors.hide(container);
            },

            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var nav = section.find('.js-tab-nav');
                var content = section.find('.js-tab-content');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.tabItem()
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                nav.append(PageBuilderModel.sections.tabs.navItems.getHtml(model));
                content.append(PageBuilderModel.sections.tabs.contentItems.getHtml(model));

                PageBuilderModel.editors.text.tinymce.init(content);

                PageBuilderModel.plugins.tabs.show(nav.find('.js-tab-nav-item:last-child a'));

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                var section = container.closest('.js-page-section');
                var id = container.children('a').attr('href');

                $(id).remove();
                container.remove();

                PageBuilderModel.plugins.tabs.show(section.find('.js-tab-nav-item:first-child a'));
            }
        },

        servicesGridItem: {
            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemsRow = section.find('.js-t63-services-grid-row');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.servicesGridItem()
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsRow.append(PageBuilderModel.sections.services.items.getHtml(model));

                PageBuilderModel.editors.title.tinymce.init(itemsRow);
                PageBuilderModel.editors.text.tinymce.init(itemsRow);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                container.parent().remove();
            }
        },

        mediaObjectsGridItem: {
            partial:
                `<div class="controls popover js-section-controls hidden">
                    <div class="form-group">
                        <label class="form-label">Item size</label>
                        <div class="custom-input-group">
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="1" {{#js_if "this.size == 1"}}checked{{/js_if}}>
                                <span>Small</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="2" {{#js_if "this.size == 2"}}checked{{/js_if}}>
                                <span>Medium</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="3" {{#js_if "this.size == 3"}}checked{{/js_if}}>
                                <span>Large</span>
                            </label>
                        </div>
                    </div>
                </div>`,

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var itemSize = +container.find('.js-size-input:checked').val();

                container.attr('data-size', itemSize);
                container.parent().removeAttr('class').addClass(PageBuilderModel.settings.components.mediaObjectsGridItem().columnClassesNames[itemSize - 1]);

                //
                PageBuilderModel.editors.hide(container);
            },

            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemSize = +_this.attr('data-size');
                var itemsRow = section.find('.js-media-objects-grid-row');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.mediaObjectsGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.mediaObjectsGridItem().columnClassesNames[itemSize - 1],
                            size: itemSize,
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsRow.append(PageBuilderModel.sections.mediaObjectsGrid.items.getHtml(model));

                PageBuilderModel.editors.title.tinymce.init(itemsRow);
                PageBuilderModel.editors.text.tinymce.init(itemsRow);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                container.parent().remove();
            }
        },

        packagesGridItem: {
            partial:
                `<div class="controls popover js-section-controls hidden">
                    <div class="form-group">
                        <label class="form-label">Item size</label>
                        <div class="custom-input-group">
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="1" {{#js_if "this.size == 1"}}checked{{/js_if}}>
                                <span>Small</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="2" {{#js_if "this.size == 2"}}checked{{/js_if}}>
                                <span>Medium</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="3" {{#js_if "this.size == 3"}}checked{{/js_if}}>
                                <span>Large</span>
                            </label>
                        </div>
                    </div>
                </div>`,

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var itemSize = +container.find('.js-size-input:checked').val();

                container.attr('data-size', itemSize);
                container.parent().removeAttr('class').addClass(PageBuilderModel.settings.components.packagesGridItem().columnClassesNames[itemSize - 1]);

                //
                PageBuilderModel.editors.hide(container);
            },

            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemSize = +_this.attr('data-size');
                var itemsRow = section.find('.js-packages-grid-row');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.packagesGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.packagesGridItem().columnClassesNames[itemSize - 1],
                            size: itemSize,
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsRow.append(PageBuilderModel.sections.packagesGrid.items.getHtml(model));

                PageBuilderModel.editors.title.tinymce.init(itemsRow);
                PageBuilderModel.editors.text.tinymce.init(itemsRow);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                container.parent().remove();
            }
        },

        flipCardsGridItem: {
            partial:
                `<div class="controls popover js-section-controls hidden">
                    <div class="form-group">
                        <label class="form-label">Item size</label>
                        <div class="custom-input-group">
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="1" {{#js_if "this.size == 1"}}checked{{/js_if}}>
                                <span>Small</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="2" {{#js_if "this.size == 2"}}checked{{/js_if}}>
                                <span>Medium</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="3" {{#js_if "this.size == 3"}}checked{{/js_if}}>
                                <span>Large</span>
                            </label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Item Rotate event</label>
                        <div class="custom-input-group">
                            <label>
                                <input class="js-flip-card-item-rotate-event-input" type="radio" name="rotate-event-{{id}}" value="false" {{#unless rotateOnClick}}checked{{/unless}}>
                                <span>Hover</span>
                            </label>
                            <label>
                                <input class="js-flip-card-item-rotate-event-input" type="radio" name="rotate-event-{{id}}" value="true" {{#if rotateOnClick}}checked{{/if}}>
                                <span>Click</span>
                            </label>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Media</label>
                        <div class="custom-input-group">
                            <label>
                                <input class="js-flip-card-item-media-input" type="radio" name="media-{{id}}" value="image" {{#if hasImage}}checked{{/if}}>
                                <span>Image</span>
                            </label>
                            <label>
                                <input class="js-flip-card-item-media-input" type="radio" name="media-{{id}}" value="icon" {{#if hasIcon}}checked{{/if}}>
                                <span>Icon</span>
                            </label>
                        </div>
                    </div>
                </div>
                <button class="editor-btn-rounded js-flip-card-item-rotate-btn js-additional-action-buttons"><img src="/plugins/63bits-pageBuilder/images/icons/rotate.svg" alt="settings"></button>`,

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var itemSize = +container.find('.js-size-input:checked').val();
                container.attr('data-size', itemSize);
                container.parent().removeAttr('class').addClass(PageBuilderModel.settings.components.flipCardsGridItem().columnClassesNames[itemSize - 1]);

                var mediaType;
                if (container.find('.js-flip-card-item-media-input:checked').val() === 'image') {
                    mediaType = 'image';
                } else {
                    ;
                    mediaType = 'icon';
                }
                container.find('.js-media-container').attr('data-media-type', mediaType);

                var isClickable = container.find('.js-flip-card-item-rotate-event-input:checked').val();
                container.attr('data-clickable', isClickable);

                //
                PageBuilderModel.editors.hide(container);
            },

            changeMediaElement: function (container) {
                var html, mediaType;
                if (container.find('.js-flip-card-item-media-input:checked').val() === 'image') {
                    //html = PageBuilderModel.sections.components.image.helper(PageBuilderModel.settings.components.flipCardsGridItem().components.image, PageBuilderModel.editors.actionButtons.settings.image);
                    mediaType = 'image';
                } else {
                    //html = PageBuilderModel.sections.components.fontAwesomeIcon.helper(PageBuilderModel.settings.components.flipCardsGridItem().components.fontAwesomeIcon, PageBuilderModel.editors.actionButtons.settings.fontAwesomeIcon);
                    mediaType = 'icon';
                }

                container.find('.js-media-container').attr('data-media-type', mediaType).html(html)
            },

            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemSize = +_this.attr('data-size');
                var itemsRow = section.find('.js-flip-cards-grid-row');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.flipCardsGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.flipCardsGridItem().columnClassesNames[itemSize - 1],
                            size: itemSize,
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsRow.append(PageBuilderModel.sections.flipCardsGrid.items.getHtml(model));

                PageBuilderModel.editors.title.tinymce.init(itemsRow);
                PageBuilderModel.editors.text.tinymce.init(itemsRow);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                container.parent().remove();
            }
        },

        pdfViewerIframe: {
            helper: function (model) {
                return (
                    `<div class="controls popover js-section-controls hidden">
                        <div class="d-flex align-items-center justify-content-between">
                            <label class="toggler toggler-sm js-pdfViewerIframe-state-toggler">
                                <input type="checkbox" ${model.downloadEnabled ? 'checked' : ''}>
                                <i></i>
                                <span>Enable Download</span>
                            </label>
                            <button class="btn btn-sm btn-primary js-pdf-upload-btn">Choose</button>
                        </div>
                        <input class="js-pdfViewerIframe-input" type="hidden" value="${model.url}"/>
                    </div>`
                )
            },

            fileManager: {
                onSelectedFilesChooseClientCallback: function (files) {
                    var container = PageBuilderModel.editors.pdfViewerIframe.container;

                    if (files) {
                        var urlDownload = files[0].urlDownload; //.replace(/'|`| /g, '')
                        container.find('.js-pdfViewerIframe-input').val(urlDownload);
                    }

                    PageBuilderModel.plugins.fileManager.hide();
                    PageBuilderModel.editors.pdfViewerIframe.done(container)
                }
            },

            container: '',

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);

                PageBuilderModel.editors.pdfViewerIframe.container = container;

                container.find('.js-pdf-upload-btn').off('click');
                container.find('.js-pdf-upload-btn').click(function () {
                    PageBuilderModel.plugins.fileManager.show(PageBuilderModel.plugins.pdfViewer.getFileUrl());
                });
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var pdfUrl = $.trim(container.find('.js-pdfViewerIframe-input').val());
                var downloadEnabled = container.find('.js-pdfViewerIframe-state-toggler input').is(':checked');

                var url = PageBuilderModel.plugins.pdfViewer.url + '?CanDownload=' + downloadEnabled + '&UrlPdfFile=' + pdfUrl;

                if (pdfUrl !== '') {
                    container.find('.js-pdfViewerIframe-container iframe').attr({ 'src': url, 'data-pdf-url': pdfUrl, 'data-pdfviewer-url': url });
                }

                //
                PageBuilderModel.editors.hide(container);
            }
        },

        videoGridItem: {
            partial:
                `<div class="controls popover js-section-controls hidden">
                    <div class="form-group">
                        <label class="form-label">Item size</label>
                        <div class="custom-input-group">
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="1" {{#js_if "this.size == 1"}}checked{{/js_if}}>
                                <span>Small</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="2" {{#js_if "this.size == 2"}}checked{{/js_if}}>
                                <span>Medium</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="3" {{#js_if "this.size == 3"}}checked{{/js_if}}>
                                <span>Large</span>
                            </label>
                        </div>
                    </div>
                </div>`,

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var itemSize = +container.find('.js-size-input:checked').val();


                container.attr('data-size', itemSize);
                container.parent().removeAttr('class').addClass(PageBuilderModel.settings.components.videoGridItem().columnClassesNames[itemSize - 1]);

                //
                PageBuilderModel.editors.hide(container);
            },

            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemSize = +_this.attr('data-size');
                var itemsRow = section.find('.js-video-grid-row');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.videoGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.videoGridItem().columnClassesNames[itemSize - 1],
                            size: itemSize,
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsRow.append(PageBuilderModel.sections.videoGrid.items.getHtml(model));

                //PageBuilderModel.sections.jwPlayerGrid.sortable(itemsRow);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                container.parent().remove();
            }
        },

        jwPlayerGridItem: {
            partial:
                `<div class="controls popover js-section-controls hidden">
                    <div class="form-group">
                        <label class="form-label">Item size</label>
                        <div class="custom-input-group">
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="1" {{#js_if "this.size == 1"}}checked{{/js_if}}>
                                <span>Small</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="2" {{#js_if "this.size == 2"}}checked{{/js_if}}>
                                <span>Medium</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="3" {{#js_if "this.size == 3"}}checked{{/js_if}}>
                                <span>Large</span>
                            </label>
                        </div>
                    </div>
                </div>`,

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var itemSize = +container.find('.js-size-input:checked').val();


                container.attr('data-size', itemSize);
                container.parent().removeAttr('class').addClass(PageBuilderModel.settings.components.jwPlayerGridItem().columnClassesNames[itemSize - 1]);

                //
                PageBuilderModel.editors.hide(container);
            },

            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemSize = +_this.attr('data-size');
                var itemsRow = section.find('.js-jwp-grid-row');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.jwPlayerGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.jwPlayerGridItem().columnClassesNames[itemSize - 1],
                            size: itemSize,
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsRow.append(PageBuilderModel.sections.jwPlayerGrid.items.getHtml(model));

                //--- init player
                var itemID = model.items[0].id;
                var jwPayerWrap = $('[data-id="' + itemID + '"] .js-jwPlayer-wrap');

                new VideoPlayer({
                    Selector: jwPayerWrap.children('div'),
                    File: jwPayerWrap.attr('data-video-url'),
                    Image: jwPayerWrap.attr('data-cover-url')
                }).Init();

                //PageBuilderModel.sections.jwPlayerGrid.sortable(itemsRow);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                container.parent().remove();
            }
        },

        postCardsGridItem: {
            partial:
                `<div class="controls popover js-section-controls hidden">
                    <div class="form-group">
                        <label class="form-label">Item size</label>
                        <div class="custom-input-group">
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="1" {{#js_if "this.size == 1"}}checked{{/js_if}}>
                                <span>Small</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="2" {{#js_if "this.size == 2"}}checked{{/js_if}}>
                                <span>Medium</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="3" {{#js_if "this.size == 3"}}checked{{/js_if}}>
                                <span>Large</span>
                            </label>
                        </div>
                    </div>
                </div>`,

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var itemSize = +container.find('.js-size-input:checked').val();

                container.attr('data-size', itemSize);
                container.parent().removeAttr('class').addClass(PageBuilderModel.settings.components.postCardsGridItem().columnClassesNames[itemSize - 1]);

                //
                PageBuilderModel.editors.hide(container);
            },

            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemSize = +_this.attr('data-size');
                var itemsRow = section.find('.js-post-cards-grid-row');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.postCardsGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.postCardsGridItem().columnClassesNames[itemSize - 1],
                            size: itemSize,
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsRow.append(PageBuilderModel.sections.postCardsGrid.items.getHtml(model));

                PageBuilderModel.editors.title.tinymce.init(itemsRow);
                PageBuilderModel.editors.text.tinymce.init(itemsRow);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                container.parent().remove();
            }
        },

        booksGridItem: {
            partial:
                `<div class="controls popover js-section-controls hidden">
                    <div class="form-group">
                        <label class="form-label">Item size</label>
                        <div class="custom-input-group">
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="1" {{#js_if "this.size == 1"}}checked{{/js_if}}>
                                <span>Small</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="2" {{#js_if "this.size == 2"}}checked{{/js_if}}>
                                <span>Medium</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="3" {{#js_if "this.size == 3"}}checked{{/js_if}}>
                                <span>Large</span>
                            </label>
                        </div>
                    </div>
                </div>`,

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var itemSize = +container.find('.js-size-input:checked').val();

                container.attr('data-size', itemSize);
                container.parent().removeAttr('class').addClass(PageBuilderModel.settings.components.booksGridItem().columnClassesNames[itemSize - 1]);

                //
                PageBuilderModel.editors.hide(container);
            },

            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemSize = +_this.attr('data-size');
                var itemsRow = section.find('.js-books-grid-row');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.booksGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.booksGridItem().columnClassesNames[itemSize - 1],
                            size: itemSize,
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsRow.append(PageBuilderModel.sections.booksGrid.items.getHtml(model));

                PageBuilderModel.editors.title.tinymce.init(itemsRow);
                PageBuilderModel.editors.text.tinymce.init(itemsRow);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                container.parent().remove();
            }
        },

        peopleGridItem: {
            partial:
                `<div class="controls popover js-section-controls hidden">
                    <div class="form-group">
                        <label class="form-label">Item size</label>
                        <div class="custom-input-group">
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="1" {{#js_if "this.size == 1"}}checked{{/js_if}}>
                                <span>Small</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="2" {{#js_if "this.size == 2"}}checked{{/js_if}}>
                                <span>Medium</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="3" {{#js_if "this.size == 3"}}checked{{/js_if}}>
                                <span>Large</span>
                            </label>
                        </div>
                    </div>
                </div>`,

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var itemSize = +container.find('.js-size-input:checked').val();

                container.attr('data-size', itemSize);
                container.parent().removeAttr('class').addClass(PageBuilderModel.settings.components.peopleGridItem().columnClassesNames[itemSize - 1]);

                //
                PageBuilderModel.editors.hide(container);
            },

            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemSize = +_this.attr('data-size');
                var itemsRow = section.find('.js-people-grid-row');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.peopleGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.peopleGridItem().columnClassesNames[itemSize - 1],
                            size: itemSize,
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsRow.append(PageBuilderModel.sections.peopleGrid.items.getHtml(model));

                PageBuilderModel.editors.title.tinymce.init(itemsRow);
                PageBuilderModel.editors.text.tinymce.init(itemsRow);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                container.parent().remove();
            }
        },

        cardsGridItem: {
            partial:
                `<div class="controls popover js-section-controls hidden">
                    <div class="form-group">
                        <label class="form-label">Item size</label>
                        <div class="custom-input-group">
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="1" {{#js_if "this.size == 1"}}checked{{/js_if}}>
                                <span>Small</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="2" {{#js_if "this.size == 2"}}checked{{/js_if}}>
                                <span>Medium</span>
                            </label>
                            <label>
                                <input class="js-size-input" type="radio" name="size-{{id}}" value="3" {{#js_if "this.size == 3"}}checked{{/js_if}}>
                                <span>Large</span>
                            </label>
                        </div>
                    </div>
                </div>`,

            edit: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                PageBuilderModel.editors.show(container);
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);

                //
                var itemSize = +container.find('.js-size-input:checked').val();

                container.attr('data-size', itemSize);
                container.parent().removeAttr('class').addClass(PageBuilderModel.settings.components.cardsGridItem().columnClassesNames[itemSize - 1]);

                //
                PageBuilderModel.editors.hide(container);
            },

            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemSize = +_this.attr('data-size');
                var itemsRow = section.find('.js-cards-grid-row');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.cardsGridItem(),
                            id: PageBuilderModel.guid.s15(),
                            columnClassesNames: PageBuilderModel.settings.components.cardsGridItem().columnClassesNames[itemSize - 1],
                            size: itemSize,
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsRow.append(PageBuilderModel.sections.cardsGrid.items.getHtml(model));

                PageBuilderModel.editors.title.tinymce.init(itemsRow);
                PageBuilderModel.editors.text.tinymce.init(itemsRow);

                //animations
                PageBuilderModel.editors.section.initAnimations(section);
            },

            remove: function (container) {
                container.parent().remove();
            }
        },

        done: function () {
            $('.js-done-btn:visible').trigger('click');
        },

        show: function (container) {
            container.children('.js-section-controls').removeClass('hidden');
        },

        hide: function (container) {
            container.find('.js-section-controls:not(.image-controls)').addClass('hidden');
        },

        remove: function (container) {
            container = container || $(PageBuilderModel.sections.selector);
            container.find('.js-section--container[data-container="section"], .js-section-controls, .js-action-buttons, .js-additional-action-buttons').remove();
            container.find('.js-tinymce')
                .removeClass('js-tinymce mce-content-body mce-edit-focus')
                .removeAttr('style id contenteditable spellcheck');

            container.find('.js-slider-settings-container').remove();
            container.find('.js-t63-slider-container > div').removeAttr('class');
        },

        init: function () {
            //
            $(PageBuilderModel.currentViewSelector).on('click', '.js-edit-btn', function () {
                var container = $(this).closest('.js-section--container');
                var containerName = container.attr('data-container');

                PageBuilderModel.editors[containerName].edit(container);

                //
                if (containerName !== 'imgGridItem') {
                    container.find('input[type="text"]').first().select().focus();
                }

                //
                $('body').click(function (e) {
                    if (!$(e.target).closest('.js-action-buttons').length && !$(e.target).hasClass('.js-section-controls') && !$(e.target).closest('.js-section-controls').length) {
                        PageBuilderModel.editors.done();
                        $('body').off('click');
                    }
                });
            });
            $(PageBuilderModel.currentViewSelector).on('click', '.js-done-btn', function () {
                var container = $(this).closest('.js-section--container');
                var containerName = container.attr('data-container');

                PageBuilderModel.editors[containerName].done(container);
            });
            $(PageBuilderModel.currentViewSelector).on('click', '.js-remove-btn', function () {
                var container = $(this).closest('.js-section--container');
                var containerName = container.attr('data-container');

                PageBuilderModel.editors[containerName].remove(container);
            });

            //
            $(PageBuilderModel.currentViewSelector).on('click', '.js-choose-file-btn', function () {
                var section = $(this).closest('.js-page-section');

                PageBuilderModel.editors.file.choose(section);
            });

            //--- add grid/list item
            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-grid-col-btn', function () {
                PageBuilderModel.editors.imgGridItem.add($(this));
            });

            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-articles-grid-item-btn', function () {
                PageBuilderModel.editors.articlesGridItem.add($(this));
            });

            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-articles-list-item-btn', function () {
                PageBuilderModel.editors.articlesListItem.add($(this));
            });

            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-services-grid-item-btn', function () {
                PageBuilderModel.editors.servicesGridItem.add($(this));
            });

            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-media-objects-grid-col-btn', function () {
                PageBuilderModel.editors.mediaObjectsGridItem.add($(this));
            });

            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-packages-grid-item-btn', function () {
                PageBuilderModel.editors.packagesGridItem.add($(this));
            });

            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-tab-item-btn', function () {
                PageBuilderModel.editors.tabItem.add($(this));
            });

            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-video-grid-col-btn', function () {
                PageBuilderModel.editors.videoGridItem.add($(this));
            });

            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-jwp-grid-col-btn', function () {
                PageBuilderModel.editors.jwPlayerGridItem.add($(this));
            });

            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-post-cards-grid-col-btn', function () {
                PageBuilderModel.editors.postCardsGridItem.add($(this));
            });

            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-books-grid-col-btn', function () {
                PageBuilderModel.editors.booksGridItem.add($(this));
            });

            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-people-grid-col-btn', function () {
                PageBuilderModel.editors.peopleGridItem.add($(this));
            });

            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-cards-grid-col-btn', function () {
                PageBuilderModel.editors.cardsGridItem.add($(this));
            });

            //--- add accordion item
            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-accordion-item-btn', function () {
                PageBuilderModel.editors.accordionItem.add($(this));
            });

            //
            $(PageBuilderModel.currentViewSelector).on('change', '.js-component-visibility-toggler input', function () {
                var container = $(this).closest('[data-container]');
                if ($(this).is(':checked')) {
                    container.removeClass('component-disabled');
                } else {
                    container.addClass('component-disabled');
                }
            });

            //--- simple text
            $(PageBuilderModel.currentViewSelector).on('blur', '.js-plain-text-input', function () {
                var container = $(this).closest('.js-section--container');
                var containerName = container.attr('data-container');

                PageBuilderModel.editors[containerName].done(container);
            });

            //--- flip card
            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-flip-cards-grid-item-btn', function () {
                PageBuilderModel.editors.flipCardsGridItem.add($(this));
            });

            $(PageBuilderModel.currentViewSelector).on('click', '.js-flip-card-item-rotate-btn', function () {
                $(this).closest('.js-flip-cards-grid-item').toggleClass('flip-card-item--rotate');
            });

            //--- full screen
            $(PageBuilderModel.currentViewSelector).on('change', '.js-fullscreen-toggler input', function () {
                var section = $(this).closest('.js-page-section');
                if ($(this).is(':checked')) {
                    section.find('.js-fullheight-toggler').removeClass('disabled');
                } else {
                    section.find('.js-fullheight-toggler').addClass('disabled').find('input').prop('checked', false);
                }
            });
        }
    },

    pageScripts: {
        footerSectionHtml: null,

        isUpdated: false,

        fileManager: {
            onSelectedFilesChooseClientCallback: function (files) {
                PageBuilderModel.pageScripts.files.addFromFileManager(files);
            }
        },

        files: {
            partial:
                `{{#each items}}
                    <li class="file-item" data-url="{{url}}" data-name="{{name}}" data-extension="{{extension}}">
                        <i class="handle ui-sortable-handle"><img src="/plugins/63bits-pageBuilder/images/icons/drug.svg" alt="drag icon"></i>
                        <img class="icon" src="/plugins/63bits-pageBuilder/images/icons/{{extension}}.svg" alt="file icon">
                        <div class="name">
                            <a class="link js-file-name" href="{{url}}" target="blank">{{name}}</a>
                        </div>
                        <div class="action-buttons">
                            <button class="editor-btn-rounded action-btn component-remove-btn js-remove-btn"></button>
                        </div>
                    </li>
				{{/each}}`,

            template: '{{> cssJsFilesPartial}}',

            getHtml: function (model) {
                return PageBuilderModel.pageScripts.files.template(model);
            },

            addFromFileManager: function (files) {
                if (files) {

                    var model = {
                        css: {
                            items: []
                        },
                        js: {
                            items: []
                        }
                    };

                    var urlDownload, filename, fileExtension, isAllowedFiles;

                    for (var i = 0; i < files.length; i++) {
                        urlDownload = files[i].urlDownload;
                        filename = files[i].name;
                        fileExtension = filename.slice(filename.lastIndexOf('.') + 1);

                        isAllowedFiles =
                            $('.js-' + fileExtension + '-files-container [data-name="' + filename + '"]').length == 0
                            && !filename.includes('bootstrap.css')
                            && !filename.includes('bootstrap.min.css')
                            && !filename.includes('jquery.js')
                            && !filename.includes('jquery.min.js')
                            && !filename.includes('jquery-ui.js')
                            && !filename.includes('jquery-ui.min.js')
                            && !filename.includes('bootstrap.js')
                            && !filename.includes('bootstrap.min.js');

                        if (isAllowedFiles) {
                            model[fileExtension].items[i] = {
                                url: urlDownload,
                                name: filename,
                                extension: fileExtension
                            };
                        } else {
                            alert(filename + ' already exist');
                        }
                    }

                    $('.js-' + fileExtension + '-files-container').append(PageBuilderModel.pageScripts.files.getHtml(model[fileExtension]));

                    //PageBuilderModel.pageScripts.isUpdated = true;
                }
                //
                PageBuilderModel.plugins.fileManager.hide();
            },

            addOnLoad: function () {
                if (PageBuilderModel.data.fromServer.pageScripts?.headerSection) {
                    var headerScriptsModel = {
                        items: PageBuilderModel.data.fromServer.pageScripts.headerSection
                    };
                    $('.js-css-files-container').append(PageBuilderModel.pageScripts.files.getHtml(headerScriptsModel));
                }
                if (PageBuilderModel.data.fromServer.pageScripts?.footerSection) {
                    var footerScriptsModel = {
                        items: PageBuilderModel.data.fromServer.pageScripts.footerSection
                    }
                    $('.js-js-files-container').append(PageBuilderModel.pageScripts.files.getHtml(footerScriptsModel));
                }
            },

            remove: function (_this) {
                _this.closest('li').remove();
                PageBuilderModel.pageScripts.isUpdated = true;
            },

            sortable: function () {
                $('.js-page-scripts-popup .js-sortable').sortable({
                    handle: '.ui-sortable-handle',
                    placeholder: 'file-item-placeholder',
                });
                $('.js-page-scripts-popup .js-sortable').disableSelection();
            }
        },

        getHeaderSectionHtml: function () {
            var html = '';

            $('.js-css-files-container li').each(function (inedx, item) {
                html += '<link rel="stylesheet" href="' + $(item).attr('data-url') + '" />';
            });

            return html;
        },

        getFooterSectionHtml: function () {
            var html = '';

            $('.js-js-files-container li').each(function (inedx, item) {
                html += '<script src="' + $(item).attr('data-url') + '"></script>';
            });

            return html;
        },

        appendFooterSectionToBody: function () {
            PageBuilderModel.data.fromServer.pageScripts.footerSection.forEach(function (item) {
                $('body').append('<script src="' + item.url + '"></script>');
            });
        },

        init: function () {
            PageBuilderModel.pageScripts.files.addOnLoad();
            PageBuilderModel.pageScripts.files.sortable();

            //--- toggle page settings popup
            $('.js-show-page-scripts-popup-btn').click(function () {
                $('.js-page-scripts-popup').addClass('show');
            });
            $('.js-page-scripts-popup .js-close-popup-btn').click(function () {
                $('.js-page-scripts-popup').removeClass('show');
            });

            //--- choose file
            $('.js-choose-css-files-btn').click(function () {
                PageBuilderModel.plugins.fileManager.show(PageBuilderModel.plugins.fileManager.getCssFilesUrl());
            });

            $('.js-choose-js-files-btn').click(function () {
                PageBuilderModel.plugins.fileManager.show(PageBuilderModel.plugins.fileManager.getJsFilesUrl());
            });

            //--- remove file
            $('.js-page-scripts-popup').on('click', '.js-remove-btn', function () {
                PageBuilderModel.pageScripts.files.remove($(this));
            });
        }
    },

    compileTemplates: function () {
        //--- components heplers
        Template7.registerHelper('titleHepler', PageBuilderModel.sections.components.title.helper);
        Template7.registerHelper('textHepler', PageBuilderModel.sections.components.text.helper);
        Template7.registerHelper('textPlainHelper', PageBuilderModel.sections.components.textPlain.helper);
        Template7.registerHelper('buttonHepler', PageBuilderModel.sections.components.button.helper);
        Template7.registerHelper('imageHepler', PageBuilderModel.sections.components.image.helper);
        Template7.registerHelper('bgImageHepler', PageBuilderModel.sections.components.bgImage.helper);
        Template7.registerHelper('iconHepler', PageBuilderModel.sections.components.icon.helper);
        Template7.registerHelper('fontAwesomeIconHepler', PageBuilderModel.sections.components.fontAwesomeIcon.helper);
        Template7.registerHelper('videoHepler', PageBuilderModel.sections.components.video.helper);
        Template7.registerHelper('jwPlayerHepler', PageBuilderModel.sections.components.jwPlayer.helper);
        Template7.registerHelper('multimediaHepler', PageBuilderModel.sections.components.multimedia.helper);

        //--- actionButtons & editors heplers
        Template7.registerHelper('actionButtonsHepler', PageBuilderModel.editors.actionButtons.helper);
        Template7.registerHelper('titleControlsHepler', PageBuilderModel.editors.title.helper);
        Template7.registerHelper('textControlsHepler', PageBuilderModel.editors.text.helper);
        Template7.registerHelper('textPlainControlsHepler', PageBuilderModel.editors.textPlain.helper);
        Template7.registerHelper('buttonControlsHepler', PageBuilderModel.editors.button.helper);
        Template7.registerHelper('iconControlsHepler', PageBuilderModel.editors.icon.helper);
        Template7.registerHelper('fontAwesomeIconControlsHepler', PageBuilderModel.editors.fontAwesomeIcon.helper);
        Template7.registerHelper('imageControlsHepler', PageBuilderModel.editors.image.helper);
        Template7.registerHelper('videoControlsHepler', PageBuilderModel.editors.video.helper);
        Template7.registerHelper('jwPlayerControlsHepler', PageBuilderModel.editors.jwPlayer.helper);
        Template7.registerHelper('iframeControlsHepler', PageBuilderModel.editors.iframe.helper);
        Template7.registerHelper('pdfViewerIframeControlsHepler', PageBuilderModel.editors.pdfViewerIframe.helper);
        //Template7.registerHelper('multimediaControlsHepler', PageBuilderModel.editors.multimedia.helper);

        //components partials
        Template7.registerPartial('titlePartial', PageBuilderModel.sections.components.title.partial);
        Template7.registerPartial('textPartial', PageBuilderModel.sections.components.text.partial);
        Template7.registerPartial('textPlainPartial', PageBuilderModel.sections.components.textPlain.partial);
        Template7.registerPartial('buttonPartial', PageBuilderModel.sections.components.button.partial);
        Template7.registerPartial('iconPartial', PageBuilderModel.sections.components.icon.partial);
        Template7.registerPartial('fontAwesomeIconPartial', PageBuilderModel.sections.components.fontAwesomeIcon.partial);
        Template7.registerPartial('imagePartial', PageBuilderModel.sections.components.image.partial);
        Template7.registerPartial('bgImagePartial', PageBuilderModel.sections.components.bgImage.partial);
        Template7.registerPartial('videoPartial', PageBuilderModel.sections.components.video.partial);
        Template7.registerPartial('jwPlayerPartial', PageBuilderModel.sections.components.jwPlayer.partial);
        Template7.registerPartial('iframePartial', PageBuilderModel.sections.components.iframe.partial);
        Template7.registerPartial('attachemetsContainerPartial', PageBuilderModel.sections.components.attachements.containerPartial);
        Template7.registerPartial('filesPartial', PageBuilderModel.sections.components.attachements.files.partial);
        Template7.registerPartial('imgGridItemsPartial', PageBuilderModel.sections.imgGrid.items.partial);
        Template7.registerPartial('articlesGridItemsPartial', PageBuilderModel.sections.articlesGrid.items.partial);
        Template7.registerPartial('articlesListItemsPartial', PageBuilderModel.sections.articlesList.items.partial);
        Template7.registerPartial('sliderItemsPartial', PageBuilderModel.sections.slider.items.partial);
        Template7.registerPartial('accordionItemsPartial', PageBuilderModel.sections.accordion.items.partial);
        Template7.registerPartial('tabNavItemsPartial', PageBuilderModel.sections.tabs.navItems.partial);
        Template7.registerPartial('tabContentItemsPartial', PageBuilderModel.sections.tabs.contentItems.partial);
        Template7.registerPartial('testimonialsItemsPartial', PageBuilderModel.sections.testimonials.items.partial);
        Template7.registerPartial('servicesGridItemsPartial', PageBuilderModel.sections.services.items.partial);
        Template7.registerPartial('mediaObjectsGridItemsPartial', PageBuilderModel.sections.mediaObjectsGrid.items.partial);
        Template7.registerPartial('packagesGridItemsPartial', PageBuilderModel.sections.packagesGrid.items.partial);
        Template7.registerPartial('flipCardsGridItemsPartial', PageBuilderModel.sections.flipCardsGrid.items.partial);
        Template7.registerPartial('pdfViewerIframePartial', PageBuilderModel.sections.components.pdfViewerIframe.partial);
        Template7.registerPartial('videoGridItemsPartial', PageBuilderModel.sections.videoGrid.items.partial);
        Template7.registerPartial('jwPlayerGridItemsPartial', PageBuilderModel.sections.jwPlayerGrid.items.partial);
        Template7.registerPartial('postCardsGridItemsPartial', PageBuilderModel.sections.postCardsGrid.items.partial);
        Template7.registerPartial('booksGridItemsPartial', PageBuilderModel.sections.booksGrid.items.partial);
        Template7.registerPartial('peopleGridItemsPartial', PageBuilderModel.sections.peopleGrid.items.partial);
        Template7.registerPartial('cardsGridItemsPartial', PageBuilderModel.sections.cardsGrid.items.partial);

        //--- components templates
        PageBuilderModel.sections.components.title.template = Template7.compile(PageBuilderModel.sections.components.title.template);
        PageBuilderModel.sections.components.text.template = Template7.compile(PageBuilderModel.sections.components.text.template);
        PageBuilderModel.sections.components.textPlain.template = Template7.compile(PageBuilderModel.sections.components.textPlain.template);
        PageBuilderModel.sections.components.button.template = Template7.compile(PageBuilderModel.sections.components.button.template);
        PageBuilderModel.sections.components.icon.template = Template7.compile(PageBuilderModel.sections.components.icon.template);
        PageBuilderModel.sections.components.fontAwesomeIcon.template = Template7.compile(PageBuilderModel.sections.components.fontAwesomeIcon.template);
        PageBuilderModel.sections.components.image.template = Template7.compile(PageBuilderModel.sections.components.image.template);
        PageBuilderModel.sections.components.bgImage.template = Template7.compile(PageBuilderModel.sections.components.bgImage.template);
        PageBuilderModel.sections.components.video.template = Template7.compile(PageBuilderModel.sections.components.video.template);
        PageBuilderModel.sections.components.jwPlayer.template = Template7.compile(PageBuilderModel.sections.components.jwPlayer.template);
        PageBuilderModel.sections.components.attachements.files.template = Template7.compile(PageBuilderModel.sections.components.attachements.files.template);
        PageBuilderModel.sections.components.multimedia.template = Template7.compile(PageBuilderModel.sections.components.multimedia.template);
        PageBuilderModel.sections.articlesGrid.items.template = Template7.compile(PageBuilderModel.sections.articlesGrid.items.template);
        PageBuilderModel.sections.articlesList.items.template = Template7.compile(PageBuilderModel.sections.articlesList.items.template);
        PageBuilderModel.sections.imgGrid.items.template = Template7.compile(PageBuilderModel.sections.imgGrid.items.template);
        PageBuilderModel.sections.slider.items.template = Template7.compile(PageBuilderModel.sections.slider.items.template);
        PageBuilderModel.sections.accordion.items.template = Template7.compile(PageBuilderModel.sections.accordion.items.template);
        PageBuilderModel.sections.tabs.navItems.template = Template7.compile(PageBuilderModel.sections.tabs.navItems.template);
        PageBuilderModel.sections.tabs.contentItems.template = Template7.compile(PageBuilderModel.sections.tabs.contentItems.template);
        PageBuilderModel.sections.testimonials.items.template = Template7.compile(PageBuilderModel.sections.testimonials.items.template);
        PageBuilderModel.sections.services.items.template = Template7.compile(PageBuilderModel.sections.services.items.template);
        PageBuilderModel.sections.mediaObjectsGrid.items.template = Template7.compile(PageBuilderModel.sections.mediaObjectsGrid.items.template);
        PageBuilderModel.sections.packagesGrid.items.template = Template7.compile(PageBuilderModel.sections.packagesGrid.items.template);
        PageBuilderModel.sections.flipCardsGrid.items.template = Template7.compile(PageBuilderModel.sections.flipCardsGrid.items.template);
        PageBuilderModel.sections.videoGrid.items.template = Template7.compile(PageBuilderModel.sections.videoGrid.items.template);
        PageBuilderModel.sections.jwPlayerGrid.items.template = Template7.compile(PageBuilderModel.sections.jwPlayerGrid.items.template);
        PageBuilderModel.sections.postCardsGrid.items.template = Template7.compile(PageBuilderModel.sections.postCardsGrid.items.template);
        PageBuilderModel.sections.booksGrid.items.template = Template7.compile(PageBuilderModel.sections.booksGrid.items.template);
        PageBuilderModel.sections.peopleGrid.items.template = Template7.compile(PageBuilderModel.sections.peopleGrid.items.template);
        PageBuilderModel.sections.cardsGrid.items.template = Template7.compile(PageBuilderModel.sections.cardsGrid.items.template);

        //--- sections templates
        PageBuilderModel.sections.slide.template = Template7.compile(PageBuilderModel.sections.slide.template);
        PageBuilderModel.sections.slider.template = Template7.compile(PageBuilderModel.sections.slider.template);
        PageBuilderModel.sections.article.template = Template7.compile(PageBuilderModel.sections.article.template);
        PageBuilderModel.sections.article2Col.template = Template7.compile(PageBuilderModel.sections.article2Col.template);
        PageBuilderModel.sections.articleText2Col.template = Template7.compile(PageBuilderModel.sections.articleText2Col.template);
        PageBuilderModel.sections.article2ColWithImg.template = Template7.compile(PageBuilderModel.sections.article2ColWithImg.template);
        PageBuilderModel.sections.article2ColWithVideo.template = Template7.compile(PageBuilderModel.sections.article2ColWithVideo.template);
        PageBuilderModel.sections.article2ColWithJwPlayer.template = Template7.compile(PageBuilderModel.sections.article2ColWithJwPlayer.template);
        PageBuilderModel.sections.articleWithFiles.template = Template7.compile(PageBuilderModel.sections.articleWithFiles.template);
        PageBuilderModel.sections.articleWithVideo.template = Template7.compile(PageBuilderModel.sections.articleWithVideo.template);
        PageBuilderModel.sections.video.template = Template7.compile(PageBuilderModel.sections.video.template);
        PageBuilderModel.sections.jwPlayer.template = Template7.compile(PageBuilderModel.sections.jwPlayer.template);
        PageBuilderModel.sections.iframe.template = Template7.compile(PageBuilderModel.sections.iframe.template);
        PageBuilderModel.sections.imgGrid.template = Template7.compile(PageBuilderModel.sections.imgGrid.template);
        PageBuilderModel.sections.articlesGrid.template = Template7.compile(PageBuilderModel.sections.articlesGrid.template);
        PageBuilderModel.sections.articlesList.template = Template7.compile(PageBuilderModel.sections.articlesList.template);
        PageBuilderModel.sections.accordion.template = Template7.compile(PageBuilderModel.sections.accordion.template);
        PageBuilderModel.sections.tabs.template = Template7.compile(PageBuilderModel.sections.tabs.template);
        PageBuilderModel.sections.html.template = Template7.compile(PageBuilderModel.sections.html.template);
        PageBuilderModel.sections.testimonials.template = Template7.compile(PageBuilderModel.sections.testimonials.template);
        PageBuilderModel.sections.services.template = Template7.compile(PageBuilderModel.sections.services.template);
        PageBuilderModel.sections.pageHead.template = Template7.compile(PageBuilderModel.sections.pageHead.template);
        PageBuilderModel.sections.mediaObject.template = Template7.compile(PageBuilderModel.sections.mediaObject.template);
        PageBuilderModel.sections.mediaObjectsGrid.template = Template7.compile(PageBuilderModel.sections.mediaObjectsGrid.template);
        PageBuilderModel.sections.card2Col.template = Template7.compile(PageBuilderModel.sections.card2Col.template);
        PageBuilderModel.sections.flipCardsGrid.template = Template7.compile(PageBuilderModel.sections.flipCardsGrid.template);
        PageBuilderModel.sections.packagesGrid.template = Template7.compile(PageBuilderModel.sections.packagesGrid.template);
        PageBuilderModel.sections.quote.template = Template7.compile(PageBuilderModel.sections.quote.template);
        PageBuilderModel.sections.article2colWithBgImgAndImg.template = Template7.compile(PageBuilderModel.sections.article2colWithBgImgAndImg.template);
        PageBuilderModel.sections.pdfViewer.template = Template7.compile(PageBuilderModel.sections.pdfViewer.template);
        PageBuilderModel.sections.jwPlayerGrid.template = Template7.compile(PageBuilderModel.sections.jwPlayerGrid.template);
        PageBuilderModel.sections.videoGrid.template = Template7.compile(PageBuilderModel.sections.videoGrid.template);
        PageBuilderModel.sections.postCardsGrid.template = Template7.compile(PageBuilderModel.sections.postCardsGrid.template);
        PageBuilderModel.sections.booksGrid.template = Template7.compile(PageBuilderModel.sections.booksGrid.template);
        PageBuilderModel.sections.peopleGrid.template = Template7.compile(PageBuilderModel.sections.peopleGrid.template);
        PageBuilderModel.sections.cardsGrid.template = Template7.compile(PageBuilderModel.sections.cardsGrid.template);

        //--- actionButtons templates
        PageBuilderModel.editors.actionButtons.template = Template7.compile(PageBuilderModel.editors.actionButtons.template);

        //--- controls partials & tepmates
        Template7.registerPartial('sectionEditorContainerPartial', PageBuilderModel.editors.section.controls.partial);
        Template7.registerPartial('alignmentWrapPartial', PageBuilderModel.editors.section.controls.alignmentWrapPartial);
        Template7.registerPartial('fileControlsPartial', PageBuilderModel.editors.file.partial);
        Template7.registerPartial('imgGridItemControlsPartial', PageBuilderModel.editors.imgGridItem.partial);
        Template7.registerPartial('tabItemControlsPartial', PageBuilderModel.editors.tabItem.partial);
        Template7.registerPartial('mediaObjectsGridItemControlsPartial', PageBuilderModel.editors.packagesGridItem.partial);
        Template7.registerPartial('packagesGridItemControlsPartial', PageBuilderModel.editors.packagesGridItem.partial);
        Template7.registerPartial('flipCardsGridItemControlsPartial', PageBuilderModel.editors.flipCardsGridItem.partial);
        Template7.registerPartial('videoGridItemControlsPartial', PageBuilderModel.editors.videoGridItem.partial);
        Template7.registerPartial('jwPlayerGridItemControlsPartial', PageBuilderModel.editors.jwPlayerGridItem.partial);
        Template7.registerPartial('postCardsGridItemControlsPartial', PageBuilderModel.editors.postCardsGridItem.partial);
        Template7.registerPartial('booksGridItemControlsPartial', PageBuilderModel.editors.booksGridItem.partial);
        Template7.registerPartial('peopleGridItemControlsPartial', PageBuilderModel.editors.peopleGridItem.partial);
        Template7.registerPartial('cardsGridItemControlsPartial', PageBuilderModel.editors.cardsGridItem.partial);

        Template7.registerPartial('sliderItemsControlsPartial', PageBuilderModel.editors.sliderItem.controls.containerPartial);
        Template7.registerPartial('sliderItemsControlsItemPartial', PageBuilderModel.editors.sliderItem.controls.items.partial);

        PageBuilderModel.editors.sliderItem.controls.items.template = Template7.compile(PageBuilderModel.editors.sliderItem.controls.items.template);


        //--- scrollTo navigation
        PageBuilderModel.scrollToNavigation.container.template = Template7.compile(PageBuilderModel.scrollToNavigation.container.template);
        PageBuilderModel.scrollToNavigation.items.template = Template7.compile(PageBuilderModel.scrollToNavigation.items.template);


        //--- page settings popup
        Template7.registerPartial('cssJsFilesPartial', PageBuilderModel.pageScripts.files.partial);
        PageBuilderModel.pageScripts.files.template = Template7.compile(PageBuilderModel.pageScripts.files.template);
    },

    plugins: {
        tinymce: {
            init: function (selector, options) {
                $(selector).tinymce({
                    width: '100%',

                    menubar: 'edit view insert format tools table tc help',
                    menu: {
                        edit: { title: 'Edit', items: 'undo redo | cut copy paste | selectall | searchreplace' },
                        view: { title: 'View', items: 'code | visualchars visualblocks | spellchecker | preview fullscreen' },
                        insert: { title: 'Insert', items: 'image link media template codesample inserttable | charmap emoticons hr | pagebreak nonbreaking anchor toc | insertdatetime' },
                        format: { title: 'Format', items: 'bold italic underline strikethrough superscript subscript | ' + (options.block_formats !== null ? 'blockformats ' : '') + 'fontformats ' + (options.fontsize_formats !== null ? 'fontsizes ' : '') + 'align | forecolor backcolor | removeformat' },
                        tools: { title: 'Tools', items: 'code wordcount' },
                        table: { title: 'Table', items: 'inserttable | cell row column | tableprops deletetable' },
                        help: { title: 'Help', items: 'help' }
                    },

                    toolbar_items_size: 'small',
                    toolbar_mode: 'floating',
                    statusbar: false,
                    inline: true,

                    toolbar: options.toolbar,
                    toolbar_sticky: options.toolbar_sticky,
                    plugins: options.plugins + ' noneditable',
                    block_formats: options.block_formats,
                    paste_word_valid_elements: options.paste_word_valid_elements,

                    image_advtab: options.image_advtab,
                    file_picker_types: options.file_picker_types,
                    file_picker_callback: options.file_picker_callback,

                    force_br_newlines: false,
                    force_p_newlines: true,
                    forced_root_block: options.forced_root_block,

                    fontsize_formats: '10px 11px 12px 14px 16px 18px 20px 22px 24px 26px 30px',
                    font_formats: 'PT Serif=PT Serif,sans-serif; Avenir LT=Avenir LT; Dancing Script=Dancing Script,cursive',
                    indentation: '24px',

                    link_class_list: [
                        { title: 'None', value: '' },
                        { title: 'Primary', value: 'btn btn-primary' },
                        { title: 'Secondary', value: 'btn btn-secondary' }
                    ],

                    table_class_list: [
                        { title: 'None', value: '' },
                        { title: 'Plain', value: 'table-plain' },
                        { title: 'Striped', value: 'table-striped' },
                        { title: 'Light', value: 'table-light' },
                        { title: 'Dark', value: 'table-dark' },
                        { title: 'Light Striped', value: 'table-light table-striped' },
                        { title: 'Dark Striped', value: 'table-dark table-striped' }
                    ],
                    table_advtab: false,

                    contextmenu: 'link linkchecker image imagetools media table spellchecker configurepermanentpen bold italic',

                    remove_linebreaks: false,
                    remove_trailing_nbsp: false,
                    verify_html: false,
                    apply_source_formatting: true,
                    relative_urls: false,
                    remove_script_host: false,
                    convert_urls: false,
                    autoresize_bottom_margin: 0,
                    autoresize_min_height: 50,

                    formats: {
                        noTooltip: { inline: 'dfn', attributes: { title: 'Tooltip disabled' }, classes: 'no-tooltip' }
                    },
                    setup: function (editor) {
                        editor.ui.registry.addToggleButton('disableTooltip', {
                            icon: 'table-delete-table',
                            tooltip: 'Disable Tooltip',
                            onAction: function (button) {
                                editor.execCommand('mceToggleFormat', false, 'noTooltip');
                            },
                            onSetup: function (api) {
                                editor.formatter.formatChanged('noTooltip', function (state) {
                                    api.setActive(state);
                                });
                            }
                        });
                    },
                });
            },
        },
        fileManager: {
            url: null,
            getSingleImageUrl: function () {
                if (PageBuilderModel.plugins.fileManager.url) {
                    return PageBuilderModel.plugins.fileManager.url + '&AllowedExtensions=.jpg,.jpeg,.png,.gif&AllowSelectMultiple=false&OnSelectedFilesChooseClientCallback=PageBuilderModel.plugins.fileManager.onSelectedFilesChooseClientCallback';
                }
            },
            getSingleFileUrl: function () {
                if (PageBuilderModel.plugins.fileManager.url) {
                    return PageBuilderModel.plugins.fileManager.url + '&AllowSelectMultiple=false&OnSelectedFilesChooseClientCallback=PageBuilderModel.plugins.fileManager.onSelectedFilesChooseClientCallback';
                }
            },
            getJwPlayerUrl: function () {
                if (PageBuilderModel.plugins.fileManager.url) {
                    return PageBuilderModel.plugins.fileManager.url + '&AllowSelectMultiple=false&OnSelectedFilesChooseClientCallback=PageBuilderModel.editors.jwPlayer.fileManager.onSelectedFilesChooseClientCallback';
                }
            },
            getMultipleFilesUrl: function () {
                if (PageBuilderModel.plugins.fileManager.url) {
                    return PageBuilderModel.plugins.fileManager.url + '&AllowSelectMultiple=true&OnSelectedFilesChooseClientCallback=PageBuilderModel.plugins.fileManager.onSelectedFilesChooseClientCallback';
                }
            },
            getCssFilesUrl: function () {
                if (PageBuilderModel.plugins.fileManager.url) {
                    return PageBuilderModel.plugins.fileManager.url + '&AllowedExtensions=.css&AllowSelectMultiple=true&OnSelectedFilesChooseClientCallback=PageBuilderModel.pageScripts.fileManager.onSelectedFilesChooseClientCallback';
                }
            },
            getJsFilesUrl: function () {
                if (PageBuilderModel.plugins.fileManager.url) {
                    return PageBuilderModel.plugins.fileManager.url + '&AllowedExtensions=.js&AllowSelectMultiple=true&OnSelectedFilesChooseClientCallback=PageBuilderModel.pageScripts.fileManager.onSelectedFilesChooseClientCallback';
                }
            },
            getIconUrl: function () {
                if (PageBuilderModel.plugins.fileManager.url) {
                    return PageBuilderModel.plugins.fileManager.url + '&AllowedExtensions=.svg,.jpg,.jpeg,.png,.gif&AllowSelectMultiple=false&OnSelectedFilesChooseClientCallback=PageBuilderModel.editors.icon.fileManager.onSelectedFilesChooseClientCallback';
                }
            },
            getMultimediaFilesUrl: function () {
                if (PageBuilderModel.plugins.fileManager.url) {
                    return PageBuilderModel.plugins.fileManager.url + '&AllowedExtensions=.jpg,.jpeg,.png,.gif.mp4&AllowSelectMultiple=false&OnSelectedFilesChooseClientCallback=PageBuilderModel.editors.multimedia.fileManager.onSelectedFilesChooseClientCallback';
                }
            },
            file: {
                isImage: true,
            },
            isTinymce: false,
            tinymce: {
                isEnabled: false,
                callback: null,
                type: null
            },
            show: function (fileManagerUrl) {
                FancyBox.Init({
                    slideClass: 'fileManager-iframe',
                    src: fileManagerUrl,
                    smallBtn: true
                }).ShowIframePopup();
            },
            hide: function () {
                window.FancyBox.ClosePopup();
            },
            onSelectedFilesChooseClientCallback: function (files) {
                if (PageBuilderModel.plugins.fileManager.tinymce.isEnabled) {
                    if (PageBuilderModel.plugins.fileManager.tinymce.type == 'image') {
                        PageBuilderModel.editors.text.tinymce.addImage(files, PageBuilderModel.plugins.fileManager.tinymce.callback);
                    } else {
                        PageBuilderModel.editors.text.tinymce.addMedia(files, PageBuilderModel.plugins.fileManager.tinymce.callback);
                    }
                } else {
                    if (PageBuilderModel.plugins.fileManager.file.isImage) {
                        PageBuilderModel.editors.image.add(files);
                    } else {
                        PageBuilderModel.editors.file.add(files);
                    }
                }
            }
        },
        pdfViewer: {
            url: null,
            getFileUrl: function () {
                if (PageBuilderModel.plugins.fileManager.url) {
                    return PageBuilderModel.plugins.fileManager.url + '&AllowedExtensions=.pdf&AllowSelectMultiple=false&OnSelectedFilesChooseClientCallback=PageBuilderModel.editors.pdfViewerIframe.fileManager.onSelectedFilesChooseClientCallback';
                }
            },
        },
        tabs: {
            show: function (navItem) {
                var container = $(navItem).closest('.js-page-section');
                var id = $(navItem).attr('href');

                container.find('.js-tab-nav-item a').removeClass('active');
                container.find('.js-tab-content-item').removeClass('show').removeClass('active');

                $(navItem).addClass('active');
                $(id).addClass('active').addClass('show');
            },
            init: function () {
                $('.js-editor-page').on('click', '.js-tab-nav-item a', function (e) {
                    PageBuilderModel.plugins.tabs.show($(this));
                    return false;
                });
            }
        },
        init: function () {
            $('[data-toggle="tooltip"]').tooltip();
            PageBuilderModel.plugins.tabs.init();

            $('.js-show-filemanager-popup-btn').click(function () {
                if (PageBuilderModel.plugins.fileManager.url) {
                    PageBuilderModel.plugins.fileManager.show(PageBuilderModel.plugins.fileManager.url + '&AllowSelectMultiple=false')
                }
            });
        },
    },

    save: {
        getCleanHtml: function (Container) {
            var pageContent = $(Container).clone();
            pageContent = $(pageContent);
            PageBuilderModel.editors.remove(pageContent);
            pageContent.find('.component-disabled').remove();
            pageContent.find('.js-section-title-container:empty,.js-section-text-container:empty').remove();

            //icons
            pageContent.find('[data-icon-type="font"] .js-img-icon').remove();
            pageContent.find('[data-icon-type="image"] .js-font-icon').remove();

            //replace special img with audio
            pageContent.find('img.mce-object-audio').each(function (index, item) {
                var audioFile = $(item).attr('data-mce-p-src');
                $(item).replaceWith('<audio controls><source src="' + audioFile + '" type="audio/mpeg"></audio>');
            });

            //replace special img with video
            pageContent.find('img.mce-object-video').each(function (index, item) {
                var controls = $(item).attr('data-mce-p-controls') || false;
                var autoplay = $(item).attr('data-mce-p-autoplay') || false;
                var source1 = unescape($(item).attr('data-mce-html'));
                var width = $(item).attr('width');
                var height = $(item).attr('height');
                var poster = $(item).attr('data-mce-p-poster') || false;
                $(item).replaceWith('<video width="' + width + '" height="' + height + '"' + (poster ? ' poster="' + poster + '"' : '') + ' controls="' + controls + '" autoplay="' + autoplay + '">\n' + source1 + '</video>');
            });

            //tabs
            pageContent.find('.js-tab-nav-item').removeClass('active');
            pageContent.find('.js-tab-content-item').removeClass('active').removeClass('show');

            //flip cards
            pageContent.find('.js-flip-cards-grid-item').removeClass('flip-card-item--rotate');

            //external plugins
            //pageContent.find('[data-plagin-container]').html('<div></div>');
            pageContent.find('[data-plagin-container]').removeClass('mceNonEditable').removeAttr('contenteditable');

            //jwplayer
            pageContent.find('.js-jwPlayer-wrap').html('<div></div>');

            //links
            pageContent.find('[data-media-link="true"]').each(function (index, item) {
                var href = $(item).find('.js-title a').attr('href');
                var target = $(item).find('.js-title a').attr('target') || '_self';

                if (href) {
                    $(item).find('.js-img img').wrap('<a href="' + href + '" target="' + target + '"></a>');
                    $(item).find('.t63-icon-wrap').append('<a href="' + href + '" target="' + target + '"></a>');
                } else {
                    $(item).find('.js-img a img').unwrap();
                    $(item).find('.t63-icon-wrap a').remove();
                }
            });

            //multimedia
            pageContent.find('[data-multimedia-type="image"] .js-multimedia-video-container').remove();
            pageContent.find('[data-multimedia-type="video"] .js-multimedia-image-container').remove();

            //plain html
            pageContent.find('[data-section="html"]').each(function (index, item) {
                var textarea = $(item).find('.js-plain-html-input');
                $(item).find('.js-plain-html-wrap').html(textarea.val());
                textarea.remove();
            });

            //animations
            pageContent.find('[data-children-animation="false"] [data-animation]').removeClass('t63-invisible').removeClass('js-animate').removeAttr('data-animation');

            return pageContent.html();
        },
        getSubmitModel: function () {
            return Page = {
                Action: 'Save',
                PageTitle: $('[name="PageTitle"]').val(),
                PageText: PageBuilderModel.save.getCleanHtml(PageBuilderModel.currentViewSelector),
                Language: PageBuilderModel.language,
                IsPublished: $('[name="IsPublished"]').is(':checked') ? true : false,
                PageData: PageBuilderModel.data.getFromPage(),
                HeaderSectionHtml: PageBuilderModel.pageScripts.getHeaderSectionHtml(),
                FooterSectionHtml: PageBuilderModel.pageScripts.getFooterSectionHtml()
            }
        },
        validation: {
            ShowErrors: function (Errors) {
                $.each(Errors, function (Index, Item) {

                    var Selector = Item.Key;
                    var Tab = $(Selector).closest('.form-group').attr('data-editor-tab');
                    if (Index == 0 && typeof Tab != 'undefined') {
                        PageBuilderModel.plugins.tabs.init(Tab);
                    }
                    $(Selector).closest('.form-group').addClass('has-error');
                    $(Selector).siblings('.tooltip').find('.tooltip-inner').text(Item.Value);

                    $('.js-loader-img').addClass('hidden');
                    $('.js-save-img').removeClass('hidden');
                });
            },
            HideErrors: function () {
                $('.has-error').removeClass('has-error');
            }
        },
        promise: function (Url) {
            PageBuilderModel.save.validation.HideErrors();
            var submitModel = PageBuilderModel.save.getSubmitModel();

            return new Promise(function (Resolve, Reject) {
                $.ajax({
                    type: 'POST',
                    url: PageBuilderModel.urlSave,
                    data: submitModel,
                    dataType: 'json',
                    beforeSend: function () {
                        preloader.show();
                        $('.js-save-img').addClass('hidden');
                        $('.js-loader-img').removeClass('hidden');
                    },
                    success: function (res) {
                        if (res.IsSuccess) {
                            Resolve(Url);
                            if (PageBuilderModel.pageScripts.isUpdated) {
                                location.reload();
                            }
                        }
                        else if (res.Data) {
                            Reject(res.Data);
                        }
                    },
                    error: function (response) {
                        alert(PageBuilderModel.textError);
                    },
                    complete: function () {
                        preloader.hide();
                    }
                });
            });
        },
        successAnimation: function () {
            $('.js-loader-img').addClass('hidden');
            $('.js-success-img').removeClass('hidden');
            setTimeout(function () {
                $('.js-success-img').addClass('hidden');
                $('.js-save-img').removeClass('hidden');
            }, 1000);
        }
    },

    Sideabr: {
        Show: function () {
            $('.js-editor-sidebar-toggle-btn').addClass('is-active');
            $('.js-editor-sidebar').addClass('show');
        },
        Hide: function () {
            $('.js-editor-sidebar-toggle-btn').removeClass('is-active');
            $('.js-editor-sidebar').removeClass('show');
        },
        IsOpen: function () {
            return $('.js-editor-sidebar.show').length > 0;
        },
        Init: function () {
            if ($('.js-editor-sidebar').length > 0) {
                $('.desktop .js-editor-sidebar-scrollable-container').mCustomScrollbar({
                    theme: 'minimal-dark',
                    scrollInertia: 100
                });

                $('.js-editor-sidebar-toggle-btn').click(function () {
                    if (PageBuilderModel.Sideabr.IsOpen()) {
                        PageBuilderModel.Sideabr.Hide();
                    } else {
                        PageBuilderModel.Sideabr.Show();
                    }
                });

                $('.js-editor-sidebar-close-btn').click(function () {
                    PageBuilderModel.Sideabr.Hide();
                });
            }
        }
    },

    init: function () {
        PageBuilderModel.plugins.init();
        PageBuilderModel.compileTemplates();
        PageBuilderModel.pageScripts.init();
        PageBuilderModel.sections.init();
        PageBuilderModel.builder.init();
        PageBuilderModel.editors.init();
        PageBuilderModel.scrollToNavigation.init();

        $('.js-save-btn').click(function () {
            PageBuilderModel.editors.done();
            PageBuilderModel.save.promise().then(PageBuilderModel.save.successAnimation).catch(PageBuilderModel.save.validation.ShowErrors);
        });

        PageBuilderModel.Sideabr.Init();
    }
};

$(function () {
    PageBuilderModel.init();

    //$('.js-copy-to-clipboard-btn').click(function () {
    //    copyToClipboard(PageBuilderModel.save.getCleanHtml(PageBuilderModel.currentViewSelector));
    //});
});

//function copyToClipboard (html) {
//    const el = document.createElement('textarea');
//    el.value = html;
//    document.body.appendChild(el);
//    el.select();
//    document.execCommand('copy');
//    document.body.removeChild(el);
//};

//window.onbeforeunload = function (e) {
//    e = e || window.event;

//    // For IE and Firefox prior to version 4
//    if (e) {
//        e.returnValue = 'Sure?';
//    }

//    // For Safari
//    return 'Sure?';
//};