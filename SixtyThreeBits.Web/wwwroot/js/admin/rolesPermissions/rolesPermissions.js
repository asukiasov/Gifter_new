const model = {    
    grid: null,
    tree: null,
    urlPermissionsGetByRole: null,
    urlSave: null,
    urlUpdate: null,    
    roleIDRocused: null,    
    isTreeContentReady:false,

    onGridInit: function (e) {
        model.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component);
    },
    onGridFocusedRowChanged: function (e) {

        if (!model.isTreeContentReady) {
            setTimeout(function () {
                model.onGridFocusedRowChanged(e);
            }, 1000);

            return;
        }

        const roleID = model.roleIDRocused = e.row.key;
        $.ajax({
            type: 'GET',
            url: model.urlPermissionsGetByRole,
            data: { RoleID: roleID },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    model.tree.selectRows(res.Data);
                }
            },
            complete: function () {
                preloader.hide();
            }
        });
    },
    onTreeInit: function (e) {
        model.tree = e.component;
        globals.devexpress.setGridFullHeight(e.component);
    },
    onTreeContentReady: function (e) {
        model.isTreeContentReady = true;
    }
};

$(function () {
    $(globals.selectors.buttonSave).click(function () {
        const permissionIDs = model.tree.getSelectedRowKeys();

        $.ajax({
            type: 'POST',
            url: model.urlSave,
            data: { RoleID: model.roleIDRocused, PermissionIDs: permissionIDs },
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