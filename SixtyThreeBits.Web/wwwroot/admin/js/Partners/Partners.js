const PartnersModel = {
    PartnersGrid: null,
    OnPartnersGridInitialized: function (s) {
        PartnersModel.PartnersGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(PartnersModel.PartnersGrid, s.element[0]);
    }    
};

$(function () {
    $('.js-add-new-button').click(function () {
        PartnersModel.PartnersGrid.addRow();
    });
});