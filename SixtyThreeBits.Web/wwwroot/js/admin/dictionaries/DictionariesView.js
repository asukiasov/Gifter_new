const model = {    
    tree: null,
    urlUpdate: null,

    onTreeInit: function (e) {
        model.tree = e.component;
        globals.devexpress.setGridFullHeight(e.component);
    },

    onTreeRowUpdating: function (e) {
        globals.devexpress.onRowUpdatingSendAllColumnsData(e);        
    }
};

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        model.tree.addRow();
    });
});