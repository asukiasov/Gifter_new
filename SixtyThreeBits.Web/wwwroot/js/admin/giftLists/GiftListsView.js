const model = {
    grid: null,
    urlLoadFollowers: '',
    onGridInit: function (e) {
        model.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component);
    },
    onGridRowUpdating: function (e) {
        globals.devexpress.onRowUpdatingSendAllColumnsData(e);
    },
    renderFollowersDetail: function (container, options) {
        var masterData = options.data;
        var followingUserID = masterData.GiftListUserID;

        if (!followingUserID) {
            $('<div>')
                .addClass('p-3 text-muted')
                .text('No owner assigned to this gift list.')
                .appendTo(container);
            return;
        }

        var $detailGrid = $('<div>')
            .addClass('p-3')
            .appendTo(container);

        $('<h6>')
            .addClass('mb-3')
            .text('Followers')
            .appendTo($detailGrid);

        var $gridContainer = $('<div>')
            .appendTo($detailGrid);

        $gridContainer.dxDataGrid({
            dataSource: {
                store: {
                    type: 'odata',
                    url: model.urlLoadFollowers + followingUserID,
                    key: 'FollowerID'
                }
            },
            columns: [
                { dataField: 'FollowerID', caption: '#', width: 60 },
                { dataField: 'FollowingUserFullname', caption: 'Follower', width: 200 },
                { dataField: 'FollowingUserEmail', caption: 'Email', width: 250 },
                { dataField: 'FollowerDateCreated', caption: 'Since', width: 150, dataType: 'datetime', format: 'yyyy-MM-dd HH:mm' }
            ],
            showBorders: true,
            showRowLines: true,
            allowColumnResizing: true,
            paging: {
                enabled: true,
                pageSize: 10
            },
            noDataText: 'No one is following this user.'
        });
    }
};

$(function () {
    model.urlLoadFollowers = modelConfig.urlLoadFollowers;

    $(globals.selectors.buttonAddNew).click(function () {
        model.grid.addRow();
    });
});
