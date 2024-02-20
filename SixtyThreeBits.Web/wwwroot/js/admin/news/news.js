const newsModel = {
    grid: null,
    onGridInit: function (e) {
        newsModel.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    }
};

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        newsModel.grid.addRow();
    });
});