const teamMembersModel = {
    grid: null,
    urlSync: null,
    
    onGridInit: function (e) {
        teamMembersModel.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    },
    onGridReorder: function (e) {
        teamMembersModel.syncSortIndexes(e);
    },
    syncSortIndexes: function (e) {
        const sortIndexes = globals.devexpress.getGridSortIndexes('TeamMemberID', teamMembersModel.grid, e);
        
        $.ajax({
            type: 'POST',
            url: teamMembersModel.urlSync,
            data: { SortIndexes: sortIndexes },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    teamMembersModel.grid.refresh()
                }
                else {
                    components63Bits.dialog.error();
                }
            },
            error: function () {
                components63Bits.dialog.error();
            },
            complete: function () {
                preloader.hide();
            }
        });
    }
}

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        teamMembersModel.grid.addRow();
    });
});