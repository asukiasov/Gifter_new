const DictionariesModel = {    
    DictionariesTree: null,
    UrlUpdate: null,

    OnDictionariesTreeInit: function (s) {
        DictionariesModel.DictionariesTree = s.component;
        Globals.Devexpress.SetGridFullHeight(DictionariesModel.DictionariesTree, s.element[0]);
    },
    OnDictionariesTreeToolbarPreparing: function (s) {
        s.toolbarOptions.visible = false;
    },
    OnDictionariesTreeReorder: function (s) {

        const DictionaryID = s.itemData.DictionaryID
        let DictionaryParentID = Globals.Constants.NullValueFor.Int;
        
        if (s.dropInsideItem) {
            visibleRows = DictionariesModel.DictionariesTree.getVisibleRows();
            const Parent = visibleRows[s.toIndex].data;
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