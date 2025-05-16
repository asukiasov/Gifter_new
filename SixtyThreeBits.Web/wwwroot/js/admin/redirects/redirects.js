const model = {
    grid: null,
    onGridInit: function (e) {
        model.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    }        
};

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        model.grid.addRow();
    });
});