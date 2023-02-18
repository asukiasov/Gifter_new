const Components63Bits = {
    Dialog: {
        ButtonColors: {
            Blue: 'btn-blue',
            Red: 'btn-danger',
            Green: 'btn-success',
            Orange: 'btn-warning'
        },
        Sizes: {
            XSmall: 'xsmall',
            Small: 'small',
            Medium: 'medium',
            Large: 'large',
            XLarge: 'xlarge'
        },
        Alert: function (Options) {
            var Title = Options.Title ? Options.Title : '';
            var TextAlert = Options.TextAlert ? Options.TextAlert : '';
            var Buttons = Options.Buttons;

            $.alert({
                title: Title,
                content: TextAlert,
                buttons: Buttons
            });
        },
        Confirm: function (Options) {
            var Title = Options.Title ? Options.Title : '';
            var TextConfirm = Options.TextConfirm ? Options.TextConfirm : '';
            var Resolve = Options.Resolve ? Options.Resolve : null;
            var ConfirmButtonColor = Options.ConfirmButtonColor ? Options.ConfirmButtonColor : Components63Bits.Dialog.ButtonColors.Blue;

            $.confirm({
                title: Title,
                content: TextConfirm,
                type: 'yellow',
                animation: 'scale',
                closeAnimation: 'scale',
                scrollToPreviousElement:true,
                buttons: {
                    'Yes': {
                        btnClass: ConfirmButtonColor,
                        action: function () {
                            if (Resolve) {
                                Resolve();
                            }
                        }
                    },
                    'No': function () {

                    }
                }
            });
        },
        Error: function (ErrorMessage) {
            $.alert({
                title: 'Error',
                content: ErrorMessage ? ErrorMessage : Globals.TextError,
                icon: 'fas fa-exclamation-triangle',
                closeIcon: true,
                type: 'red',
                animation: 'scale',
                closeAnimation: 'scale',
                animateFromElement: false
            });
        },
        Prompt: function (Options) {

            const Title = Options.Title
            const Label = Options.Label ? Options.Label : '';
            const InputPlaceHolder = Options.InputPlaceHolder ? Options.InputPlaceHolder : '';
            const ButtonColor = Options.ButtonColor ? Options.ButtonColor : Components63Bits.Dialog.ButtonColors.Blue;
            const Resolve = Options.Resolve;

            $.confirm({
                title: Title,
                content: '<div class="form-group"><label>' + Label + '</label><input type="text" placeholder="' + InputPlaceHolder +'" class="form-control js-63bits-components-prompt" /></div>',
                buttons: {
                    OK: {
                        text: 'OK',
                        btnClass: ButtonColor,
                        action: function () {
                            const InputValue = this.$content.find('.js-63bits-components-prompt').val();
                            if (Resolve) {
                                Resolve(InputValue);
                            }
                        }
                    },
                    Cancel: function () {
                        
                    },
                }                
            });
        },
        Success: function (Options) {

            const SuccessTitle = Options ? Options.SuccessTitle : null;
            const SuccessMessage = Options ? Options.SuccessMessage : null;
            const Size = Options ? (Options.Size ? Options.Size : Components63Bits.Dialog.Sizes.Small) : Components63Bits.Dialog.Sizes.Small;

            $.alert({
                title: SuccessTitle,
                content: SuccessMessage ? SuccessMessage : Globals.TextSuccess,
                icon: 'fal fa-shield-check fa-lg',
                closeIcon: true,
                type: 'green',
                animation: 'scale',
                closeAnimation: 'scale',
                columnClass: Size,
                animateFromElement: false
            });
        },
        Warning: function (WarningMessage) {
            $.alert({
                title: 'Warning',
                content: WarningMessage,
                icon: 'fas fa-exclamation-triangle fa-lg',
                closeIcon: true,
                type: 'orange',
                animation: 'scale',
                closeAnimation: 'scale',
                animateFromElement: false
            });
        },
    },
    Select2: {
        InitSimple: function (Selector, Searchable) {
            if (Selector) {
                $(Selector).select2({
                    minimumResultsForSearch: Searchable ? 1 : -1,                    
                    theme: 'bootstrap4',
                    placeholder: '...',
                    language: {
                        noResults: function () {
                            return ' ';
                        }
                    },
                });
            }
        },
        InitSimpleWithData: function (Options) {
            var Selector = Options.Selector;
            var Data = Options.Data;
            var KeyFieldName = Options.KeyFieldName;
            var ValueFieldName = Options.ValueFieldName;
            var PlaceHolder = Options.PlaceHolder ? Options.PlaceHolder : '...';
            var AllowClear = Options.AllowClear ? true : false;

            if (Selector) {
                var Select2Data = Components63Bits.Select2.ConvertToSelect2DataArray({ Data: Data, KeyFieldName: KeyFieldName, ValueFieldName: ValueFieldName });

                if (Select2Data.length > 0) {
                    $(Selector).select2({                        
                        theme: 'bootstrap4',
                        placeholder: PlaceHolder,
                        data: Select2Data,
                        allowClear: AllowClear,
                        language: {
                            noResults: function () {
                                return ' ';
                            }
                        }
                    });
                }
            }
        },
        ConvertToSelect2DataArray: function (Options) {
            var Data = Options.Data;
            var KeyFieldName = Options.KeyFieldName;
            var ValueFieldName = Options.ValueFieldName;
            var Select2DataArray = new Array();

            if (Data) {
                Select2DataArray.push({ id: '', text: '...' });
                $(Data).each(function (Index, Item) {
                    Select2DataArray.push({
                        id: Item[KeyFieldName],
                        text: Item[ValueFieldName]
                    });
                });
            }

            return Select2DataArray;
        },
        ClearData: function (Options) {
            var Selector = Options.Selector;
            if (Selector) {
                $(Selector).empty();
            }
        },
        ClearSelection: function (Options) {
            var Selector = Options.Selector;
            if (Selector) {
                $(Selector).val(null).trigger('change');
            }
        },
        ShowDropDown: function (Options) {
            var Selector = Options.Selector;
            if (Selector) {
                $(Selector).select2('open');
            }
        },
        HideDropDown: function (Options) {
            var Selector = Options.Selector;
            if (Selector) {
                $(Selector).select2('close');
            }
        },
        SetSelectedValue: function (Options) {
            var Selector = Options.Selector;
            var SelectedValue = Options.SelectedValue;
            if (Selector) {
                $(Selector).val(SelectedValue).trigger('change');
            }
        },
        UpdateData: function (Options) {
            this.ClearData(Options);
            this.InitSimpleWithData(Options);
        },
        Destroy: function (Options) {
            var Selector = Options.Selector;

            var ClassName = $(Selector).attr('class');
            if (ClassName && ClassName.indexOf('select2') > -1) {
                $(Selector).select2('destroy');
            }
        }
    }
};

