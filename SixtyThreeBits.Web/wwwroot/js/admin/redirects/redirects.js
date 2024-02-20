const redirectsModel = {
    grid: null,
    onGridInit: function (e) {
        redirectsModel.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    }        
};

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        redirectsModel.grid.addRow();
    });
});