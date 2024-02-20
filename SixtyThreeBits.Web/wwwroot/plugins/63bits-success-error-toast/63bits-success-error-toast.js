const successErrorToast63Bits = {
    isTop: false,
    message: null,
    showError: false,
    showSuccess: false,
    hideSuccessMessageAutomatically: true,

    init: function (options) {
        if (options != undefined) {
            successErrorToast63Bits.message = options.message;
            successErrorToast63Bits.showSuccess = options.showSuccess;
            successErrorToast63Bits.showError = options.showError;
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

        if (successErrorToast63Bits.showError) {
            $('.succes-error').addClass('error opened');
        }
        else if (successErrorToast63Bits.showSuccess) {
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

    showError: function (errorMessage) {
        successErrorToast63Bits.init({ showError: true, message: errorMessage }).showMessage();
    },

    showSuccess: function (successMessage) {
        successErrorToast63Bits.init({ showSuccess: true, message: successMessage }).showMessage();
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