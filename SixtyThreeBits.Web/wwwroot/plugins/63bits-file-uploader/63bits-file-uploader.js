var FileUplaoderClass = {
    inputElement: null,
    urlFileUplaod: null,
    requestFileKey: 'Files',
    requestData: null,
    onAbort: null,
    onProgressCallback: null,
    onFinishUploadCallback: null,
    onStartCallback: null,
    onComplete: null,
    isReportProgressIndividual: false,
    uploadProcessStartedCount: 0,
    uploadProcessFinishedCount: 0,
    xhrArray: [],

    abort: function () {
        this.xhrArray.forEach(function (xhr, index) {
            xhr.abort();
        });
    },

    initEvents: function (xhr) {
        https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest/upload

        var _this = this;

        //The upload has begun.
        xhr.upload.onloadstart = function (e) {
            _this.uploadProcessStartedCount++;

            if (_this.onStartCallback) {
                _this.onStartCallback({
                    filename: xhr.fileName,
                    xhr: xhr
                });
            }
        }

        //Periodically delivered to indicate the amount of progress made so far.
        if (_this.onProgressCallback) {
            xhr.upload.onprogress = function (e) {
                const percent = Math.ceil(100 * e.loaded / e.total);
                _this.onProgressCallback({
                    filename: xhr.fileName,
                    bytesUploaded: e.loaded,
                    bytesTotal: e.total,
                    progressPercent: percent,
                    xhr: xhr
                });
            }
        }

        //The upload operation was aborted.
        xhr.upload.onAbort = function (e) {
            if (_this.onAbort) {
                _this.onAbort({
                    filename: xhr.fileName,
                    xhr: xhr
                });
            }
        }

        //The upload failed due to an error.
        xhr.upload.onerror = function (e) {
            if (_this.OnErrorCallback) {
                _this.OnErrorCallback({
                    filename: xhr.fileName,
                    xhr: xhr,
                    e: e
                });
            }
        }
                
        //The upload timed out because a reply did not arrive within the time interval specified by the XMLHttpRequest.timeout. 
        xhr.upload.ontimeout = function (e) {
            if (_this.onErrorCallback) {
                _this.onErrorCallback({
                    filename: xhr.fileName,
                    xhr: xhr,
                    e: e
                });
            }
        },

        //The upload completed successfully.
        xhr.onloadend = function (e) {
            _this.uploadProcessFinishedCount++;

            if (_this.onFinishUploadCallback) {                
                let Result = {};
                try {
                    Result = JSON.parse(xhr.response);
                }
                catch {
                    Result.ResponseContent = xhr.response;
                }
                Result.Status = xhr.status;
                Result.StatusText = xhr.statusText;
                Result.Filename = xhr.fileName;
                Result.xhr = xhr;

                _this.onFinishUploadCallback(Result)
            }

            _this.checkUploadProcessComplete();
        }
    },

    upload: function () {
        const _this = this;

        _this.uploadProcessStartedCount = 0;
        _this.uploadProcessFinishedCount = 0;

        if (_this.inputElement.files.length) {

            if (_this.isReportProgressIndividual) {
                _this.uploadWithReportProgressIndividual();
            }
            else {
                _this.UploadWithReportProgressGrouped();
            }
        }
        else {
            throw 'No files to upload';
        }
    },

    uploadWithReportProgressIndividual: function () {
        const _this = this;
        for (let i = 0; i < _this.inputElement.files.length; i++) {

            const xhr = new XMLHttpRequest();
            const data = new FormData();
            const file = _this.inputElement.files[i];

            data.append(_this.requestFileKey, file);

            if (_this.requestData) {
                _this.requestData.forEach(function (item, index) {
                    data.append(item.key, item.value);
                });                
            }
            xhr.fileName = file.name;

            _this.initEvents(xhr);
            xhr.open('POST', _this.urlFileUplaod);
            xhr.send(data);
        }
    },

    uploadWithReportProgressGrouped: function () {
        const _this = this;
        const xhr = new XMLHttpRequest();
        const data = new FormData();
        const files = _this.inputElement.files;
        const filenames = new Array();
        for (let i = 0; i < files.length; i++) {
            data.append(_this.requestFileKey, files[i]);
            filenames.push(Files[i].name);
        }
        xhr.fileName = filenames.join('; ');

        if (_this.requestData) {
            _this.requestData.forEach(function (item, Index) {
                Data.append(item.key, item.value);
            });
        }

        _this.initEvents(xhr);
        xhr.open('POST', _this.urlFileUplaod);
        xhr.send(Data);
    },

    checkUploadProcessComplete: function () {
        const _this = this;

        if (_this.uploadProcessStartedCount == _this.uploadProcessFinishedCount) {
            if (_this.onComplete) {
                _this.onComplete();
            }
        }
    }
}

function FileUplaoder(options) {
    if (options) {
        const _this = this;

        _this.inputElement = options.inputElement ? options.inputElement : null;
        _this.urlFileUplaod = options.urlFileUplaod ? options.urlFileUplaod : null;
        _this.onStartCallback = options.onStartCallback ? options.onStartCallback : null;
        _this.onProgressCallback = options.onProgressCallback ? options.onProgressCallback : null;
        _this.onAbort = options.onAbort ? options.onAbort : null;
        _this.onFinishUploadCallback = options.onFinishUploadCallback ? options.onFinishUploadCallback : null;
        _this.onErrorCallback = options.onErrorCallback ? options.onErrorCallback : null;
        _this.onComplete = options.onComplete ? options.onComplete : null;
        _this.isReportProgressIndividual = options.isReportProgressIndividual ? true : false;
        _this.requestData = options.requestData ? options.requestData : null;

        if (_this.inputElement == null) {
            throw new Error('Invalid input type file element is provided');
        }
        if (_this.urlFileUplaod == null) {
            throw new Error('urlFileUplaod is not provided');
        }

        _this.requestFileKey = _this.inputElement.getAttribute('name') ? _this.inputElement.getAttribute('name') : _this.requestFileKey;

        if (_this.requestData != null) {
            if (Array.isArray(_this.requestData)) {
                _this.requestData.forEach(function (item, index) {
                    if (item.key == undefined || item.value == undefined) {
                        throw new Error('requestData must be an array of Key-Value object [{ Key: "MyKey", Value: "MyValue"}]');
                    }
                });
            }
            else {
                throw new Error('requestData must be an array of Key-Value object [{ Key: "MyKey", Value: "MyValue"}]');
            }
        }
    }
}

FileUplaoder.prototype = FileUplaoderClass;