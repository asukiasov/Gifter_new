const dictionariesModel = {    
    tree: null,
    urlUpdate: null,

    onTreeInit: function (e) {
        dictionariesModel.tree = e.component;
        globals.devexpress.setGridFullHeight(dictionariesModel.tree, e.element[0]);
    },

    //In order to pass those values back, that remain unchanged
    onRowUpdating: function (options) {        
        $.extend(options.newData, $.extend({}, options.oldData, options.newData));
    }
};

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        dictionariesModel.tree.addRow();
    });
});