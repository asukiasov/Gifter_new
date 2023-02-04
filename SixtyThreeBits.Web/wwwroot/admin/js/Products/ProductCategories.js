const ProductCategoriesModel = {
    UrlAddNew: null,
    UrlUpdate: null,
    UrlDelete: null,
    UrlSort: null,

    TextConfirmDeleteRecord: null,
    TextConfirmDeleteRecursive: null,    

    StartCreateNewCategoryProcess: function () {
        const ProductCategoryName = $('.js-category-name-textbox').val();
        let ProductCategoryParentID = $('.js-category-parent-id-hf').val();
        ProductCategoryParentID = ProductCategoryParentID > 0 ? ProductCategoryParentID : null;
        
        if (ProductCategoryName) {
            ProductCategoriesModel.CreateNewCategoryPromise(ProductCategoryName, ProductCategoryParentID).then(function (Item) {
                $('.js-create-new-category-modal').modal('hide');
                const ParentUL = Item.ParentID > 0 ? $('.js-file-tree-editor-item[data-id="' + Item.ParentID + '"] > ul') : $('.js-file-tree-editor');
                const Html = ProductCategoriesModel.Templates.TreeItem({ Children: [Item] });

                ParentUL.prepend(Html);
                ProductCategoriesModel.InitTree();
                ProductCategoriesModel.SyncParentsAndSortIndexes();
            });
        }
        else {
            $('.js-category-name-textbox').closest('.form-control').Shake();
        }
    },

    CreateNewCategoryPromise: function (ProductCategoryName, ProductCategoryParentID) {
        return new Promise(function (Resolve, Reject) {
            ProductCategoryParentID = ProductCategoryParentID === undefined ? null : ProductCategoryParentID;

            $.ajax({
                type: 'POST',
                url: ProductCategoriesModel.UrlAddNew,
                data: {
                    ProductCategoryParentID: ProductCategoryParentID,
                    ProductCategoryName: ProductCategoryName
                },
                dataType: 'json',
                beforeSend: function () {
                    preloader.show();
                },
                success: function (res) {
                    if (res.IsSuccess && res.Data) {
                        Resolve(res.Data);
                    }
                    else if (res.Data) {
                        Validation.Init({
                            ErrorsJson: res.Data.Errors
                        }).ShowErrors();
                        Reject();
                    } else {
                        Components63Bits.Dialog.Error();
                        Reject();
                    }
                },
                error: function () {
                    Components63Bits.Dialog.Error();
                    Reject();
                },
                complete: function () {
                    preloader.hide();
                }
            });
        });
    },
    DeleteCategory: function (ProductCategoryID) {

        const TextConfirm = $('.js-file-tree-editor-item[data-id="' + ProductCategoryID + '"]').find('.js-file-tree-editor-item').length > 0 ? ProductCategoriesModel.TextConfirmDeleteRecursive : ProductCategoriesModel.TextConfirmDeleteRecord;

        Components63Bits.Dialog.Confirm({
            TextConfirm: TextConfirm,
            ConfirmButtonColor: Components63Bits.Dialog.ButtonColors.Red,
            Resolve: function () {
                $.ajax({
                    type: 'POST',
                    url: ProductCategoriesModel.UrlDelete,
                    data: { ProductCategoryID: ProductCategoryID },
                    dataType: 'json',
                    success: function (res) {
                        if (res.IsSuccess) {
                            $('.js-file-tree-editor-item[data-id="' + ProductCategoryID + '"]').slideUp(200, function () {
                                $(this).remove();
                            });

                        }
                        else {
                            Components63Bits.Dialog.Error();
                        }
                    },
                    error: function () {
                        alert(Globals.TextError);
                    }
                });
            }
        });
    },
    InitTree: function () {

        $('.js-file-tree-editor').nestedSortable({
            forcePlaceholderSize: true,
            disableNesting: 'js-no-nesting',
            errorClass: 'sortable-error',
            handle: '.drag',
            helper: 'clone',
            listType: 'ul',
            items: 'li',
            opacity: .6,
            placeholder: 'placeholder',
            revert: 250,
            tabSize: 25,
            tolerance: 'pointer',
            toleranceElement: '> div',
            maxLevels: 4,
            isTree: true,
            expandOnHover: 700,
            startCollapsed: false,
            sort: function (event, ui) {
            },
            update: function (event, ui) {
                ProductCategoriesModel.SyncParentsAndSortIndexes();
            }
        });
    },
    SyncParentsAndSortIndexes: function () {
        const SortIndexes = new Array();

        $('.js-file-tree-editor-item').each(function (Index, Item) {

            Item = $(Item);
            const NodeID = Item.attr('data-id');
            const ParentID = Item.parent().closest('.js-file-tree-editor-item').attr('data-id');
            const SortIndex = Item.index();

            SortIndexes.push({ ID: NodeID, ParentID: ParentID, SortIndex: SortIndex });
        });

        $.ajax({
            type: 'POST',
            url: ProductCategoriesModel.UrlSort,
            data: { SortIndexes: SortIndexes },
            dataType: 'json',
            success: function (res) {
                if (res.IsSuccess) {
                }
                else {
                    alert(Globals.TextError);
                }
            },
            error: function () {
                alert(Globals.TextError);
            }
        });
    },
    Templates: {
        Compile: function () {
            Template7.registerPartial('Children', $('#file-tree-editor-partial-template').html())
            ProductCategoriesModel.Templates.TreeItem = Template7.compile(ProductCategoriesModel.Templates.TreeItem);
        },
        TreeItem: '{{> "Children"}}'
    }
}

$(function () {
    ProductCategoriesModel.Templates.Compile();

    $('.js-show-add-new-modal-button').click(function () {
        $('.js-category-name-textbox').val('');
        $('.js-category-parent-id-hf').val('');
        $('.js-create-new-category-modal').modal('show');
        setTimeout(function () {
            $('.js-category-name-textbox').focus();
        }, 500);
    });

    $('.js-save-new-category-button').click(function () {
        ProductCategoriesModel.StartCreateNewCategoryProcess();
    });

    ProductCategoriesModel.InitTree();

    $('.js-file-tree-editor').on('click', '.js-file-tree-editor-item-add-new-button', function (e) {
        e.preventDefault();
        const ParentID = $(this).closest('.js-file-tree-editor-item').attr('data-id');
        $('.js-category-name-textbox').val('');
        $('.js-category-parent-id-hf').val(ParentID);
        $('.js-create-new-category-modal').modal('show');
        setTimeout(function () {
            $('.js-page-title-textbox').focus();
        }, 500);
    });

    $('.js-file-tree-editor').on('click', '.js-file-tree-editor-item-delete-button', function (e) {
        e.preventDefault();
        const ProductCategoryID = $(this).closest('.js-file-tree-editor-item').attr('data-id');
        ProductCategoriesModel.DeleteCategory(ProductCategoryID);

    });
});