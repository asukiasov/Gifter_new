const brandsModel = {
    grid: null,
    onGridInit: function (e) {
        brandsModel.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    }    
};

$(function () {
    $('.js-add-new-button').click(function () {
        brandsModel.grid.addRow();
    });
});