var UsersModel = {
    UsersGrid:null,
    OnUsersGridInit: function (s) {
        UsersModel.UsersGrid = s.component;
        
        Globals.Devexpress.SetGridFullHeight(UsersModel.UsersGrid,s.element[0]);        
    }
};

$(function () {

});