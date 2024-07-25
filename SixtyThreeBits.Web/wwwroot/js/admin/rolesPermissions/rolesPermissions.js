const rolesPermissionsModel = {    
    rolesGrid: null,
    permissionsTree: null,
    urlPermissionsGetByRole: null,
    urlSave: null,
    urlUpdate: null,    
    roleIDRocused: null,    
    isPermissionsTreeContentReady:false,

    onRolesGridInit: function (e) {
        rolesPermissionsModel.rolesGrid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    },    
    onPermissionsTreeInit: function (e) {
        rolesPermissionsModel.permissionsTree = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    },
    onPermissionsTreeContentReady: function (e) {
        rolesPermissionsModel.isPermissionsTreeContentReady = true;
    },
    onRolesGridFocusedRowChanged: function (e) {        

        if (!rolesPermissionsModel.isPermissionsTreeContentReady) {
            setTimeout(function () {
                rolesPermissionsModel.onRolesGridFocusedRowChanged(e);
            }, 1000);

            return;
        }

        const roleID = rolesPermissionsModel.roleIDRocused = e.row.key;
        $.ajax({
            type: 'GET',
            url: rolesPermissionsModel.urlPermissionsGetByRole,
            data: { RoleID: roleID  },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    rolesPermissionsModel.permissionsTree.selectRows(res.Data);
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
        const permissionIDs = rolesPermissionsModel.permissionsTree.getSelectedRowKeys();

        $.ajax({
            type: 'POST',
            url: rolesPermissionsModel.urlSave,
            data: { RoleID: rolesPermissionsModel.roleIDRocused, PermissionIDs: permissionIDs },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    successErrorToast63Bits.showSuccessMessage(globals.textSuccess);
                }
            },
            complete: function () {
                preloader.hide();
            }
        });
    });
});