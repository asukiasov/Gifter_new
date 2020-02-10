const RolePermissionsModel = {    
    RolesGrid: null,
    PermissionsTree: null,
    UrlGetRolePermissions: null,
    UrlSave: null,
    UrlUpdate: null,    
    RoleIDRocused: null,    
    IsPermissionsTreeContentReady:false,

    OnRolesGridInit: function (e) {
        RolePermissionsModel.RolesGrid = e.component;
        Globals.Devexpress.SetGridFullHeight(RolePermissionsModel.RolesGrid, e.element[0]);
    },    
    OnPermissionsTreeInit: function (e) {
        RolePermissionsModel.PermissionsTree = e.component;
        Globals.Devexpress.SetGridFullHeight(RolePermissionsModel.PermissionsTree, e.element[0]);
    },
    OnPermissionsTreeContentReady: function (e) {
        RolePermissionsModel.IsPermissionsTreeContentReady = true;
    },
    OnRolesGridFocusedRowChanged: function (e) {        

        if (!RolePermissionsModel.IsPermissionsTreeContentReady) {
            setTimeout(function () {
                RolePermissionsModel.OnRolesGridFocusedRowChanged(e);
            }, 1000);

            return;
        }

        const RoleID = RolePermissionsModel.RoleIDRocused = e.row.key;
        $.ajax({
            type: 'GET',
            url: RolePermissionsModel.UrlGetRolePermissions,
            data: { RoleID: RoleID  },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    RolePermissionsModel.PermissionsTree.selectRows(res.Data);
                }
            },
            complete: function () {                
                preloader.hide();
            }
        });
    }
};

$(function () {
    $('.js-save-button').click(function () {
        const PermissionIDs = RolePermissionsModel.PermissionsTree.getSelectedRowKeys();

        $.ajax({
            type: 'PUT',
            url: RolePermissionsModel.UrlSave,
            data: { RoleID: RolePermissionsModel.RoleIDRocused, PermissionIDs: PermissionIDs },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    SuccessErrorMessageObject.ShowGlobalSuccess();
                }
            },
            complete: function () {
                preloader.hide();
            }
        });
    });
});