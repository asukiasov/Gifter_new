const rolesModel = {
    grid: null,
    onGridInit: function (e) {
        rolesModel.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    }
};

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        rolesModel.grid.addRow();
    });
});