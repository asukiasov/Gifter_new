const pagesTreeModel = {
    urlCreateNew: null,
    urlUpdate: null,
    urlDelete: null,
    urlSyncParentsAndSortIndexes: null,

    textConfirmDeleteRecord: null,
    textConfirmDeleteRecursive: null,
    validationRequiredPageTitle: null,

    createNewPageModal: null,
    sortTimeout: null,

    startCreateNewPageProcess: function (pageParentID, pageTitle) {
        if (pageTitle) {
            pagesTreeModel.createNewPagePromise(pageParentID, pageTitle).then(function (item) {
                pagesTreeModel.createNewPageModal.hide();
                
                const parentUL = item.ParentID > 0 ? $('.js-file-tree-editor-item[data-id="' + item.ParentID + '"] > ul') : $('.js-file-tree-editor');
                const html = pagesTreeModel.templates.treeItem({ Children: [item] });

                parentUL.prepend(html);
                pagesTreeModel.initTree();
                pagesTreeModel.syncParentsAndSortIndexes();
            });
        }
        else {
            $('.js-page-title-textbox').closest('.form-control').Shake();
        }
    },
    createNewPagePromise: function (pageParentID, pageTitle) {        
        return new Promise(function (resolve, reject) {
            pageParentID = pageParentID > 0 ? pageParentID : null;
            
            $.ajax({
                type: 'POST',
                url: pagesTreeModel.urlCreateNew,
                data: {
                    PageParentID: pageParentID,
                    PageTitle: pageTitle
                },
                dataType: 'json',
                beforeSend: function () {
                    preloader.show();
                },
                success: function (res) {
                    if (res.IsSuccess && res.Data) {                        
                        resolve(res.Data);
                    }
                    else if (res.Data) {
                        validation.init({
                            errorsJson: res.Data.Errors
                        }).showErrors();
                        reject();
                    } else {
                        components63Bits.dialog.error();
                        reject();
                    }
                },
                error: function (response) {
                    components63Bits.dialog.error();
                    reject();
                },
                complete: function () {
                    preloader.hide();
                }
            });
        });
    },
    deletePage: function (pageID) {
        
        const textConfirm = $('.js-file-tree-editor-item[data-id="' + pageID +'"]').find('.js-file-tree-editor-item').length > 0 ? pagesTreeModel.textConfirmDeleteRecursive : pagesTreeModel.textConfirmDeleteRecord;

        components63Bits.dialog.confirm({
            textConfirm: textConfirm,
            confirmButtonColor: components63Bits.dialog.buttonColors.Red,
            resolve: function () {
                $.ajax({
                    type: 'POST',
                    url: pagesTreeModel.urlDelete,
                    data: { PageID: pageID },
                    dataType: 'json',
                    success: function (res) {
                        if (res.IsSuccess) {
                            $('.js-file-tree-editor-item[data-id="' + pageID + '"]').slideUp(200, function () {
                                $(this).remove();
                            });

                        }
                        else {
                            components63Bits.dialog.error();
                        }
                    },
                    error: function () {
                        components63Bits.dialog.error();
                    }
                });
            }
        });        
    },
    initTree: function () {

        const uls = document.getElementsByClassName('js-tree-items-container');
        uls.forEach(function (ul, Index) {
            new Sortable(ul, {
                handle: '.drag',
                group: 'page',
                animation: 150,
                fallbackOnBody: true,
                swapThreshold: 0.65,
                onSort: function (e) {
                    pagesTreeModel.syncParentsAndSortIndexesWithTimeout();
                },
            });
        });                
    },
    syncParentsAndSortIndexesWithTimeout: function () {
        if (pagesTreeModel.sortTimeout) {
            clearTimeout(PagesTreeModel.SortTimeout);
        }
        pagesTreeModel.sortTimeout = setTimeout(function () {
            pagesTreeModel.syncParentsAndSortIndexes();
        }, 200);
    },
    syncParentsAndSortIndexes: function () {
        const sortIndexes = new Array();

        $('.js-file-tree-editor-item').each(function (index, item) {

            item = $(item);
            const nodeID = item.attr('data-id');
            const parentID = item.parent().closest('.js-file-tree-editor-item').attr('data-id');
            const sortIndex = item.index();            

            sortIndexes.push({ ID: nodeID, ParentID: parentID, SortIndex: sortIndex });
        });

        $.ajax({
            type: 'POST',
            url: pagesTreeModel.urlSyncParentsAndSortIndexes,
            data: { SortIndexes: sortIndexes },
            dataType: 'json',
            success: function (res) {
                if (res.IsSuccess) {
                }
                else {
                    components63Bits.dialog.error();
                }
            },
            error: function () {
                components63Bits.dialog.error();
            }
        });
    },
    updatePage: function (options) {

        const pageID = options.pageID;
        const pageIsPublished = options.pageIsPublished == true ? true : (options.pageIsPublished == false ? false : null);
        const pageIsMenuItem = options.pageIsMenuItem == true ? true : (options.pageIsMenuItem == false ? false : null);
        const pageIsFooterItem = options.pageIsFooterItem == true ? true : (options.pageIsFooterItem == false ? false : null);

        $.ajax({
            type: 'POST',
            url: pagesTreeModel.urlUpdate,
            data: { PageID: pageID, PageIsPublished: pageIsPublished, PageIsMenuItem: pageIsMenuItem, PageIsFooterItem: pageIsFooterItem },
            dataType: 'json',
            success: function (res) {
                if (res.IsSuccess) {

                }
                else {
                    components63Bits.dialog.error();                    
                }
            },
            error: function () {
                components63Bits.dialog.error();
            }
        });
    },

    templates: {
        compile: function() {
            Template7.registerPartial('Children', $('#file-tree-editor-partial-template').html())
            pagesTreeModel.templates.treeItem = Template7.compile(pagesTreeModel.templates.treeItem);
        },
        treeItem: '{{> "Children"}}'
    }
};

