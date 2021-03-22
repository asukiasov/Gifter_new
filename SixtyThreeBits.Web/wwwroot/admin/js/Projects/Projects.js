const ProjectsModel = {
    ProjectsGrid: null,
    OnProjectsGridInit: function (s) {
        ProjectsModel.ProjectsGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(ProjectsModel.ProjectsGrid, s.element[0]);
    },
    GetDetailsButtonColumnCellHtml: function (Element, CellInfo) {
        Element.append('<a href=\"' + CellInfo.data.UrlProjectsProperties + '\"><i class=\"fas fa-info-circle\"></i></a>')
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        ProjectsModel.ProjectsGrid.addRow();
    });
});