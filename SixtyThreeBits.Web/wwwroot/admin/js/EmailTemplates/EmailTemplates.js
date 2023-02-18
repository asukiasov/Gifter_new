const EmailTemplatesModel = {
    Grid:null,
    OnGridInit: function (s) {
        EmailTemplatesModel.Grid = s.component;
        Globals.Devexpress.SetGridFullHeight(s.component, s.element[0]);
    }    
};