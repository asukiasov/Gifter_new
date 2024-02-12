jwplayer.key = '10rlDJHyHzxLnnjPfSkloG/gMlj7bIHr4VBVStEvd9g=';

var VideoPlayerClass = {
    selector: null,
    playerJQueryElement: null,
    playerHtmlElement: null,
    playerInstance: null,
    videoTitle: 'video',
    file: null,
    image: null,
    height: null,
    width: null,
    shouldAutostart: false,    
    startTime: null,    
    onTime: null,
    onPause: null,
    onComplete: null,
    onError: null,
    tracks: new Array(),
    logo: null,
    urlDownload: null,
    urlMarkers: null,    
    urlWatermark: null,

    init: function () {
        this.initTracks();
        this.initWatermark()
        this.playerInstance = jwplayer(this.playerHtmlElement).setup({
            file: this.file,
            image: this.image,
            height: this.height,
            width: this.width,
            aspectratio: '16:9',
            playbackRateControls: true,
            autostart: this.shouldAutostart ? 'viewable' : false,
            logo: this.logo,
            tracks: this.tracks
        });
        this.initEvents();
        this.initDownloadButton();

        if (this.startTime > 0) {
            var _this = this;
            setTimeout(function () {
                _this.Seek(_this.startTime);
            }, 1000);
        }

    },
    initTracks: function () {
        if (this.urlMarkers) {
            this.tracks.push({
                file: this.urlMarkers,
                kind: 'chapters'
            });
        }
    },
    initWatermark: function () {
        if (this.urlWatermark && this.urlWatermark.length) {
            this.logo = {
                file: this.urlWatermark,                                
                position: 'top-left'
            }
        }
    },
    initDownloadButton: function () {
        if (this.urlDownload) {
            const buttonID = 'download-video-button';
            const iconPath = '/plugins/jwplayer8/img/download.svg';
            const tooltipText = 'Download Video';
            const _this = this;
            this.playerInstance.addButton(
                iconPath,
                tooltipText,
                function () {
                    var anchor = document.createElement('a');
                    anchor.setAttribute('href', _this.urlDownload);
                    anchor.setAttribute('target', '_blank');
                    anchor.setAttribute('download', _this.videoTitle);
                    anchor.style.display = 'none';
                    document.body.appendChild(anchor);
                    anchor.click();
                    document.body.removeChild(anchor);
                },
                buttonID
            );
        }
    },

    play: function () {
        if (this.playerInstance.getState() != 'playing') {
            this.playerInstance.play();
        }
    },
    pause: function () {
        this.playerInstance.pause();
    },
    seek: function (positionSeconds) {
        this.playerInstance.seek(positionSeconds);
    },
    getCurrentPosition: function () {
        return this.playerInstance.getPosition();
    },

    _videoTimeCurrent: 0,
    _videoTimeSpent: 0,
    _videoTimeTicks: 0,

    initEvents: function () {
        var _this = this;
        this.playerInstance.on('time', function (e) {
            ++_this._videoTimeTicks;
            var Diff = e.position - _this._videoTimeCurrent;
            _this._videoTimeCurrent = e.position;
            if (Diff > 0 && Diff < 1.5) {
                _this._videoTimeSpent += Diff;
            }
            if (_this.onTime) {
                _this.onTime(_this._videoTimeCurrent, _this._videoTimeSpent, _this._videoTimeTicks);
            }
        });

        this.playerInstance.on('pause', function (e) {
            if (_this.onPause) {
                _this.onPause(_this._videoTimeCurrent, _this._videoTimeSpent);
            }
        });

        this.playerInstance.on('complete', function (e) {
            if (_this.onComplete) {
                _this.onComplete(_this._videoTimeCurrent, _this._videoTimeSpent);
            }
        });

        this.playerInstance.on('error', function (e) {
            if (_this.onError) {
                _this.onError(e);
            }
        });
    }
}

function VideoPlayer(options) {
    this.selector = options.selector;
    this.playerJQueryElement = $(this.selector);

    if (this.playerJQueryElement.length) {
        this.playerHtmlElement = $(this.playerJQueryElement)[0];
        this.videoTitle = options.videoTitle ? options.videoTitle : 'video';
        this.file = options.file;
        this.image = options.image;
        this.height = options.height ? options.height : '100%';
        this.width = options.width ? options.width : '100%';
        this.urlMarkers = options.urlMarkers;
        this.urlDownload = options.urlDownload;
        this.shouldAutostart = options.shouldAutostart ? true : false;
        this.startTime = options.startTime;
        this.urlWatermark = options.urlWatermark;

        this.onTime = options.onTime;
        this.onPause = options.onPause;
        this.onComplete = options.onComplete;
        this.onError = options.onError;
    }
    else {
        throw new DOMException('Element not found by given selector "' + options.selector + '"');
    }
}

VideoPlayer.prototype = VideoPlayerClass;