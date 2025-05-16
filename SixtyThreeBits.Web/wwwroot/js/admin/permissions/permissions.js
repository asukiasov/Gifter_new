const model = {    
    tree: null,
    urlUpdate: null,

    onTreeInit: function (e) {
        model.tree = e.component;
        globals.devexpress.setGridFullHeight(e.component);
    },
    onTreeInitNewRow: function (e) {
        e.data.PermissionIsMenuItem = false;
    },    
    onTreeReorder: function (e) {

        const permissionID = e.itemData.PermissionID
        let permissionParentID = globals.constants.nullValueFor.int;
        
        if (e.dropInsideItem) {
            visibleRows = model.tree.getVisibleRows();
            const parent = visibleRows[e.toIndex].data;
            permissionParentID = parent.PermissionID;
        }

        $.ajax({
            type: 'PUT',
            url: model.urlUpdate,
            data: { key: permissionID, values: JSON.stringify({ PermissionParentID: permissionParentID }) },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },            
            complete: function () {
                model.tree.refresh();
                preloader.hide();
            }
        });
    }
};

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        model.tree.addRow();
    });
});