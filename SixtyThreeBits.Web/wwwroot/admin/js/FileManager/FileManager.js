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
        if (args.itemData.commandName == 'ChoosePictureButton') {
            var SelectedFiles = [];
            FileManagerModel.FileManager.getSelectedItems().forEach(function (Item, Index) {
                SelectedFiles.push({
                    urlDownload: FileManagerModel.FileManagerFolderHttpPath + Item.name,
                    name: Item.name
                });
            });
            //console.log(SelectedFiles);
            if (FileManagerModel.OnSelectedFilesChooseClientCallback) {
                FileManagerModel.SelectedFiles = SelectedFiles;
                eval('window.parent.' + FileManagerModel.OnSelectedFilesChooseClientCallback + '(FileManagerModel.SelectedFiles);');
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