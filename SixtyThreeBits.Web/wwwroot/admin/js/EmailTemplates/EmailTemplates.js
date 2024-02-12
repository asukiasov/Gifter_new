const emailTemplatesModel = {
    grid:null,
    onGridInit: function (e) {
        emailTemplatesModel.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component, e.element[0]);
    }    
};