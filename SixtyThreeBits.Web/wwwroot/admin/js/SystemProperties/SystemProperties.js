const SystemPropertiesModel = {    
    
};

$(function () {
    $('.js-google-iframe-help').click(function (e) {
        e.preventDefault();
        var src = $(this).attr('href');

        FancyBox.Init({
            src: src
        }).ShowImagePopup();    
    });
    
});