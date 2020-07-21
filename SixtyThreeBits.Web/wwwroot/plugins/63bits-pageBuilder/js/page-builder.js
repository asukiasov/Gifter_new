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
                    $(PageBuilderModel.builder.pageItems.selector).Hide();
                }
            },

            toggleContainerByEmptyState: function () {
                if ($(PageBuilderModel.builder.pageItems.selector + ' li').length > 0) {
                    $(PageBuilderModel.builder.pageItems.selector).Show();
                } else {
                    $(PageBuilderModel.builder.pageItems.selector).Hide();
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
                $(PageBuilderModel.builder.closeBtnSelector).Show();
            }
            else {

                $(PageBuilderModel.builder.closeBtnSelector).Hide();
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
                $(PageBuilderModel.builder.pageItems.selector).Show();
                $(PageBuilderModel.builder.newItems.selector).Show();
                $(PageBuilderModel.builder.pageItems.selector).addClass('droppable');
                PageBuilderModel.builder.toggleCloseBtnByEmtyState();
            });
            $('.js-close-new-items-container').click(function () {
                $(PageBuilderModel.builder.newItems.selector).Hide();
                $(PageBuilderModel.builder.pageItems.selector).removeClass('droppable');
                PageBuilderModel.builder.toggleCloseBtnByEmtyState();
                PageBuilderModel.builder.pageItems.toggleContainerByEmptyState();
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
            animations: null,

            displayName: '',
            isScrollToNavItem: false,

            components: null
        },

        components: {
            title: {
                isActive: true,
                html: '<h2>Lorem Ipsum dolor sit Amet!</h2>'
            },

            slideTitle: {
                isActive: true,
                html: '<h2>Lorem Ipsum <em>dolor sit</em> Amet!</h2>',
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

            video: {
                isActive: true,
                iframe: null,
                url: 'https://www.youtube.com/embed/cVFzblT5VPE?rel=0'
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
                title: {
                    isActive: true,
                    value: 'Name of the block'
                },
                text: {
                    isActive: true,
                    value: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer auctor erat'
                },
                button: {
                    isActive: true,
                    name: 'see more',
                    href: '#',
                    isTargetBlank: false
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
                        title: PageBuilderModel.settings.components.slideTitle,
                        button: PageBuilderModel.settings.components.button,
                        image: PageBuilderModel.settings.components.image
                    }
                }
            },

            faqItem: function () {
                return {
                    components: {
                        title: PageBuilderModel.settings.components.title,
                        text: PageBuilderModel.settings.components.text
                    }
                }
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
                            html: '$9.99'
                        },
                        text: {
                            ...PageBuilderModel.settings.components.text,
                            html: 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat'
                        },
                        button: PageBuilderModel.settings.components.button,
                        image: {
                            ...PageBuilderModel.settings.components.image,
                            url: '/plugins/63bits-pageBuilder/images/demo/services_item.svg',
                            hasControls: true
                        }
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

                    components: {
                        title: PageBuilderModel.settings.components.slideTitle,
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

                    items: [
                        { ...PageBuilderModel.settings.components.sliderItem() },
                        {
                            components: {
                                title: PageBuilderModel.settings.components.sliderItem().components.title,
                                button: PageBuilderModel.settings.components.sliderItem().components.button,
                                image: {
                                    ...PageBuilderModel.settings.components.sliderItem().components.image,
                                    url: '/plugins/63bits-pageBuilder/images/demo/slide2.jpg'
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

                    components: {
                        title: PageBuilderModel.settings.components.title,
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
                        title: PageBuilderModel.settings.components.title,
                        text: PageBuilderModel.settings.components.text,
                        button: PageBuilderModel.settings.components.button,

                        title2: PageBuilderModel.settings.components.title,
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
                        title: PageBuilderModel.settings.components.title,
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
                        title: PageBuilderModel.settings.components.title,
                        text: PageBuilderModel.settings.components.text,
                        button: PageBuilderModel.settings.components.button,
                        image: PageBuilderModel.settings.components.image,
                    },
                }
            },

            articleWithFiles: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'articleWithFiles',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),

                    components: {
                        title: PageBuilderModel.settings.components.title,
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
                        title: PageBuilderModel.settings.components.title,
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
                    contentSizeSelected: 'fluid',

                    components: {
                        video: PageBuilderModel.settings.components.video,
                    },
                }
            },

            html: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'html',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),
                    contentSizeSelected: 'fluid',

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
                        title: PageBuilderModel.settings.components.title,
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
                        title: PageBuilderModel.settings.components.title,
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
                        title: PageBuilderModel.settings.components.title,
                    },

                    items: [
                        { ...PageBuilderModel.settings.components.articlesGridItem() }
                    ]
                };
            },

            faq: function () {
                return {
                    ...PageBuilderModel.settings.defaults,
                    name: 'faq',
                    contentSizes: PageBuilderModel.settings.defaults.contentSizes(),
                    contentSizeSelected: 'sm',

                    components: {
                        title: {
                            isActive: true,
                            html: '<h3>FAQ</h3>'
                        }
                    },

                    items: [
                        { ...PageBuilderModel.settings.components.faqItem() }
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
                            html: '<h2>What People Say About Us</h2>',
                        },
                    },

                    items: [
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
                            html: '<h2>Services</h2>',
                        },
                    },

                    items: [
                        { ...PageBuilderModel.settings.components.servicesGridItem() }
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
                    additionalClassNames: 'flex-row-reverse',

                    //for demo
                    components: {
                        ...sections.article2ColWithImg().components,
                        image: {
                            ...sections.article2ColWithImg().components.image,
                            url: '/plugins/63bits-pageBuilder/images/demo/02.jpg'
                        },
                    },
                },
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
                    ...sections.faq()
                },
                {
                    ...sections.testimonials()
                },
                {
                    ...sections.services()
                },
            ];
        },

        fromServer: null,

        getFromPage: function () {
            var components = {
                title: function (container) {
                    return {
                        isActive: $(container).find('.js-title-visibility-toggler input:checked').length > 0,
                        html: $(container).find('.js-title').html().replace(/"/g, '&quot'),
                    };
                },
                text: function (container) {
                    return {
                        isActive: $(container).find('.js-text-visibility-toggler input:checked').length > 0,
                        isFullfeatured: $(container).find('.js-tinymce-full').length > 0,
                        html: $(container).find('.js-text-wrap').html().replace(/"/g, '&quot'),
                    };
                },
                textPlain: function (container) {
                    return {
                        isActive: $(container).find('.js-plain-text-visibility-toggler input:checked').length > 0,
                        additionalClassNames: container.attr('data-additional-classes') || null,
                        html: $(container).find('.js-plain-text').html().replace(/"/g, '&quot'),
                    };
                },
                button: function (container) {
                    return {
                        isActive: $(container).find('.js-btn-visibility-toggler input:checked').length > 0,
                        name: $(container).find('.js-btn').text().replace(/"/g, '&quot'),
                        href: $(container).find('.js-btn').attr('href'),
                        isTargetBlank: $(container).find('.js-btn-target-checkbox:checked').length > 0
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
            };

            var data = [];

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
                    additionalClassNames: section.attr('data-additional-classes') || null,
                    contentSizes: PageBuilderModel.settings.sections[sectionName]().contentSizes,
                    contentSizeSelected: section.attr('data-content-size') || null,
                    animations: null,

                    displayName: section.attr('data-display-name'),
                    isScrollToNavItem: section.attr('data-isScrollToNavItem') === 'true',
                };

                if (sectionName == PageBuilderModel.settings.sections.slide().name) {
                    model.contentAlignment = section.find('[data-align-content]').attr('data-align-content');

                    model.components = {
                        title: components.title(section),
                        button: components.button(section),
                        image: components.image(section)
                    }
                }

                if (sectionName == PageBuilderModel.settings.sections.slider().name) {

                    model.contentAlignment = section.find('[data-align-content]').attr('data-align-content');
                    model.items = [];

                    section.find('.js-t63-slider > div').each(function () {
                        var slide = $(this);
                        var item = {
                            components: {
                                title: components.title(slide),
                                button: components.button(slide),
                                image: components.image(slide)
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

                if (sectionName == PageBuilderModel.settings.sections.imgGrid().name) {
                    model.components = {
                        title: components.title(section)
                    }

                    model.items = [];

                    section.find('.js-img-grid-item').each(function () {
                        var gridItem = $(this);
                        var item = {
                            id: gridItem.attr('data-id'),
                            size: gridItem.attr('data-size'),
                            columnClassesNames: PageBuilderModel.settings.components.imgGridItem.columnClassesNames[+gridItem.attr('data-size') - 1],
                            title: {
                                isActive: $(gridItem).find('.js-title-toggler input:checked').length > 0,
                                value: $(gridItem).find('.js-title').html().replace(/"/g, '&quot'),
                            },
                            text: {
                                isActive: $(gridItem).find('.js-text-toggler input:checked').length > 0,
                                value: $(gridItem).find('.js-text').html().replace(/"/g, '&quot'),
                            },
                            button: {
                                ...components.button(gridItem),
                                isActive: $(gridItem).find('.js-btn-toggler input:checked').length > 0,
                                isTargetBlank: $(gridItem).find('.js-btn-target-checkbox:checked').length > 0
                            },
                            image: components.image(gridItem)
                        };
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.articlesGrid().name) {
                    model.components = {
                        title: components.title(section)
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
                        title: components.title(section)
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

                if (sectionName == PageBuilderModel.settings.sections.faq().name) {
                    model.components = {
                        title: components.title(section)
                    };

                    model.items = [];

                    section.find('.js-faq-item').each(function () {
                        var gridItem = $(this);
                        var item = {
                            components: {
                                title: components.title(gridItem),
                                text: components.text(gridItem)
                            }
                        };
                        model.items.push(item);
                    });
                }

                if (sectionName == PageBuilderModel.settings.sections.html().name) {
                    model.components = {
                        text: components.text(section)
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
                        title: components.title(section)
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
                                image: {
                                    ...components.image(gridItem),
                                    hasControls: PageBuilderModel.settings.components.servicesGridItem().components.image.hasControls
                                }
                            }
                        };

                        model.items.push(item);
                    });
                }

                data.push(model);
            });

            return JSON.stringify(data);
        }
    },

    scrollToNavigation: {
        selector: '.js-t63-scrollto-nav',

        container: {
            template:
                `<div class="t63-section t63-scrollto-nav js-t63-scrollto-nav">
                    <div><ul class="container js-t63-scrollto-nav-wrap"></ul></div>
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
                    `<div class="section-row title-container js-section--container{{#unless components.title.isActive}} component-disabled{{/unless}}" data-container="title">
                        <div class="title js-title js-tinymce">{{components.title.html}}</div>
                        {{titleControlsHepler components.title}}
                    </div>`,

                template:
                    `<div class="section-row title-container js-section--container{{#unless title.isActive}} component-disabled{{/unless}}" data-container="title">
                        <div class="title js-title js-tinymce">{{title.html}}</div>
                        {{titleControlsHepler title}}
                    </div>`,

                helper: function (title, actionButtons) {
                    return PageBuilderModel.sections.components.title.template({ title, actionButtons });
                }
            },

            text: {
                partial:
                    `<div class="section-row text-container js-section--container{{#unless components.text.isActive}} component-disabled{{/unless}}" data-container="text">
                        <div class="text-wrap js-text-wrap js-tinymce {{#if components.text.isFullfeatured}}js-tinymce-full{{else}}js-tinymce-basic{{/if}}">{{components.text.html}}</div>
                        {{textControlsHepler components.text}}
                    </div>`,

                template:
                    `<div class="section-row text-container js-section--container{{#unless text.isActive}} component-disabled{{/unless}}" data-container="text">
                        <div class="text-wrap js-text-wrap js-tinymce  {{#if text.isFullfeatured}}js-tinymce-full{{else}}js-tinymce-basic{{/if}}">{{text.html}}</div>
                        {{textControlsHepler text}}
                    </div>`,

                helper: function (text, actionButtons) {
                    return PageBuilderModel.sections.components.text.template({ text, actionButtons });
                }
            },

            textPlain: {
                partial:
                    `<div class="section-row plain-text-container js-section--container {{components.textPlain.additionalClassNames}}{{#unless components.textPlain.isActive}} component-disabled{{/unless}}" data-container="textPlain" data-additional-classes="{{components.textPlain.additionalClassNames}}">
						<div class="text js-plain-text">{{components.textPlain.html}}</div>
                        {{textPlainControlsHepler components.textPlain}}
					</div>`,

                template:
                    `<div class="section-row plain-text-container js-section--container {{textPlain.additionalClassNames}}{{#unless textPlain.isActive}} component-disabled{{/unless}}" data-container="textPlain" data-additional-classes="{{textPlain.additionalClassNames}}">
                        <div class="text js-plain-text">{{textPlain.html}}</div>
                        {{textPlainControlsHepler textPlain}}
                    </div>`,

                helper: function (textPlain, actionButtons) {
                    return PageBuilderModel.sections.components.textPlain.template({ textPlain, actionButtons });
                }
            },

            button: {
                partial:
                    `<div class="section-row btn-container js-section--container{{#unless components.button.isActive}} component-disabled{{/unless}}" data-container="button">
                        <a href="{{components.button.href}}" class="btn btn-primary js-btn"{{#if components.button.isTargetBlank}} target="_blank"{{/if}}>{{components.button.name}}</a>
                        {{actionButtonsHepler actionButtons.button}}
						{{buttonControlsHepler components.button}}
                    </div>`,

                template:
                    `<div class="section-row btn-container js-section--container{{#unless button.isActive}} component-disabled{{/unless}}" data-container="button">
                        <a href="{{button.href}}" class="btn btn-primary js-btn"{{#if button.isTargetBlank}} target="_blank"{{/if}}>{{button.name}}</a>
                        {{actionButtonsHepler actionButtons}}
						{{buttonControlsHepler button}}
                    </div>`,

                helper: function (button, actionButtons) {
                    return PageBuilderModel.sections.components.button.template({ button, actionButtons });
                }
            },

            image: {
                partial:
                    `<div class="img-container js-section--container{{#unless components.image.isActive}} component-disabled{{/unless}}" data-container="image">
                        <span class="bg-img js-img" style="background-image: url({{components.image.url}})" data-img-src="{{components.image.url}}"></span>
                        {{actionButtonsHepler actionButtons.image }}
                        {{#if components.image.hasControls}}
                            {{imageControlsHepler components.image}}
                        {{/if}}
                    </div>`,

                template:
                    `<div class="img-container js-section--container{{#unless image.isActive}} component-disabled{{/unless}}" data-container="image">
                        <span class="bg-img js-img" style="background-image: url({{image.url}})" data-img-src="{{image.url}}"></span>
                        {{actionButtonsHepler actionButtons}}
                        {{#if image.hasControls}}
                            {{imageControlsHepler image}}
                        {{/if}}
                    </div>`,

                helper: function (image, actionButtons) {
                    return PageBuilderModel.sections.components.image.template({ image, actionButtons });
                }
            },

            video: {
                partial:
                    `<div class="section-row video-container js-section--container" data-container="video">
                        <div class="embed-responsive embed-responsive-16by9 js-iframe-container">
                            <iframe class="embed-responsive-item" src="{{components.video.url}}" frameborder="0" allow="encrypted-media" allowfullscreen=""></iframe>
                        </div>
                        {{actionButtonsHepler actionButtons.video }}
						{{videoControlsHepler components.video}}
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
						<li class="file-item js-section--container" data-container="file">
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
            }
        },

        //-- sections
        slide: {
            template:
                `<div class="t63-section img-section js-page-section" data-section="slide" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}">
					{{> "imagePartial"}}
					<div class="container slide-content" data-align-content="{{contentAlignment}}">
						<section>
							<div class="section-row slide-title-container js-section--container{{#unless components.title.isActive}} component-disabled{{/unless}}" data-container="title">
								<div class="title slide-title js-title js-tinymce">{{components.title.html}}</div>
								{{titleControlsHepler components.title}}
							</div>
							{{> "buttonPartial"}}
						</section>
					</div>
					{{> "sectionEditorContainerPartial"}}
				</div>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.slide.template(model);
            }
        },

        slider: {
            template:
                `<div class="t63-section slider-section js-page-section" data-section="slider" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}">
                    <div class="t63-slider js-t63-slider js-t63-slider-container">
                        {{> "sliderItemsPartial"}}
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
                        {{imageHepler components.image ../actionButtons.image}}
                        <div class="container slide-content" data-align-content="{{contentAlignment}}">
                            <section>
                                <div class="section-row slide-title-container js-section--container{{#unless components.title.isActive}} component-disabled{{/unless}}" data-container="title">
                                    <div class="title slide-title js-title js-tinymce">{{components.title.html}}</div>
                                    {{titleControlsHepler components.title}}
                                </div>
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
                `<section class="t63-section js-page-section" data-section="article" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-additional-classes="{{additionalClassNames}}" data-content-size="{{contentSizeSelected}}">
					<article class="container t63-article {{additionalClassNames}}">
						{{> "titlePartial"}}
						{{> "textPartial"}}
					</article>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.article.template(model);
            }
        },

        article2Col: {
            template:
                `<section class="t63-section js-page-section" data-section="article2Col" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}">
                    <div class="container">
					<div class="row text2col">
						<div class="col-lg-6 js-left-col">
							<article class="t63-article">
								{{titleHepler components.title}}
								{{textHepler components.text}}
								{{buttonHepler components.button actionButtons.button }}
							</article>
						</div>
						<div class="col-lg-6 js-right-col">
							<article class="t63-article">
								{{titleHepler components.title2}}
								{{textHepler components.text2}}
								{{buttonHepler components.button2 actionButtons.button }}
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
                `<section class="t63-section js-page-section" data-section="articleText2Col" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}">
					<article class="container t63-article">
						{{> "titlePartial"}}
						<div class="row text2col">
							<div class="col-lg-6 js-left-col">
								{{textHepler components.text}}
                                {{buttonHepler components.button actionButtons.button }}
							</div>
							<div class="col-lg-6 js-right-col">
								{{textHepler components.text2}}
                                {{buttonHepler components.button2 actionButtons.button }}
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
                `<section class="t63-section js-page-section d-lg-flex align-items-stretch {{additionalClassNames}}" data-section="article2ColWithImg" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-additional-classes="{{additionalClassNames}}" data-content-size="{{contentSizeSelected}}">
					<div class="col-lg-6 d-lg-flex align-items-center justify-content-end text-col">
						<div class="t63-article">
							{{> "titlePartial"}}
							{{> "textPartial"}}
							{{> "buttonPartial"}}
						</div>
					</div>
					<div class="col-lg-6 img-col">
						{{> "imagePartial"}}
					</div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.article2ColWithImg.template(model);
            }
        },

        articleWithFiles: {
            template:
                `<section class="t63-section js-page-section" data-section="articleWithFiles" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}">
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
                `<section class="t63-section js-page-section" data-section="articleWithVideo" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}">
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
                `<section class="t63-section js-page-section" data-section="video" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}">
					<div class="container">
                        {{> "videoPartial"}}
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.video.template(model);
            }
        },

        imgGrid: {
            template:
                `<section class="t63-section t63-img-grid-section js-page-section" data-section="imgGrid" data-id="{{id}}" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}">
					<div class="container t63-padding-v">
						{{> "titlePartial"}}
						<div class="row js-img-grid-row">
                            {{> "imgGridItemsPartial"}}
                        </div>
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
						<div class="grid-item js-section--container js-img-grid-item" data-container="imgGridItem" data-id="{{id}}" data-size="{{size}}">
							<div class="img-container js-section--container" data-container="image">
								<span class="bg-img js-img" style="background-image: url({{image.url}})"></span>
								{{actionButtonsHepler ../actionButtons.image}}
							</div>
							<h3 class="grid-item-title js-title{{#unless title.isActive}} component-disabled{{/unless}}">{{title.value}}</h3>

							<div class="overlay{{#js_if "!this.title.isActive && !this.text.isActive && !this.button.isActive"}} component-disabled{{/js_if}}">
								<div>
									<span class="grid-item-title js-title{{#unless title.isActive}} component-disabled{{/unless}}">{{title.value}}</span>
									<p class="grid-item-text js-text{{#unless text.isActive}} component-disabled{{/unless}}">{{text.value}}</p>
									<a class="btn btn-outline-white grid-item-btn js-btn{{#unless button.isActive}} component-disabled{{/unless}}" href="{{button.url}}">{{button.name}}</a>
								</div>
							</div>
							{{actionButtonsHepler ../actionButtons.imgGridItem}}
							{{> "imgGridItemControlsPartial"}}
						</div>
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
                `<section class="t63-section t63-articles-grid-section js-page-section" data-section="articlesGrid" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}">
					<div class="container t63-padding-v">
                        {{> "titlePartial"}}
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
                        <div class="grid-item js-articles-grid-item js-section--container" data-container="articlesGridItem">
                            {{imageHepler components.image ../actionButtons.image}}
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
                `<section class="t63-section t63-articles-list-section js-page-section" data-section="articlesList" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}">
					<div class="container t63-padding-v">
                        {{> "titlePartial"}}
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
                    <div class="list-item js-articles-list-item js-section--container" data-container="articlesListItem">
                        {{imageHepler components.image ../actionButtons.image}}
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

        faq: {
            template:
                `<section class="t63-section js-page-section" data-section="faq" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}">
                    <div class="container t63-padding-v faq-container">
                        {{> "titlePartial"}}
                        <div class="js-faq-items-row">
					        {{> "faqItemsPartial"}}
						</div>
					    <div class="btn-row add-faq-item-btn-wrap js-additional-action-buttons">
							<div class="justify-content-center">
								<button class="btn editor-btn js-add-faq-item-btn">New item</button>
							</div>
						</div>
                    </div>
                    {{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.faq.template(model);
            },

            items: {
                partial:
                    `{{#each items}}
					<article class="faq-item js-section--container js-faq-item" data-container="faqItem">
                        <i class="handle js-additional-action-buttons"><img src="/plugins/63bits-pageBuilder/images/icons/drug.svg" alt="drag icon"></i>
						<div class="faq-item-head js-faq-item-head">
                            {{titleHepler components.title}}
                        </div>

						<div class="faq-item-body js-faq-item-body">
						    {{textHepler components.text}}
						</div>
                        {{actionButtonsHepler ../actionButtons.faqItem}}
					</article>
					{{/each}}`,

                template: '{{> "faqItemsPartial"}}',

                getHtml: function (model) {
                    return PageBuilderModel.sections.faq.items.template(model);
                }
            },

            sortable: function (container) {
                if (container || $('.js-faq-items-row').length > 0) {
                    container = container || '.js-faq-items-row';

                    $(container).sortable({
                        handle: '.handle',
                        placeholder: 'faq-item-placeholder',
                        start: function (event, ui) {
                            var className = $(ui.item).attr('class').replace('ui-sortable-handle', '');
                            $('.faq-item-placeholder').addClass(className);
                        }
                    });
                }
            }
        },

        html: {
            template:
                `<section class="t63-section js-page-section" data-section="html" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}">
					<div class="container t63-padding-v">
                        {{> "textPartial"}}
                    </div>
					{{> "sectionEditorContainerPartial"}}
				</section>`,

            getHtml: function (model) {
                return PageBuilderModel.sections.html.template(model);
            }
        },

        testimonials: {
            template:
                `<div class="t63-section testimonials-section js-page-section" data-section="testimonials" data-type="{{type}}" data-id="{{id}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}">
                    <div class="container t63-padding-v">
					    {{> "titlePartial"}}
					
					    <div class="t63-testimonials-slider js-t63-testimonials-slider js-t63-slider-container">
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
                                {{imageHepler components.image ../actionButtons.image}}
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
                `<section class="t63-section t63-services-section js-page-section" data-section="services" data-type="{{type}}" data-isScrolltoNavItem="{{isScrollToNavItem}}" data-display-name="{{displayName}}" data-content-size="{{contentSizeSelected}}">
					<div class="container t63-padding-v">
                        {{> "titlePartial"}}
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
                        <div class="grid-item js-t63-services-grid-item js-section--container" data-container="articlesGridItem">
                            {{imageHepler components.image ../actionButtons.image}}
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
                if (PageBuilderModel.data.fromServer) {
                    PageBuilderModel.data.fromServer.forEach(function (item) {
                        item.actionButtons = PageBuilderModel.editors.actionButtons.settings;
                        sectionHtml = PageBuilderModel.sections[item.name].getHtml(item).replace(/&quot/g, '"');
                        $(PageBuilderModel.currentViewSelector).append(sectionHtml);
                    });
                    currentSection = $(PageBuilderModel.sections.selector);
                }
            }



            //init plugins
            PageBuilderModel.editors.title.tinymce.init(currentSection);
            PageBuilderModel.editors.text.tinymce.init(currentSection);

            PageBuilderModel.sections.imgGrid.sortable();
            PageBuilderModel.sections.faq.sortable();

            PageBuilderModel.editors.sliderItem.init(currentSection);
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
                faqItem: {
                    editBtn: null,
                    doneBtn: null,
                    removeBtn: true,
                },
                servicesGridItem: {
                    editBtn: null,
                    doneBtn: null,
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
                container.find('.js-togglable-buttons').toggleClass('hidden');
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
							{{#js_if "this.name === 'slide' || this.name === 'slider'"}}
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

                section.attr('data-display-name', displayName);
                $(PageBuilderModel.builder.pageItems.selector + ' li').eq(section.index()).find('.js-builder-item-name').text(displayName);

                section.attr('data-isScrolltoNavItem', isScrollToNavItem);
                PageBuilderModel.scrollToNavigation.init();

                section.find('[data-align-content]').attr('data-align-content', $('.js-alignment-input:checked').val());

                section.attr('data-content-size', itemSize);
                //
                PageBuilderModel.editors.hide(container);
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
                    var options = {
                        toolbar: 'formatselect | alignleft aligncenter alignright alignjustify | bold italic | link | removeformat | code',
                        plugins: 'paste autoresize link code help wordcount emoticons charmap',
                        block_formats: 'Header 1=h1;Header 2=h2;Header 3=h3;Header 4=h4;Header 5=h5;Header 6=h6',
                        paste_word_valid_elements: 'i,em',
                        forced_root_block: ''
                    };

                    if ($(section).find('.js-tinymce.js-title').length > 0) {
                        PageBuilderModel.plugins.tinymce.init($(section).find('.js-tinymce.js-title'), options);
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
                        var urlDownload = escape(files[0].urlDownload);
                        PageBuilderModel.plugins.fileManager.tinymce.callback(urlDownload, { alt: '' });
                    }
                    PageBuilderModel.plugins.fileManager.hide();

                    PageBuilderModel.plugins.fileManager.tinymce.isEnabled = false;
                    PageBuilderModel.plugins.fileManager.tinymce.callback = null;
                },
                addMedia: function (files) {
                    if (files) {
                        var urlDownload = escape(files[0].urlDownload);
                        PageBuilderModel.plugins.fileManager.tinymce.callback(urlDownload, { source2: '', poster: '' });
                    }
                    PageBuilderModel.plugins.fileManager.hide();

                    PageBuilderModel.plugins.fileManager.tinymce.isEnabled = false;
                    PageBuilderModel.plugins.fileManager.tinymce.callback = null;
                },
                init: function (section) {
                    var optionsBasic = {
                        toolbar: 'formatselect | blockquote | alignleft aligncenter alignright alignjustify | bullist numlist | removeformat | code',
                        plugins: 'paste autoresize link hr lists advlist code help wordcount emoticons charmap visualblocks searchreplace',
                        block_formats: 'Header 1=h1;Header 2=h2;Header 3=h3;Header 4=h4;Header 5=h5;Header 6=h6; Paragraph=p',
                        paste_word_valid_elements: 'b,strong,i,em,ul,li,ol,p,br,sub,sup',
                        forced_root_block: ''
                    };

                    var optionsFullFeatured = {
                        toolbar: [
                            'formatselect | bullist numlist outdent indent | link | hr | image media | table | code',
                            'fontsizeselect | forecolor | bold italic underline strikethrough superscript subscript | blockquote | alignleft aligncenter alignright alignjustify | removeformat'
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
                    var urlDownload = escape(files[0].urlDownload);
                    PageBuilderModel.editors.image.container.children('.js-img').css({ 'background-image': 'url(' + urlDownload + ')' }).attr('data-img-src', urlDownload);

                    var slider = PageBuilderModel.editors.image.container.closest('.js-t63-slider-container');

                    if ($(slider).length > 0) {
                        $(slider).closest('.js-page-section').find('.js-slider-settings-container li.active .js-go-to-slide-btn').css({ 'background-image': 'url(' + urlDownload + ')' });
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
                            <input class="form-control js-video-input" placeholder="Video url (youtube, vimeo )"></input>
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
                    container.find('.js-video-input').val('');
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
                container.find('.js-file-name').Hide();
                //
                PageBuilderModel.editors.show(container);
                container.find('.js-file-name-input').focus();
            },
            done: function (container) {
                PageBuilderModel.editors.actionButtons.toggleVisibility(container);
                //
                container.find('.js-file-name').text(container.find('.js-file-name-input').val());
                container.find('.js-file-name').Show();
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

                        urlDownload = escape(files[i].urlDownload);
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
                    
                    <div class="d-flex align-items-center justify-content-between">
                        <label class="form-label">Button</label>
                        <label class="toggler toggler-sm js-btn-toggler">
                            <span>Is Active {{button.isActive}}</span>
                            <input type="checkbox" {{#if button.isActive}}checked{{/if}}>
                            <i></i>
                        </label>
                    </div>
                    <div class="form-group">
                        <input type="text" class="form-control js-btn-name-input" placeholder="Button name" value="{{button.name}}">
                    </div>
                    <div class="form-group">
                        <input type="text" class="form-control js-btn-url-input" placeholder="http://example.com" value="{{button.url}}">
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

                var title = {
                    isActive: container.find('.js-title-toggler input:checked').length > 0,
                    value: container.find('.js-title-input').val()
                };
                var text = {
                    isActive: container.find('.js-text-toggler input:checked').length > 0,
                    value: container.find('.js-text-input').val()
                };
                var button = {
                    isActive: container.find('.js-btn-toggler input:checked').length > 0,
                    name: container.find('.js-btn-name-input').val(),
                    url: container.find('.js-btn-url-input').val()
                };

                //
                container.find('.js-title').html(title.value);
                container.find('.js-text').html(text.value);
                container.find('.js-btn').html(button.name).attr('href', button.url);

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

                if (button.isActive) {
                    container.find('.js-btn').removeClass('component-disabled');
                }
                else {
                    container.find('.js-btn').addClass('component-disabled');
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
                                <button class="go-to-slide-btn js-go-to-slide-btn" style="background-image: url({{components.image.url}})"></button>
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

                _this.parent().addClass('active');
                slide.eq(_this.parent().index()).addClass('active');
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

        faqItem: {
            add: function (_this) {
                var section = _this.closest('.js-page-section');
                var itemsRow = section.find('.js-faq-items-row');

                var model = {
                    items: [
                        {
                            ...PageBuilderModel.settings.components.faqItem()
                        }
                    ],
                    actionButtons: PageBuilderModel.editors.actionButtons.settings
                };

                itemsRow.append(PageBuilderModel.sections.faq.items.getHtml(model));

                PageBuilderModel.editors.title.tinymce.init(itemsRow);
                PageBuilderModel.editors.text.tinymce.init(itemsRow);
            },

            remove: function (container) {
                container.remove();
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
            },

            remove: function (container) {
                container.parent().remove();
            }
        },

        done: function () {
            $('.js-done-btn:visible').trigger('click');
        },

        show: function (container) {
            container.find('.js-section-controls').Show();
        },

        hide: function (container) {
            container.find('.js-section-controls').Hide();
        },

        remove: function (container) {
            container = container || $(PageBuilderModel.sections.selector);
            container.find('.js-section-controls, .js-action-buttons, .js-additional-action-buttons').remove();
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

            //--- add faq item
            $(PageBuilderModel.currentViewSelector).on('click', '.js-add-faq-item-btn', function () {
                PageBuilderModel.editors.faqItem.add($(this));
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

            // simple text
            $(PageBuilderModel.currentViewSelector).on('blur', '.js-plain-text-input', function () {
                var container = $(this).closest('.js-section--container');
                var containerName = container.attr('data-container');

                PageBuilderModel.editors[containerName].done(container);
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

        //--- actionButtons & editors heplers
        Template7.registerHelper('actionButtonsHepler', PageBuilderModel.editors.actionButtons.helper);
        Template7.registerHelper('titleControlsHepler', PageBuilderModel.editors.title.helper);
        Template7.registerHelper('textControlsHepler', PageBuilderModel.editors.text.helper);
        Template7.registerHelper('textPlainControlsHepler', PageBuilderModel.editors.textPlain.helper);
        Template7.registerHelper('imageControlsHepler', PageBuilderModel.editors.image.helper);
        Template7.registerHelper('buttonControlsHepler', PageBuilderModel.editors.button.helper);
        Template7.registerHelper('videoControlsHepler', PageBuilderModel.editors.video.helper);

        //components partials
        Template7.registerPartial('titlePartial', PageBuilderModel.sections.components.title.partial);
        Template7.registerPartial('textPartial', PageBuilderModel.sections.components.text.partial);
        Template7.registerPartial('textPlainPartial', PageBuilderModel.sections.components.textPlain.partial);
        Template7.registerPartial('buttonPartial', PageBuilderModel.sections.components.button.partial);
        Template7.registerPartial('imagePartial', PageBuilderModel.sections.components.image.partial);
        Template7.registerPartial('videoPartial', PageBuilderModel.sections.components.video.partial);
        Template7.registerPartial('attachemetsContainerPartial', PageBuilderModel.sections.components.attachements.containerPartial);
        Template7.registerPartial('filesPartial', PageBuilderModel.sections.components.attachements.files.partial);
        Template7.registerPartial('imgGridItemsPartial', PageBuilderModel.sections.imgGrid.items.partial);
        Template7.registerPartial('articlesGridItemsPartial', PageBuilderModel.sections.articlesGrid.items.partial);
        Template7.registerPartial('articlesListItemsPartial', PageBuilderModel.sections.articlesList.items.partial);
        Template7.registerPartial('sliderItemsPartial', PageBuilderModel.sections.slider.items.partial);
        Template7.registerPartial('faqItemsPartial', PageBuilderModel.sections.faq.items.partial);
        Template7.registerPartial('testimonialsItemsPartial', PageBuilderModel.sections.testimonials.items.partial);
        Template7.registerPartial('servicesGridItemsPartial', PageBuilderModel.sections.services.items.partial);

        //--- components templates
        PageBuilderModel.sections.components.title.template = Template7.compile(PageBuilderModel.sections.components.title.template);
        PageBuilderModel.sections.components.text.template = Template7.compile(PageBuilderModel.sections.components.text.template);
        PageBuilderModel.sections.components.textPlain.template = Template7.compile(PageBuilderModel.sections.components.textPlain.template);
        PageBuilderModel.sections.components.button.template = Template7.compile(PageBuilderModel.sections.components.button.template);
        PageBuilderModel.sections.components.image.template = Template7.compile(PageBuilderModel.sections.components.image.template);
        PageBuilderModel.sections.components.attachements.files.template = Template7.compile(PageBuilderModel.sections.components.attachements.files.template);
        PageBuilderModel.sections.articlesGrid.items.template = Template7.compile(PageBuilderModel.sections.articlesGrid.items.template);
        PageBuilderModel.sections.articlesList.items.template = Template7.compile(PageBuilderModel.sections.articlesList.items.template);
        PageBuilderModel.sections.imgGrid.items.template = Template7.compile(PageBuilderModel.sections.imgGrid.items.template);
        PageBuilderModel.sections.slider.items.template = Template7.compile(PageBuilderModel.sections.slider.items.template);
        PageBuilderModel.sections.faq.items.template = Template7.compile(PageBuilderModel.sections.faq.items.template);
        PageBuilderModel.sections.testimonials.items.template = Template7.compile(PageBuilderModel.sections.testimonials.items.template);
        PageBuilderModel.sections.services.items.template = Template7.compile(PageBuilderModel.sections.services.items.template);

        //--- sections templates
        PageBuilderModel.sections.slide.template = Template7.compile(PageBuilderModel.sections.slide.template);
        PageBuilderModel.sections.slider.template = Template7.compile(PageBuilderModel.sections.slider.template);
        PageBuilderModel.sections.article.template = Template7.compile(PageBuilderModel.sections.article.template);
        PageBuilderModel.sections.article2Col.template = Template7.compile(PageBuilderModel.sections.article2Col.template);
        PageBuilderModel.sections.articleText2Col.template = Template7.compile(PageBuilderModel.sections.articleText2Col.template);
        PageBuilderModel.sections.article2ColWithImg.template = Template7.compile(PageBuilderModel.sections.article2ColWithImg.template);
        PageBuilderModel.sections.articleWithFiles.template = Template7.compile(PageBuilderModel.sections.articleWithFiles.template);
        PageBuilderModel.sections.articleWithVideo.template = Template7.compile(PageBuilderModel.sections.articleWithVideo.template);
        PageBuilderModel.sections.video.template = Template7.compile(PageBuilderModel.sections.video.template);
        PageBuilderModel.sections.imgGrid.template = Template7.compile(PageBuilderModel.sections.imgGrid.template);
        PageBuilderModel.sections.articlesGrid.template = Template7.compile(PageBuilderModel.sections.articlesGrid.template);
        PageBuilderModel.sections.articlesList.template = Template7.compile(PageBuilderModel.sections.articlesList.template);
        PageBuilderModel.sections.faq.template = Template7.compile(PageBuilderModel.sections.faq.template);
        PageBuilderModel.sections.html.template = Template7.compile(PageBuilderModel.sections.html.template);
        PageBuilderModel.sections.testimonials.template = Template7.compile(PageBuilderModel.sections.testimonials.template);
        PageBuilderModel.sections.services.template = Template7.compile(PageBuilderModel.sections.services.template);

        //--- actionButtons templates
        PageBuilderModel.editors.actionButtons.template = Template7.compile(PageBuilderModel.editors.actionButtons.template);

        //--- controls partials & tepmates
        Template7.registerPartial('sectionEditorContainerPartial', PageBuilderModel.editors.section.controls.partial);
        Template7.registerPartial('alignmentWrapPartial', PageBuilderModel.editors.section.controls.alignmentWrapPartial);
        Template7.registerPartial('fileControlsPartial', PageBuilderModel.editors.file.partial);
        Template7.registerPartial('imgGridItemControlsPartial', PageBuilderModel.editors.imgGridItem.partial);

        Template7.registerPartial('sliderItemsControlsPartial', PageBuilderModel.editors.sliderItem.controls.containerPartial);
        Template7.registerPartial('sliderItemsControlsItemPartial', PageBuilderModel.editors.sliderItem.controls.items.partial);

        PageBuilderModel.editors.sliderItem.controls.items.template = Template7.compile(PageBuilderModel.editors.sliderItem.controls.items.template);


        //--- scrollTo navigation
        PageBuilderModel.scrollToNavigation.container.template = Template7.compile(PageBuilderModel.scrollToNavigation.container.template);
        PageBuilderModel.scrollToNavigation.items.template = Template7.compile(PageBuilderModel.scrollToNavigation.items.template);
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
                        format: { title: 'Format', items: 'bold italic underline strikethrough superscript subscript | blockformats fontsizes align | forecolor | removeformat' },
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
                    plugins: options.plugins,
                    block_formats: options.block_formats,
                    paste_word_valid_elements: options.paste_word_valid_elements,

                    image_advtab: options.image_advtab,
                    file_picker_types: options.file_picker_types,
                    file_picker_callback: options.file_picker_callback,

                    force_br_newlines: true,
                    force_p_newlines: false,
                    forced_root_block: options.forced_root_block,

                    fontsize_formats: '10px 11px 12px 14px 16px 18px 20px 22px 24px 26px 30px',
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
            getMultipleFilesUrl: function () {
                if (PageBuilderModel.plugins.fileManager.url) {
                    return PageBuilderModel.plugins.fileManager.url + '&AllowSelectMultiple=true&OnSelectedFilesChooseClientCallback=PageBuilderModel.plugins.fileManager.onSelectedFilesChooseClientCallback';
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
        init: function () {
            $('[data-toggle="tooltip"]').tooltip();
        },
    },

    save: {
        getCleanHtml: function (Container) {
            var pageContent = $(Container).clone();
            pageContent = $(pageContent);
            PageBuilderModel.editors.remove(pageContent);
            pageContent.find('.component-disabled').remove();

            //replace special img with audio
            pageContent.find('img.mce-object-audio').each(function (index, item) {
                var audioFile = $(item).attr('data-mce-p-src');
                $(item).replaceWith('<audio controls><source src="' + audioFile + '" type="audio/mpeg"></audio>');
            });

            return pageContent.html();
        },
        getSubmitModel: function () {
            return Page = {
                PageTitle: $('[name="PageTitle"]').val(),
                PageSlug: $('[name="PageSlug"]').val(),
                PageText: PageBuilderModel.save.getCleanHtml(PageBuilderModel.currentViewSelector),
                Language: PageBuilderModel.language,
                IsPublished: $('[name="IsPublished"]').is(':checked') ? true : false,
                PageData: PageBuilderModel.data.getFromPage()
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

                    $('.js-loader-img').Hide();
                    $('.js-save-img').Show();
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
                        $('.js-save-img').Hide();
                        $('.js-loader-img').Show();
                    },
                    success: function (res) {
                        if (res.IsSuccess) {
                            Resolve(Url);
                            //PageBuilderModel.pageName.change($('.js-editor-tab-active .js-page-name-input').val());
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
            $('.js-loader-img').Hide();
            $('.js-success-img').Show();
            setTimeout(function () {
                $('.js-success-img').Hide();
                $('.js-save-img').Show();
            }, 1000);
        }
    },

    init: function () {

        PageBuilderModel.plugins.init();
        PageBuilderModel.compileTemplates();
        PageBuilderModel.sections.init();
        PageBuilderModel.builder.init();
        PageBuilderModel.editors.init();
        PageBuilderModel.scrollToNavigation.init();

        $('.js-page-slug-input').change(function () {
            $(this).val($(this).ToSlug());
        });

        $('.js-save-btn').click(function () {
            PageBuilderModel.editors.done();
            PageBuilderModel.save.promise().then(PageBuilderModel.save.successAnimation).catch(PageBuilderModel.save.validation.ShowErrors);
        });
    }
};

$(function () {
    PageBuilderModel.init();
});