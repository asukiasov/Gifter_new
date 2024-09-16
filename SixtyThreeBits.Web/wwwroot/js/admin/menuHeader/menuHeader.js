const menuHeaderModel = {
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
    textConfirmDeleteRecursiveWithTypeDelete: null,

    initSortable: function () {

        const uls = document.getElementsByClassName('js-sortable');
        
        uls.forEach(function (ul, index) {
            new Sortable(ul, {
                handle: '.js-tree-node-drag',
                group: 'page',
                animation: 150,
                fallbackOnBody: true,
                swapThreshold: 0.65,
                onSort: function (e) {
                    menuHeaderModel.sort();
                },
            });
        });
    },
    addNodeToTree: function (parentID, treeNodeHtml) {
        if (parentID > 0) {
            $('.js-tree-node[data-id="' + parentID + '"] > ul').prepend(treeNodeHtml);
        }
        else {
            $('.js-tree').prepend(treeNodeHtml);
        }
    },
    updateTreeNode: function (id, treeNodeHtml) {
        $('.js-tree-node[data-id="' + id + '"]').replaceWith(treeNodeHtml);
        menuHeaderModel.initSortable();
    },

    onPagesSelectBoxInitialized: function (e) {
        menuHeaderModel.pagesSelectBox = e.component;
    },
    onPagesSelectBoxSelectionChanged: function (e) {
        if (e.selectedItem) {
            const pageID = e.selectedItem.Key;
            menuHeaderModel.setModalPageComponentsDataBySelectedPageIDPromise(pageID).then(function () { }).catch(function (e) { console.error(e); });
        }
    },

    startEditTreeNode: function (menuHeaderID) {
        preloader.show();
        menuHeaderModel.setModalComponentsDataByMenuHeaderIDPromise(menuHeaderID).then(function () {
            menuHeaderModel.initModalComponentsState();
            menuHeaderModel.modal.show();
            preloader.hide();
        }).catch(function () {
            components63Bits.dialog.error();
        });
    },

    initModalComponentsState: function () {
        const isExternalPageCheckboxChecked = $('.js-modal-MenuHeaderIsExternalPage-checkbox').is(':checked');

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
        $('.js-modal-MenuHeaderID-hf').val(null);
        $('.js-modal-MenuHeaderParentID-hf').val(null);
        $('.js-modal-MenuHeaderIsExternalPage-checkbox').prop('checked', false);
        menuHeaderModel.pagesSelectBox.option('value', null);
        $('.js-modal-PageTitle-input').val(null);
        $('.js-modal-PageTitleEng-input').val(null);
        $('.js-modal-PageTitleRus-input').val(null);
        $('.js-modal-PageSlug-input').val(null);
        $('.js-modal-PageIsPublished-checkbox').prop('checked', false);

        $('.js-modal-MenuHeaderExternalPageUrl-input').val(null);
        $('.js-modal-MenuHeaderTitle-input').val(null);
        $('.js-modal-MenuHeaderTitleEng-input').val(null);
        $('.js-modal-MenuHeaderTitleRus-input').val(null);
        $('.js-modal-MenuHeaderIsPublished-checkbox').prop('checked', false);
        $('.js-modal-MenuHeaderIsTargetBlank-checkbox').prop('checked', false)
    },
    setModalComponentsDataByMenuHeaderIDPromise: function (menuHeaderID) {
        return new Promise(function (resolve, reject) {
            $.ajax({
                type: 'GET',
                url: menuHeaderModel.urlGet + '/' + menuHeaderID,
                dataType: 'json',                
                success: function (e) {
                    if (e.IsSuccess) {
                        $('.js-modal-MenuHeaderID-hf').val(e.Data.MenuHeaderID);
                        $('.js-modal-MenuHeaderParentID-hf').val(e.Data.MenuHeaderParentID);
                        $('.js-modal-MenuHeaderIsExternalPage-checkbox').prop('checked', e.Data.MenuHeaderIsExternalPage)
                        menuHeaderModel.pagesSelectBox.option('value', e.Data.PageID);
                        $('.js-modal-PageTitle-input').val(e.Data.PageTitle);
                        $('.js-modal-PageTitleEng-input').val(e.Data.PageTitleEng);
                        $('.js-modal-PageTitleRus-input').val(e.Data.PageTitleRus);
                        $('.js-modal-PageSlug-input').val(e.Data.PageSlug);
                        $('.js-modal-PageIsPublished-checkbox').prop('checked', e.Data.PageIsPublished)

                        $('.js-modal-MenuHeaderExternalPageUrl-input').val(e.Data.MenuHeaderExternalPageUrl);
                        $('.js-modal-MenuHeaderTitle-input').val(e.Data.MenuHeaderTitle);
                        $('.js-modal-MenuHeaderTitleEng-input').val(e.Data.MenuHeaderTitleEng);
                        $('.js-modal-MenuHeaderTitleRus-input').val(e.Data.MenuHeaderTitleRus);
                        $('.js-modal-MenuHeaderIsPublished-checkbox').prop('checked', e.Data.MenuHeaderIsPublished)
                        $('.js-modal-MenuHeaderIsTargetBlank-checkbox').prop('checked', e.Data.MenuHeaderIsTargetBlank)
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
        const urlGetPage = menuHeaderModel.urlGetPage.replace('0', pageID);
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
        const submitModel = menuHeaderModel.getModalFormData();
        $.ajax({
            type: 'POST',
            url: menuHeaderModel.urlAdd,
            data: submitModel,
            dataType: 'json',
            beforeSend: function () {
                validation.hideErrors();
                preloader.show();
            },
            success: function (e) {
                if (e.IsSuccess) {
                    const parentID = submitModel.MenuHeaderParentID;
                    const treeNodeHtml = e.Data;
                    menuHeaderModel.addNodeToTree(parentID, treeNodeHtml);
                    menuHeaderModel.modal.hide();
                    menuHeaderModel.initSortable();
                    menuHeaderModel.sort();
                    menuHeaderModel.refreshPagesSelectBox();
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
        const submitModel = menuHeaderModel.getModalFormData();
        $.ajax({
            type: 'POST',
            url: menuHeaderModel.urlUpdate,
            data: submitModel,
            dataType: 'json',
            beforeSend: function () {
                validation.hideErrors();
                preloader.show();
            },
            success: function (e) {
                if (e.IsSuccess) {
                    const treeNodeHtml = e.Data;                    
                    menuHeaderModel.updateTreeNode(submitModel.MenuHeaderID, treeNodeHtml);
                    menuHeaderModel.modal.hide();
                    menuHeaderModel.initSortable();
                    menuHeaderModel.refreshPagesSelectBox();
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
    delete: function (menuHeaderID) {
        $.ajax({
            type: 'POST',
            url: menuHeaderModel.urlDelete,
            data: {
                menuHeaderID: menuHeaderID
            },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    $('.js-tree-node[data-id="' + menuHeaderID + '"]').slideUp(function () {
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
            const parentID = item.parent().closest('.js-tree-node').attr('data-id');
            const sortIndex = item.index();

            sortIndexes.push({ ID: ID, ParentID: parentID, SortIndex: sortIndex });
        });

        $.ajax({
            type: 'POST',
            url: menuHeaderModel.urlSort,
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
            url: menuHeaderModel.urlGetPages,
            dataType: 'json',            
            success: function (e) {
                if (e.IsSuccess) {
                    var ds = new DevExpress.data.DataSource({
                        store: e.Data
                    });
                    menuHeaderModel.pagesSelectBox.option('dataSource', ds)
                }                
            }
        });
    },

    getModalFormData: function () {
        const menuHeaderID = $('.js-modal-MenuHeaderID-hf').val();
        const menuHeaderParentID = $('.js-modal-MenuHeaderParentID-hf').val();        
        const menuHeaderIsExternalPage = $('.js-modal-MenuHeaderIsExternalPage-checkbox').is(':checked');
        const menuHeaderTitle = $('.js-modal-MenuHeaderTitle-input').val();
        const menuHeaderTitleEng = $('.js-modal-MenuHeaderTitleEng-input').val();
        const menuHeaderTitleRus = $('.js-modal-MenuHeaderTitleRus-input').val();
        const menuHeaderExternalPageUrl = $('.js-modal-MenuHeaderExternalPageUrl-input').val();
        const menuHeaderIsPublished = $('.js-modal-MenuHeaderIsPublished-checkbox').is(':checked');
        const menuHeaderIsTargetBlank = $('.js-modal-MenuHeaderIsTargetBlank-checkbox').is(':checked');

        const pageID = menuHeaderModel.pagesSelectBox.option('value')
        const pageTitle = $('.js-modal-PageTitle-input').val();
        const pageTitleEng = $('.js-modal-PageTitleEng-input').val();
        const pageTitleRus = $('.js-modal-PageTitleRus-input').val();
        const pageSlug = $('.js-modal-PageSlug-input').val();
        const pageIsPublished = $('.js-modal-PageIsPublished-checkbox').is(':checked');

        return {
            MenuHeaderID: menuHeaderID,
            MenuHeaderParentID: menuHeaderParentID,            
            MenuHeaderTitle: menuHeaderTitle,
            MenuHeaderTitleEng: menuHeaderTitleEng,
            MenuHeaderTitleRus: menuHeaderTitleRus,
            MenuHeaderIsExternalPage: menuHeaderIsExternalPage,
            MenuHeaderExternalPageUrl: menuHeaderExternalPageUrl,
            MenuHeaderIsPublished: menuHeaderIsPublished,
            MenuHeaderIsTargetBlank: menuHeaderIsTargetBlank,

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
    menuHeaderModel.initSortable();
    menuHeaderModel.modal = components63Bits.modal.create('.js-modal');

    $(globals.selectors.buttonAddNew).click(function () {
        menuHeaderModel.clearModalComponentsData();
        menuHeaderModel.initModalComponentsState();
        menuHeaderModel.modal.show();
    });
    
    $('.js-tree').on('click', '.js-tree-node-title', function (e) {
        e.preventDefault();
        const menuHeaderID = $(this).closest('.js-tree-node').data('id');
        menuHeaderModel.startEditTreeNode(menuHeaderID);
    });
    $('.js-tree').on('click', '.js-tree-node-add-new-button', function (e) {
        e.preventDefault();
        menuHeaderModel.clearModalComponentsData();
        menuHeaderModel.initModalComponentsState();
        const menuHeaderParentID = $(this).closest('.js-tree-node').data('id');
        $('.js-modal-MenuHeaderParentID-hf').val(menuHeaderParentID)
        menuHeaderModel.modal.show();
    });
    $('.js-tree').on('click', '.js-tree-node-edit-button', function (e) {
        e.preventDefault();
        const menuHeaderID = $(this).closest('.js-tree-node').data('id');
        menuHeaderModel.startEditTreeNode(menuHeaderID);
    });
    $('.js-tree').on('click', '.js-tree-node-delete-button', function (e) {
        e.preventDefault();
        const menuHeaderID = $(this).closest('.js-tree-node').data('id');
        const childrenCount = $(this).closest('.js-tree-node').find('.js-tree-node').length;
        if (childrenCount > 0) {
            components63Bits.dialog.prompt({
                text: menuHeaderModel.textConfirmDeleteRecursiveWithTypeDelete,
                size: components63Bits.dialog.sizes.medium,
                buttonColor: components63Bits.dialog.buttonColors.red,
                resolve: function (e) {
                    if (e == 'delete') {
                        menuHeaderModel.delete(menuHeaderID);
                    }
                }
            });
        }
        else {
            components63Bits.dialog.confirm({
                textConfirm: menuHeaderModel.textConfirmDelete,
                confirmButtonColor: components63Bits.dialog.buttonColors.red,
                resolve: function () {
                    menuHeaderModel.delete(menuHeaderID);
                }
            });
        }
        
    });

    $('.js-modal-MenuHeaderIsExternalPage-checkbox').click(function () {
        menuHeaderModel.initModalComponentsState();
    });
    $('.js-modal-save-button').click(function () {
        const data = menuHeaderModel.getModalFormData();
        if (data.MenuHeaderID > 0) {
            menuHeaderModel.update();            
        }
        else {
            menuHeaderModel.addNew();
        }
    });
});