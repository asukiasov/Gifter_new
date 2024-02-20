const permissionsModel = {    
    tree: null,
    urlUpdate: null,

    onTreeInit: function (e) {
        permissionsModel.tree = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    },
    onTreeInitNewRow: function (e) {
        e.data.PermissionIsMenuItem = false;
    },    
    onTreeReorder: function (e) {

        const permissionID = e.itemData.PermissionID
        let permissionParentID = globals.constants.nullValueFor.int;
        
        if (e.dropInsideItem) {
            visibleRows = permissionsModel.tree.getVisibleRows();
            const parent = visibleRows[e.toIndex].data;
            permissionParentID = parent.PermissionID;
        }

        $.ajax({
            type: 'PUT',
            url: permissionsModel.urlUpdate,
            data: { key: permissionID, values: JSON.stringify({ PermissionParentID: permissionParentID }) },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },            
            complete: function () {
                permissionsModel.tree.refresh();
                preloader.hide();
            }
        });
    }
};

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        permissionsModel.tree.addRow();
    });
});