const EmailTemplatesModel = {
    EmailTemplatesGrid:null,
    OnEmailTemplatesGridInit: function (s) {
        EmailTemplatesModel.EmailTemplatesGrid = s.component;
        Globals.Devexpress.SetGridFullHeight(s.component, s.element[0]);
    },
    GetDetailsButtonColumnCellHtml: function (Element, CellInfo) {
        Element.append('<a href=\"' + CellInfo.data.UrlEmailTemplate + '\"><i class=\"fas fa-info-circle\"></i></a>')
    }
};