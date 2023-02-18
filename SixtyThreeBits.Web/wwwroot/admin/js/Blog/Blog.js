const BlogModel = {
    Grid: null,
    OnGridInit: function (s) {
        BlogModel.Grid = s.component;
        Globals.Devexpress.SetGridFullHeight(BlogModel.Grid, s.element[0]);
    }    
};

$(function () {
    $('.js-add-new-button').click(function () {
        BlogModel.Grid.addRow();
    });
});