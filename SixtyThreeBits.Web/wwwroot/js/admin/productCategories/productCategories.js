const productCategoriesModel = {
    urlAddNew: null,
    urlUpdate: null,
    urlDelete: null,
    urlSort: null,
    textConfirmDeleteRecord: null,
    textConfirmDeleteRecursive: null,    

    createNewCategoryModal: null,

    startCreateNewCategoryProcess: function () {
        const productCategoryName = $('.js-create-new-category-modal-input').val();
        let productCategoryParentID = $('.js-create-new-category-modal-category-parent-id-hf').val();
        productCategoryParentID = productCategoryParentID > 0 ? productCategoryParentID : null;
        
        if (productCategoryName) {
            productCategoriesModel.createNewCategoryPromise(productCategoryName, productCategoryParentID).then(function (item) {
                $('.js-create-new-category-modal').modal('hide');
                const parentUL = item.ParentID > 0 ? $('.js-file-tree-editor-item[data-id="' + item.ParentID + '"] > ul') : $('.js-file-tree-editor');
                const html = productCategoriesModel.templates.treeItem({ Children: [item] });

                parentUL.prepend(html);
                productCategoriesModel.initTree();
                productCategoriesModel.syncParentsAndSortIndexes();
            });
        }
        else {
            $('.js-category-name-textbox').closest('.form-control').Shake();
        }
    },
    createNewCategoryPromise: function (productCategoryName, productCategoryParentID) {
        return new Promise(function (resolve, reject) {
            productCategoryParentID = productCategoryParentID === undefined ? null : productCategoryParentID;

            $.ajax({
                type: 'POST',
                url: productCategoriesModel.urlAddNew,
                data: {
                    productCategoryParentID: productCategoryParentID,
                    productCategoryName: productCategoryName
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
                        Validation.Init({
                            ErrorsJson: res.Data.Errors
                        }).ShowErrors();
                        reject();
                    } else {
                        components63Bits.dialog.error();
                        reject();
                    }
                },
                error: function () {
                    components63Bits.dialog.error();
                    reject();
                },
                complete: function () {
                    preloader.hide();
                }
            });
        });
    },
    deleteCategory: function (productCategoryID) {

        const textConfirm = $('.js-file-tree-editor-item[data-id="' + productCategoryID + '"]').find('.js-file-tree-editor-item').length > 0 ? productCategoriesModel.textConfirmDeleteRecursive : productCategoriesModel.textConfirmDeleteRecord;

        components63Bits.dialog.confirm({
            textConfirm: textConfirm,
            confirmButtonColor: components63Bits.dialog.buttonColors.red,
            resolve: function () {
                $.ajax({
                    type: 'POST',
                    url: productCategoriesModel.urlDelete,
                    data: { ProductCategoryID: productCategoryID },
                    dataType: 'json',
                    success: function (e) {
                        if (e.IsSuccess) {
                            $('.js-file-tree-editor-item[data-id="' + productCategoryID + '"]').slideUp(200, function () {
                                $(this).remove();
                            });

                        }
                        else {
                            components63Bits.dialog.error(e.Data);
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
                    productCategoriesModel.syncParentsAndSortIndexes();
                },
            });
        });                
    },
    syncParentsAndSortIndexes: function () {

        const sortIndexes = new Array();

        $('.js-file-tree-editor-item').each(function (Index, item) {

            item = $(item);
            const nodeID = item.attr('data-id');
            const parentID = item.parent().closest('.js-file-tree-editor-item').attr('data-id');
            const sortIndex = item.index();

            sortIndexes.push({ ID: nodeID, ParentID: parentID, SortIndex: sortIndex });
        });

        $.ajax({
            type: 'POST',
            url: productCategoriesModel.urlSort,
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

    templates: {
        compile: function () {
            Template7.registerPartial('Children', $('#file-tree-editor-partial-template').html())
            productCategoriesModel.templates.treeItem = Template7.compile(productCategoriesModel.templates.treeItem);
        },
        treeItem: '{{> "Children"}}'
    }
}

$(function () {
    productCategoriesModel.templates.compile();
    productCategoriesModel.createNewCategoryModal = components63Bits.modal.create('.js-create-new-category-modal');

    $('.js-show-create-new-page-modal-button').click(function () {
        $('.js-create-new-category-modal-input').val('');
        $('.js-create-new-category-modal-category-parent-id-hf').val('');
        productCategoriesModel.createNewCategoryModal.show();
        setTimeout(function () {
            $('.js-create-new-category-modal-input').focus();
        }, 500);
    });

    $('.js-create-new-category-modal-save-button').click(function () {
        productCategoriesModel.startCreateNewCategoryProcess();
    });

    productCategoriesModel.initTree();

    $('.js-file-tree-editor').on('click', '.js-file-tree-editor-item-add-new-button', function (e) {
        e.preventDefault();
        const parentID = $(this).closest('.js-file-tree-editor-item').attr('data-id');
        $('.js-create-new-category-modal-input').val('');
        $('.js-create-new-category-modal-category-parent-id-hf').val(parentID);
        productCategoriesModel.createNewCategoryModal.show();
        setTimeout(function () {
            $('.js-page-title-textbox').focus();
        }, 500);
    });

    $('.js-file-tree-editor').on('click', '.js-file-tree-editor-item-delete-button', function (e) {
        e.preventDefault();
        const productCategoryID = $(this).closest('.js-file-tree-editor-item').attr('data-id');
        productCategoriesModel.deleteCategory(productCategoryID);
    });
});