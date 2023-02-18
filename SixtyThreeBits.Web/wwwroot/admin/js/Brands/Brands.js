const BrandsModel = {
    Grid: null,
    OnGridInit: function (s) {
        BrandsModel.Grid = s.component;
        Globals.Devexpress.SetGridFullHeight(BrandsModel.Grid, s.element[0]);
    },

    GetDetailsButtonColumnCellHtml: function (Element, CellInfo) {
        Element.append('<a href=\"' + CellInfo.data.UrlBrandProperties + '\"><i class=\"fas fa-info-circle\"></i></a>')
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        BrandsModel.Grid.addRow();
    });
});