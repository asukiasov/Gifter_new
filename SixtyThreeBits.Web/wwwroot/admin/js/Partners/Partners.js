const partnersModel = {
    grid: null,
    onGridInit: function (e) {
        partnersModel.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    }    
};

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        partnersModel.grid.addRow();
    });
});