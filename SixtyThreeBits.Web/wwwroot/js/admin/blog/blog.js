const blogModel = {
    grid: null,
    onGridInit: function (s) {
        blogModel.grid = s.component;
        globals.devexpress.setGridFullHeight(blogModel.grid, s.element[0]);
    }    
};

$(function () {
    $('.js-add-new-button').click(function () {
        blogModel.grid.addRow();
    });
});