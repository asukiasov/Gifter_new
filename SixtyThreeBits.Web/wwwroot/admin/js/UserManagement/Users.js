const usersModel = {
    grid: null,
    onGridInit: function (e) {
        usersModel.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    },
    getDetailsButtonColumnCellHtml: function (element, cellInfo) {
        //element.append('<a href=\"' + cellInfo.data.UrlDetails + '\"><i class=\"fas fa-info-circle\"></i></a>');
    }
};

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        usersModel.grid.addRow();
    });
});