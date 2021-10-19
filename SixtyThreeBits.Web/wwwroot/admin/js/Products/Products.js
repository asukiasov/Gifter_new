const ProductsModel = {
    ProductsGrid: null,
    IsError: null,
    OnProductsGridInit: function (s) {
        ProductsModel.ProductsGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(ProductsModel.ProductsGrid, s.element[0]);
    },
    GetDetailsButtonColumnCellHtml: function (Element, CellInfo) {
        Element.append('<a href=\"' + CellInfo.data.UrlTeamMembersProperties + '\"><i class=\"fas fa-info-circle\"></i></a>')
    }
};

$(function () {
    $('.js-add-new-button').click(function () {
        ProductsModel.ProductsGrid.addRow();
    });

    $('.js-upload-excel-button').click(function () {

        $('.js-upload-excel-file-modal').modal("show");       
    });          

    $(".js-upload-excel-file-modal").on("hidden.bs.modal", function () {
        $(".js-error-list").remove();
    });

    if (ProductsModel.IsError) {
        $('.js-upload-excel-file-modal').modal("show");
    }
    
    //$('.js-upload-excel-file-button').click(function () {        

    //}); 

    //$('.js-download-excel-file-button').click(function () {        

    //}); 

});
