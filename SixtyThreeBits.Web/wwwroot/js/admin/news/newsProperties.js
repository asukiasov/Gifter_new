var model = {
    urlFileManager: null
};

$(function () {
    
    new TinyMCE({
        selector: '.js-apply-tinymce', width: '100%', height: 250,
        fileManagerPath: model.urlFileManager
    }).displaySimplified();
});