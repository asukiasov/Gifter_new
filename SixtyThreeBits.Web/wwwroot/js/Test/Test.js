const testModel = {
    uploader: null
}

$(function () {
    testModel.uploader = new FileUplaoder({
        inputElement: $('.js-file-uploader')[0],
        urlFileUplaod: '/test/upload/',
        isReportProgressIndividual: false,
        requestData: [{ Key: 'ProductID', Value: '123' }, { Key: 'ProductName', Value: 'iPhone' }],        
        onStartCallback: function (e) {
            console.log('OnStartCallback');
            console.log(e);
        },
        onProgressCallback: function (e) {
            console.log('OnProgressCallback');
            console.log(e);
        },
        onFinishUploadCallback: function (e) {
            console.log('OnFinishUploadCallback');
            console.log(e);
        },        
        onErrorCallback: function (e) {
            console.log('OnErrorCallback');
            console.log(e);
        }
    });


    $('.js-upload-button').click(function () {
        testModel.uploader.upload();
    });
});