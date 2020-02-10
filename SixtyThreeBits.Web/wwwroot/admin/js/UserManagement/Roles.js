const RolesModel = {
    RolesGrid: null,
    OnRolesGridInit: function (s) {
        RolesModel.RolesGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(RolesModel.RolesGrid, s.element[0]);
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        RolesModel.RolesGrid.addRow();
    });
});