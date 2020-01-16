var SuccessErrorMessageObject = {
    IsTop: false,
    Message: null,
    ShowError: false,
    ShowSuccess: false,
    HideSuccessMessageAutomatically: true,

    Init: function (options) {

        if (options != undefined) {
            SuccessErrorMessageObject.Message = options.Message;
            SuccessErrorMessageObject.ShowSuccess = options.ShowSuccess;
            SuccessErrorMessageObject.ShowError = options.ShowError;
            SuccessErrorMessageObject.HideSuccessMessageAutomatically = options.HideSuccessMessageAutomatically ? options.HideSuccessMessageAutomatically : SuccessErrorMessageObject.HideSuccessMessageAutomatically;
        }

        return SuccessErrorMessageObject;
    },

    ShowMessage: function () {

        $('.succes-error span').html(SuccessErrorMessageObject.Message);
        $('.succes-error').removeClass('hidden');
        $('.succes-error').removeClass('error');

        if (!SuccessErrorMessageObject.IsTop) {
            $('.succes-error').addClass('bottom');
        }

        if (SuccessErrorMessageObject.ShowError) {
            $('.succes-error').addClass('error opened');
        }
        else if (SuccessErrorMessageObject.ShowSuccess) {
            $('.succes-error').addClass('opened');
            if (SuccessErrorMessageObject.HideSuccessMessageAutomatically) {
                setTimeout(function () {
                    SuccessErrorMessageObject.HideMessage();
                }, 5000);
            }
        }
        else {
            $('.succes-error').addClass('hidden');
        }
    },

    HideMessage: function () {
        $('.succes-error').removeClass('opened');
    },

    ShowGlobalError: function () {
        SuccessErrorMessageObject.Init({ ShowError: true, Message: Globals.TextError }).ShowMessage();
    },

    ShowGlobalSuccess: function () {
        SuccessErrorMessageObject.Init({ ShowSuccess: true, Message: Globals.TextSuccess }).ShowMessage();
    }
}

$(function () {
    $('.succes-error .close-btn').click(function () {
        SuccessErrorMessageObject.HideMessage();
        $('.succes-error').removeClass('error')
        return false;
    });
    if (SuccessErrorMessageObject.HideSuccessMessageAutomatically) {
        if (!$('.succes-error').hasClass('error')) {
            setTimeout(function () {
                $('.succes-error .close-btn').trigger('click');
            }, 5000);
        }
    }
});
