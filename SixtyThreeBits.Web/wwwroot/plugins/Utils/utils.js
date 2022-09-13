const Utilities = {
    ConsoleLogDevelopedBy: function () {
        console.log("DEVELOPED BY")
        console.log(`%c                                                                               
        66666666    333333333333333   BBBBBBBBBBBBBBBBB   IIIIIIIIIITTTTTTTTTTTTTTTTTTTTTTT   SSSSSSSSSSSSSSS 
       6::::::6    3:::::::::::::::33 B::::::::::::::::B  I::::::::IT:::::::::::::::::::::T SS:::::::::::::::S
      6::::::6     3::::::33333::::::3B::::::BBBBBB:::::B I::::::::IT:::::::::::::::::::::TS:::::SSSSSS::::::S
     6::::::6      3333333     3:::::3BB:::::B     B:::::BII::::::IIT:::::TT:::::::TT:::::TS:::::S     SSSSSSS
    6::::::6                   3:::::3  B::::B     B:::::B  I::::I  TTTTTT  T:::::T  TTTTTTS:::::S            
   6::::::6                    3:::::3  B::::B     B:::::B  I::::I          T:::::T        S:::::S            
  6::::::6             33333333:::::3   B::::BBBBBB:::::B   I::::I          T:::::T         S::::SSSS         
 6::::::::66666        3:::::::::::3    B:::::::::::::BB    I::::I          T:::::T          SS::::::SSSSS    
6::::::::::::::66      33333333:::::3   B::::BBBBBB:::::B   I::::I          T:::::T            SSS::::::::SS  
6::::::66666:::::6             3:::::3  B::::B     B:::::B  I::::I          T:::::T               SSSSSS::::S 
6:::::6     6:::::6            3:::::3  B::::B     B:::::B  I::::I          T:::::T                    S:::::S
6:::::6     6:::::6            3:::::3  B::::B     B:::::B  I::::I          T:::::T                    S:::::S
6::::::66666::::::63333333     3:::::3BB:::::BBBBBB::::::BII::::::II      TT:::::::TT      SSSSSSS     S:::::S
 66:::::::::::::66 3::::::33333::::::3B:::::::::::::::::B I::::::::I      T:::::::::T      S::::::SSSSSS:::::S
   66:::::::::66   3:::::::::::::::33 B::::::::::::::::B  I::::::::I      T:::::::::T      S:::::::::::::::SS 
     666666666      333333333333333   BBBBBBBBBBBBBBBBB   IIIIIIIIII      TTTTTTTTTTT       SSSSSSSSSSSSSSS
`, "color: #3CB986;");         
    },
    Date: {
        MonthShortNames: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        WeekDaysShortNames: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
        WEEKDAYS: {
            MONDAY: 1,
            TUESDAY: 2,
            WEDNESDAY: 3,
            THURSDAY: 4,
            FRIDAY: 5,
            SATURDAY: 6,
            SUNDAY: 0
        },
        AddDays: function (Input, DaysToAdd) {
            const D = new Date(Input);
            DaysToAdd = parseInt(DaysToAdd);
            if (Input && (DaysToAdd >= 0 || DaysToAdd < 0) && !isNaN(D.getTime())) {
                D.setDate(D.getDate() + DaysToAdd);
                return D;
            }
            else {
                return null;
            }
        },
        AddBusinessDays: function (Input, BusinessDaysToAdd) {
            const D = new Date(Input);
            BusinessDaysToAdd = parseInt(BusinessDaysToAdd);
            if (Input && (BusinessDaysToAdd >= 0 || BusinessDaysToAdd < 0) && !isNaN(D.getTime())) {
                const wks = Math.floor(BusinessDaysToAdd / 5);
                let dys = Utilities.Numbers.Mod(BusinessDaysToAdd, 5);
                let dy = D.getDay();
                if (dy === 6 && dys > -1) {
                    if (dys === 0) {
                        dys -= 2;
                        dy += 2;
                    }
                    dys++;
                    dy -= 6;
                }
                if (dy === 0 && dys < 1) {
                    if (dys === 0) {
                        dys += 2;
                        dy -= 2;
                    }
                    dys--;
                    dy += 6;
                }
                if (dy + dys > 5) dys += 2;
                if (dy + dys < 1) dys -= 2;

                var DateToReturn = new Date(D);
                DateToReturn.setDate(D.getDate() + wks * 7 + dys);
                return DateToReturn;
            }
            else {
                return null;
            }
        },
        GetDateWithoutTime: function (Input) {
            const D = new Date(Input);
            if (Input && !isNaN(D.getTime())) {
                const Year = D.getFullYear();
                let Month = (D.getMonth() + 1);
                let Day = D.getDate();

                Month = Month < 10 ? ('0' + Month) : Month;
                Day = Day < 10 ? ('0' + Day) : Day;
                return new Date(Year + '-' + Month + '-' + Day + 'T00:00:00');
            }
            else {
                return null;
            }
        },
        IsWeekend: function (Input) {
            const D = new Date(Input);
            if (Input && !isNaN(D.getTime())) {
                const Day = D.getDay();
                return Day == Utilities.Date.WEEKDAYS.SATURDAY || Day == Utilities.Date.WEEKDAYS.SUNDAY;
            }
            else {
                return false;
            }
        },        
        ToShortDate: function (Input) {
            const D = new Date(Input);
            if (Input && !isNaN(D.getTime())) {
                const Year = D.getFullYear();
                const Month = Utilities.Date.MonthShortNames[D.getMonth()]
                const Day = D.getDate();
                return Month + ' ' + Day + ', ' + Year;
            }
            else {
                return null;
            }
        },
        ToShortDateTime: function (Input) {
            const D = new Date(Input);
            if (Input && !isNaN(D.getTime())) {
                const Year = D.getFullYear();
                const Month = Utilities.Date.MonthShortNames[D.getMonth()];
                const Day = D.getDate();
                const Hours = D.getHours();
                const Minutes = D.getMinutes();
                return Month + ' ' + Day + ', ' + Year + ' ' + Hours + ':' + Minutes;
            }
            else {
                return null;
            }
        },
        ToTime: function (Input) {
            var D = new Date(Input);
            if (Input && !isNaN(D.getTime())) {
                var Hours = (D.getHours() < 10 ? '0' : '') + D.getHours();
                var Minutes = (D.getMinutes() < 10 ? '0' : '') + D.getMinutes();
                return Hours + ':' + Minutes;
            }
            else {
                return null;
            }
        },
        ToWeekDayShortDate: function (Input) {
            const D = new Date(Input);
            if (Input && !isNaN(D.getTime())) {
                const Year = D.getFullYear();
                let DayOfMonth = D.getDate();
                DayOfMonth = (DayOfMonth > 9 ? DayOfMonth : '0' + DayOfMonth);
                const Month = Utilities.Date.MonthShortNames[D.getMonth()];
                const Weekday = Utilities.Date.WeekDaysShortNames[D.getDay()];
                return Month + ' ' + DayOfMonth + ', ' + Year + ', ' + Weekday;
            }
            else {
                return null;
            }
        },
    },

    String: {
        EndsWith: function (suffix) {
            return this.indexOf(suffix, this.length - suffix.length) !== -1;
        },
        StripHtml: function (InputString) {
            return InputString.replace(/<[^>]*>/g, '');
        }
    },

    Numbers: {
        Mod: function (Input, n) {
            return ((Input % n) + n) % n;
        }
    },

    BytesToSize: function (bytes) {
        if (bytes >= 0) {

            var k = 1024;
            var sizes = ['B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB']
            var i = Math.floor(Math.log(bytes) / Math.log(k))
            return (bytes / Math.pow(k, i)).toPrecision(3) + ' ' + sizes[i]
        }
        else {
            return null;
        }
    },

    GetBase64FromInputFilePromise: function (Selector) {
        return new Promise(function (Resolve, Reject) {
            const Element = document.querySelector(Selector);
            if (Element && Element.files && Element.files.length > 0) {
                const file = Element.files[0];
                const reader = new FileReader();
                reader.readAsDataURL(file);
                reader.onerror = function (error) {
                    Reject(error)
                };
                reader.onload = function () {

                    const SliceIndex = reader.result.indexOf(',') + 1;

                    Resolve({
                        Filename: file.name,
                        FileBase64: reader.result.slice(SliceIndex),
                        FileBase64Original: reader.result
                    });
                };

            }
            else {
                Resolve({
                    Filename: null,
                    FileBase64: null,
                    FileBase64Original: null
                });
            }
        });
    },

    GetBase64ArrayFromInputFileMultiplePromise: function (Selector) {
        return new Promise(function (Resolve, Reject) {
            var PromiseArray = [];
            var files = document.querySelector(Selector).files;
            var filesLength = files.length;

            for (i = 0; i < filesLength; i++) {
                var p = new Promise(function (Resolve1, Reject1) {
                    var file = files[i];
                    var reader = new FileReader();

                    reader.readAsDataURL(file);
                    reader.onload = function (event) {

                        var SliceIndex = reader.result.indexOf(',') + 1;
                        Resolve1({
                            Filename: file.name,
                            FileBase64: reader.result.slice(SliceIndex),
                            FileBase64Original: reader.result
                        });
                    };
                    reader.onerror = function (error) {
                        Reject1(error)
                    };
                });

                PromiseArray.push(p);
            }

            Promise.all(PromiseArray).then(function (values) {
                Resolve(values)
            }).catch(function (error) {
                Reject(error)
            });
        })
    },

    GUP: function (name, url) {
        name = name.replace(/[\[]/, '\\\[').replace(/[\]]/, '\\\]');
        var regexS = '[\\?&]' + name + '=([^&#]*)';
        var regex = new RegExp(regexS);
        if (url == undefined || url == null) {
            url = window.location.href;
        }
        var results = regex.exec(url);
        if (results == null)
            return null;
        else
            return results[1];
    },

    HMSToSeconds: function (hms) {
        try {
            var a = hms.split(':');
            if ((+a[0]) > 12 || (+a[1] > 59) || (+a[2] > 59)) {
                return -1;
            }
            var res = parseInt(seconds = (+a[0]) * 60 * 60 + (+a[1]) * 60 + (+a[2]));
            return isNaN(res) ? -1 : res;
        } catch (ex) {
            return -1;
        }
    },

    NewID: function () {

        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
        };

        return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
    },

    SecondsToHMS: function (TotalSeconds, option) {
        if (TotalSeconds >= 0) {
            switch (option) {
                case 0:
                    {

                        var sec_num = Math.round(TotalSeconds);
                        var hours = Math.floor(sec_num / 3600);
                        var minutes = Math.floor((sec_num - (hours * 3600)) / 60);
                        var seconds = sec_num - (hours * 3600) - (minutes * 60);

                        if (hours < 10) { hours = '0' + hours; }
                        if (minutes < 10) { minutes = '0' + minutes; }
                        if (seconds < 10) { seconds = '0' + seconds; }
                        var time = hours + ':' + minutes + ':' + seconds;
                        return time;
                    }
                default:
                    {
                        TotalSeconds = parseInt(TotalSeconds);
                        var hours = Math.floor(TotalSeconds / 3600);
                        TotalSeconds -= hours * 3600;
                        var minutes = Math.floor(TotalSeconds / 60);
                        TotalSeconds -= minutes * 60;

                        return result = (hours < 10 ? '0' + hours : hours) + 'h ' + (minutes < 10 ? '0' + minutes : minutes) + 'm ' + (TotalSeconds < 10 ? '0' + TotalSeconds : TotalSeconds) + 's';
                    }
            }
        }
        else {
            return '&mdash;';
        }
    },

    SetCookie: function (cname, cvalue, exdays) {

        if (exdays == undefined) { exdays = 7; }

        var d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        var expires = 'expires=' + d.toUTCString();
        document.cookie = cname + '=' + cvalue + ';' + expires + ';path=/';
    },

    GetCookie: function (cname) {
        var name = cname + '=';
        var decodedCookie = decodeURIComponent(document.cookie);
        var ca = decodedCookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return '';
    }

};

