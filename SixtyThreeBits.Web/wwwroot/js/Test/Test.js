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
        },
        OnProgressCallback: function (e) {
            console.log(e);
        },
        OnFinishCallback: function (e) {
        },
        OnCompleteCallback: function (e) {
        },
        OnErrorCallback: function (e) {
        }
    });


    $('.js-upload-button').click(function () {
        TestModel.Uploader.Upload();
    });
});