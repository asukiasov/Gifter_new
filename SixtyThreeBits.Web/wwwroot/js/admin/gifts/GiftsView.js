const model = {
    grid: null,
    onGridInit: function (e) {
        model.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component);
    },
    onGridRowUpdating: function (e) {
        globals.devexpress.onRowUpdatingSendAllColumnsData(e);
    }
};

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        model.grid.addRow();
    });
});