$.fn.extend({
    GetExtension: function () {
        var val = this.selector.match(/\.[^.]+$/);
        return val == null || val.length == 0 ? undefined : val[0].toLowerCase();
    },

    Show: function () {
        this.removeClass('d-none');
        this.removeClass('hidden');
    },
    Hide: function () {
        this.addClass('d-none');
    },
    Toggle: function () {
        if (this.hasClass('d-none')) {
            this.removeClass('d-none');
        }
        else {
            this.addClass('d-none');
        }
    },

    DisableWithOverlay: function () {
        var html =
            '<div class="js-overlay-disable" style="position:absolute;top:0;left:0;z-index:99999;width:100%;height:100%;">\
    <div style="position:absolute;top:0;left:0;z-index:1;width:100%;height:100%;opacity:0.5;background-color:#fff"></div>\
    <div style="position:absolute;top:50%;left:50%;z-index:2;transform:translate(-50%,-50%)"></div>\
</div>';
        this.append(html)
    },
    Enable: function () {
        this.removeClass('disabled');
        this.removeAttr('disabled');
        this.find('.js-overlay-disable').remove();
    },
    ScrollTo: function (selector, milliseconds, offsetTop) {
        var _this = this;
        return new Promise(function (Resolve) {
            if (this != null && this != undefined && _this.length != 0) {

                selector = selector == undefined ? 'html' : selector;
                milliseconds = milliseconds == undefined ? 500 : milliseconds;
                offsetTop = offsetTop == undefined ? 100 : offsetTop;

                if ($(selector).length > 0) {
                    $(selector).animate({
                        scrollTop: _this.offset().top - offsetTop
                    }, milliseconds, function () {
                        Resolve();
                    });
                }
            }
        });
    },
    Shake: function (AnimateSide) {
        var _this = $(this)
        _this.addClass('custom-shake');
        setTimeout(function () {
            _this.removeClass('custom-shake');
        }, 300);
    },
    Strike: function () {
        this.css({
            'text-decoration': 'line-through'
        });
    },
    UnStrike: function () {
        this.css({
            'text-decoration': ''
        });
    },
    ToSlug: function (opt) {
        var s = this.val();
        s = String(s);
        opt = Object(opt);

        var defaults = {
            'delimiter': '-',
            'limit': undefined,
            'lowercase': true,
            'replacements': {},
            'transliterate': (typeof (XRegExp) === 'undefined') ? true : false
        };

        // Merge options
        for (var k in defaults) {
            if (!opt.hasOwnProperty(k)) {
                opt[k] = defaults[k];
            }
        }

        var char_map = {
            // Georgian
            'ა': 'a', 'ბ': 'b', 'გ': 'g', 'დ': 'd', 'ე': 'e', 'ვ': 'v', 'ზ': 'z', 'თ': 't',
            'ი': 'i', 'კ': 'k', 'ლ': 'l', 'მ': 'm', 'ნ': 'N', 'ო': 'o', 'პ': 'p', 'ჟ': 'zh',
            'რ': 'r', 'ს': 's', 'ტ': 't', 'უ': 'u', 'ფ': 'f', 'ქ': 'k', 'ღ': 'gh', 'ყ': 'k',
            'შ': 'sh', 'ჩ': 'ch', 'ც': 'c', 'ძ': 'dz', 'წ': 'ts', 'ჭ': 'ch', 'ხ': 'kh', 'ჯ': 'j',
            'ჰ': 'h',

            // Latin symbols
            '©': '(c)',
           
            // Russian
            'А': 'A', 'Б': 'B', 'В': 'V', 'Г': 'G', 'Д': 'D', 'Е': 'E', 'Ё': 'Yo', 'Ж': 'Zh',
            'З': 'Z', 'И': 'I', 'Й': 'J', 'К': 'K', 'Л': 'L', 'М': 'M', 'Н': 'N', 'О': 'O',
            'П': 'P', 'Р': 'R', 'С': 'S', 'Т': 'T', 'У': 'U', 'Ф': 'F', 'Х': 'H', 'Ц': 'C',
            'Ч': 'Ch', 'Ш': 'Sh', 'Щ': 'Sh', 'Ъ': '', 'Ы': 'Y', 'Ь': '', 'Э': 'E', 'Ю': 'Yu',
            'Я': 'Ya',
            'а': 'a', 'б': 'b', 'в': 'v', 'г': 'g', 'д': 'd', 'е': 'e', 'ё': 'yo', 'ж': 'zh',
            'з': 'z', 'и': 'i', 'й': 'j', 'к': 'k', 'л': 'l', 'м': 'm', 'н': 'n', 'о': 'o',
            'п': 'p', 'р': 'r', 'с': 's', 'т': 't', 'у': 'u', 'ф': 'f', 'х': 'h', 'ц': 'c',
            'ч': 'ch', 'ш': 'sh', 'щ': 'sh', 'ъ': '', 'ы': 'y', 'ь': '', 'э': 'e', 'ю': 'yu',
            'я': 'ya',

           
        };

        // Make custom replacements
        for (var k in opt.replacements) {
            s = s.replace(RegExp(k, 'g'), opt.replacements[k]);
        }

        // Transliterate characters to ASCII
        if (opt.transliterate) {
            for (var k in char_map) {
                s = s.replace(RegExp(k, 'g'), char_map[k]);
            }
        }

        // Replace non-alphanumeric characters with our delimiter
        var alnum = (typeof (XRegExp) === 'undefined') ? RegExp('[^a-z0-9]+', 'ig') : XRegExp('[^\\p{L}\\p{N}]+', 'ig');
        s = s.replace(alnum, opt.delimiter);

        // Remove duplicate delimiters
        s = s.replace(RegExp('[' + opt.delimiter + ']{2,}', 'g'), opt.delimiter);

        // Truncate slug to max. characters
        s = s.substring(0, opt.limit);

        // Remove delimiter from ends
        s = s.replace(RegExp('(^' + opt.delimiter + '|' + opt.delimiter + '$)', 'g'), '');

        return opt.lowercase ? s.toLowerCase() : s;
    },
    SetScrollHeight: function (height) {
        if (!(height > 0)) {
            height = $(this).outerHeight();
        }

        this.css({
            'overflow': 'auto',
            'height': height + 'px',
            '-webkit-overflow-scrolling': 'touch'
        });
    },
    SetFullHeight: function (HeightCorrectionInPixels) {
        // Making sure that number is passed, if not HeightCorrectionInPixels will be zero.
        HeightCorrectionInPixels = HeightCorrectionInPixels % 1 === 0 ? HeightCorrectionInPixels : 0;
        const ScreenHeight = $(window).outerHeight();


        const PaddingBottom = 25;
        const OffsetTop = $(this).offset().top;
        const ElementHeight = ScreenHeight - OffsetTop - PaddingBottom + HeightCorrectionInPixels;

        this.css({
            'overflow': 'auto',
            'height': ElementHeight + 'px',
            '-webkit-overflow-scrolling': 'touch'
        });
    },
    GetFullHeight: function (HeightCorrectionInPixels) {
        HeightCorrectionInPixels = HeightCorrectionInPixels % 1 === 0 ? HeightCorrectionInPixels : 0;
        const ScreenHeight = $(window).outerHeight();
        const PaddingBottom = 25;
        const OffsetTop = $(this).offset().top;
        const ElementHeight = ScreenHeight - OffsetTop - PaddingBottom + HeightCorrectionInPixels;
        return ElementHeight;
    },
    MaskPhoneNumberGeorgian: function () {
        $.mask.definitions['9'] = '';
        $.mask.definitions['d'] = '[0-9]';
        this.mask('+995 (ddd) dd-dd-dd');
    },
    MaskUserPersonalNumberGeorgian: function () {
        $.mask.definitions['9'] = '';
        $.mask.definitions['d'] = '[0-9]';
        this.mask('?ddddddddddd');
    },
    MaskCompanyNumberGeorgian: function () {
        $.mask.definitions['9'] = '';
        $.mask.definitions['d'] = '[0-9]';
        this.mask('ddddddddd');
    },
    // call example $(['path to image1','path to image2', '...']).PreloadImages();
    PreloadImages: function () {
        this.each(function () {
            $('<img/>')[0].src = this;
        });
    }
});