var FileUplaoderClass = {
    InputElement: null,
    UrlFileUplaod: null,
    RequestFileKey: 'Files',
    RequestData: null,
    OnAbort: null,
    OnProgressCallback: null,
    OnFinishUploadCallback: null,
    OnStartCallback: null,
    OnComplete: null,
    IsReportProgressIndividual: false,
    UploadProcessStartedCount: 0,
    UploadProcessFinishedCount: 0,
    XhrArray: [],

    Abort: function () {
        this.XhrArray.forEach(function (xhr, index) {
            xhr.abort();
        });
    },

    InitEvents: function (xhr) {
        https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest/upload

        var _this = this;

        //The upload has begun.
        xhr.upload.onloadstart = function (e) {
            _this.UploadProcessStartedCount++;

            if (_this.OnStartCallback) {
                _this.OnStartCallback({
                    Filename: xhr.fileName,
                    xhr: xhr
                });
            }
        }

        //Periodically delivered to indicate the amount of progress made so far.
        if (_this.OnProgressCallback) {
            xhr.upload.onprogress = function (e) {
                const Percent = Math.ceil(100 * e.loaded / e.total);
                _this.OnProgressCallback({
                    Filename: xhr.fileName,
                    BytesUploaded: e.loaded,
                    BytesTotal: e.total,
                    ProgressPercent: Percent,
                    xhr: xhr
                });
            }
        }

        //The upload operation was aborted.
        xhr.upload.onabort = function (e) {
            if (_this.OnAbort) {
                _this.OnAbort({
                    Filename: xhr.fileName,
                    xhr: xhr
                });
            }
        }

        //The upload failed due to an error.
        xhr.upload.onerror = function (e) {
            if (_this.OnErrorCallback) {
                _this.OnErrorCallback({
                    Filename: xhr.fileName,
                    xhr: xhr,
                    e: e
                });
            }
        }
                
        //The upload timed out because a reply did not arrive within the time interval specified by the XMLHttpRequest.timeout. 
        xhr.upload.ontimeout = function (e) {
            if (_this.OnErrorCallback) {
                _this.OnErrorCallback({
                    Filename: xhr.fileName,
                    xhr: xhr,
                    e: e
                });
            }
        },

        //The upload completed successfully.
        xhr.onloadend = function (e) {
            _this.UploadProcessFinishedCount++;

            if (_this.OnFinishUploadCallback) {                
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

                _this.OnFinishUploadCallback(Result)
            }

            _this.CheckUploadProcessComplete();
        }
    },

    Upload: function () {
        const _this = this;

        _this.UploadProcessStartedCount = 0;
        _this.UploadProcessFinishedCount = 0;

        if (_this.InputElement.files.length) {

            if (_this.IsReportProgressIndividual) {
                _this.UploadWithReportProgressIndividual();
            }
            else {
                _this.UploadWithReportProgressGrouped();
            }
        }
        else {
            throw 'No files to upload';
        }
    },

    UploadWithReportProgressIndividual: function () {
        const _this = this;
        for (let i = 0; i < _this.InputElement.files.length; i++) {

            const xhr = new XMLHttpRequest();
            const Data = new FormData();
            const File = _this.InputElement.files[i];

            Data.append(_this.RequestFileKey, File);

            if (_this.RequestData) {
                _this.RequestData.forEach(function (Item, Index) {
                    Data.append(Item.Key, Item.Value);
                });                
            }
            xhr.fileName = File.name;

            _this.InitEvents(xhr);
            xhr.open('POST', _this.UrlFileUplaod);
            xhr.send(Data);
        }
    },

    UploadWithReportProgressGrouped: function () {
        const _this = this;
        const xhr = new XMLHttpRequest();
        const Data = new FormData();
        const Files = _this.InputElement.files;
        const Filenames = new Array();
        for (let i = 0; i < Files.length; i++) {
            Data.append(_this.RequestFileKey, Files[i]);
            Filenames.push(Files[i].name);
        }
        xhr.fileName = Filenames.join('; ');

        if (_this.RequestData) {
            _this.RequestData.forEach(function (Item, Index) {
                Data.append(Item.Key, Item.Value);
            });
        }

        _this.InitEvents(xhr);
        xhr.open('POST', _this.UrlFileUplaod);
        xhr.send(Data);
    },

    CheckUploadProcessComplete: function () {
        const _this = this;

        if (_this.UploadProcessStartedCount == _this.UploadProcessFinishedCount) {
            if (_this.OnComplete) {
                _this.OnComplete();
            }
        }
    }
}

