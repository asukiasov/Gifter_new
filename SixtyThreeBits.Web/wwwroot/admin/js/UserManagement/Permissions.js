const PermissionsModel = {    
    PermissionsTree: null,
    UrlUpdate: null,

    OnPermissionsTreeInit: function (e) {
        PermissionsModel.PermissionsTree = e.component;
        Globals.Devexpress.SetGridFullHeight(PermissionsModel.PermissionsTree, e.element[0]);
    },
    OnPermissionsTreeToolbarPreparing: function (e) {
        e.toolbarOptions.visible = false;
        //const ToolbarItems = e.toolbarOptions.items; 
        //var AddButton = ToolbarItems[0];  // find the index of add button or loop and find
        //AddButton.visible = false; //hide the item
        //e.event.toolbarOptions.items = []; // clear the toolbar
    },
    OnPermissionsTreeReorder: function (e) {

        const PermissionID = e.itemData.PermissionID
        let PermissionParentID = Globals.Constants.NullValueFor.Int;
        
        if (e.dropInsideItem) {
            visibleRows = PermissionsModel.PermissionsTree.getVisibleRows();
            const Parent = visibleRows[e.toIndex].data;
            PermissionParentID = Parent.PermissionID;
        }

        $.ajax({
            type: 'PUT',
            url: PermissionsModel.UrlUpdate,
            data: { key: PermissionID, values: JSON.stringify({ PermissionParentID: PermissionParentID }) },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },            
            complete: function () {
                PermissionsModel.PermissionsTree.refresh();
                preloader.hide();
            }
        });
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        PermissionsModel.PermissionsTree.addRow();
    });
});