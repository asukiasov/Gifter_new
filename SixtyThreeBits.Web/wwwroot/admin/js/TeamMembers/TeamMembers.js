const TeamMembersModel = {

    TeamMemberID: null,
    UrlSync: null,

    TeamMembersGrid: null,
    OnTeamMembersGridInit: function (s) {
        TeamMembersModel.TeamMembersGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(TeamMembersModel.TeamMembersGrid, s.element[0]);
    },
    GetDetailsButtonColumnCellHtml: function (Element, CellInfo) {
        Element.append('<a href=\"' + CellInfo.data.UrlTeamMemberProperties + '\"><i class=\"fas fa-info-circle\"></i></a>')
    },
    OnReorder: function (e) {
        TeamMembersModel.SyncTeamMemberSortIndexes(e);
    },
    SyncTeamMemberSortIndexes: function (e) {
        const TeamMembersSortIndexes = Globals.Devexpress.GetGridSortIndexes('TeamMemberID', TeamMembersModel.TeamMembersGrid, e);
        
        $.ajax({
            type: 'POST',
            url: TeamMembersModel.UrlSync,
            data: { SortIndexes: TeamMembersSortIndexes },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    TeamMembersModel.TeamMembersGrid.refresh()
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
    }
}

$(function () {
    $('.js-add-new-button').click(function () {
        TeamMembersModel.TeamMembersGrid.addRow();
    });
});