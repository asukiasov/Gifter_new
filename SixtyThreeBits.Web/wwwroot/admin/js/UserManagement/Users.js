const UsersModel = {
    UsersGrid:null,
    OnUsersGridInit: function (s) {
        UsersModel.UsersGrid = s.component;        
        Globals.Devexpress.SetGridFullHeight(UsersModel.UsersGrid,s.element[0]);        
    },
    GetDetailsButtonColumnCellHtml: function (Element, CellInfo) {
        //Element.append('<a href=\"' + CellInfo.data.UrlDetails + '\"><i class=\"fas fa-info-circle\"></i></a>');
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        UsersModel.UsersGrid.addRow();
    });
});