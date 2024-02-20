const projectsModel = {    
    grid: null,
    urlSync: null,    

    onGridInit: function (e) {
        projectsModel.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    },
    onGridReorder: function (e) {
        projectsModel.syncSortIndexes(e);        
    },
    syncSortIndexes: function (e) {
        const sortIndexes = globals.devexpress.getGridSortIndexes('ProjectID', projectsModel.grid, e);

        $.ajax({
            type: 'POST',
            url: projectsModel.urlSync,
            data: { SortIndexes: sortIndexes },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    projectsModel.grid.refresh()
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
};

$(function () {
    $('.js-add-new-button').click(function () {
        projectsModel.grid.addRow();
    });
});