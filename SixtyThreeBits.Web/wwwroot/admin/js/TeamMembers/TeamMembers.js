const TeamMembersModel={
    TeamMembersGrid: null,
    OnTeamMembersGridInit: function (s) {
        TeamMembersModel.TeamMembersGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(TeamMembersModel.TeamMembersGrid, s.element[0]);
    },
    GetDetailsButtonColumnCellHtml: function (Element, CellInfo) {
        Element.append('<a href=\"' + CellInfo.data.UrlTeamMemberProperties + '\"><i class=\"fas fa-info-circle\"></i></a>')
    },
    OnTeamMembersGridInitNewRow: function (s) {
        s.data.TeamMemberIsPublished = false;
    }
}

$(function () {
    $('.js-add-new-button').click(function () {
        TeamMembersModel.TeamMembersGrid.addRow();
    });
});