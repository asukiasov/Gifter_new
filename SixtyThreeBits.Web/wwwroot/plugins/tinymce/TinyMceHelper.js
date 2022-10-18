var TinyMCEClass = {
    Selector: 'textarea',
    Width: '100%',
    Height: 250,
    AutoSize: false,
    FileManagerPath: null,
    CustomToolbars: '',
    Setup: null,
    Editor: null,
    PlaceHolders: [],

    Display: function () {
        $(this.Selector).tinymce({
            width: this.Width,
            height: this.Height,
            menubar: false,
            
            toolbar:
                [
                    'formatselect | bullist numlist outdent indent | link | hr | image media | table | filemanager code' + this.CustomToolbars,
                    'fontsizeselect | forecolor backcolor | bold italic underline strikethrough superscript subscript | blockquote | alignleft aligncenter alignright alignjustify | removeformat | disableTooltip'
                ],
            plugins: 'paste autoresize image link media table hr lists advlist code help wordcount emoticons charmap visualblocks searchreplace'+ ((this.FileManagerPath == null ? '' : ',filemanager') + (this.AutoSize ? ',autoresize' : '')),            

            filemanager_path: this.FileManagerPath,
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
            autoresize_min_height: this.Height,

            setup: this.Setup
        });        
    },

    DisplaySimplified: function () {                
        $(this.Selector).tinymce({
            width: this.Width,
            height: this.Height,
            menubar: false,
            toolbar_items_size: 'small',
            toolbar: 'bold italic underline strikethrough | alignleft aligncenter alignjustify alignright sub sup | link | bullist numlist | code ' + this.CustomToolbars,
            plugins: 'link textcolor code paste image lists',
            paste_word_valid_elements: 'b,strong,i,em,ul,li,ol,p,br,sub,sup',
            forced_root_block: false,
            force_p_newlines: false,
            remove_linebreaks: false,
            force_br_newlines: true,
            remove_trailing_nbsp: false,
            verify_html: false,
            apply_source_formatting: true,

            setup: this.Setup
        });        
    },

    Focus: function () {
        return this.Editor.focus();
    },

    GetContent: function () {
        return this.Editor.getContent();
    },

    SetContent: function (Content) {
        return this.Editor.setContent(Content);
    },

    RemovePoweredBy: function () {        
        $('[aria-label="Powered by Tiny"]').remove();
    }
};

function TinyMCE(Options) {
    if (Options) {
        const _this = this;
        _this.Selector = Options.Selector ? Options.Selector : _this.Selector;
        _this.Width = Options.Width ? Options.Width : _this.Width;
        _this.Height = Options.Height ? Options.Height : _this.Height;
        _this.AutoSize = Options.AutoSize ? Options.AutoSize : _this.AutoSize;
        _this.FileManagerPath = Options.FileManagerPath ? Options.FileManagerPath : _this.FileManagerPath;
        _this.CustomToolbars = Options.CustomToolbars ? Options.CustomToolbars : _this.CustomToolbars;
        _this.PlaceHolders = Options.PlaceHolders ? Options.PlaceHolders : _this.PlaceHolders;
        
        if (_this.PlaceHolders && _this.PlaceHolders.length) {
            _this.CustomToolbars += (' Placeholders');
        }

        _this.Setup = function (editor) {
            _this.Editor = editor;
            if (Options.Setup) {
                Options.Setup(editor);
            }
            
            if (_this.PlaceHolders && _this.PlaceHolders.length) {
                
                editor.ui.registry.addSplitButton('Placeholders', {
                    text: 'Insert',
                    onAction: function () {

                    },
                    onItemAction: function (api, value) {
                        editor.insertContent(value);
                    },
                    fetch: function (callback) {                                                
                        const Menu = new Array();
                        _this.PlaceHolders.forEach(function (Item, Index) {
                            Menu.push({
                                type: 'choiceitem',
                                text: Item.Key,
                                value: Item.Value
                            });
                        });
                        
                        callback(Menu);
                    }
                });

            }

            editor.on('init', function (e) {
                _this.RemovePoweredBy();
            });
        }
    }    
}

TinyMCE.prototype = TinyMCEClass;