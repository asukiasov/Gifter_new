const model = {
    grid: null,
    urlSync: null,
    
    onGridInit: function (e) {
        model.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component);
    },
    onGridReorder: function (e) {
        model.syncSortIndexes(e);
    },
    syncSortIndexes: function (e) {
        const sortIndexes = globals.devexpress.getGridSortIndexes('TeamMemberID', model.grid, e);
        
        $.ajax({
            type: 'POST',
            url: model.urlSync,
            data: { SortIndexes: sortIndexes },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    model.grid.refresh()
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
        model.grid.addRow();
    });
});