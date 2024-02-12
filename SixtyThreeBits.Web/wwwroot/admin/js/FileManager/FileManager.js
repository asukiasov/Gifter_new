var fileManagerModel = {
    fileManager: null,    
    onSelectedFilesChooseClientCallback: null,
    selectedFiles: null,    

    urlGetFiles: null,
    urlUpload: null,
    urlDelete: null,

    customizeThumbnail: function (e) {        
        if (e.dataItem) {
            $('.dx-filemanager-thumbnails-item:last').addClass('js-img-lazy-load');
            $('.dx-filemanager-thumbnails-item:last').attr('data-src', e.dataItem.urlThumbnail);
            if (e.dataItem.isLastItem) {
                setTimeout(function () {
                    fileManagerModel.lazyLoadImages();
                }, 500);
            }
            return '/images/icons/filemanager/default.svg'
            //return e.dataItem.urlThumbnail;
        }
        else {
            return null;
        }        
    },
    deleteFile: function (e) {
        return new Promise(function (resolve, reject) {
            $.ajax({
                type: 'POST',
                url: fileManagerModel.urlDelete,
                data: {
                    Filename: e.key
                },
                dataType: 'json',
                beforeSend: function () {

                },
                success: function (res) {
                    if (res.IsSuccess) {
                        resolve()
                    }
                    else {
                        reject(new DevExpress.fileManagement.FileSystemError(500, null, res.Data));
                    }
                },
                error: function () {
                    components63Bits.dialog.error();
                    reject(new DevExpress.fileManagement.FileSystemError(500, null, null));
                },
                complete: function () {
                }
            });
        });
    },
    downloadFile: function (e) {
        const file = e[0];
        window.open(file.dataItem.urlDownload);
    },
    getFiles: function (e) {
        //const SearchFileTypes = fileManagerModel.SearchFileTypes.join(",");
        //const SearchPhrase = $('.js-file-manager-search').val();

        return new Promise(function (resolve) {
            $.ajax({
                type: 'GET',
                url: fileManagerModel.urlGetFiles,
                data: {
                    //SearchPhrase: SearchPhrase,
                    //SearchFileTypes: SearchFileTypes,
                    //FiletypeIntCode: fileManagerModel.FiletypeIntCode
                },
                dataType: 'json',                
                success: function (res) {
                    if (res.IsSuccess && res.Data) {
                        const filesJson = JSON.parse(res.Data);
                        resolve(filesJson);
                    }
                },
                error: function () {                    
                    components63Bits.dialog.error();
                },
                complete: function () {
                    preloader.hide();
                }
            });
        });
    },
    lazyLoadImages: function () {        
        const lazyloadImages = $('.js-img-lazy-load');
        if (lazyloadImages.length) {
            lazyloadImages.each(function (index, item) {
                const imageUrl = item.getAttribute('data-src');
                const imageElem = item.querySelector('img');
                if (imageElem) {
                    imageElem.src = imageUrl;
                }
            });
        }
    },
    onCustomButtonClick: function (e) {
        var selectedFiles = [];
        fileManagerModel.FileManager.getSelectedItems().forEach(function (item, index) {
            selectedFiles.push({
                urlDownload: item.dataItem.urlDownload,
                name: item.name
            });
        });

        if (e.itemData.commandName == 'choosePictureButton') {            
            if (fileManagerModel.onSelectedFilesChooseClientCallback) {
                fileManagerModel.selectedFiles = selectedFiles;                
                eval('window.parent.' + fileManagerModel.onSelectedFilesChooseClientCallback + '(fileManagerModel.selectedFiles);');
            }
        }
        else if (e.itemData.commandName == 'getLinkButton') {
            if (selectedFiles.length > 0) {
                prompt('File Url', selectedFiles[0].urlDownload);
            }
        }
    },
    onInitialized: function (e) {                
        fileManagerModel.FileManager = e.component;
        //globals.devexpress.setGridFullHeight(fileManagerModel.FileManager, e.element[0], 10);
    },
    uploadFileChunk: function (file, uploadInfo, destinationDirectory) {
        const formData = new FormData();

        formData.append('Filename', file.name);
        formData.append('FileChunk', uploadInfo.chunkBlob);
        formData.append('ChunkIndex', uploadInfo.chunkIndex);
        formData.append('ChunkCount', uploadInfo.chunkCount);

        return new Promise(function (resolve, reject) {
            try {
                $.ajax({
                    url: fileManagerModel.urlUpload,
                    data: formData,
                    type: 'POST',
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    async: false,
                    success: function (res) {
                        if (res.IsSuccess) {
                            resolve()
                        }
                        else {
                            reject(new DevExpress.fileManagement.FileSystemError(500, null, res.Data));
                        }
                    },
                    error: function () {
                        components63Bits.dialog.error();
                        reject(new DevExpress.fileManagement.FileSystemError(500, null, null));
                    }
                });
            }
            catch (e) {
                console.error(e)
            }
        });
    },    
}

$(function () {
    // hide unnecessary sections
    $('.dx-filemanager-dirs-panel').parent().addClass('dx-state-invisible');
    $('.dx-filemanager-breadcrumbs').addClass('dx-state-invisible');
    //$('.dx-drawer-panel-content').remove();
});