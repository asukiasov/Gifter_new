const pagesModel = {
    grid: null,
    onGridInit: function (e) {
        pagesModel.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    }
}

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        pagesModel.grid.addRow();
    });
});