function FileUplaoder(Options) {
    if (Options) {
        const _this = this;

        _this.InputElement = Options.InputElement ? Options.InputElement : null;
        _this.UrlFileUplaod = Options.UrlFileUplaod ? Options.UrlFileUplaod : null;
        _this.OnStartCallback = Options.OnStartCallback ? Options.OnStartCallback : null;
        _this.OnProgressCallback = Options.OnProgressCallback ? Options.OnProgressCallback : null;
        _this.OnAbort = Options.OnAbort ? Options.OnAbort : null;
        _this.OnFinishUploadCallback = Options.OnFinishUploadCallback ? Options.OnFinishUploadCallback : null;
        _this.OnErrorCallback = Options.OnErrorCallback ? Options.OnErrorCallback : null;
        _this.OnComplete = Options.OnComplete ? Options.OnComplete : null;
        _this.IsReportProgressIndividual = Options.IsReportProgressIndividual ? true : false;
        _this.RequestData = Options.RequestData ? Options.RequestData : null;

        if (_this.InputElement == null) {
            throw new Error('Invalid input type file element is provided');
        }
        if (_this.UrlFileUplaod == null) {
            throw new Error('UrlFileUplaod is not provided');
        }

        _this.RequestFileKey = _this.InputElement.getAttribute('name') ? _this.InputElement.getAttribute('name') : _this.RequestFileKey;

        if (_this.RequestData != null) {
            if (Array.isArray(_this.RequestData)) {
                _this.RequestData.forEach(function (Item, Index) {
                    if (Item.Key == undefined || Item.Value == undefined) {
                        throw new Error('RequestData must be an array of Key-Value object [{ Key: "MyKey", Value: "MyValue"}]');
                    }
                });
            }
            else {
                throw new Error('RequestData must be an array of Key-Value object [{ Key: "MyKey", Value: "MyValue"}]');
            }
        }
    }
}

FileUplaoder.prototype = FileUplaoderClass;