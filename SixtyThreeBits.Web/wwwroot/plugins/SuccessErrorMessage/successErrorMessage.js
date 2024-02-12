var successErrorMessageObject = {
    isTop: false,
    message: null,
    showError: false,
    showSuccess: false,
    hideSuccessMessageAutomatically: true,

    init: function (options) {

        if (options != undefined) {
            successErrorMessageObject.message = options.message;
            successErrorMessageObject.showSuccess = options.showSuccess;
            successErrorMessageObject.showError = options.showError;
            successErrorMessageObject.hideSuccessMessageAutomatically = options.hideSuccessMessageAutomatically ? options.hideSuccessMessageAutomatically : successErrorMessageObject.hideSuccessMessageAutomatically;
        }

        return successErrorMessageObject;
    },

    showMessage: function () {

        $('.succes-error span').html(successErrorMessageObject.message);
        $('.succes-error').removeClass('hidden');
        $('.succes-error').removeClass('error');

        if (!successErrorMessageObject.isTop) {
            $('.succes-error').addClass('bottom');
        }

        if (successErrorMessageObject.showError) {
            $('.succes-error').addClass('error opened');
        }
        else if (successErrorMessageObject.showSuccess) {
            $('.succes-error').addClass('opened');
            if (successErrorMessageObject.hideSuccessMessageAutomatically) {
                setTimeout(function () {
                    successErrorMessageObject.hideMessage();
                }, 5000);
            }
        }
        else {
            $('.succes-error').addClass('hidden');
        }
    },

    hideMessage: function () {
        $('.succes-error').removeClass('opened');
    },

    showGlobalError: function () {
        successErrorMessageObject.init({ showError: true, message: globals.textError }).showMessage();
    },

    showGlobalSuccess: function () {
        successErrorMessageObject.init({ showSuccess: true, message: globals.textSuccess }).showMessage();
    }
}

$(function () {
    $('.succes-error .close-btn').click(function () {
        successErrorMessageObject.hideMessage();
        $('.succes-error').removeClass('error')
        return false;
    });
    if (successErrorMessageObject.hideSuccessMessageAutomatically) {
        if (!$('.succes-error').hasClass('error')) {
            setTimeout(function () {
                $('.succes-error .close-btn').trigger('click');
            }, 5000);
        }
    }
});
