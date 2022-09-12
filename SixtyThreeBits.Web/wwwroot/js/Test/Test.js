const TestModel = {
    Uploader: null
}

$(function () {
    TestModel.Uploader = new FileUplaoder({
        InputElement: $('.js-file-uploader')[0],
        UrlFileUplaod: '/test/upload/',
        IsReportProgressIndividual: false,
        RequestData: [{ Key: 'ProductID', Value: '123' }, { Key: 'ProductName', Value: 'iPhone' }],        
        OnStartCallback: function (e) {
            console.log('OnStartCallback');
            console.log(e);
        },
        OnProgressCallback: function (e) {
            console.log('OnProgressCallback');
            console.log(e);
        },
        OnFinishUploadCallback: function (e) {
            console.log('OnFinishUploadCallback');
            console.log(e);
        },        
        OnErrorCallback: function (e) {
            console.log('OnErrorCallback');
            console.log(e);
        }
    });


    $('.js-upload-button').click(function () {
        TestModel.Uploader.Upload();
    });
});