const UsersModel = {
    UsersGrid:null,
    OnUsersGridInit: function (s) {
        UsersModel.UsersGrid = s.component;
        
        Globals.Devexpress.SetGridFullHeight(UsersModel.UsersGrid,s.element[0]);        
    },
    OnUsersGridInitNewRow: function (s) {
        s.data.UserIsActive = false;
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        UsersModel.UsersGrid.addRow();
    });
});