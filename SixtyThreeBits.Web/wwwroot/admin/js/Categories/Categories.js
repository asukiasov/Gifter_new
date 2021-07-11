const CategoriesModel = {
    UrlAddNew: null,
    UrlUpdate: null,
    UrlDelete: null,
    UrlSync: null,

    TextConfirmDeleteRecord: null,
    TextConfirmDeleteRecursive: null,    

    StartCreateNewCategoryProcess: function () {
        const CategoryName = $('.js-category-name-textbox').val();
        let CategoryParentID = $('.js-category-parent-id-hf').val();
        CategoryParentID = CategoryParentID > 0 ? CategoryParentID : null;
        
        if (CategoryName) {
            CategoriesModel.CreateNewCategoryPromise(CategoryName, CategoryParentID).then(function (Item) {
                $('.js-create-new-category-modal').modal('hide');
                const ParentUL = Item.ParentID > 0 ? $('.js-file-tree-editor-item[data-id="' + Item.ParentID + '"] > ul') : $('.js-file-tree-editor');
                const Html = CategoriesModel.Templates.TreeItem({ Children: [Item] });

                ParentUL.prepend(Html);
                CategoriesModel.InitTree();
                CategoriesModel.SyncParentsAndSortIndexes();
            });
        }
        else {
            $('.js-category-name-textbox').closest('.form-control').Shake();
        }
    },

    CreateNewCategoryPromise: function (CategoryName, CategoryParentID) {
        return new Promise(function (Resolve, Reject) {
            CategoryParentID = CategoryParentID === undefined ? null : CategoryParentID;

            $.ajax({
                type: 'POST',
                url: CategoriesModel.UrlAddNew,
                data: {
                    CategoryParentID: CategoryParentID,
                    CategoryName: CategoryName
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
    DeleteCategory: function (CategoryID) {

        const TextConfirm = $('.js-file-tree-editor-item[data-id="' + CategoryID + '"]').find('.js-file-tree-editor-item').length > 0 ? CategoriesModel.TextConfirmDeleteRecursive : CategoriesModel.TextConfirmDeleteRecord;

        Components63Bits.Dialog.Confirm({
            TextConfirm: TextConfirm,
            ConfirmButtonColor: Components63Bits.Dialog.ButtonColors.Red,
            Resolve: function () {
                $.ajax({
                    type: 'POST',
                    url: CategoriesModel.UrlDelete,
                    data: { CategoryID: CategoryID },
                    dataType: 'json',
                    success: function (res) {
                        if (res.IsSuccess) {
                            $('.js-file-tree-editor-item[data-id="' + CategoryID + '"]').slideUp(200, function () {
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
                CategoriesModel.SyncParentsAndSortIndexes();
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
            url: CategoriesModel.UrlSync,
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
            CategoriesModel.Templates.TreeItem = Template7.compile(CategoriesModel.Templates.TreeItem);
        },
        TreeItem: '{{> "Children"}}'
    }
}

$(function () {
    CategoriesModel.Templates.Compile();

    $('.js-show-add-new-modal-button').click(function () {
        $('.js-category-name-textbox').val('');
        $('.js-category-parent-id-hf').val('');
        $('.js-create-new-category-modal').modal('show');
        setTimeout(function () {
            $('.js-category-name-textbox').focus();
        }, 500);
    });

    $('.js-save-new-category-button').click(function () {
        CategoriesModel.StartCreateNewCategoryProcess();
    });

    CategoriesModel.InitTree();

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
        const CategoryID = $(this).closest('.js-file-tree-editor-item').attr('data-id');
        CategoriesModel.DeleteCategory(CategoryID);

    });
});