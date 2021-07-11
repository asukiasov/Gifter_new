const BrandsModel = {
    BrandsGrid: null,
    OnBrandsGridInit: function (s) {
        BrandsModel.BrandsGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(BrandsModel.BrandsGrid, s.element[0]);
    },

    GetDetailsButtonColumnCellHtml: function (Element, CellInfo) {
        Element.append('<a href=\"' + CellInfo.data.UrlBrandProperties + '\"><i class=\"fas fa-info-circle\"></i></a>')
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        BrandsModel.BrandsGrid.addRow();
    });
});