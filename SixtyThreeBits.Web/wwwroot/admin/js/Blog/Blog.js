const BlogModel = {
    BlogGrid: null,
    OnBlogGridInit: function (s) {
        BlogModel.BlogGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(BlogModel.BlogGrid, s.element[0]);
    },
    GetDetailsButtonColumnCellHtml: function (Element, CellInfo) {
        Element.append('<a href=\"' + CellInfo.data.UrlBlogProperties + '\"><i class=\"fas fa-info-circle\"></i> Detalis </a>')
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        BlogModel.BlogGrid.addRow();
    });
});