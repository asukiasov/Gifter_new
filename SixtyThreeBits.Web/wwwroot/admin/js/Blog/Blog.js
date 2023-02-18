const BlogModel = {
    Grid: null,
    OnGridInit: function (s) {
        BlogModel.Grid = s.component;
        Globals.Devexpress.SetGridFullHeight(BlogModel.Grid, s.element[0]);
    },
    GetDetailsButtonColumnCellHtml: function (Element, CellInfo) {
        Element.append('<a href=\"' + CellInfo.data.UrlBlogPost + '\"><i class=\"fas fa-info-circle\"></i></a>');
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        BlogModel.Grid.addRow();
    });
});