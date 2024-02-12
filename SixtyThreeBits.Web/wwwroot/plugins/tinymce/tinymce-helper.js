var tinyMCEHelper = {
    filePickerCallback: null,
    filePickerDialog: null,
    onSelectedImageChoose: function (files) {
        if (files && files.length) {
            var urlDownload = files[0].urlDownload;            
            tinyMCEHelper.filePickerCallback(urlDownload, { alt: '' });
        }
        if (tinyMCEHelper.filePickerDialog) {
            tinyMCEHelper.filePickerDialog.close();
        }
    },
    onSelectedFilesChoose: function (files) {
        if (files && files.length) {
            let html = '';
            files.forEach(function (file, index) {
                html += ('<a href="' + file.urlDownload + '">' + file.name + '</a><br/>');
            });
            tinyMCE.activeEditor.insertContent(html);
        }
        if (tinyMCEHelper.filePickerDialog) {
            tinyMCEHelper.filePickerDialog.close();
        }
    }
}

var TinyMCEClass = {
    selector: 'textarea',
    width: '100%',
    height: 250,
    autoSize: false,
    fileManagerPath: null,
    customToolbars: '',
    setup: null,
    editor: null,
    placeHolders: [],

    display: function () {
        $(this.selector).tinymce({
            width: this.width,
            height: this.height,
            menubar: false,
            
            toolbar:
                [
                    'formatselect | bullist numlist outdent indent | link | hr | image media | table | filemanager code' + this.customToolbars,
                    'fontsizeselect | forecolor backcolor | bold italic underline strikethrough superscript subscript | blockquote | alignleft aligncenter alignright alignjustify | removeformat | disableTooltip'
                ],
            plugins: 'paste autoresize image link media table hr lists advlist code help wordcount emoticons charmap visualblocks searchreplace'+ ((this.fileManagerPath == null ? '' : ',filemanager') + (this.autoSize ? ',autoresize' : '')),            

            filemanager_path: this.fileManagerPath,
            filemanager_title: 'My Files',

            block_formats: 'Header 1=h1;Header 2=h2;Header 3=h3;Header 4=h4;Header 5=h5;Header 6=h6; Paragraph=p',
            paste_word_valid_elements: 'b,strong,i,em,ul,li,ol,p,br,sub,sup',
            forced_root_block: false,
            force_p_newlines: false,
            remove_linebreaks: false,
            force_br_newlines: true,
            remove_trailing_nbsp: false,
            verify_html: false,
            apply_source_formatting: true,
            relative_urls: false,
            remove_script_host: false,
            convert_urls: false,
            autoresize_bottom_margin: 10,
            autoresize_min_height: this.height,

            setup: this.setup
        });        
    },

    displaySimplified: function () {                
        const _this = this;
        const fileManagerPath = this.fileManagerPath;
        const hasFileManager = fileManagerPath && fileManagerPath.length > 0;

        const toolbar = hasFileManager ? ',| image | media | filemanager' : ''
        const plugins = (hasFileManager ? ' image, media, filemanager' : '') + (this.autoSize ? ', autoresize' : '')

        $(this.selector).tinymce({
            width: this.width,
            height: this.height,
            menubar: false,
            toolbar_items_size: 'small',
            toolbar: 'bold italic underline strikethrough | alignleft aligncenter alignjustify alignright sub sup | link | bullist numlist ' + toolbar + ' | code ' + this.customToolbars,
            plugins: 'link textcolor code paste lists' + plugins,
            paste_word_valid_elements: 'b,strong,i,em,ul,li,ol,p,br,sub,sup',
            forced_root_block: false,
            force_p_newlines: true,
            remove_linebreaks: false,
            force_br_newlines: true,
            remove_trailing_nbsp: false,
            verify_html: false,
            apply_source_formatting: true,
            convert_urls: false,

            image_advtab: true,
            file_picker_types: 'image',
            file_picker_callback: function (callback, value, meta) {

                let url = fileManagerPath;
                if (url.indexOf('callback=') == -1) {
                    const parameterSeparator = url.indexOf('?') == -1 ? '?' : '&';
                    url += (parameterSeparator + 'multichoice=false&ext=.jpg,.jpeg,.png,.gif,.svg&callback=tinyMCEHelper.onSelectedImageChoose')
                }

                tinyMCEHelper.filePickerCallback = callback;
                tinyMCEHelper.filePickerDialog = tinyMCE.activeEditor.windowManager.openUrl({
                    title: 'File Manager',                    
                    url: url 
                });
            },

            filemanager_path: this.fileManagerPath,
            filemanager_title: 'My Files',

            setup: this.setup
        });        
    },

    focus: function () {
        return this.editor.focus();
    },

    getContent: function () {
        return this.editor.getContent();
    },

    setContent: function (content) {
        return this.editor.setContent(content);
    },

    removePoweredBy: function () {        
        $('[aria-label="Powered by Tiny"]').remove();
    }
};

function TinyMCE(options) {
    if (options) {
        const _this = this;
        _this.selector = options.selector ? options.selector : _this.selector;
        _this.width = options.width ? options.width : _this.width;
        _this.height = options.height ? options.height : _this.height;
        _this.autoSize = options.autoSize ? options.autoSize : _this.autoSize;
        _this.fileManagerPath = options.fileManagerPath ? options.fileManagerPath : _this.fileManagerPath;
        _this.customToolbars = options.customToolbars ? options.customToolbars : _this.customToolbars;
        _this.placeHolders = options.placeHolders ? options.placeHolders : _this.placeHolders;
        
        if (_this.placeHolders && _this.placeHolders.length) {
            _this.customToolbars += (' placeHolders');
        }

        _this.setup = function (editor) {
            _this.editor = editor;
            if (options.setup) {
                options.setup(editor);
            }
            
            if (_this.placeHolders && _this.placeHolders.length) {
                
                editor.ui.registry.addSplitButton('placeHolders', {
                    text: 'Insert',
                    onAction: function () {

                    },
                    onItemAction: function (api, value) {
                        editor.insertContent(value);
                    },
                    fetch: function (callback) {                                                
                        const menu = new Array();
                        _this.placeHolders.forEach(function (Item, Index) {
                            menu.push({
                                type: 'choiceitem',
                                text: Item.Key,
                                value: Item.Value
                            });
                        });
                        
                        callback(menu);
                    }
                });

            }

            editor.on('init', function (e) {
                _this.removePoweredBy();
            });
        }
    }    
}

TinyMCE.prototype = TinyMCEClass;