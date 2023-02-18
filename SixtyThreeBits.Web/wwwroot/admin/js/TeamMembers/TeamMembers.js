const TeamMembersModel = {

    TeamMemberID: null,
    UrlSync: null,

    Grid: null,
    OnGridInit: function (s) {
        TeamMembersModel.Grid = s.component;
        Globals.Devexpress.SetGridFullHeight(TeamMembersModel.Grid, s.element[0]);
    },
    OnGridReorder: function (e) {
        TeamMembersModel.SyncTeamMemberSortIndexes(e);
    },
    GetDetailsButtonColumnCellHtml: function (Element, CellInfo) {
        Element.append('<a href=\"' + CellInfo.data.UrlTeamMemberProperties + '\"><i class=\"fas fa-info-circle\"></i></a>')
    },    
    SyncTeamMemberSortIndexes: function (e) {
        const TeamMembersSortIndexes = Globals.Devexpress.GetGridSortIndexes('TeamMemberID', TeamMembersModel.Grid, e);
        
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
                    TeamMembersModel.Grid.refresh()
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
        TeamMembersModel.Grid.addRow();
    });
});