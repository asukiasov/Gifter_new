const RedirectsModel = {
    RedirectsGrid: null,
    OnGridInit: function (s) {
        RedirectsModel.RedirectsGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(RedirectsModel.RedirectsGrid, s.element[0]);
    }        
};

$(function () {
    $(Globals.Selectors.ButtonAddNew).click(function () {
        RedirectsModel.RedirectsGrid.addRow();
    });
});