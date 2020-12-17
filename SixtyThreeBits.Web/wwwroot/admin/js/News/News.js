const NewsModel = {
    NewsGrid: null,
    OnNewsGridInit: function (s) {
        NewsModel.NewsGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(NewsModel.NewsGrid, s.element[0]);
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        NewsModel.NewsGrid.addRow();
    });
});