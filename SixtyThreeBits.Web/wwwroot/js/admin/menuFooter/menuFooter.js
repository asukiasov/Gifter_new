const menuFooterModel = {
    urlAdd: null,
    urlUpdate: null,
    urlDelete: null,
    urlGet: null,
    urlGetPage: null,
    urlGetPages: null,
    urlSort: null,
    modal: null,
    pagesSelectBox: null,
    textAdd: null,
    textUpdate: null,
    textConfirmDelete: null,

    initSortable: function () {

        const uls = document.getElementsByClassName('js-sortable');
        
        uls.forEach(function (ul, index) {
            new Sortable(ul, {
                handle: '.js-tree-node-drag',
                animation: 150,
                fallbackOnBody: true,
                swapThreshold: 0.65,
                onSort: function (e) {
                    menuFooterModel.sort();
                },
            });
        });
    },
    addNodeToTree: function (treeNodeHtml) {        
        $('.js-tree').prepend(treeNodeHtml);
        menuFooterModel.initSortable();
    },
    updateTreeNode: function (id, treeNodeHtml) {
        $('.js-tree-node[data-id="' + id + '"]').replaceWith(treeNodeHtml);        
    },

    onPagesSelectBoxInitialized: function (e) {
        menuFooterModel.pagesSelectBox = e.component;
    },
    onPagesSelectBoxSelectionChanged: function (e) {
        if (e.selectedItem) {
            const pageID = e.selectedItem.Key;
            menuFooterModel.setModalPageComponentsDataBySelectedPageIDPromise(pageID).then(function () { }).catch(function (e) { console.error(e); });
        }
    },

    startEditTreeNode: function (menuFooterID) {
        preloader.show();
        menuFooterModel.setModalComponentsDataByMenuFooterIDPromise(menuFooterID).then(function () {
            menuFooterModel.initModalComponentsState();
            menuFooterModel.modal.show();
            preloader.hide();
        }).catch(function () {
            components63Bits.dialog.error();
        });
    },

    initModalComponentsState: function () {
        const isExternalPageCheckboxChecked = $('.js-modal-MenuFooterIsExternalPage-checkbox').is(':checked');

        if (isExternalPageCheckboxChecked) {
            $('.js-modal-page-components').hideElement();
            $('.js-modal-menu-item-components').showElement();
        }
        else {
            $('.js-modal-page-components').showElement();
            $('.js-modal-menu-item-components').hideElement();
        }
    },
    clearModalComponentsData: function () {
        $('.js-modal-MenuFooterID-hf').val(null);        
        $('.js-modal-MenuFooterIsExternalPage-checkbox').prop('checked', false);
        menuFooterModel.pagesSelectBox.option('value', null);
        $('.js-modal-PageTitle-input').val(null);
        $('.js-modal-PageTitleEng-input').val(null);
        $('.js-modal-PageTitleRus-input').val(null);
        $('.js-modal-PageSlug-input').val(null);
        $('.js-modal-PageIsPublished-checkbox').prop('checked', false);

        $('.js-modal-MenuFooterExternalPageUrl-input').val(null);
        $('.js-modal-MenuFooterTitle-input').val(null);
        $('.js-modal-MenuFooterTitleEng-input').val(null);
        $('.js-modal-MenuFooterTitleRus-input').val(null);
        $('.js-modal-MenuFooterIsPublished-checkbox').prop('checked', false);
        $('.js-modal-MenuFooterIsTargetBlank-checkbox').prop('checked', false)
    },
    setModalComponentsDataByMenuFooterIDPromise: function (menuFooterID) {
        return new Promise(function (resolve, reject) {
            $.ajax({
                type: 'GET',
                url: menuFooterModel.urlGet + '/' + menuFooterID,
                dataType: 'json',                
                success: function (e) {                    
                    if (e.IsSuccess) {                        
                        $('.js-modal-MenuFooterID-hf').val(e.Data.MenuFooterID);                        
                        $('.js-modal-MenuFooterIsExternalPage-checkbox').prop('checked', e.Data.MenuFooterIsExternalPage)
                        menuFooterModel.pagesSelectBox.option('value', e.Data.PageID);
                        $('.js-modal-PageTitle-input').val(e.Data.PageTitle);
                        $('.js-modal-PageTitleEng-input').val(e.Data.PageTitleEng);
                        $('.js-modal-PageTitleRus-input').val(e.Data.PageTitleRus);
                        $('.js-modal-PageSlug-input').val(e.Data.PageSlug);
                        $('.js-modal-PageIsPublished-checkbox').prop('checked', e.Data.PageIsPublished)

                        $('.js-modal-MenuFooterExternalPageUrl-input').val(e.Data.MenuFooterExternalPageUrl);
                        $('.js-modal-MenuFooterTitle-input').val(e.Data.MenuFooterTitle);
                        $('.js-modal-MenuFooterTitleEng-input').val(e.Data.MenuFooterTitleEng);
                        $('.js-modal-MenuFooterTitleRus-input').val(e.Data.MenuFooterTitleRus);
                        $('.js-modal-MenuFooterIsPublished-checkbox').prop('checked', e.Data.MenuFooterIsPublished)
                        $('.js-modal-MenuFooterIsTargetBlank-checkbox').prop('checked', e.Data.MenuFooterIsTargetBlank)
                        resolve();
                    }
                    else {
                        reject();
                    }
                },
                error: function () {
                    reject();
                }
            });
        });
    },
    setModalPageComponentsDataBySelectedPageIDPromise: function (pageID) {
        const urlGetPage = menuFooterModel.urlGetPage.replace('0', pageID);
        return new Promise(function (resolve, reject) {
            $.ajax({
                type: 'GET',
                url: urlGetPage,
                dataType: 'json',
                success: function (e) {
                    if (e.IsSuccess) {
                        $('.js-modal-PageTitle-input').val(e.Data.PageTitle);
                        $('.js-modal-PageTitleEng-input').val(e.Data.PageTitleEng);
                        $('.js-modal-PageTitleRus-input').val(e.Data.PageTitleRus);
                        $('.js-modal-PageSlug-input').val(e.Data.PageSlug);
                        $('.js-modal-PageIsPublished-checkbox').prop('checked', e.Data.PageIsPublished);
                        resolve();
                    }
                    else {
                        reject();
                    }
                },
                error: function () {
                    reject();
                }
            });
        });
    },    

    addNew: function () {
        const submitModel = menuFooterModel.getModalFormData();
        $.ajax({
            type: 'POST',
            url: menuFooterModel.urlAdd,
            data: submitModel,
            dataType: 'json',
            beforeSend: function () {
                validation.hideErrors();
                preloader.show();
            },
            success: function (e) {
                if (e.IsSuccess) {                    
                    const treeNodeHtml = e.Data;
                    menuFooterModel.addNodeToTree(treeNodeHtml);
                    menuFooterModel.modal.hide();
                    menuFooterModel.initSortable();
                    menuFooterModel.sort();
                    menuFooterModel.refreshPagesSelectBox();
                }
                else if (e.Data) {
                    validation.init({ errorsJson: e.Data }).showErrors();
                }
                else {
                    components63Bits.dialog.error();
                }
            },
            complete: function () {
                preloader.hide();
            }
        });
    },    
    update: function () {
        const submitModel = menuFooterModel.getModalFormData();
        $.ajax({
            type: 'POST',
            url: menuFooterModel.urlUpdate,
            data: submitModel,
            dataType: 'json',
            beforeSend: function () {
                validation.hideErrors();
                preloader.show();
            },
            success: function (e) {
                if (e.IsSuccess) {
                    const treeNodeHtml = e.Data;
                    menuFooterModel.updateTreeNode(submitModel.MenuFooterID, treeNodeHtml);
                    menuFooterModel.modal.hide();
                    menuFooterModel.initSortable();
                    menuFooterModel.refreshPagesSelectBox();
                }
                else if (e.Data) {
                    validation.init({ errorsJson: e.Data }).showErrors();
                }
                else {
                    components63Bits.dialog.error();
                }
            },
            complete: function () {
                preloader.hide();
            }
        });
    },
    delete: function (menuFooterID) {
        $.ajax({
            type: 'POST',
            url: menuFooterModel.urlDelete,
            data: {
                menuFooterID: menuFooterID
            },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    $('.js-tree-node[data-id="' + menuFooterID + '"]').slideUp(function () {
                        $(this).remove();
                    });
                }
                else {
                    components63Bits.dialog.error();
                }
            },
            complete: function () {
                preloader.hide();
            }
        });
    },
    sort: function () {
        const sortIndexes = new Array();
        $('.js-tree-node').each(function (index, item) {

            item = $(item);
            const ID = item.attr('data-id');            
            const sortIndex = item.index();

            sortIndexes.push({ ID: ID, SortIndex: sortIndex });
        });

        $.ajax({
            type: 'POST',
            url: menuFooterModel.urlSort,
            data: {
                SortIndexes: sortIndexes
            },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {

                }
            },
            complete: function () {
                preloader.hide();
            }
        });
    },

    refreshPagesSelectBox: function () {
        $.ajax({
            type: 'GET',
            url: menuFooterModel.urlGetPages,
            dataType: 'json',
            success: function (e) {
                if (e.IsSuccess) {
                    var ds = new DevExpress.data.DataSource({
                        store: e.Data
                    });
                    menuFooterModel.pagesSelectBox.option('dataSource', ds)
                }
            }
        });
    },
    
    getModalFormData: function () {
        const menuFooterID = $('.js-modal-MenuFooterID-hf').val();
        const menuFooterIsExternalPage = $('.js-modal-MenuFooterIsExternalPage-checkbox').is(':checked');
        const menuFooterTitle = $('.js-modal-MenuFooterTitle-input').val();
        const menuFooterTitleEng = $('.js-modal-MenuFooterTitleEng-input').val();
        const menuFooterTitleRus = $('.js-modal-MenuFooterTitleRus-input').val();
        const menuFooterExternalPageUrl = $('.js-modal-MenuFooterExternalPageUrl-input').val();
        const menuFooterIsPublished = $('.js-modal-MenuFooterIsPublished-checkbox').is(':checked');
        const menuFooterIsTargetBlank = $('.js-modal-MenuFooterIsTargetBlank-checkbox').is(':checked');

        const pageID = menuFooterModel.pagesSelectBox.option('value')
        const pageTitle = $('.js-modal-PageTitle-input').val();
        const pageTitleEng = $('.js-modal-PageTitleEng-input').val();
        const pageTitleRus = $('.js-modal-PageTitleRus-input').val();
        const pageSlug = $('.js-modal-PageSlug-input').val();
        const pageIsPublished = $('.js-modal-PageIsPublished-checkbox').is(':checked');

        return {
            MenuFooterID: menuFooterID,
            MenuFooterTitle: menuFooterTitle,
            MenuFooterTitleEng: menuFooterTitleEng,
            MenuFooterTitleRus: menuFooterTitleRus,
            MenuFooterIsExternalPage: menuFooterIsExternalPage,
            MenuFooterExternalPageUrl: menuFooterExternalPageUrl,
            MenuFooterIsPublished: menuFooterIsPublished,
            MenuFooterIsTargetBlank: menuFooterIsTargetBlank,

            PageID: pageID,
            PageTitle: pageTitle,
            PageTitleEng: pageTitleEng,
            PageTitleRus: pageTitleRus,
            PageSlug: pageSlug,
            PageIsPublished: pageIsPublished
        }
    }
}

