const DictionariesModel = {    
    Tree: null,
    UrlUpdate: null,

    OnTreeInit: function (s) {
        DictionariesModel.Tree = s.component;
        Globals.Devexpress.SetGridFullHeight(DictionariesModel.Tree, s.element[0]);
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        DictionariesModel.Tree.addRow();
    });
});