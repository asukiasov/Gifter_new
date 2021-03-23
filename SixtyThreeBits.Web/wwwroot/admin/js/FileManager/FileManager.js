var FileManagerModel = {
    FileManager: null,
    FileManagerFolderHttpPath: null,
    SelectedFiles: null,
    OnInitialized: function (e) {
        FileManagerModel.FileManager = e.component;
    },
    OnSelectedFileOpened: function (s) {
        FancyBox.Init({ src: s.file.dataItem.url }).ShowImagePopup();
    },
    OnSelectedFilesChooseClientCallback: null,
    OnFileManagerCustomCommand: function (args) {
        var SelectedFiles = [];
        FileManagerModel.FileManager.getSelectedItems().forEach(function (Item, Index) {
            SelectedFiles.push({
                urlDownload: FileManagerModel.FileManagerFolderHttpPath + Item.name,
                name: Item.name
            });
        });

        if (args.itemData.commandName == 'ChoosePictureButton') {
            if (FileManagerModel.OnSelectedFilesChooseClientCallback) {
                FileManagerModel.SelectedFiles = SelectedFiles;
                eval('window.parent.' + FileManagerModel.OnSelectedFilesChooseClientCallback + '(FileManagerModel.SelectedFiles);');
            }
        }
        else if (args.itemData.commandName == 'GetLinkButton') {
            if (SelectedFiles.length > 0) {
                prompt('File Url', SelectedFiles[0].urlDownload);
            }
        }
    }
}

$(function () {
    // hide unnecessary sections
    $('.dx-filemanager-dirs-panel').parent().addClass('dx-state-invisible');
    $('.dx-filemanager-breadcrumbs').addClass('dx-state-invisible');
    //$('.dx-drawer-panel-content').remove();
});