$(function () {
    menuFooterModel.initSortable();
    menuFooterModel.modal = components63Bits.modal.create('.js-modal');

    $(globals.selectors.buttonAddNew).click(function () {
        menuFooterModel.clearModalComponentsData();
        menuFooterModel.initModalComponentsState();
        menuFooterModel.modal.show();
    });
    
    $('.js-tree').on('click', '.js-tree-node-title', function (e) {
        e.preventDefault();
        const menuFooterID = $(this).closest('.js-tree-node').data('id');
        menuFooterModel.startEditTreeNode(menuFooterID);
    });    
    $('.js-tree').on('click', '.js-tree-node-edit-button', function (e) {
        e.preventDefault();
        const menuFooterID = $(this).closest('.js-tree-node').data('id');
        menuFooterModel.startEditTreeNode(menuFooterID);
    });
    $('.js-tree').on('click', '.js-tree-node-delete-button', function (e) {
        e.preventDefault();
        const menuFooterID = $(this).closest('.js-tree-node').data('id');
        const childrenCount = $(this).closest('.js-tree-node').find('.js-tree-node').length;
        if (childrenCount > 0) {
            components63Bits.dialog.prompt({
                text: menuFooterModel.textConfirmDeleteRecursiveWithTypeDelete,
                size: components63Bits.dialog.sizes.medium,
                buttonColor: components63Bits.dialog.buttonColors.red,
                resolve: function (e) {
                    if (e == 'delete') {
                        menuFooterModel.delete(menuFooterID);
                    }
                }
            });
        }
        else {
            components63Bits.dialog.confirm({
                textConfirm: menuFooterModel.textConfirmDelete,
                confirmButtonColor: components63Bits.dialog.buttonColors.red,
                resolve: function () {
                    menuFooterModel.delete(menuFooterID);
                }
            });
        }
        
    });

    $('.js-modal-MenuFooterIsExternalPage-checkbox').click(function () {
        menuFooterModel.initModalComponentsState();
    });
    $('.js-modal-save-button').click(function () {
        const data = menuFooterModel.getModalFormData();
        if (data.MenuFooterID > 0) {
            menuFooterModel.update();            
        }
        else {
            menuFooterModel.addNew();
        }
    });
});