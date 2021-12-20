const PartnersModel = {
    PartnersGrid: null,
    OnPartnersGridInitialized: function (s) {
        PartnersModel.PartnersGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(PartnersModel.PartnersGrid, s.element[0]);
    },
    GetDetailsButtonColumnCellHtml: function (Element, CellInfo) {
        Element.append('<a href=\"' + CellInfo.data.UrlPartnerProperties+ '\"><i class=\"fas fa-info-circle\"></i></a>')
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        PartnersModel.PartnersGrid.addRow();
    });

});