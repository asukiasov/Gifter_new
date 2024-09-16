const model = {
    textLogClearConfirm: null,
    urlClear: null,
    clear: function () {
        $.ajax({
            method: 'POST',
            url: model.urlClear,
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    utilities.reloadPage();
                }
                else {
                    preloader.hide();
                    components63Bits.dialog.error(res.Data);
                }
            },
            error: function () {
                preloader.hide();
                components63Bits.dialog.error();
            }
        });
    }
}
$(function () {
    $('.js-clear-button').click(function () {
        components63Bits.dialog.confirm({
            textConfirm: model.textLogClearConfirm,
            resolve: function () {
                model.clear();
            }
        }) 
    });
});