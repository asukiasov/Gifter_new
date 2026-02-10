$(function(){
    //--- auth modal
    $('.js-auth-modal-show-btn').click(function (){
        if(layoutModel.modalActive){
            layoutModel.modalActive.hide();
        }

        layoutModel.modalActive = new bootstrap.Modal('.js-auth-modal', {
            backdrop: 'static'
        });
        layoutModel.modalActive.show();

        /*$('.js-auth-modal')[0].addEventListener('hidden.bs.modal', event => {
            console.log('a')
        })*/
    });

    OTP('.js-auth-otp-input', function (code) {
        console.log(code);
        if (code.length === 4){
            $('.js-auth-modal-submit-btn').removeClass('disabled');
        } else{
            $('.js-auth-modal-submit-btn').addClass('disabled');
        }
    });

    $('.js-auth-otp-request-btn').click(function () {
        $('.js-auth-otp-card').slideDown(200);
        $('.js-auth-otp-input:first-child').focus();
    });

    //--- change phone number modal
    $('.js-change-phone-number-modal-show-btn').click(function (){
        if(layoutModel.modalActive){
            layoutModel.modalActive.hide();
        }

        layoutModel.modalActive = new bootstrap.Modal('.js-change-phone-number-modal', {
            backdrop: 'static'
        });
        layoutModel.modalActive.show();
    });
});