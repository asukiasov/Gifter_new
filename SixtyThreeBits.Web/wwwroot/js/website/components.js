$(function(){
    //--- virtual select
    VirtualSelect.init({
        ele: '.js-combo',
        hideClearButton: true,
        optionHeight: '44px'
    });

    VirtualSelect.init({
        ele: '.js-combo-multiple',
        optionHeight: '44px'
    });


    //--- file upload
    $('.js-file-uploader-input').on('change', function (event) {
        const container = $(this).closest('.js-file-uploader-container');
        const filesList = container.find('.js-files-list');

        const files = event.target.files;
        const maxFiles = 3;
        let filesCount = container.find('.js-files-list-item').length;

        for (let i = 0; i < (maxFiles - filesCount); i++) {
            const file = files[i];
            if (file) {
                const reader = new FileReader();

                reader.onload = function(e) {
                    const base64String = e.target.result;

                    const html = `
                        <div class="files-list-item js-files-list-item">
                            <img src="images/icons/file_light.svg" alt="file icon">
                            <span class="line-clamp-1">${file.name}</span>
                            <button class="btn p-0 btn-auto js-file-remove-btn"><img src="/images/icons/trash_light.svg" alt="trash icon"></button>
                            <input type="hidden" value="${base64String}">
                        </div>
                    `;
                    filesList.append($(html));

                    filesCount = container.find('.js-files-list-item').length;

                    if(filesCount >= maxFiles){
                        container.find('.js-file-uploader').addClass('disabled');
                    }
                };

                reader.onerror = function() {
                    console.error('Error reading file:', file.name);
                };

                reader.readAsDataURL(file);
            }
        }
    });

    //--- file remove
    $('.js-file-uploader-container').on('click', '.js-file-remove-btn', function (){
        const container = $(this).closest('.js-file-uploader-container');

        container.find('.js-file-uploader').removeClass('disabled');
        $(this).closest('.js-files-list-item').remove();
    });


    //--- modals
    $('.js-modal-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-modal', {
            backdrop: 'static'
        });
        modal.show();

        $('.js-modal')[0].addEventListener('hidden.bs.modal', event => {
            console.log('a')
        })
    });

    $('.js-modal-sm-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-modal-sm', {
            backdrop: 'static'
        });
        modal.show();
    });

    $('.js-modal-lg-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-modal-lg', {
            backdrop: 'static'
        });
        modal.show();
    });

    $('.js-modal-xl-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-modal-xl', {
            backdrop: 'static'
        });
        modal.show();
    });


    //--- success
    $('.js-success-modal-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-success-modal', {
            backdrop: 'static'
        });
        modal.show();
    });

    //--- error
    $('.js-error-modal-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-error-modal', {
            backdrop: 'static'
        });
        modal.show();
    });

    //--- confirm
    $('.js-confirm-modal-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-confirm-modal', {
            backdrop: 'static'
        });
        modal.show();
    });

    //--- chack date
    $('.js-check-date-success-modal-1-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-check-date-success-modal-1', {
            backdrop: 'static'
        });
        modal.show();
    });
    $('.js-check-date-success-modal-2-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-check-date-success-modal-2', {
            backdrop: 'static'
        });
        modal.show();
    });
    $('.js-check-date-success-modal-3-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-check-date-success-modal-3', {
            backdrop: 'static'
        });
        modal.show();
    });

    $('.js-check-date-error-modal-1-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-check-date-error-modal-1', {
            backdrop: 'static'
        });
        modal.show();
    });
    $('.js-check-date-error-modal-2-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-check-date-error-modal-2', {
            backdrop: 'static'
        });
        modal.show();
    });
    $('.js-check-date-error-modal-3-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-check-date-error-modal-3', {
            backdrop: 'static'
        });
        modal.show();
    });
    $('.js-check-date-error-modal-4-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-check-date-error-modal-4', {
            backdrop: 'static'
        });
        modal.show();
    });
    $('.js-check-date-error-modal-5-show-btn').click(function (){
        const modal = new bootstrap.Modal('.js-check-date-error-modal-5', {
            backdrop: 'static'
        });
        modal.show();
    });
});