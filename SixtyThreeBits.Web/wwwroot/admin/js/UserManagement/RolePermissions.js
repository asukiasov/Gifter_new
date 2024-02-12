const rolePermissionsModel = {    
    rolesGrid: null,
    permissionsTree: null,
    urlGetRolePermissions: null,
    urlSave: null,
    urlUpdate: null,    
    roleIDRocused: null,    
    isPermissionsTreeContentReady:false,

    onRolesGridInit: function (e) {
        rolePermissionsModel.rolesGrid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    },    
    onPermissionsTreeInit: function (e) {
        rolePermissionsModel.permissionsTree = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    },
    onPermissionsTreeContentReady: function (e) {
        rolePermissionsModel.isPermissionsTreeContentReady = true;
    },
    onRolesGridFocusedRowChanged: function (e) {        

        if (!rolePermissionsModel.isPermissionsTreeContentReady) {
            setTimeout(function () {
                rolePermissionsModel.onRolesGridFocusedRowChanged(e);
            }, 1000);

            return;
        }

        const roleID = rolePermissionsModel.roleIDRocused = e.row.key;
        $.ajax({
            type: 'GET',
            url: rolePermissionsModel.urlGetRolePermissions,
            data: { RoleID: roleID  },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    rolePermissionsModel.permissionsTree.selectRows(res.Data);
                }
            },
            complete: function () {                
                preloader.hide();
            }
        });
    }
};

$(function () {
    $(globals.selectors.buttonSave).click(function () {
        const permissionIDs = rolePermissionsModel.permissionsTree.getSelectedRowKeys();

        $.ajax({
            type: 'PUT',
            url: rolePermissionsModel.urlSave,
            data: { RoleID: rolePermissionsModel.roleIDRocused, PermissionIDs: permissionIDs },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    successErrorMessageObject.showGlobalSuccess();
                }
            },
            complete: function () {
                preloader.hide();
            }
        });
    });
});