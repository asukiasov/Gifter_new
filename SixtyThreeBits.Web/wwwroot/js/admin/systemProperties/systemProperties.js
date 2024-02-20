const systemPropertiesModel = {
    urlTestEmailSmtp: null,
    urlTestEmailMailgun: null,
    urlTestEmailOffice365: null,
    urlTestAws: null
};

$(function () {

    $('.js-test-smtp-email-button').click(function () {
        components63Bits.dialog.prompt({
            title: 'Test SMTP',
            label: 'Email',
            resolve: function (emailTo) {
                const smtpAddress = $('.js-smtp-address-input').val();
                const smtpPort = $('.js-smtp-port-input').val();
                const smtpUseSsl = $('.js-smtp-use-ssl-checkbox').is(':checked');
                const smtpUsername = $('.js-smtp-username-input').val();
                const smtpPassword = $('.js-smtp-password-input').val();
                const smtpFrom = $('.js-smtp-from-input').val();

                $.ajax({
                    type: 'POST',
                    url: systemPropertiesModel.urlTestEmailSmtp,
                    data: {
                        EmailTo: emailTo,
                        SmtpAddress: smtpAddress,
                        SmtpPort: smtpPort,
                        SmtpUsername: smtpUsername,
                        SmtpPassword: smtpPassword,
                        SmtpUseSsl: smtpUseSsl,
                        SmtpFrom: smtpFrom
                    },
                    dataType: 'json',
                    beforeSend: function () {                        
                        preloader.show();
                    },
                    success: function (res) {
                        if (res.IsSuccess) {
                            components63Bits.dialog.success();
                        }
                        else {
                            components63Bits.dialog.error(res.Data);
                        }
                    },                    
                    error: function () {
                        components63Bits.dialog.error();
                    },
                    complete: function () {
                        preloader.hide();
                    },
                });
            }
        })
    });
    $('.js-test-mailgun-email-button').click(function () {
        components63Bits.dialog.prompt({
            title: 'Test Mailgun',
            label: 'Email',
            resolve: function (emailTo) {
                const mailgunBaseUrl = $('.js-mailgun-base-url-input').val();
                const mailgunApiKey = $('.js-mailgun-api-key-input').val();
                const mailgunDomain = $('.js-mailgun-domain-input').val();
                const mailgunFrom = $('.js-mailgun-from-input').val();
                const mailgunWebhookWebhookSigningKey = $('.js-mailgun-webhook-signing-key-input').val();                

                $.ajax({
                    type: 'POST',
                    url: systemPropertiesModel.urlTestEmailMailgun,
                    data: {
                        EmailTo: emailTo,
                        MailgunBaseUrl: mailgunBaseUrl,
                        MailgunApiKey: mailgunApiKey,
                        MailgunDomain: mailgunDomain,
                        MailgunFrom: mailgunFrom,
                        MailgunWebhookWebhookSigningKey: mailgunWebhookWebhookSigningKey
                    },
                    dataType: 'json',
                    beforeSend: function () {                        
                        preloader.show();
                    },
                    success: function (res) {
                        if (res.IsSuccess) {                            
                            components63Bits.dialog.success();
                        }                        
                        else {
                            components63Bits.dialog.error(res.Data);
                        }
                    },
                    error: function () {
                        components63Bits.dialog.error();
                    },
                    complete: function () {
                        preloader.hide();
                    },
                });
            }
        })
    });
    $('.js-test-office365-email-button').click(function () {
        components63Bits.dialog.prompt({
            title: 'Test Office365',
            label: 'Email',
            resolve: function (emailTo) {
                const microsoftGraphServiceTenant = $('.js-microsoft-graph-service-tenant-input').val();
                const microsoftGraphServiceClientID = $('.js-microsoft-graph-service-cliendID-input').val();                
                const microsoftGraphServiceClientSecret = $('.js-microsoft-graph-service-secret-input').val();
                const microsoftGraphServiceUserID = $('.js-microsoft-graph-service-userID-input').val();                

                $.ajax({
                    type: 'POST',
                    url: systemPropertiesModel.urlTestEmailOffice365,
                    data: {
                        EmailTo: emailTo,
                        MicrosoftGraphServiceTenant: microsoftGraphServiceTenant,
                        MicrosoftGraphServiceClientID: microsoftGraphServiceClientID,
                        MicrosoftGraphServiceClientSecret: microsoftGraphServiceClientSecret,
                        microsoftGraphServiceUserID: microsoftGraphServiceUserID
                    },
                    dataType: 'json',
                    beforeSend: function () {                        
                        preloader.show();
                    },
                    success: function (res) {
                        if (res.IsSuccess) {
                            components63Bits.dialog.success();
                        }
                        else {
                            components63Bits.dialog.error(res.Data);
                        }
                    },
                    error: function () {
                        components63Bits.dialog.error();
                    },
                    complete: function () {
                        preloader.hide();
                    },
                });
            }
        })
    });    

    $('.js-test-aws-button').click(function () {

        $.ajax({
            type: 'POST',
            url: systemPropertiesModel.urlTestAws,
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    components63Bits.dialog.success();
                }
                else {
                    components63Bits.dialog.error();
                }
            },
            error: function () {
                components63Bits.dialog.error();
            },
            complete: function () {
                preloader.hide();
            },
        });

    });

    $('.js-google-iframe-help').click(function (e) {
        e.preventDefault();
        var src = $(this).attr('href');

        fancyBox.init({
            src: src
        }).showImagePopup();    
    });
    
});