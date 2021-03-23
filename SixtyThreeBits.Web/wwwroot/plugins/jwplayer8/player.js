jwplayer.key = '10rlDJHyHzxLnnjPfSkloG/gMlj7bIHr4VBVStEvd9g=';
var VideoTimes = new Object();

var VideoPlayerClass = {
    Selector: null,
    PlayerJQueryElement: null,
    PlayerHtmlElement: null,
    PlayerInstance: null,
    VideoTitle: 'video',
    File: null,
    Image: null,
    Height: null,
    Width: null,
    ShouldAutostart: false,    
    StartTime: null,
    UrlMarkers: null,
    UrlDownload: null,
    OnTime: null,
    OnPause: null,
    OnComplete: null,
    OnError: null,
    Tracks: new Array(),
    Logo: null,
    UrlWatermark: null,

    Init: function () {
        this.InitTracks();
        this.InitWatermark()
        this.PlayerInstance = jwplayer(this.PlayerHtmlElement).setup({
            file: this.File,
            image: this.Image,
            height: this.Height,
            width: this.Width,
            aspectratio: '16:9',
            playbackRateControls: true,
            autostart: this.ShouldAutostart ? 'viewable' : false,
            logo: this.Logo,
            tracks: this.Tracks
        });
        this.InitEvents();
        this.InitDownloadButton();

        if (this.StartTime > 0) {
            var _this = this;
            setTimeout(function () {
                _this.Seek(_this.StartTime);
            }, 1000);
        }

    },
    InitTracks: function () {
        if (this.UrlMarkers) {
            this.Tracks.push({
                file: this.UrlMarkers,
                kind: 'chapters'
            });
        }
    },
    InitWatermark: function () {
        if (this.UrlWatermark && this.UrlWatermark.length) {
            this.Logo = {
                file: this.UrlWatermark,                                
                position: 'top-left'
            }
        }
    },
    InitDownloadButton: function () {
        if (this.UrlDownload) {
            var ButtonID = 'download-video-button';
            var IconPath = '/plugins/jwplayer8/img/download.svg';
            var TooltipText = 'Download Video';
            var _this = this;
            this.PlayerInstance.addButton(
                IconPath,
                TooltipText,
                function () {
                    var anchor = document.createElement('a');
                    anchor.setAttribute('href', _this.UrlDownload);
                    anchor.setAttribute('target', '_blank');
                    anchor.setAttribute('download', _this.VideoTitle);
                    anchor.style.display = 'none';
                    document.body.appendChild(anchor);
                    anchor.click();
                    document.body.removeChild(anchor);
                },
                ButtonID
            );
        }
    },

    Play: function () {
        if (this.PlayerInstance.getState() != 'playing') {
            this.PlayerInstance.play();
        }
    },
    Pause: function () {
        this.PlayerInstance.pause();
    },
    Seek: function (PositionSeconds) {
        this.PlayerInstance.seek(PositionSeconds);
    },
    GetCurrentPosition: function () {
        return this.PlayerInstance.getPosition();
    },

    VideoTimeCurrent: 0,
    VideoTimeSpent: 0,
    VideoTimeTicks: 0,

    InitEvents: function () {
        var _this = this;
        this.PlayerInstance.on('time', function (e) {
            ++_this.VideoTimeTicks;
            var Diff = e.position - _this.VideoTimeCurrent;
            _this.VideoTimeCurrent = e.position;
            if (Diff > 0 && Diff < 1.5) {
                _this.VideoTimeSpent += Diff;
            }
            if (_this.OnTime) {
                _this.OnTime(_this.VideoTimeCurrent, _this.VideoTimeSpent, _this.VideoTimeTicks);
            }
        });

        this.PlayerInstance.on('pause', function (e) {
            if (_this.OnPause) {
                _this.OnPause(_this.VideoTimeCurrent, _this.VideoTimeSpent);
            }
        });

        this.PlayerInstance.on('complete', function (e) {
            if (_this.OnComplete) {
                _this.OnComplete(_this.VideoTimeCurrent, _this.VideoTimeSpent);
            }
        });

        this.PlayerInstance.on('error', function (e) {
            if (_this.OnError) {
                _this.OnError(e);
            }
        });
    }
}

function VideoPlayer(Options) {
    this.Selector = Options.Selector;
    this.PlayerJQueryElement = $(this.Selector);

    if (this.PlayerJQueryElement.length) {
        this.PlayerHtmlElement = $(this.PlayerJQueryElement)[0];
        this.VideoTitle = Options.VideoTitle ? Options.VideoTitle : 'video';
        this.File = Options.File;
        this.Image = Options.Image;
        this.Height = Options.Height ? Options.Height : '100%';
        this.Width = Options.Width ? Options.Width : '100%';
        this.UrlMarkers = Options.UrlMarkers;
        this.UrlDownload = Options.UrlDownload;
        this.ShouldAutostart = Options.ShouldAutostart ? true : false;
        this.StartTime = Options.StartTime;
        this.UrlWatermark = Options.UrlWatermark;

        this.OnTime = Options.OnTime;
        this.OnPause = Options.OnPause;
        this.OnComplete = Options.OnComplete;
        this.OnError = Options.OnError;
    }
    else {
        throw new DOMException('Element not found by given selector "' + Options.Selector + '"');
    }
}

VideoPlayer.prototype = VideoPlayerClass;