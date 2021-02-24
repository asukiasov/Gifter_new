const SystemPropertiesModel = {
    UrlTestSmtp: null
};

$(function () {

    $('.js-test-smtp-button').click(function () {
        Components63Bits.Dialog.Prompt({
            Title: 'Test SMTP',
            Label: 'Your Email',
            Resolve: function (EmailTo) {
                const SMTPAddress = $('.js-smtp-address-textbox').val();
                const SMTPPort = $('.js-smtp-port-textbox').val();
                const SMTPUseSSL = $('.js-smtp-use-ssl-checkbox').is(':checked');
                const SMTPUsername = $('.js-smtp-username-textbox').val();
                const SMTPPassword = $('.js-smtp-password-textbox').val();
                const SMTPFrom = $('.js-smtp-from-textbox').val();

                $.ajax({
                    type: 'POST',
                    url: SystemPropertiesModel.UrlTestSmtp,
                    data: {
                        EmailTo: EmailTo,
                        SMTPAddress: SMTPAddress,
                        SMTPPort: SMTPPort,
                        SMTPUsername: SMTPUsername,
                        SMTPPassword: SMTPPassword,
                        SMTPUseSSL: SMTPUseSSL,
                        SMTPFrom: SMTPFrom
                    },
                    dataType: 'json',
                    beforeSend: function () {
                        $('.js-test-smtp-success-message,.js-test-smtp-error-message').Hide();
                        preloader.show();
                    },
                    success: function (res) {
                        if (res.IsSuccess) {
                            $('.js-test-smtp-success-message').Show();
                            setTimeout(function () {
                                $('.js-test-smtp-success-message').Hide();
                            }, 4000);
                        }
                        else if (res.Data) {
                            $('.js-test-smtp-error-message').text(res.Data);
                            $('.js-test-smtp-error-message').Show();
                        }
                        else {
                            $('.js-test-smtp-error-message').text(Globals.TextError);
                            $('.js-test-smtp-error-message').Show();                            
                        }                  
                    },                    
                    error: function () {
                        alert(Globals.TextError);
                    },
                    complete: function () {
                        preloader.hide();
                    },
                });
            }
        })
    });

    $('.js-google-iframe-help').click(function (e) {
        e.preventDefault();
        var src = $(this).attr('href');

        FancyBox.Init({
            src: src
        }).ShowImagePopup();    
    });
    
});