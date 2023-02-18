const ProjectsModel = {

    ProjectID: null,
    UrlSync: null,

    ProjectsGrid: null,
    OnProjectsGridInit: function (s) {
        ProjectsModel.ProjectsGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(ProjectsModel.ProjectsGrid, s.element[0]);
    }    

    OnReorder: function (e) {
        const ProjectsSortIndexes = Globals.Devexpress.GetGridSortIndexes(e, 'ProjectID');
        var SortIndexes = new Array();
        console.log(JSON.stringify(ProjectsSortIndexes));
        $.ajax({
            type: 'POST',
            url: ProjectsModel.UrlSync,
            data: { SortIndexes: ProjectsSortIndexes },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                }
                else {
                    Components63Bits.Dialog.Error();
                }
            },
            error: function () {
                Components63Bits.Dialog.Error();
            },
            complete: function () {
                preloader.hide();
            }
        });
        e.component.refresh()
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        ProjectsModel.ProjectsGrid.addRow();
    });
});