const utilities = {
    consoleLogDevelopedBy: function () {
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
    date: {
        monthShortNames: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        weekDaysShortNames: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
        weekDays: {
            MONDAY: 1,
            TUESDAY: 2,
            WEDNESDAY: 3,
            THURSDAY: 4,
            FRIDAY: 5,
            SATURDAY: 6,
            SUNDAY: 0
        },
        addDays: function (input, daysToAdd) {
            const date = new Date(input);
            daysToAdd = parseInt(daysToAdd);
            if (input && (daysToAdd >= 0 || daysToAdd < 0) && !isNaN(date.getTime())) {
                date.setDate(date.getDate() + daysToAdd);
                return date;
            }
            else {
                return null;
            }
        },
        addBusinessDays: function (input, businessDaysToAdd) {
            const date = new Date(input);
            businessDaysToAdd = parseInt(businessDaysToAdd);
            if (input && (businessDaysToAdd >= 0 || businessDaysToAdd < 0) && !isNaN(date.getTime())) {
                const wks = Math.floor(businessDaysToAdd / 5);
                let dys = utilities.numbers.mod(businessDaysToAdd, 5);
                let dy = date.getDay();
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

                var dateToReturn = new Date(date);
                dateToReturn.setDate(date.getDate() + wks * 7 + dys);
                return dateToReturn;
            }
            else {
                return null;
            }
        },
        getDateWithoutTime: function (input) {
            const date = new Date(input);
            if (input && !isNaN(date.getTime())) {
                const year = date.getFullYear();
                let month = (date.getMonth() + 1);
                let day = date.getDate();

                month = month < 10 ? ('0' + month) : month;
                day = day < 10 ? ('0' + day) : day;
                return new Date(year + '-' + month + '-' + day + 'T00:00:00');
            }
            else {
                return null;
            }
        },
        isWeekend: function (input) {
            const date = new Date(input);
            if (input && !isNaN(date.getTime())) {
                const day = date.getDay();
                return day == utilities.date.weekDays.SATURDAY || day == utilities.date.weekDays.SUNDAY;
            }
            else {
                return false;
            }
        },        
        toShortDate: function (input) {
            const date = new Date(input);
            if (input && !isNaN(date.getTime())) {
                const year = date.getFullYear();
                const month = utilities.date.monthShortNames[date.getMonth()]
                const day = date.getDate();
                return month + ' ' + day + ', ' + year;
            }
            else {
                return null;
            }
        },
        toShortDateTime: function (input) {
            const date = new Date(input);
            if (input && !isNaN(date.getTime())) {
                const year = date.getFullYear();
                const month = utilities.date.monthShortNames[date.getMonth()];
                const day = date.getDate();
                const hours = date.getHours();
                const minutes = date.getMinutes();
                return month + ' ' + day + ', ' + year + ' ' + hours + ':' + minutes;
            }
            else {
                return null;
            }
        },
        toTime: function (input) {
            const date = new Date(input);
            if (input && !isNaN(date.getTime())) {
                const hours = (date.getHours() < 10 ? '0' : '') + date.getHours();
                const minutes = (date.getMinutes() < 10 ? '0' : '') + date.getMinutes();
                return hours + ':' + minutes;
            }
            else {
                return null;
            }
        },
        toWeekDayShortDate: function (input) {
            const D = new Date(input);
            if (input && !isNaN(date.getTime())) {
                const year = date.getFullYear();
                let dayOfMonth = date.getDate();
                dayOfMonth = (dayOfMonth > 9 ? dayOfMonth : '0' + dayOfMonth);
                const month = utilities.date.monthShortNames[date.getMonth()];
                const weekday = utilities.date.weekDaysShortNames[date.getDay()];
                return month + ' ' + dayOfMonth + ', ' + year + ', ' + weekday;
            }
            else {
                return null;
            }
        },
    },

    string: {
        endsWith: function (suffix) {
            return this.indexOf(suffix, this.length - suffix.length) !== -1;
        },
        stripHtml: function (InputString) {
            return InputString.replace(/<[^>]*>/g, '');
        }
    },

    numbers: {
        mod: function (input, n) {
            return ((input % n) + n) % n;
        }
    },

    bytesToSize: function (bytes) {
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

    getBase64FromInputFilePromise: function (selector) {
        return new Promise(function (resolve, reject) {
            const element = document.querySelector(selector);
            if (element && element.files && element.files.length > 0) {
                const file = element.files[0];
                const reader = new FileReader();
                reader.readAsDataURL(file);
                reader.onerror = function (error) {
                    reject(error)
                };
                reader.onload = function () {

                    const sliceIndex = reader.result.indexOf(',') + 1;

                    resolve({
                        filename: file.name,
                        fileBase64: reader.result.slice(sliceIndex),
                        fileBase64Original: reader.result
                    });
                };

            }
            else {
                resolve({
                    Filename: null,
                    FileBase64: null,
                    FileBase64Original: null
                });
            }
        });
    },

    getBase64ArrayFromInputFileMultiplePromise: function (selector) {
        return new Promise(function (resolve, reject) {
            var promiseArray = [];
            var files = document.queryselector(selector).files;
            var filesLength = files.length;

            for (i = 0; i < filesLength; i++) {
                var p = new Promise(function (resolve1, reject1) {
                    var file = files[i];
                    var reader = new FileReader();

                    reader.readAsDataURL(file);
                    reader.onload = function (event) {

                        var sliceIndex = reader.result.indexOf(',') + 1;
                        resolve1({
                            filename: file.name,
                            fileBase64: reader.result.slice(sliceIndex),
                            fileBase64Original: reader.result
                        });
                    };
                    reader.onerror = function (error) {
                        reject1(error)
                    };
                });

                promiseArray.push(p);
            }

            Promise.all(promiseArray).then(function (values) {
                resolve(values)
            }).catch(function (error) {
                reject(error)
            });
        })
    },

    gup: function (name, url) {
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

    htsToSeconds: function (hms) {
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

    newID: function () {

        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
        };

        return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
    },

    secondsToHMS: function (totalSeconds, option) {
        if (totalSeconds >= 0) {
            switch (option) {
                case 0:
                    {

                        var sec_num = Math.round(totalSeconds);
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
                        totalSeconds = parseInt(totalSeconds);
                        var hours = Math.floor(totalSeconds / 3600);
                        totalSeconds -= hours * 3600;
                        var minutes = Math.floor(totalSeconds / 60);
                        totalSeconds -= minutes * 60;

                        return result = (hours < 10 ? '0' + hours : hours) + 'h ' + (minutes < 10 ? '0' + minutes : minutes) + 'm ' + (totalSeconds < 10 ? '0' + totalSeconds : totalSeconds) + 's';
                    }
            }
        }
        else {
            return '&mdash;';
        }
    },

    setCookie: function (cname, cvalue, exdays) {

        if (exdays == undefined) { exdays = 7; }

        const date = new Date();
        date.setTime(date.getTime() + (exdays * 24 * 60 * 60 * 1000));
        const expires = 'expires=' + date.toUTCString();
        document.cookie = cname + '=' + cvalue + ';' + expires + ';path=/';
    },

    getCookie: function (cname) {
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
    showElement: function () {
        this.removeClass('d-none');
        this.removeClass('hidden');
    },
    hideElement: function () {
        this.addClass('d-none');
    },    
    disableElement: function () {
        this.addClass('disabled');
        this.prop('disabled', true);
    },
    disableElementWithOverlay: function () {
        var html =
            '<div class="js-overlay-disable" style="position:absolute;top:0;left:0;z-index:99999;width:100%;height:100%;">\
    <div style="position:absolute;top:0;left:0;z-index:1;width:100%;height:100%;opacity:0.5;background-color:#fff"></div>\
    <div style="position:absolute;top:50%;left:50%;z-index:2;transform:translate(-50%,-50%)"></div>\
</div>';
        this.append(html)
    },
    enableElement: function () {
        this.removeClass('disabled');
        this.removeAttr('disabled');
        this.find('.js-overlay-disable').remove();
    },
    scrollToElement: function (selector, milliseconds, offsetTop) {
        var _this = this;
        return new Promise(function (resolve) {
            if (this != null && this != undefined && _this.length != 0) {

                selector = selector == undefined ? 'html' : selector;
                milliseconds = milliseconds == undefined ? 500 : milliseconds;
                offsetTop = offsetTop == undefined ? 100 : offsetTop;

                if ($(selector).length > 0) {
                    $(selector).animate({
                        scrollTop: _this.offset().top - offsetTop
                    }, milliseconds, function () {
                        resolve();
                    });
                }
            }
        });
    },
    shakeElement: function () {
        var _this = $(this)
        _this.addClass('custom-shake');
        setTimeout(function () {
            _this.removeClass('custom-shake');
        }, 300);
    },
    strikeElement: function () {
        this.css({
            'text-decoration': 'line-through'
        });
    },
    unStrikeElement: function () {
        this.css({
            'text-decoration': ''
        });
    },
    toSlug: function (opt) {
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
    setScrollHeight: function (height) {
        if (!(height > 0)) {
            height = $(this).outerHeight();
        }

        this.css({
            'overflow': 'auto',
            'height': height + 'px',
            '-webkit-overflow-scrolling': 'touch'
        });
    },
    setFullHeight: function (heightCorrectionInPixels) {
        // Making sure that number is passed, if not heightCorrectionInPixels will be zero.
        heightCorrectionInPixels = heightCorrectionInPixels % 1 === 0 ? heightCorrectionInPixels : 0;
        const screenHeight = $(window).outerHeight();

        const paddingBottom = 25;
        const offsetTop = $(this).offset().top;
        const elementHeight = screenHeight - offsetTop - paddingBottom + heightCorrectionInPixels;

        this.css({
            'overflow': 'auto',
            'height': elementHeight + 'px',
            '-webkit-overflow-scrolling': 'touch'
        });
    },
    getFullHeight: function (heightCorrectionInPixels) {
        heightCorrectionInPixels = heightCorrectionInPixels % 1 === 0 ? heightCorrectionInPixels : 0;
        const screenHeight = $(window).outerHeight();
        const paddingBottom = 25;
        const offsetTop = $(this).offset().top;
        const elementHeight = screenHeight - offsetTop - paddingBottom + heightCorrectionInPixels;
        return elementHeight;
    },
    maskPhoneNumberGeorgian: function () {
        $.mask.definitions['9'] = '';
        $.mask.definitions['d'] = '[0-9]';
        this.mask('+995 (ddd) dd-dd-dd');
    },
    maskUserPersonalNumberGeorgian: function () {
        $.mask.definitions['9'] = '';
        $.mask.definitions['d'] = '[0-9]';
        this.mask('?ddddddddddd');
    },
    maskCompanyNumberGeorgian: function () {
        $.mask.definitions['9'] = '';
        $.mask.definitions['d'] = '[0-9]';
        this.mask('ddddddddd');
    },
    // call example $(['path to image1','path to image2', '...']).PreloadImages();
    preloadImages: function () {
        this.each(function () {
            $('<img/>')[0].src = this;
        });
    }
});

if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
                ? args[number]
                : match
                ;
        });
    };
}