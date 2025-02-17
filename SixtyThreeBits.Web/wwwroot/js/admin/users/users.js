const model = {
    grid: null,
    onGridInit: function (e) {
        model.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    },
    onGridRowUpdating: function (e) {        
        globals.devexpress.onRowUpdatingSendAllColumnsData(e);
    },
    getDetailsButtonColumnCellHtml: function (element, cellInfo) {
        //element.append('<a href=\"' + cellInfo.data.UrlDetails + '\"><i class=\"fas fa-info-circle\"></i></a>');
    }
};

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        model.grid.addRow();
    });
});