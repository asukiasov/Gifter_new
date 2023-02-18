const BrandsModel = {
    Grid: null,
    OnGridInit: function (s) {
        BrandsModel.Grid = s.component;
        Globals.Devexpress.SetGridFullHeight(BrandsModel.Grid, s.element[0]);
    }    
};

$(function () {
    $('.js-add-new-button').click(function () {
        BrandsModel.Grid.addRow();
    });
});