const NewsModel = {
    NewsGrid: null,
    OnNewsGridInit: function (s) {
        NewsModel.NewsGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(NewsModel.NewsGrid, s.element[0]);
    },
    GetDetailsButtonColumnCellHtml: function (Element, CellInfo) {
        Element.append('<a href=\"' + CellInfo.data.UrlNewsProperties + '\"><i class=\"fas fa-info-circle\"></i></a>')
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        NewsModel.NewsGrid.addRow();
    });
});