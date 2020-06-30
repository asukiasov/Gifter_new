var FileManagerModel = {
    FileManager: null,
    OnInitialized: function (e) {
        FileManagerModel.FileManager = e.component;
    },
    InsertIntoTinyMce: function (args) {
        var Html = null;
        var SelectedFile = FileManagerModel.FileManager.getSelectedItems()[0];
        switch (args.itemData.position) {
            case "Default": {
                var Html = '<img src="' + SelectedFile.dataItem.url + '" alt="" />';
                break;
            }
            case "LeftAligned": {
                var Html = '<img style="float:left; margin-right:10px;" src="' + SelectedFile.dataItem.url + '"  alt="" />';
                break;
            }
            case "RightAligned": {
                var Html = '<img style="float:right; margin-left:10px;" src="' + SelectedFile.dataItem.url + '"  alt="" />';
                break;
            }
            case "CenterAligned": {
                var Html = '<div style="width:100%; text-align:center"><img src="' + SelectedFile.dataItem.url + '"  alt="" /></div>';
                break;
            }
        }

        if (Html != null) {

            parent.tinymce.activeEditor.insertContent(Html);
            parent.tinymce.activeEditor.windowManager.close();
        }
    },
    OnSelectedFileOpened: function (s) {
        FancyBox.Init({ src: s.file.dataItem.url }).ShowImagePopup();
    }
}

$(function () {
    // hide unnecessary sections
    $('.dx-filemanager-dirs-panel').parent().addClass('dx-state-invisible');
    $('.dx-filemanager-breadcrumbs').addClass('dx-state-invisible');
});