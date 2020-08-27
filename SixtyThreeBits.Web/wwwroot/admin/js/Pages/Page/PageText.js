var PageTextModel = {
    urlSave: null,
    textError: null,
    language: null,

    editors: {
        text: {
            tinymce: {
                addImage: function (files) {
                    if (files) {
                        var urlDownload = escape(files[0].urlDownload);
                        PageTextModel.plugins.fileManager.tinymce.callback(urlDownload, { alt: '' });
                    }
                    PageTextModel.plugins.fileManager.hide();
                    PageTextModel.plugins.fileManager.tinymce.callback = null;
                },
                addMedia: function (files) {
                    if (files) {
                        var urlDownload = escape(files[0].urlDownload);
                        PageTextModel.plugins.fileManager.tinymce.callback(urlDownload, { source2: '', poster: '' });
                    }
                    PageTextModel.plugins.fileManager.hide();
                    PageTextModel.plugins.fileManager.tinymce.callback = null;
                },
                init: function (section) {

                    var optionsFullFeatured = {
                        //toolbar: [
                        //    'formatselect | bullist numlist outdent indent | link | hr | image media | table | code',
                        //    'fontsizeselect | forecolor | bold italic underline strikethrough superscript subscript | blockquote | alignleft aligncenter alignright alignjustify | removeformat'
                        //],
                        toolbar: 'formatselect | fontsizeselect | forecolor | bold italic underline strikethrough superscript subscript | blockquote | alignleft aligncenter alignright alignjustify | removeformat | bullist numlist outdent indent | link | hr | image media | table | code',
                        plugins: 'paste autoresize image link media table hr lists advlist code help wordcount emoticons charmap visualblocks searchreplace',
                        block_formats: 'Header 1=h1;Header 2=h2;Header 3=h3;Header 4=h4;Header 5=h5;Header 6=h6; Paragraph=p',
                        paste_word_valid_elements: 'b,strong,i,em,ul,li,ol,p,br,sub,sup',
                        forced_root_block: '',

                        image_advtab: true,

                        file_picker_types: 'image media',
                        file_picker_callback: function (callback, value, meta) {
                            PageTextModel.plugins.fileManager.tinymce.callback = callback;
                            PageTextModel.plugins.fileManager.tinymce.type = meta.filetype;

                            if (meta.filetype == 'media') {
                                PageTextModel.plugins.fileManager.show(PageTextModel.plugins.fileManager.getSingleFileUrl());
                            } else {
                                PageTextModel.plugins.fileManager.show(PageTextModel.plugins.fileManager.getSingleImageUrl());
                            }
                        },

                        toolbar_sticky: true
                    };

                    if ($(section).find('.js-tinymce').length > 0) {
                        PageTextModel.plugins.tinymce.init($(section).find('.js-tinymce'), optionsFullFeatured);
                    }
                }
            }
        },

        init: function () {
            PageTextModel.editors.text.tinymce.init($('.js-editor-page'));
        }
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

                    placeholder: "Enter Text ...",
                });
            },
        },
        fileManager: {
            url: null,
            getSingleImageUrl: function () {
                if (PageTextModel.plugins.fileManager.url) {
                    return PageTextModel.plugins.fileManager.url + '&AllowedExtensions=.jpg,.jpeg,.png,.gif&AllowSelectMultiple=false&OnSelectedFilesChooseClientCallback=PageTextModel.plugins.fileManager.onSelectedFilesChooseClientCallback';
                }
            },
            getSingleFileUrl: function () {
                if (PageTextModel.plugins.fileManager.url) {
                    return PageTextModel.plugins.fileManager.url + '&AllowSelectMultiple=false&OnSelectedFilesChooseClientCallback=PageTextModel.plugins.fileManager.onSelectedFilesChooseClientCallback';
                }
            },
            isTinymce: false,
            tinymce: {
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
                if (PageTextModel.plugins.fileManager.tinymce.type == 'image') {
                    PageTextModel.editors.text.tinymce.addImage(files, PageTextModel.plugins.fileManager.tinymce.callback);
                } else {
                    PageTextModel.editors.text.tinymce.addMedia(files, PageTextModel.plugins.fileManager.tinymce.callback);
                }
            }
        },
        init: function () {
            $('[data-toggle="tooltip"]').tooltip();
        },
    },

    save: {
        getHtml: function (Container) {
            var pageContent = $(Container).clone();
            pageContent = $(pageContent);

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

            return pageContent.html();
        },
        getSubmitModel: function () {
            return Page = {
                PageTitle: $('[name="PageTitle"]').val(),
                PageSlug: $('[name="PageSlug"]').val(),
                PageText: PageTextModel.save.getHtml('.js-tinymce'),
                Language: PageTextModel.language,
                IsPublished: $('[name="IsPublished"]').is(':checked') ? true : false
            }
        },
        validation: {
            ShowErrors: function (Errors) {
                $.each(Errors, function (Index, Item) {

                    var Selector = Item.Key;
                    var Tab = $(Selector).closest('.form-group').attr('data-editor-tab');
                    if (Index == 0 && typeof Tab != 'undefined') {
                        PageTextModel.plugins.tabs.init(Tab);
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
            PageTextModel.save.validation.HideErrors();
            var submitModel = PageTextModel.save.getSubmitModel();

            return new Promise(function (Resolve, Reject) {
                $.ajax({
                    type: 'POST',
                    url: PageTextModel.urlSave,
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
                        }
                        else if (res.Data) {
                            Reject(res.Data);
                        }
                    },
                    error: function (response) {
                        alert(PageTextModel.textError);
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
        PageTextModel.plugins.init();
        PageTextModel.editors.init();

        $('.js-page-slug-input').change(function () {
            $(this).val($(this).ToSlug());
        });

        $('.js-save-btn').click(function () {
            PageTextModel.save.promise().then(PageTextModel.save.successAnimation).catch(PageTextModel.save.validation.ShowErrors);
        });
    }
}

$(function () {
    PageTextModel.init();
});