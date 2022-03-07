const PagesTreeModel = {
    UrlCreateNew: null,
    UrlUpdate: null,
    UrlDelete: null,
    UrlSyncParentsAndSortIndexes: null,

    TextConfirmDeleteRecord: null,
    TextConfirmDeleteRecursive: null,
    ValidationRequiredPageTitle: null,

    StartCreateNewPageProcess: function () {
        const PageTitle = $('.js-page-title-textbox').val();
        let ParentID = $('.js-page-parent-id-hf').val();
        ParentID = ParentID > 0 ? ParentID : null;

        if (PageTitle) {
            PagesTreeModel.CreateNewPagePromise(ParentID, PageTitle).then(function (Item) {
                $('.js-create-new-page-modal').modal('hide');
                const ParentUL = Item.ParentID > 0 ? $('.js-file-tree-editor-item[data-id="' + Item.ParentID + '"] > ul') : $('.js-file-tree-editor');
                const Html = PagesTreeModel.Templates.TreeItem({ Children: [Item] });

                ParentUL.prepend(Html);
                PagesTreeModel.InitTree();
                PagesTreeModel.SyncParentsAndSortIndexes();
            });
        }
        else {
            $('.js-page-title-textbox').closest('.form-control').Shake();
        }
    },

    CreateNewPagePromise: function (ParentID, PageTitle) {        
        return new Promise(function (Resolve, Reject) {
            ParentID = ParentID === undefined ? null : ParentID;

            $.ajax({
                type: 'POST',
                url: PagesTreeModel.UrlCreateNew,
                data: {
                    ParentID: ParentID,
                    PageTitle: PageTitle
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
                        alert(Globals.TextError);
                        Reject();
                    }
                },
                error: function (response) {
                    alert(Globals.TextError);
                    Reject();
                },
                complete: function () {
                    preloader.hide();
                }
            });
        });
    },
    DeletePage: function (PageID) {
        
        const TextConfirm = $('.js-file-tree-editor-item[data-id="' + PageID+'"]').find('.js-file-tree-editor-item').length > 0 ? PagesTreeModel.TextConfirmDeleteRecursive : PagesTreeModel.TextConfirmDeleteRecord;

        Components63Bits.Dialog.Confirm({
            TextConfirm: TextConfirm,
            ConfirmButtonColor: Components63Bits.Dialog.ButtonColors.Red,
            Resolve: function () {
                $.ajax({
                    type: 'POST',
                    url: PagesTreeModel.UrlDelete,
                    data: { PageID: PageID },
                    dataType: 'json',
                    success: function (res) {
                        if (res.IsSuccess) {
                            $('.js-file-tree-editor-item[data-id="' + PageID + '"]').slideUp(200, function () {
                                $(this).remove();
                            });

                        }
                        else {
                            Components63Bits.Dialog.Error();                            
                        }
                    },
                    error: function (response) {
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
                PagesTreeModel.SyncParentsAndSortIndexes();

                $('.js-file-tree-editor-item').each(function (index, item) {
                    const toggleBtn = $(item).children('div').find('.js-tree-item-toggle-btn');

                    if ($(item).children('ul').children().length) {
                        toggleBtn.removeClass('hidden');
                    } else {
                        toggleBtn.addClass('hidden');
                    }
                });
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
            url: PagesTreeModel.UrlSyncParentsAndSortIndexes,
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
    UpdatePage: function (Options) {

        const PageID = Options.PageID;
        const PageIsPublished = Options.PageIsPublished == true ? true : (Options.PageIsPublished == false ? false : null);
        const PageIsMenuItem = Options.PageIsMenuItem == true ? true : (Options.PageIsMenuItem == false ? false : null);
        const PageIsFooterItem = Options.PageIsFooterItem == true ? true : (Options.PageIsFooterItem == false ? false : null);

        $.ajax({
            type: 'POST',
            url: PagesTreeModel.UrlUpdate,
            data: { PageID: PageID, PageIsPublished: PageIsPublished, PageIsMenuItem: PageIsMenuItem, PageIsFooterItem: PageIsFooterItem },
            dataType: 'json',
            success: function (res) {
                if (res.IsSuccess) {

                }
                else {
                    Components63Bits.Dialog.Error();                    
                }
            },
            error: function () {
                Components63Bits.Dialog.Error();
            }
        });
    },

    Templates: {
        Compile: function() {
            Template7.registerPartial('Children', $('#file-tree-editor-partial-template').html())
            PagesTreeModel.Templates.TreeItem = Template7.compile(PagesTreeModel.Templates.TreeItem);
        },
        TreeItem: '{{> "Children"}}'
    }
};

$(function () {
    PagesTreeModel.Templates.Compile();

    $('.js-show-create-new-page-modal-button').click(function () {
        $('.js-page-title-textbox').val('');
        $('.js-page-parent-id-hf').val('');
        $('.js-create-new-page-modal').modal({ show: true, backdrop: 'static' });
        setTimeout(function () {
            $('.js-page-title-textbox').focus();
        }, 500);
    });

    $('.js-create-new-page-button').click(function () {
        PagesTreeModel.StartCreateNewPageProcess();
    });

    $('.js-page-title-textbox').keyup(function (e) {
        if (e.which == 13) {
            e.preventDefault();
            PagesTreeModel.StartCreateNewPageProcess();
        }
    })
        
    PagesTreeModel.InitTree();

    $('.js-file-tree-editor').on('click', '.js-file-tree-editor-item-add-new-button', function (e) {
        e.preventDefault();
        const ParentID = $(this).closest('.js-file-tree-editor-item').attr('data-id');
        $('.js-page-title-textbox').val('');
        $('.js-page-parent-id-hf').val(ParentID);        
        $('.js-create-new-page-modal').modal('show');        
        setTimeout(function () {
            $('.js-page-title-textbox').focus();
        }, 500);
    });

    $('.js-file-tree-editor').on('click', '.js-file-tree-editor-item-delete-button', function (e) {
        e.preventDefault();
        const PageID = $(this).closest('.js-file-tree-editor-item').attr('data-id');        
        PagesTreeModel.DeletePage(PageID);
        
    });

    $('.js-file-tree-editor').on('change', '.js-file-tree-editor-item-toggler-1-checkbox', function (e) {
        const PageID = $(this).closest('.js-file-tree-editor-item').attr('data-id');
        const PageIsPublished = $(this).is(':checked');
        PagesTreeModel.UpdatePage({
            PageID: PageID,
            PageIsPublished: PageIsPublished
        });
    });

    $('.js-file-tree-editor').on('change', '.js-file-tree-editor-item-toggler-2-checkbox', function (e) {
        const PageID = $(this).closest('.js-file-tree-editor-item').attr('data-id');
        const PageIsMenuItem = $(this).is(':checked');
        PagesTreeModel.UpdatePage({
            PageID: PageID,
            PageIsMenuItem: PageIsMenuItem
        });
    });

    $('.js-file-tree-editor').on('change', '.js-file-tree-editor-item-toggler-3-checkbox', function (e) {
        const PageID = $(this).closest('.js-file-tree-editor-item').attr('data-id');
        const PageIsFooterItem = $(this).is(':checked');
        PagesTreeModel.UpdatePage({
            PageID: PageID,
            PageIsFooterItem: PageIsFooterItem
        });
    });



    //--- accordion
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