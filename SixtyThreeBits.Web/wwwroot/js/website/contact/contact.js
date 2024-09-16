const model = {
    recaptchaSiteKey: null,

    getFormDataPromise: function () {
        const name = $('input[name="Name"]').val();
        const email = $('input[name="Email"]').val();
        const message = $('textarea[name="Message"]').val();

        return new Promise(function (resolve, reject) {
            grecaptcha.ready(function () {
                grecaptcha.execute(model.recaptchaSiteKey, { action: 'contact' }).then(function (recaptchaClientResponseToken) {
                    resolve({
                        Name: name,
                        Email: email,
                        Message: message,
                        RecaptchaClientResponseToken: recaptchaClientResponseToken
                    })
                });
            });
        });        
    },
    submitContactForm: function () {
        preloader.show();

        model.getFormDataPromise().then(function (formData) {
            $.ajax({
                url: document.URL,
                type: 'POST',
                data: formData,
                dataType: 'json',
                beforeSend: function () {
                    validation.hideErrors();
                },
                success: function (e) {
                    if (e.IsSuccess) {
                        $('.js_contact_form').remove();
                        $('.js_contact_form_success').slideDown();
                    }
                    else if (e.Data.ErrorsJson) {
                        validation.init({ errorsJson: e.Data.ErrorsJson }).showErrors();
                    }
                    else {
                        components63Bits.dialog.error(e.Data);
                    }
                },
                complete: function () {
                    preloader.hide();
                }
            });
        });        
    }
}

$(function () {
    $('.js_send_button').click(function () {
        model.submitContactForm();
    });
})