$(function () {
    pagesTreeModel.templates.compile();
    pagesTreeModel.createNewPageModal = components63Bits.modal.create('.js-create-new-page-modal');
    pagesTreeModel.initTree();

    $('.js-show-create-new-page-modal-button').click(function (e) {
        e.preventDefault();
        $('.js-create-new-page-modal-page-title-input').val('');
        $('.js-create-new-page-modal-parent-page-id-hf').val($(this).closest('.js-file-tree-editor-item').data('id'));
        pagesTreeModel.createNewPageModal.show();
        setTimeout(function () {
            $('.js-create-new-page-modal-page-title-input').focus();
        }, 500);
    });

    $('.js-create-new-page-modal-save-button').click(function () {
        const pageTitle = $('.js-create-new-page-modal-page-title-input').val();
        const pageParentID = $('.js-create-new-page-modal-parent-page-id-hf').val();        
        pagesTreeModel.startCreateNewPageProcess(pageParentID, pageTitle);
    });

    $('.js-page-title-textbox').keyup(function (e) {
        if (e.which == 13) {
            e.preventDefault();
            pagesTreeModel.startCreateNewPageProcess();
        }
    })

    $('.js-file-tree-editor').on('click', '.js-file-tree-editor-item-add-new-button', function (e) {
        e.preventDefault();

        $('.js-create-new-page-modal-page-title-input').val('');
        $('.js-create-new-page-modal-parent-page-id-hf').val($(this).closest('.js-file-tree-editor-item').data('id'));
        pagesTreeModel.createNewPageModal.show();        
        setTimeout(function () {
            $('.js-create-new-page-modal-page-title-input').focus();
        }, 500);
    });

    $('.js-file-tree-editor').on('click', '.js-file-tree-editor-item-delete-button', function (e) {
        e.preventDefault();
        const pageID = $(this).closest('.js-file-tree-editor-item').attr('data-id');        
        pagesTreeModel.deletePage(pageID);
        
    });

    $('.js-file-tree-editor').on('change', '.js-file-tree-editor-item-toggler-1-checkbox', function (e) {
        const pageID = $(this).closest('.js-file-tree-editor-item').attr('data-id');
        const pageIsPublished = $(this).is(':checked');
        pagesTreeModel.updatePage({
            pageID: pageID,
            pageIsPublished: pageIsPublished
        });
    });

    $('.js-file-tree-editor').on('change', '.js-file-tree-editor-item-toggler-2-checkbox', function (e) {
        const pageID = $(this).closest('.js-file-tree-editor-item').attr('data-id');
        const pageIsMenuItem = $(this).is(':checked');
        pagesTreeModel.updatePage({
            pageID: pageID,
            pageIsMenuItem: pageIsMenuItem
        });
    });

    $('.js-file-tree-editor').on('change', '.js-file-tree-editor-item-toggler-3-checkbox', function (e) {
        const pageID = $(this).closest('.js-file-tree-editor-item').attr('data-id');
        const pageIsFooterItem = $(this).is(':checked');
        pagesTreeModel.updatePage({
            pageID: pageID,
            pageIsFooterItem: pageIsFooterItem
        });
    });

    $('.js-tree-item-toggle-btn').click(function () {
        const item = $(this).closest('li');
        const childrenContainer = item.children('ul');
        if ($(item).hasClass('is-open')) {
            $(item).removeClass('is-open');
            childrenContainer.slideUp(100);
        } else {
            $(item).addClass('is-open');
            childrenContainer.slideDown(100);
        }
    });
});