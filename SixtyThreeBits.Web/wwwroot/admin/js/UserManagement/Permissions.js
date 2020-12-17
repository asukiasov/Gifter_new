const PermissionsModel = {    
    PermissionsTree: null,
    UrlUpdate: null,

    OnPermissionsTreeInit: function (s) {
        PermissionsModel.PermissionsTree = s.component;
        Globals.Devexpress.SetGridFullHeight(PermissionsModel.PermissionsTree, s.element[0]);
    },
    OnPermissionsTreeInitNewRow: function (s) {
        s.data.PermissionIsMenuItem = false;
    },
    OnPermissionsTreeToolbarPreparing: function (s) {
        s.toolbarOptions.visible = false;
        //const ToolbarItems = s.toolbarOptions.items; 
        //var AddButton = ToolbarItems[0];  // find the index of add button or loop and find
        //AddButton.visible = false; //hide the item
        //s.event.toolbarOptions.items = []; // clear the toolbar
    },
    OnPermissionsTreeReorder: function (s) {

        const PermissionID = s.itemData.PermissionID
        let PermissionParentID = Globals.Constants.NullValueFor.Int;
        
        if (s.dropInsideItem) {
            visibleRows = PermissionsModel.PermissionsTree.getVisibleRows();
            const Parent = visibleRows[s.toIndex].data;
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