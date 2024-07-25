const successErrorToast63Bits = {
    isTop: false,
    message: null,
    textError: null,
    textSuccess: null,
    isError: false,
    isSuccess: false,
    hideSuccessMessageAutomatically: true,

    init: function (options) {
        if (options != undefined) {
            successErrorToast63Bits.isSuccess = options.isSuccess ? options.isSuccess : null;
            successErrorToast63Bits.isError = options.isError ? options.isError : null;
            successErrorToast63Bits.message = options.message;            
            successErrorToast63Bits.hideSuccessMessageAutomatically = options.hideSuccessMessageAutomatically ? options.hideSuccessMessageAutomatically : successErrorToast63Bits.hideSuccessMessageAutomatically;
        }
        return successErrorToast63Bits;
    },

    showMessage: function () {
        $('.succes-error span').html(successErrorToast63Bits.message);
        $('.succes-error').removeClass('hidden');
        $('.succes-error').removeClass('error');

        if (!successErrorToast63Bits.isTop) {
            $('.succes-error').addClass('bottom');
        }

        if (successErrorToast63Bits.isError) {
            $('.succes-error').addClass('error opened');
        }
        else if (successErrorToast63Bits.isSuccess) {
            $('.succes-error').addClass('opened');
            if (successErrorToast63Bits.hideSuccessMessageAutomatically) {
                setTimeout(function () {
                    successErrorToast63Bits.hideMessage();
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

    showErrorMessage: function (errorMessage) {
        errorMessage = errorMessage ? errorMessage : successErrorToast63Bits.textError;
        successErrorToast63Bits.init({ isError: true, message: errorMessage }).showMessage();
    },

    showSuccessMessage: function (successMessage) {
        successMessage = successMessage ? successMessage : successErrorToast63Bits.textSuccess;
        successErrorToast63Bits.init({ isSuccess: true, message: successMessage }).showMessage();
    }
}

$(function () {
    $('.js-succes-error-close-button').click(function (e) {
        e.preventDefault();
        successErrorToast63Bits.hideMessage();
        $('.js-succes-error').removeClass('error')
    });

    if (successErrorToast63Bits.hideSuccessMessageAutomatically) {
        const isSuccessVisible = !$('.succes-error').hasClass('error');
        if (isSuccessVisible) {
            setTimeout(function () {
                $('.js-succes-error-close-button').trigger('click');
            }, 5000);
        }
    }
});