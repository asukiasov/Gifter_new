const model = {
    grid: null,
    onGridInit: function (e) {
        model.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component);
    }    
};

$(function () {
    $('.js-add-new-button').click(function () {
        model.grid.addRow();
    });
});