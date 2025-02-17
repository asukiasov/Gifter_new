const model = {    
    tree: null,
    urlUpdate: null,

    onTreeInit: function (e) {
        model.tree = e.component;
        globals.devexpress.setGridFullHeight(model.tree, e.element[0]);
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