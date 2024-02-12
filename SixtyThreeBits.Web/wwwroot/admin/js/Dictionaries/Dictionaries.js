const dictionariesModel = {    
    tree: null,
    urlUpdate: null,

    onTreeInit: function (e) {
        dictionariesModel.tree = e.component;
        globals.devexpress.setGridFullHeight(dictionariesModel.tree, e.element[0]);
    }
};

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        dictionariesModel.tree.addRow();
    });
});