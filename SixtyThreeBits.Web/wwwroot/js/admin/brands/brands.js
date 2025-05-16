const model = {
    grid: null,
    onGridInit: function (e) {
        model.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    }    
};

$(function () {
    $('.js-add-new-button').click(function () {
        model.grid.addRow();
    });
});