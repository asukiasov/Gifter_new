const NewsModel = {
    Grid: null,
    OnGridInit: function (s) {
        NewsModel.Grid = s.component;
        Globals.Devexpress.SetGridFullHeight(NewsModel.Grid, s.element[0]);
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        NewsModel.Grid.addRow();
    });
});