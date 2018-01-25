//window.waitcount = 0;
var functionReload = [];
var CommonUtils = {

    SetDayFormat: function (inputDate, defaultFormat) {
        var date;
        if (inputDate.constructor === Date)
            date = new Date(inputDate);
        else
            date = new Date(parseInt(inputDate.substr(6)));
        date.setHours(0, 0, 0, 0);

        var today = new Date();
        today.setHours(0, 0, 0, 0);

        var yesterday = new Date();
        yesterday.setHours(0, 0, 0, 0);
        yesterday.setDate(yesterday.getDate() - 1);

        var tomorrow = new Date();
        tomorrow.setHours(0, 0, 0, 0);
        tomorrow.setDate(tomorrow.getDate() + 1);

        var strDate = "";
        var plusTime = false;

        switch (date.getTime()) {

            case today.getTime():
                strDate = Globalize.culture().calendar.today;
                if (defaultFormat && defaultFormat == 'f') plusTime = true;
                break;

            case yesterday.getTime():
                strDate = Globalize.culture().calendar.yesterday;
                if (defaultFormat && defaultFormat == 'f') plusTime = true;
                break;

            case tomorrow.getTime():
                strDate = Globalize.culture().calendar.tomorrow;
                if (defaultFormat && defaultFormat == 'f') plusTime = true;
                break;

            default:
                strDate = defaultFormat ? Globalize.format(inputDate, defaultFormat) : inputDate;
                break;
        }

        if (plusTime) {
            strDate += ' ' + Globalize.format(inputDate, 't');
        }

        return strDate;
    },
    SetDateFormat: function (inputDate, defaultFormat) {
        var date;
        if (inputDate.constructor === Date)
            date = new Date(inputDate);
        else
            date = new Date(parseInt(inputDate.substr(6)));
        date.setHours(0, 0, 0, 0);

        var strDate = "";
        var plusTime = false;

        strDate = defaultFormat ? Globalize.format(inputDate, defaultFormat) : inputDate;

        if (plusTime) {
            strDate += ' ' + Globalize.format(inputDate, 't');
        }

        return strDate;
    },
    SetTimeFormat: function (inputDate, defaultFormat, isLowerCaseDefault) {
        var diffSeconds = ((new Date()).getTime() - inputDate.getTime()) / 1000;
        var diffMinutes = Math.floor(diffSeconds / 60);

        if (diffMinutes < 1)
            return Globalize.culture().calendar.justNow;
        else if (diffMinutes < 2)
            return Globalize.culture().calendar.oneMinuteAgo;
        else if (diffMinutes < 60)
            return Globalize.culture().calendar.fewMinutesAgo.replace('m', diffMinutes);
        else {
            if (!defaultFormat)
                defaultFormat = Globalize.culture().calendar.patterns.C7;
            if (isLowerCaseDefault)
                return Globalize.format(inputDate, defaultFormat).toLowerCase();
            else
                return Globalize.format(inputDate, defaultFormat);
        }
    },
    formatDateTimeShort: function (value) {
        if (value) {
            var date;
            if (value.constructor === Date)
                date = value;
            else
                date = new Date(parseInt(value.substr(6)));
            return CommonUtils.SetDayFormat(date, 'f');
        }
        else
            return null;
    },
    showModal: function (nameModal, callback, cancelCallback, closeCallback) {
        $(nameModal).on('show.bs.modal', function () {
            $(nameModal + ' .btnCancel').bind('click', function () {
                $(nameModal).modal('hide');
                if (cancelCallback && typeof (cancelCallback) == "function") {
                    cancelCallback();
                }
            });
            $(nameModal + ' .btnOk').bind('click', function () {
                callback(true);
                $(nameModal).modal('hide');
            });
            $(nameModal).off('show.bs.modal');
        }).on('hide.bs.modal', function () {
            $(nameModal + ' .btnCancel').unbind('click');
            $(nameModal + ' .btnOk').unbind('click');
            $(nameModal).off('hide.bs.modal');
            if (closeCallback && typeof (closeCallback) == "function") {
                closeCallback();
            }
        }).modal({
            "keyboard": true,
            "show": true
        });
    },
    closeModal: function (nameModal) {
        $(nameModal).modal('hide');
    },
    confirm: function (title, msg, callback, buttonOK, cancelCallback, closeCallback) {
        buttonOK = (typeof buttonOK === 'undefined') ? 'Đồng ý' : buttonOK;
        $('#confirmModal').on('show.bs.modal', function () {
            $('#confirmModal .modal-title').text(title);
            $('#confirmModal .modal-body').html(msg);
            $('#confirmModal .btnOk').text(buttonOK);
            $('#confirmModal .btnCancel').bind('click', function () {
                $("#confirmModal").modal('hide');
                if (cancelCallback && typeof (cancelCallback) == "function") {
                    cancelCallback();
                }
            });
            $('#confirmModal .btnOk').bind('click', function () {
                callback(true);
                $("#confirmModal").modal('hide');
            });
            $('#confirmModal').off('show.bs.modal');
        }).on('hide.bs.modal', function () {
            $('#confirmModal .btnCancel').unbind('click');
            $('#confirmModal .btnOk').unbind('click');
            $('#confirmModal').off('hide.bs.modal');
            if (closeCallback && typeof (closeCallback) == "function") {
                closeCallback();
            }
        }).modal({
            "keyboard": true,
            "show": true
        });
    },
    MapArray: function (rawArray, model) {
        var mappedData = ko.utils.arrayMap(rawArray, function (value) {
            var item = new model();
            ko.mapping.fromJS(value, {}, item);
            return item;
        });
        return mappedData;
    },
    notify: function (title, msg, type) {
        var str = "error";
        switch (type) {
            case "success":
                str = "success";
                break;
            case "error":
                str = "error";
                break;
            case "warning":
                str = "warning";
                break;
        };
        $.gritter.add({
            // (string | mandatory) the heading of the notification
            title: title,
            // (string | mandatory) the text inside the notification
            text: msg,
            class_name: 'gritter-' + str
        });
    },
    addTabDynamic: function (title, url, idDiv, isReload, data) {
        if ($(idDiv).tabs('exists', title) && isReload == undefined) {
            $(idDiv).tabs('select', title);
        } else {
            if (isReload)
                $(idDiv).tabs('close', title);
            CommonUtils.showWait(true);
            $.ajax({
                type: "POST",
                url: url,
                cache: false,
                contentType: 'application/json; charset=utf-8',
                data: ko.toJSON(data),
            }).done(function (result) {
                $(idDiv).tabs('add', {
                    title: title,
                    content: result,
                    closable: true
                });

            }).always(function () {
                CommonUtils.showWait(false);
            });

            //        if (data != undefined) {
            //        CommonUtils.showWait(true);
            //        $.ajax({
            //            type: "POST",
            //            url: url,
            //            cache: false,
            //            contentType: 'application/json; charset=utf-8',
            //            data: ko.toJSON(data),
            //        }).done(function (result) {
            //            $(idDiv).tabs('add', {
            //                title: title,
            //                content: result,
            //                closable: true
            //            });

            //        }).always(function () {
            //            CommonUtils.showWait(false);
            //        });
            //    } else {
            //        CommonUtils.showWait(true);
            //        $.post(url, function (result) {
            //            $(idDiv).tabs('add', {
            //                title: title,
            //                content: result,
            //                closable: true
            //            });
            //            CommonUtils.showWait(false);
            //        });
            //    }
        }
    },
    showWait: function (show, nameclass) {
        var overlay = "<div class='overlay'></div><div class='pageloading'><img src='../Images/loading.gif' /></div>";
        var element = nameclass != undefined ? "#" + nameclass : "body";
        if (show) {
            $(element).append(overlay);
            //window.waitcount = window.waitcount + 1;
            $(element + " .pageloading").show();
            $(element + " .overlay").show();
        }
        else {
            //window.waitcount = window.waitcount - 1;
            //if (window.waitcount == 0) {
            var timeout = 0;
            //if (delay != undefined && delay > 0) {
            //    timeout = delay;
            //}
            setTimeout(function () {
                $(element + " .pageloading").hide().remove();
                $(element + " .overlay").hide().remove();
            }, timeout);

            //}
        }
    },
    datelong: function (value) {
        if (value) {
            var date;
            if (value.constructor === Date)
                date = value;
            else
                date = new Date(parseInt(value.substr(6)));
            return Globalize.format(date, 'f');
        }
    },
    dateshort: function (value) {
        if (value) {
            var date;
            if (value.constructor === Date)
                date = value;
            else
                date = new Date(parseInt(value.substr(6)));
            return Globalize.format(date, 'd');
        }
    },
    url: function (path) {
        return (location.origin + path);
    },
    randomString: function (len, charSet) {
        charSet = charSet || 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        var randomString = '';
        for (var i = 0; i < len; i++) {
            var randomPoz = Math.floor(Math.random() * charSet.length);
            randomString += charSet.substring(randomPoz, randomPoz + 1);
        }
        return randomString;
    },
    loadListCatalog: function (callback) {
        CommonUtils.showWait(true);
        $.ajax({
            type: "GET",
            url: "/Catalog/LoadLstCatalog"
        }).done(function (data) {
            CommonUtils.showWait(false);
            callback(data);
        }).fail(function () {
            CommonUtils.showWait(false);
        });

    },
    loadListSuppliers: function (callback) {
        CommonUtils.showWait(true);
        $.ajax({
            type: "GET",
            url: "/Suppliers/LoadLstSuppliers"
        }).done(function (data) {
            CommonUtils.showWait(false);
            callback(data);
        }).fail(function () {
            CommonUtils.showWait(false);
        });

    },
    loadListChannel: function (callback) {
        CommonUtils.showWait(true);
        $.ajax({
            type: "GET",
            url: "/Common/LoadLstChannel"
        }).done(function (data) {
            CommonUtils.showWait(false);
            callback(data.Data);
        }).fail(function () {
            CommonUtils.showWait(false);
        });
    },
    LoadLstBranches: function (callback) {
        CommonUtils.showWait(true);
        $.ajax({
            type: "GET",
            url: "/Common/LoadLstBranches"
        }).done(function (data) {
            CommonUtils.showWait(false);
            callback(data);
        }).fail(function () {
            CommonUtils.showWait(false);
        });
    },
    loadLstLoadEmployes: function (callback) {
        CommonUtils.showWait(true);
        $.ajax({
            type: "GET",
            url: "/Common/LoadEmployes"
        }).done(function (data) {
            CommonUtils.showWait(false);
            callback(data);
        }).fail(function () {
            CommonUtils.showWait(false);
        });
    },
    functionReloadPush: function (obj) {
        functionReload.push(obj);
    },
    functionReloadAction: function () {
        functionReload.forEach(function (entry) {
            entry();
        });
    },
    PrintDiv: function (divName) {
        var contents = document.getElementById(divName).innerHTML;
        var frame1 = document.createElement('iframe');
        frame1.name = "frame1";
        frame1.style.position = "absolute";
        frame1.style.top = "-1000000px";
        document.body.appendChild(frame1);
        var frameDoc = frame1.contentWindow ? frame1.contentWindow : frame1.contentDocument.document ? frame1.contentDocument.document : frame1.contentDocument;
        frameDoc.document.open();
        frameDoc.document.write('<html><body>');
        frameDoc.document.write(contents);
        frameDoc.document.write('</body></html>');
        frameDoc.document.close();
        setTimeout(function () {
            window.frames["frame1"].focus();
            window.frames["frame1"].print();
            document.body.removeChild(frame1);
        }, 500);
        return false;
    },
    Print: function (el) {
        var contents = document.getElementById(el).innerHTML;
        var frame1 = document.createElement('iframe');
        frame1.name = "frame1";
        frame1.style.position = "absolute";
        frame1.style.top = "-1000000px";
        document.body.appendChild(frame1);
        var frameDoc = frame1.contentWindow ? frame1.contentWindow : frame1.contentDocument.document ? frame1.contentDocument.document : frame1.contentDocument;
        frameDoc.document.open();
        frameDoc.document.write('<html><head><title>DIV Contents</title>');
        frameDoc.document.write('<link rel="stylesheet" href="/Content/css/bootstrap.min.css" type="text/css" /><link rel="stylesheet" href="/Content/css/ace.min.css" type="text/css" /><link rel="stylesheet" href="/Content/css/me/me.css" type="text/css" />  ');

        frameDoc.document.write('</head><body>');
        frameDoc.document.write(contents);
        frameDoc.document.write('</body></html>');
        frameDoc.document.close();
        setTimeout(function () {
            window.frames["frame1"].focus();
            window.frames["frame1"].print();
            document.body.removeChild(frame1);
        }, 500);
        return false;

    },
    Groupbycol: function (data, count) {
        var index, length, group,
                    result = [],
                    count = +ko.utils.unwrapObservable(count) || 1,
                    items = ko.utils.unwrapObservable(data);

        //create an array of arrays (rows/columns)
        for (index = 0, length = items.length; index < length; index++) {
            if (index % count === 0) {
                group = [];
                result.push(group);
            }

            group.push(items[index]);
        }

        return result;
    },
    Groupbyobject: function (myArray, name, objectname) {
        var resPrint = myArray.reduce(function (res, currentValue) {
            if (res.indexOf(currentValue[name]()) === -1) {
                res.push(currentValue[name]());
            }
            return res;
        }, []).map(function (name) {
            return {
                suppliers: name,
                products: myArray.filter(function (_el) {
                    return _el[objectname]() === name;
                }).map(function (_el) { return _el; })
            }
        });
        return resPrint;
    },
    GetAttrStatus: function (myArray, objectname, Key) {
        var rs = ko.utils.arrayFirst(myArray, function (obj) {
            return obj['Code'] == objectname
        })
        return rs[Key];
    },
    textMoneyDefaultSymbol: function (value) {
        var format = Globalize.cultures['vi-VN'].default.numberFormat.currency.pattern[1];
        var symbol = Globalize.cultures['vi-VN'].default.numberFormat.currency.symbol;
        var rs = Globalize.format(value, format);
        return rs + ' ' + symbol
    },
    chunkArray: function (myArray, chunk_size) {
        var index = 0;
        var arrayLength = myArray.length;
        var tempArray = [];

        for (index = 0; index < arrayLength; index += chunk_size) {
            myChunk = myArray.slice(index, index + chunk_size);
            // Do something if you want with the group
            tempArray.push(myChunk);
        }

        return tempArray;
    }
}