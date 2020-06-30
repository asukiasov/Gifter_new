const DictionariesModel = {    
    DictionariesTree: null,
    UrlUpdate: null,

    OnDictionariesTreeInit: function (e) {
        DictionariesModel.DictionariesTree = e.component;
        Globals.Devexpress.SetGridFullHeight(DictionariesModel.DictionariesTree, e.element[0]);
    },
    OnDictionariesTreeToolbarPreparing: function (e) {
        e.toolbarOptions.visible = false;
    },
    OnDictionariesTreeReorder: function (e) {

        const DictionaryID = e.itemData.DictionaryID
        let DictionaryParentID = Globals.Constants.NullValueFor.Int;
        
        if (e.dropInsideItem) {
            visibleRows = DictionariesModel.DictionariesTree.getVisibleRows();
            const Parent = visibleRows[e.toIndex].data;
            DictionaryParentID = Parent.DictionaryID;
        }

        $.ajax({
            type: 'PUT',
            url: DictionariesModel.UrlUpdate,
            data: { key: DictionaryID, values: JSON.stringify({ DictionaryParentID: DictionaryParentID }) },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },            
            complete: function () {
                DictionariesModel.DictionariesTree.refresh();
                preloader.hide();
            }
        });
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        DictionariesModel.DictionariesTree.addRow();
    });
});