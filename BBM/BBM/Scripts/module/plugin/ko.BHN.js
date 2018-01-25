(function (ko) {
    ko.bindingHandlers.googleChart = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var chart = new google.visualization.ColumnChart(element);
            $(element).data('googleChart', chart);
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var chart = $(element).data('googleChart');
            var dataTable = new google.visualization.DataTable();
            dataTable.addColumn('string', '"LabelAxis');
            dataTable.addColumn('number', '"Revenue');
            dataTable.addColumn({ type: 'string', role: 'tooltip' });
            var val = ko.utils.unwrapObservable(valueAccessor());

            for (var i = 0; i < val.length; i++) {
                dataTable.addRows([[val[i][0], val[i][1], val[i][2]]]);
            }


            var options = {
                width: 600,
                height: 300,
                is3D: true,
                legend: 'none',
                fontSize: 12,
                axisTitlesPosition: 'in',
                vAxis: {
                    viewWindowMode: 'explicit',
                    viewWindow: {
                        min: 0.0
                    },
                    minValue: 0.0,
                    textPosition: 'out'

                },
                hAxis: {
                    showTextEvery: 1,
                    slantedText: true,
                    slantedTextAngle: 40
                }
            };
            chart.draw(dataTable, options);
        }
    };
    ko.bindingHandlers.googleChartReport = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var chart = new google.visualization.ColumnChart(element);
            $(element).data('googleChart', chart);
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var chart = $(element).data('googleChart');
            var dataTable = new google.visualization.DataTable();
            dataTable.addColumn('string', '');
            dataTable.addColumn('number', valueAccessor().legend());
            dataTable.addColumn({ type: 'string', role: 'tooltip' });
            var val = ko.utils.unwrapObservable(valueAccessor().data());

            for (var i = 1; i < val.length; i++) {
                dataTable.addRows([[val[i][0], val[i][1], val[i][2]]]);
            }

            var options = {
                hAxis: {
                    title: valueAccessor().title(),
                    showTextEvery: 1,
                    slantedText: true,
                    slantedTextAngle: 40
                }

            };

            chart.draw(dataTable, options);
        }
    };
    ko.bindingHandlers.Barcharts = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var chart = Morris.Bar({
                element: element,
                barGap: 4,
                barSizeRatio: 0.55,
                hideHover: 'auto',
                data: [],
                xkey: 'x',
                ykeys: ['y'],
                labels: ['Tổng doanh thu']
            }).on('click', function (i, row) {
                console.log(i, row);
            });

            $(element).data('Chart', chart);
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

            var chart = $(element).data('Chart');

            if (chart) {
                var dataChart = [];
                var val = ko.utils.unwrapObservable(valueAccessor());
                for (var i = 0; i < val.length; i++) {
                    var obj = { x: val[i][2], y: val[i][1] };
                    dataChart.push(obj);
                }
                chart.setData(dataChart);
            }
        }
    };
    ko.bindingHandlers.BarchartsReport = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var chart = Morris.Bar({
                element: element,
                barGap: 4,
                barSizeRatio: 0.55,
                hideHover: 'auto',
                data: [],
                xkey: 'x',
                ykeys: ['y'],
                labels: ['Tổng']
            }).on('click', function (i, row) {

            });

            $(element).data('Chart', chart);
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var chart = $(element).data('Chart');
            if (chart) {
                var dataChart = [];
                var val = ko.utils.unwrapObservable(valueAccessor());
                for (var i = 1; i < val.data().length; i++) {
                    var obj = {
                        x: val.data()[i][2], y: val.data()[i][1]
                    };
                    dataChart.push(obj);
                }
                chart.setData(dataChart);
            }
        }
    };

    ko.bindingHandlers.optionsgroup = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            $(element).change(function () {
                var val = $(element).val();
                var selectvalue = allBindings().value;
                if (ko.isWriteableObservable(selectvalue)) {
                    selectvalue(val);
                }
            });
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var selectedValue = ko.utils.unwrapObservable(allBindings().value);
            var val = ko.utils.unwrapObservable(valueAccessor());
            $(element).html('');
            ko.utils.arrayForEach(val, function (group) {
                var opt = $('<optgroup/>');
                opt.attr('label', group.name);

                ko.utils.arrayForEach(group.arrays, function (it) {
                    //var ht = "";
                    var ht = $('<option/>');
                    ht.attr('value', ko.utils.unwrapObservable(it.Id));
                    ht.text(ko.utils.unwrapObservable(it.Name));
                    if (selectedValue == ko.utils.unwrapObservable(it.Id)) {
                        ht.attr('selected', 'selected');
                    }

                    opt.append(ht);
                });
                $(element).append(opt);
            });
        }
    };

    ko.bindingHandlers.grouped = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            ko.bindingHandlers.foreach.init(element, function () { return []; }, allBindings, viewModel, bindingContext);
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var options = valueAccessor();
            var groups = _(ko.utils.unwrapObservable(options.data)).chain().groupBy(options.by).map(function (value, key) { return { key: key, items: value }; }).value();

            ko.bindingHandlers.foreach.update(element, function () { return groups; }, allBindings, viewModel, bindingContext);
        }
    };

    ko.bindingHandlers.returnKey = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            ko.utils.registerEventHandler(element, 'keydown', function (evt) {
                if (evt.keyCode === 13) {
                    evt.preventDefault();
                    evt.target.blur();
                    value(viewModel);
                }
            });
        }
    };
    //Custom binding for googlemap
    ko.bindingHandlers.googlemap = {
        init: function (element, valueAccessor, allBindings) {
            var latLng = new google.maps.LatLng(1, 1);
            mapOptions = {
                zoom: 15,
                center: latLng,
                mapTypeControl: false,
                zoomControl: false,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            var map = new google.maps.Map(element, mapOptions);
            $(element).data('map', map);
        },
        update: function (element, valueAccessor) {
            var geocoder;
            var lat;
            var lng;
            var value = valueAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);
            geocoder = new google.maps.Geocoder({});
            if (!valueUnwrapped || !valueUnwrapped.address) {

                var latLng = new google.maps.LatLng(1, 1);
                var map = $(element).data('map');
                var marker = new google.maps.Marker({
                    map: map,
                    position: latLng
                });
                if (map) {
                    map.setCenter(latLng);
                }
                return;
            }
            geocoder.geocode({
                'address': valueUnwrapped.address
            }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    lat = results[0].geometry.location.A;
                    lng = results[0].geometry.location.F;
                    if (lat && lng && lat > 0 && lng > 0) {
                        var latLng = new google.maps.LatLng(lat, lng);
                        var map = $(element).data('map');
                        var marker = new google.maps.Marker({
                            map: map,
                            position: latLng
                        });
                        if (map) {
                            map.setCenter(latLng);
                        }
                    }
                }
            });
        }
    };
    //end

    ko.bindingHandlers.featherEditor = {
        update: function (element, valueAccessor, allBindings, bindingContext) {
            var initAviary = function () {
                var option = allBindings().editoroption || {
                    saveCallback: function (doneCallback) { }
                }
                var value = valueAccessor();
                var modelPush = ko.unwrap(bindingContext);
                var jElement = $(element);
                if (value) {
                    var imgElementId = value;
                    var featherEditor = new Aviary.Feather({
                        apiKey: '1b98363a6794d074',
                        apiVersion: 3,
                        theme: 'light',
                        language: 'vi',
                        onSave: function (imageID, newURL) {
                            var img = document.getElementById(imageID);
                            var arr = imageID.split('-');
                            var id = 0;
                            if (arr.length > 1) {
                                id = arr[0];
                            }
                            img.src = newURL;
                            option.saveCallback(newURL, function () {
                                featherEditor.close();
                            }, id);
                        }
                    });
                    jElement.click(function () {
                        var src = '';
                        if (modelPush.Url != undefined) {
                            src = modelPush.Url();
                        }
                        else {
                            src = document.getElementById(imgElementId).src;
                        }
                        featherEditor.launch({
                            image: imgElementId,
                            url: src
                        });
                    });
                }
            };

            if (typeof (Aviary) != 'undefined') {
                initAviary();
            } else {
                var intervalAviary = setInterval(function () {
                    if (typeof (Aviary) != 'undefined') {
                        clearInterval(intervalAviary);
                        initAviary();
                    }
                }, 1000);
            }
        }
    };

    ko.bindingHandlers.file = {
        init: function (element, valueAccessor, allBindings) {
            var option = allBindings().fileoption || {
                IsSingle: true,
                Ext: "jpg,gif,png,jpeg",
                IsKeepFileName: false
            }

            var value = valueAccessor();
            var isUploading = option.IsUploading;
            var isError = option.IsError;
            var isFinish = option.IsFinish;
            var isAutoUpload = option.AutoUpload;
            if (isAutoUpload != false) isAutoUpload = true;
            var flagUpload = option.FlagUpload ? option.FlagUpload : null;
            var errorCallback = option.ErrorCallback ? option.ErrorCallback : null;
            var successCallback = option.SuccessCallback ? option.SuccessCallback : null;
            var totalFileUpload = 0;
            var numFileUploaded = 0;
            var limitFileUpload = option.LimitFileUpload ? option.LimitFileUpload : 10;
            var additionData = option.AdditionData ? option.AdditionData : null;
            var hasChooseFile = option.HasChooseFile ? option.HasChooseFile : null;
            var jElement = $(element).get(0);
            var maxWidth = $(jElement).attr("data-max-width");
            var maxHeight = $(jElement).attr("data-max-height");
            var randomId = CommonUtils.randomId();
            if ($(jElement).attr("type") == "file") {
                $(jElement).attr("capture", "camera");
                $(jElement).attr("accept", "image/*");
                $(jElement).addClass("input-file-upload");
                $(jElement).addClass("width-100-px");
                $(jElement).after("<label id='" + randomId + "' class='note inline'>Chọn file...</label>");
            };
            var uploader = new plupload.Uploader({
                runtimes: 'html5,flash,silverlight,html4',
                multi_selection: !option.IsSingle && CommonUtils.isMobile() == false,
                browse_button: jElement,
                url: option.Url,
                flash_swf_url: CommonUtils.url('Scripts/Libs/plupload/Moxie.swf'),
                silverlight_xap_url: CommonUtils.url('Scripts/Libs/plupload/Moxie.xap'),
                resize: option.Url.endsWith('/Media/UploadStorageFile') ? null : {
                    width: 2048,
                    height: 2048,
                    quality: 90
                },
                filters: {
                    max_file_size: '20mb',
                    mime_types: [
                        { title: "Files", extensions: option.Ext }
                    ]
                },
                multipart_params: {}
            });
            var buildAdditionData = function (a) {
                if (additionData && additionData()().length > 0) {
                    a.settings.multipart_params = {};
                    ko.utils.arrayForEach(additionData()(), function (item) {
                        a.settings.multipart_params[item.name] = item.data;
                    });
                    if (maxWidth) {
                        a.settings.multipart_params["maxWidth"] = maxWidth;
                    }
                    if (maxHeight) {
                        a.settings.multipart_params["maxHeight"] = maxHeight;
                    }
                }
            };
            uploader.bind('FilesAdded', function (a, b) {
                if (!option.IsSingle && b.length > limitFileUpload) {
                    if ($(jElement).attr("type") == "file") {
                        toastr.error("Bạn đã chọn quá " + limitFileUpload + " file");
                    }
                    else {
                        $(jElement).addClass('btn-danger');
                        $(jElement).val("Bạn đã chọn quá " + limitFileUpload + " file");
                    }
                    return false;
                }

                $(jElement).removeClass('btn-danger');
                if ($(jElement).attr("type") == "file") $("#" + randomId).text(b[0].name);
                else $(jElement).val("Tải file");
                if (option.IsSingle) {
                    if (typeof (value) == 'function') value(undefined);
                    else if (option.IsKeepFileName && value) b[0].name = value.FileName();

                    if (hasChooseFile)
                        hasChooseFile(true);
                }
                buildAdditionData(a);
                if (isAutoUpload) setTimeout(function () { a.start(); }, 100);
            });
            uploader.bind('BeforeUpload', function (a, b) { });
            uploader.bind('UploadProgress', function (a, b) {
                totalFileUpload = a.files.length;
                if (isUploading) isUploading(true);
                if ($(jElement).attr("type") != "file") $(jElement).val('Đang tải lên...');
            });
            uploader.bind('FileUploaded', function (a, b, rp) {
                if (isUploading) isUploading(false);
                if ($(jElement).attr("type") != "file") $(jElement).val('Tải file');
                var rt = ko.mapping.fromJSON(rp.response);
                if (rt.IsError && rt.IsError()) {
                    if (isError) isError(true);
                    if (isFinish) isFinish(true);
                    if ($(jElement).attr("type") == "file")
                        toastr.error(rt.Message());
                    else {
                        $(jElement).addClass('btn-danger');
                        $(jElement).val("Lỗi tải file");
                        $(jElement).attr('title', rt.Message())
                    }
                    if ((isFinish && isFinish() && !flagUpload)
                        ||
                        (option.IsSingle && isError)) {
                        a.splice();
                        a.refresh();
                    }
                    if (errorCallback && typeof (errorCallback) == "function")
                        errorCallback(b.name);
                }
                else {
                    if (rt.Data && typeof (rt.Data) == "object") {
                        var file = new Shared.mfile();
                        ko.mapping.fromJS(rt.Data, {}, file);

                        if (option.IsSingle) {
                            if (typeof (value) == 'function') value(file);
                            else ko.mapping.fromJS(rt.Data, {}, value);
                            if (isFinish) isFinish(true);
                        }
                        else {
                            value.push(file);
                            numFileUploaded++;
                            if (numFileUploaded == totalFileUpload)
                                if (isFinish) isFinish(true);
                        }
                        if (isFinish && isFinish()) {
                            a.splice();
                            a.refresh();
                            if ($(jElement).attr("type") == "file")
                                $("#" + randomId).text("Chọn file...");
                        }
                    } else {
                        if (isFinish) isFinish(true);
                    }
                    if (successCallback && typeof (successCallback) == "function") {
                        successCallback(rt);
                    }
                }
            });
            uploader.bind('Error', function (a, b) {
                if (isError) isError(true);
                if (isFinish) isFinish(true);
                if ($(jElement).attr("type") == "file")
                    toastr.error("Lỗi tải file");
                else {
                    $(jElement).addClass('btn-danger');
                    $(jElement).val("Lỗi tải file");
                }
                console.log(b);
            });
            uploader.init();
            if (flagUpload && typeof (flagUpload) == "function") {
                flagUpload.subscribe(function (data) {
                    if (data) {
                        $.each(uploader.files, function (i, f) { f.status = 1; });
                        buildAdditionData(uploader);
                        uploader.start();
                    }
                    if (isError) isError(false);
                    if (isFinish) isFinish(false);
                });
            };
        }
    };

    ko.bindingHandlers.tags = {
        init: function (element, valueAccessor, allBindings) {
            var jElement = $(element);
            jElement.tagsinput({
                confirmKeys: [13, 188]
            });
            jElement.change(function () {
                var val = jElement.val();
                var value = valueAccessor();
                if (ko.isObservable(value)) {
                    value(val);
                }
            });
        },
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);
            var jElement = $(element);
            if (jElement.val() != valueUnwrapped) {
                jElement.tagsinput('removeAll');
                jElement.tagsinput('add', valueUnwrapped);
            }
        }
    };

    ko.bindingHandlers.suggestTags = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);

            var selectedTags = allBindingsAccessor().selectedTags;
            var selectedtagsUnwrapped = ko.utils.unwrapObservable(selectedTags);
            if (selectedtagsUnwrapped)
                selectedtagsUnwrapped = selectedtagsUnwrapped.split(',');

            var jElement = $(element);
            var isShowedAll = jElement.find("div a.collapsed.collapse").length > 0 ? true : false
            var checkSelectedTags = function () {
                if (selectedtagsUnwrapped && selectedtagsUnwrapped.length > 0) {
                    jElement.find('label').each(function (i, it) {
                        $(it).addClass('item tag-color-' + (i + 1));
                        var text = $(it).text();
                        var f = ko.utils.arrayFirst(selectedtagsUnwrapped, function (a) {
                            return a == text;
                        });
                        if (f) $(it).addClass('disabled');
                    });
                }
            };
            // handle modal popup close - reset tags control
            $('.modal').on('hidden.bs.modal', function (event) {
                if ($(this).find("div.tags").length > 0) {
                    $(this).find("div.tags div a.collapsed.collapse")
                        .removeClass("collapsed")
                        .addClass("in");
                    isShowedAll = false;
                }
            });
            if (window.event) {
                if (window.event.type != 'blur') {
                    jElement.empty();
                    if (valueUnwrapped) {
                        var tags = valueUnwrapped.split(',');
                        if (tags && tags.length > 0) {
                            var maxVisibleLength = 10;
                            var elementBuild = '<div class="input-group"><ul>';
                            ko.utils.arrayForEach(tags, function (tag, index) {
                                elementBuild += '<li' + (!isShowedAll ? (index <= maxVisibleLength - 1 ? '' : ' class="collapse suggest-tag-item no-transition"') : '') + '><label class="item item tag-color-' + (index + 1) + '">' + tag + '</label></li>';
                            });
                            elementBuild += '</ul></div>';
                            if (tags.length > maxVisibleLength)
                                elementBuild += "<div class='inline'><a data-toggle='collapse' data-target='.suggest-tag-item' class='suggest-tag-item no-transition mb5 " + (!isShowedAll ? 'collapse in' : 'collapsed collapse') + "'>Hiển thị tất cả các tag</a></div>";

                            jElement.append(elementBuild);
                            checkSelectedTags();
                            if (selectedTags) {
                                jElement.find('label').click(function () {
                                    var text = $(this).text();
                                    var isremove = $(this).hasClass('disabled');

                                    if (!isremove)
                                        if (!selectedTags()) selectedTags(text);
                                        else selectedTags(selectedTags() + ',' + text);
                                    else {
                                        var val = selectedTags();

                                        if (val.indexOf(text) == 0)
                                            val = ',' + val;

                                        if (val.lastIndexOf(text) == (val.length - 1))
                                            val = val + ',';

                                        val = val.replace(',' + text, ',');

                                        if (val.indexOf(',') == 0)
                                            val = val.substring(1, val.length - 1);

                                        if (val.lastIndexOf(',') == (val.length - 1))
                                            val = val.substring(0, val.length - 1);

                                        selectedTags(val);
                                    }
                                });
                            }
                        }
                    }
                } else {
                    jElement.find("label").not(".disabled").filter(function () {
                        return $(this).text() == $(event.target).val();
                    }).click();
                    checkSelectedTags();
                }
            }
        }
    };

    ko.bindingHandlers.textMoney = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);
            $(element).text(Globalize.format(valueUnwrapped, 'n'));
        }
    };
    ko.bindingHandlers.textMoneyDefaultSymbol = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);
            var format = Globalize.cultures['vi-VN'].default.numberFormat.currency.pattern[1];
            var symbol = Globalize.cultures['vi-VN'].default.numberFormat.currency.symbol;
            var rs = Globalize.format(valueUnwrapped, format);
            $(element).text(rs + ' ' + symbol);
        }
    };

    ko.bindingHandlers.textQuantity = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);
            $(element).text(Globalize.format(valueUnwrapped, 'n0'));
        }
    };

    ko.bindingHandlers.textMoneyWithSymbol = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);
            if (valueUnwrapped == null) {
                valueUnwrapped = 0;
            }
            $(element).text(Globalize.format(valueUnwrapped, 'c'));
        }
    };

    ko.bindingHandlers.textMoneyWithRound = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {

            var vi = [{ K: 'nghìn ' }, { M: 'triệu ' }, { G: 'tỷ ' }];
            var us = [{ K: 'K ' }, { M: 'M ' }, { G: 'G ' }];
            var currency = Globalize.culture().numberFormat.currency.symbol == '$' ? us : vi;

            var num = ko.utils.unwrapObservable(valueAccessor());
            var sym = Globalize.culture().numberFormat.currency.symbol;
            var money = null;
            if (num >= 1000000000) {
                money = Globalize.format(Math.floor(num / 10000000) / 100, 'c2');
                if (money.indexOf(sym) == 0)
                    money = money + currency[2].G;
                else money = money.replace(sym, currency[2].G + sym);
            }
            else if (num >= 1000000) {
                money = Globalize.format(Math.floor(num / 10000) / 100, 'c2');
                if (money.indexOf(sym) == 0)
                    money = money + currency[1].M;
                else money = money.replace(sym, currency[1].M + sym);
            }
            else if (num >= 1000) {
                money = Globalize.format(Math.floor(num / 10) / 100, 'c2');
                if (money.indexOf(sym) == 0)
                    money = money + currency[0].K;
                else money = money.replace(sym, currency[0].K + sym);
            }
            else {
                money = Globalize.format(num, 'c2');
            }

            $(element).text(money);
        }
    };

    ko.bindingHandlers.datepicker = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var options = allBindingsAccessor().datepickerOptions || {
            };

            options.autoclose = true;
            options.format = Globalize.culture().calendar.patterns.d.replace('MM', 'mm');
            var outputType = options.output;
            $(element).datepicker(options).on('changeDate', function (e) {
                var widget = $(element).data("datepicker");
                if (widget.isDataChanging) return;

                var value = valueAccessor();

                if (ko.isObservable(value)) {
                    widget.isDataChanging = true;
                    if (outputType == 'string') {
                        value(Globalize.format(e.date, 'd'));
                    }
                    else
                        value(e.date);
                    widget.isDataChanging = false;
                }
            });
            if ($(element).attr("data-provide") == "datepicker-inline") {
                $(element).datepicker("show");

            }
        },

        update: function (element, valueAccessor, allBindingsAccessor) {

            var raw = ko.utils.unwrapObservable(valueAccessor());
            var options = allBindingsAccessor().datepickerOptions;
            var outputType = options ? options.output : '';

            var widget = $(element).data("datepicker");
            if (widget) {

                if (!widget.isDataChanging) {
                    if (outputType == 'string') {
                        widget.isDataChanging = true;
                        widget.setDate(Globalize.parseDate(raw).toString());
                        widget.isDataChanging = false;
                    }
                    else {

                        widget.isDataChanging = true;
                        if (typeof raw == 'object')
                            widget.setDate(raw);
                        else widget.setDate(raw);
                        widget.isDataChanging = false;
                    }
                }

            }

        }
    };
    ko.bindingHandlers.timepicker = {

        init: function (element, valueAccessor, allBindingsAccessor) {

            $(element).timepicker().on('changeTime.timepicker', function (e) {

                var value = valueAccessor();
                if (ko.isObservable(value)) {
                    var current = value();
                    if (!current) current = new Date();

                    if (Globalize.culture().calendar.PM[0] == e.time.meridian) {
                        if (parseInt(e.time.hours) == 12)
                            current.setHours(parseInt(e.time.hours));
                        else
                            current.setHours(12 + parseInt(e.time.hours));
                    }
                    else {
                        if (Globalize.culture().calendar.AM[0] == e.time.meridian && e.time.hours == 12)
                            current.setHours(0);
                        else
                            current.setHours(e.time.hours);
                    }
                    current.setMinutes(e.time.minutes);

                    value(current);
                }
            });
        },

        update: function (element, valueAccessor) {
            var widget = $(element).data("timepicker");
            if (widget) {
                var raw = ko.utils.unwrapObservable(valueAccessor());
                var hour = widget.hour;

                if (widget.meridian == Globalize.culture().calendar.PM[0])
                    hour += 12;

                if (widget.meridian == Globalize.culture().calendar.AM[0] && hour == 0)
                    hour = 12;

                var min = widget.minute;

                widget.setTime(raw);
            }
        }
    };

    ko.bindingHandlers.dateshort = {
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            if (value) {
                var date;
                if (value.constructor === Date)
                    date = value;
                else
                    date = new Date(parseInt(value.substr(6)));

                $(element).text(CommonUtils.SetDayFormat(date, 'd'));

            }
            else
                $(element).text(null);
        }
    };

    ko.bindingHandlers.timeshort = {
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            if (value) {
                var date;
                if (value.constructor === Date)
                    date = value;
                else
                    date = new Date(parseInt(value.substr(6)));
                $(element).text(Globalize.format(date, 't'));
            }
            else
                $(element).text(null);
        }
    };

    ko.bindingHandlers.datelong = {
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            if (value) {
                var date;
                if (value.constructor === Date)
                    date = value;
                else
                    date = new Date(value);

                $(element).text(CommonUtils.SetDayFormat(date, 'f'));

            }
            else
                $(element).text(null);
        }
    };
    ko.bindingHandlers.datetimeshort = {
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            $(element).text(CommonUtils.formatDateTimeShort(value));
        }
    };

    ko.bindingHandlers.tinymce = {
        init: function (element, valueAccessor, allBindingsAccessor, context) {

            var modelValue = valueAccessor();
            tinyMCE.baseURL = document.root + 'Scripts/Libs/tiny_mce';

            var options = allBindingsAccessor().tinymceOptions || {
                plugins: "link table advimage colorpicker textcolor code media preview ",
                menubar: false,
                forced_root_block: "",
                // extended_valid_elements: "*[class|id|src|alt|title|onmouseover|onmouseout|onclick|ondblclick|name|for|href|target|value|disabled|width|height|align|cellpadding|cellspacing|border|type|summary|rowspan|rows|rel|colspan|cols|checked|bgcolor|action|accept|abbr|align|valign]",
                fontsize_formats: "8pt 9pt 10pt 11pt 12pt 15pt 20pt 26pt 32pt 38pt 45pt 52pt 60pt",
                valid_elements: '*[*]',
                statusbar: true,
                resize: true,
                //theme_advanced_buttons3_add : "pastetext,pasteword,selectall",
                ////toolbar: "bold italic underline | bullist numlist outdent indent | aligncenter alignright alignjustify | forecolor backcolor | link unlink table image advimage removeformat"
                toolbar: "table advimage media | styleselect | bold italic underline fontsizeselect  | bullist numlist outdent indent | aligncenter alignright alignjustify | forecolor backcolor | link unlink | removeformat  | preview | code"
            };

            var freeTextBindingValue = allBindingsAccessor().freeTextBinding;

            options.setup = function (ed) {
                var ed = ed;
                var iOS = (navigator.userAgent.match(/(iPad|iPhone|iPod)/g) ? true : false);
                if (modelValue()) $(element).val(modelValue());
                ed.on('change', function () {
                    if (ko.isWriteableObservable(modelValue)) {
                        var content = ed.getContent({ format: 'raw' });
                        if (freeTextBindingValue) {
                            freeTextBindingValue(CommonUtils.stripTags(ed.getContent({ format: 'text' }), ''));
                        }
                        if (content !== modelValue()) {
                            modelValue(content);
                        }
                    }
                });

                ed.on('focus', function (e) {

                    $('meta[name="viewport"]').attr('content', 'width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no');
                });
                ed.on('blur', function (e) {
                    $('meta[name="viewport"]').attr('content', 'width=device-width, initial-scale=1.0');
                });

            };

            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).parent().find("span.mceEditor,div.mceEditor").each(function (i, node) {
                    var ed = tinyMCE.get(node.id.replace(/_parent$/, ""));
                    if (ed) {
                        ed.remove();
                    }
                });
            });

            $(element).tinymce(options);

        }
        ,
        update: function (element, valueAccessor, allBindingsAccessor, context) {
            var el = $(element);
            var value = ko.utils.unwrapObservable(valueAccessor());
            var id = el.attr('id');
            if (id !== undefined) {
                var editor = tinyMCE.get(id);
                if (editor && editor !== undefined) {
                    var link = tinyMCE.activeEditor.selection.getNode().innerHTML;
                    var content = editor.getContent({ format: 'raw' })
                    if (content !== value) {
                        $(el).val(value || "");
                    }
                }
                else $(el).val(value || "");
            }
            else $(el).val(value || "");
        }
    };

    ko.bindingHandlers.codemirror = {
        init: function (element, valueAccessor, allBindings) {
            var dataBinding = valueAccessor();
            if (dataBinding()) element.value = ko.utils.unwrapObservable(dataBinding);

            var option = allBindings().codemirroropt || {}
            var saveCallback = option.savecallback;
            var fileName = option.fileName;

            var mode = "haravan";
            if (fileName.endsWith('.scss.liquid') || fileName.endsWith('.scss')
                || fileName.endsWith('.css.liquid') || fileName.endsWith('.css')) {
                mode = "haravancss";
            } else if (fileName.endsWith('.js.liquid') || fileName.endsWith('.js') || fileName.endsWith('.json')) {
                mode = "haravanjs";
            }

            var onChangeFunc = function () {
                dataBinding(editor.getValue());
            };

            var editor = CodeMirror.fromTextArea(element, {
                mode: mode,
                lineNumbers: true,
                lineWrapping: false,
                autofocus: true,
                autoCloseTags: true,
                foldGutter: true,
                indentWithTabs: true,
                tabSize: 2,
                gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
                extraKeys: {
                    "Enter": function () {
                        editor.replaceSelection("\n", "end");
                    },
                    "Ctrl-Space": "autocomplete",
                    "Ctrl-S": function () {
                        if (saveCallback && typeof (saveCallback) == "function") {
                            saveCallback();
                        }
                    }
                }
            });
            editor.on("change", function (cm, change) { onChangeFunc(); });
            editor.getWrapperElement().style.height = $(window).height() - 145 + 'px';
            dataBinding.subscribe(function () {
                if ($(CommonUtils.getBindingRoot()).find($(editor.display.wrapper)).length > 0) {
                    var newValue = ko.utils.unwrapObservable(dataBinding);
                    if (newValue == null)
                        editor.clearHistory();
                    else if (editor && dataBinding() != editor.getValue())
                        editor.setValue(newValue);
                }
            });
        }
    };

    ko.bindingHandlers.moneyMask = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var options = allBindingsAccessor().currencyMaskOptions || {};
            ko.utils.registerEventHandler(element, 'focusout', function () {
                var observable = valueAccessor();
                var val = $(element).val().replace(/[^0-9\,\.]/g, '');

                if (val == "") val = "0";

                var numericVal = Globalize.parseFloat(val);
                numericVal = isNaN(numericVal) ? null : numericVal;

                var dis = Globalize.format(numericVal, 'n')
                numericVal = Globalize.parseFloat(dis);

                observable('[-]');
                observable(numericVal);

                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'n'));
            });

            var isValueUpdate = allBindingsAccessor().valueUpdate === "afterkeydown";

            ko.utils.registerEventHandler(element, 'focus', function () {
                var observable = valueAccessor();
                $(this).val(observable());
            });

            if (isValueUpdate) {
                ko.utils.registerEventHandler(element, 'keyup', function () {

                    var observable = valueAccessor();
                    var val = $(element).val().replace(/[^0-9\,\.]/g, '');

                    if (val == "") val = "0";

                    var numericVal = Globalize.parseFloat(val);
                    numericVal = isNaN(numericVal) ? null : numericVal;

                    var dis = Globalize.format(numericVal, 'n')
                    numericVal = Globalize.parseFloat(dis);

                    observable('[-]');
                    observable(numericVal);

                });
            }
        },

        update: function (element, valueAccessor) {

            if (!$(element).is(":focus")) {
                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'n'));
            }
        }
    };

    ko.bindingHandlers.moneyMaskNoDecimals = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var options = allBindingsAccessor().currencyMaskOptions || {};

            ko.utils.registerEventHandler(element, 'focusout', function () {
                var observable = valueAccessor();
                var val = $(element).val().replace(/[^0-9]/g, '');

                if (val == "") val = "0";

                var numericVal = Globalize.parseFloat(val);
                numericVal = isNaN(numericVal) ? null : numericVal;

                var dis = Globalize.format(numericVal, 'n')
                numericVal = Globalize.parseFloat(dis);

                observable('[-]');
                observable(numericVal);

                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'n'));
            });

            var isValueUpdate = allBindingsAccessor().valueUpdate === "afterkeydown";

            ko.utils.registerEventHandler(element, 'focus', function () {

                var observable = valueAccessor();
                $(this).val(observable());
            });

            if (isValueUpdate) {
                ko.utils.registerEventHandler(element, 'keyup', function () {

                    var observable = valueAccessor();
                    var val = $(element).val().replace(/[^0-9\,\.\-]/g, '');

                    if (val == "") val = "0";

                    var numericVal = Globalize.parseFloat(val);
                    numericVal = isNaN(numericVal) ? null : numericVal;

                    var dis = Globalize.format(numericVal, 'n')
                    numericVal = Globalize.parseFloat(dis);

                    observable('[-]');
                    observable(numericVal);

                });
            }
        },

        update: function (element, valueAccessor) {
            if (!$(element).is(":focus")) {
                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'n'));
            }
        }
    };

    ko.bindingHandlers.moneyMaskWithNegative = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var options = allBindingsAccessor().currencyMaskOptions || {};

            ko.utils.registerEventHandler(element, 'focusout', function () {
                var observable = valueAccessor();
                var val = $(element).val().replace(/[^0-9\,\.\-]/g, '');

                if (val == "") val = "0";

                var numericVal = Globalize.parseFloat(val);
                numericVal = isNaN(numericVal) ? null : numericVal;

                var dis = Globalize.format(numericVal, 'n')
                numericVal = Globalize.parseFloat(dis);

                observable('[-]');
                observable(numericVal);

                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'n'));
            });

            var isValueUpdate = allBindingsAccessor().valueUpdate === "afterkeydown";

            ko.utils.registerEventHandler(element, 'focus', function () {

                var observable = valueAccessor();
                $(this).val(observable());
            });

            if (isValueUpdate) {
                ko.utils.registerEventHandler(element, 'keyup', function () {

                    var observable = valueAccessor();
                    var val = $(element).val().replace(/[^0-9\,\.\-]/g, '');

                    if (val == "") val = "0";

                    var numericVal = Globalize.parseFloat(val);
                    numericVal = isNaN(numericVal) ? null : numericVal;

                    var dis = Globalize.format(numericVal, 'n')
                    numericVal = Globalize.parseFloat(dis);

                    observable('[-]');
                    observable(numericVal);

                });
            }
        },

        update: function (element, valueAccessor) {
            if (!$(element).is(":focus")) {
                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'n'));
            }
        }
    };
    ko.bindingHandlers.moneyMaskWithSymbol = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var options = allBindingsAccessor().currencyMaskOptions || {};

            ko.utils.registerEventHandler(element, 'focusout', function () {
                var observable = valueAccessor();
                var val = $(element).val().replace(/[^0-9\,\.]/g, '');

                if (val == "") val = "0";

                var numericVal = Globalize.parseFloat(val);
                numericVal = isNaN(numericVal) ? null : numericVal;

                var dis = Globalize.format(numericVal, 'c')
                numericVal = Globalize.parseFloat(dis);

                observable('[-]');
                observable(numericVal);

                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'c'));
            });

            var isValueUpdate = allBindingsAccessor().valueUpdate === "afterkeydown";

            ko.utils.registerEventHandler(element, 'focus', function () {

                var observable = valueAccessor();
                $(this).val(observable());
            });


            if (isValueUpdate) {
                ko.utils.registerEventHandler(element, 'keyup', function () {

                    var observable = valueAccessor();
                    var val = $(element).val().replace(/[^0-9\,\.]/g, '');

                    if (val == "") val = "0";

                    var numericVal = Globalize.parseFloat(val);
                    numericVal = isNaN(numericVal) ? null : numericVal;

                    var dis = Globalize.format(numericVal, 'c')
                    numericVal = Globalize.parseFloat(dis);

                    observable('[-]');
                    observable(numericVal);

                });
            }
        },

        update: function (element, valueAccessor) {
            if (!$(element).is(":focus")) {
                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'c'));
            }
        }
    };
    ko.bindingHandlers.numberMask = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var options = allBindingsAccessor().currencyMaskOptions || {};

            ko.utils.registerEventHandler(element, 'focusout', function () {
                var observable = valueAccessor();
                var val = $(element).val().replace(/[^0-9\,\.]/g, '');

                if (val == "") val = "0";

                var numericVal = Globalize.parseFloat(val);
                numericVal = isNaN(numericVal) ? null : numericVal;

                var dis = Globalize.format(numericVal, 'n')
                numericVal = Globalize.parseFloat(dis);

                observable('[-]');
                observable(numericVal);

                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'n'));
            });

            var isValueUpdate = allBindingsAccessor().valueUpdate === "afterkeydown";

            ko.utils.registerEventHandler(element, 'focus', function () {

                var observable = valueAccessor();
                $(this).val(observable());
            });

            if (isValueUpdate) {
                ko.utils.registerEventHandler(element, 'keyup', function () {

                    var observable = valueAccessor();
                    var val = $(element).val().replace(/[^0-9\,\.]/g, '');

                    if (val == "") val = "0";

                    var numericVal = Globalize.parseFloat(val);
                    numericVal = isNaN(numericVal) ? null : numericVal;

                    var dis = Globalize.format(numericVal, 'n')
                    numericVal = Globalize.parseFloat(dis);

                    observable('[-]');
                    observable(numericVal);

                });
            }
        },

        update: function (element, valueAccessor) {
            if (!$(element).is(":focus")) {
                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'n'));
            }
        }
    };
    ko.bindingHandlers.numberMaskNoDecimals = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var options = allBindingsAccessor().currencyMaskOptions || {};

            ko.utils.registerEventHandler(element, 'focusout', function () {
                var observable = valueAccessor();
                var val = $(element).val().replace(/[^0-9]/g, '');

                if (val == "") val = "0";

                var numericVal = Globalize.parseFloat(val);
                numericVal = isNaN(numericVal) ? null : numericVal;

                var dis = Globalize.format(numericVal, 'i')
                numericVal = Globalize.parseFloat(dis);

                observable('[-]');
                observable(numericVal);

                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'i'));
            });

            var isValueUpdate = allBindingsAccessor().valueUpdate === "afterkeydown";

            ko.utils.registerEventHandler(element, 'focus', function () {

                var observable = valueAccessor();
                $(this).val(observable());
            });

            if (isValueUpdate) {
                ko.utils.registerEventHandler(element, 'keyup', function () {
                    var observable = valueAccessor();
                    var val = $(element).val().replace(/[^0-9]/g, '');

                    if (val == "") val = "0";

                    var numericVal = Globalize.parseFloat(val);
                    numericVal = isNaN(numericVal) ? null : numericVal;

                    var dis = Globalize.format(numericVal, 'i')
                    numericVal = Globalize.parseFloat(dis);

                    observable('[-]');
                    observable(numericVal);
                });
            }
        },

        update: function (element, valueAccessor) {
            if (!$(element).is(":focus")) {
                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'i'));
            }
        }
    };
    ko.bindingHandlers.numberMaskWithNegative = {
        init: function (element, valueAccessor, allBindingsAccessor) {

            var options = allBindingsAccessor().currencyMaskOptions || {};

            ko.utils.registerEventHandler(element, 'focusout', function () {
                var observable = valueAccessor();
                var val = $(element).val().replace(/[^0-9\,\.\-]/g, '');

                if (val == "") val = "0";

                var numericVal = Globalize.parseFloat(val);
                numericVal = isNaN(numericVal) ? null : numericVal;

                var dis = Globalize.format(numericVal, 'n')
                numericVal = Globalize.parseFloat(dis);

                observable('[-]');
                observable(numericVal);

                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'n'));
            });

            var isValueUpdate = allBindingsAccessor().valueUpdate === "afterkeydown";

            ko.utils.registerEventHandler(element, 'focus', function () {

                var observable = valueAccessor();
                $(this).val(observable());
            });


            if (isValueUpdate) {
                ko.utils.registerEventHandler(element, 'keyup', function () {
                    var observable = valueAccessor();
                    var val = $(element).val().replace(/[^0-9\,\.\-]/g, '');

                    if (val == "") val = "0";

                    var numericVal = Globalize.parseFloat(val);
                    numericVal = isNaN(numericVal) ? null : numericVal;

                    var dis = Globalize.format(numericVal, 'n')
                    numericVal = Globalize.parseFloat(dis);

                    observable('[-]');
                    observable(numericVal);
                });
            }

        },

        update: function (element, valueAccessor) {
            if (!$(element).is(":focus")) {
                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'n'));
            }
        }
    };
    ko.bindingHandlers.numberMaskWithNegativeNoDecimals = {
        init: function (element, valueAccessor, allBindingsAccessor) {

            var options = allBindingsAccessor().currencyMaskOptions || {};

            ko.utils.registerEventHandler(element, 'focusout', function () {
                var observable = valueAccessor();
                var val = $(element).val().replace(/[^0-9\-]/g, '');

                if (val == "") val = "0";

                var numericVal = Globalize.parseFloat(val);
                numericVal = isNaN(numericVal) ? null : numericVal;

                var dis = Globalize.format(numericVal, 'i')
                numericVal = Globalize.parseFloat(dis);

                observable('[-]');
                observable(numericVal);

                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'i'));
            });

            var isValueUpdate = allBindingsAccessor().valueUpdate === "afterkeydown";

            ko.utils.registerEventHandler(element, 'focus', function () {

                var observable = valueAccessor();
                $(this).val(observable());
            });

            if (isValueUpdate) {
                ko.utils.registerEventHandler(element, 'keyup', function () {
                    var observable = valueAccessor();
                    var val = $(element).val().replace(/[^0-9\-]/g, '');

                    if (val == "") val = "0";

                    var numericVal = Globalize.parseFloat(val);
                    numericVal = isNaN(numericVal) ? null : numericVal;

                    var dis = Globalize.format(numericVal, 'i')
                    numericVal = Globalize.parseFloat(dis);

                    observable('[-]');
                    observable(numericVal);
                });
            }

        },

        update: function (element, valueAccessor) {
            if (!$(element).is(":focus")) {
                var value = ko.utils.unwrapObservable(valueAccessor());
                $(element).val(Globalize.format(value, 'i'));
            }
        }
    };

    ko.bindingHandlers.dynamichtml = {
        init: function () {
            return { controlsDescendantBindings: true };
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            ko.utils.setHtml(element, valueAccessor());
            ko.applyBindingsToDescendants(bindingContext, element);
        }
    };

    ko.bindingHandlers.click2Call = {
        init: function (element, valueAccessor) {
            var hasConfigCallCenter = true;

            var phoneNum = ko.utils.unwrapObservable(valueAccessor());

            if (phoneNum) {
                if (hasConfigCallCenter) {
                    $(element).html('<a onclick="CommonUtils.clickToCall(\'' + phoneNum + '\');" title="Click để gọi"><i class="fa fa-phone-square cursor-pointer mr10"></i></a>');
                } else {
                    $(element).html('<a href="//apps.haravan.com/" target="_blank"><i class="fa fa-phone-square color-gray-icons cursor-pointer mr10"></i></a>');
                }
            }
        }
    };

    ko.bindingHandlers.inputLikeText = {
        init: function (element, valueAccessor, allBindingsAccessor) {
        },
        update: function (element, valueAccessor, allBindingsAccessor) {
            var obs = valueAccessor();

            var options = allBindingsAccessor().inputLikeTextOptions || {
                placeholder: '',
                isItalic: true,
                onfocusout: function () { }
            };

            var placeholder = options.placeholder;
            var onfocusout = options.onfocusout;

            var isTextbox = false;

            var defaultText = obs() ? obs() : placeholder;
            $(element).html(options.isItalic ? '<i>' + defaultText + '</i>' : defaultText);

            $(element).unbind('click');
            $(element).unbind('focusout');
            $(element).unbind('keypress');

            $(element).bind('click', function () {
                if (!isTextbox) {
                    $(element).html('<input type="text" value="' + (obs() ? obs() : '') + '" placeholder="' + placeholder + '" class="inline_block form-control form-control-xs" />');
                    $(element).find('input').focus();
                    $(element).find('input').select();
                    isTextbox = true;
                }
            });
            $(element).bind('focusout', function () {
                enterValue();
            });
            $(element).bind('keypress', function (e) {
                if (e && e.keyCode) {
                    if (e.keyCode == 13) {
                        enterValue();
                    }
                }
            });

            var enterValue = function () {
                if (isTextbox) {
                    var oldValue = obs();
                    var newValue = $(element).find('input').val();
                    if (oldValue != newValue) {
                        obs(newValue);
                        onfocusout(function (result) {
                            var resultText = result ? (obs() ? obs() : placeholder) : (oldValue ? oldValue : placeholder);
                            $(element).html(options.isItalic ? '<i>' + resultText + '</i>' : resultText);

                            isTextbox = false;

                            if (!result) {
                                obs(oldValue ? oldValue : placeholder);
                            }
                        });
                    } else {
                        $(element).html(options.isItalic ? '<i>' + defaultText + '</i>' : defaultText);
                        isTextbox = false;
                    }
                }
            };
        }
    };

    var initMoney = ko.bindingHandlers['moneyMask'].init;

    ko.bindingHandlers['moneyMask'].init = function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

        initMoney(element, valueAccessor, allBindingsAccessor);

        return ko.bindingHandlers['validationCore'].init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
    };

    var initNumber = ko.bindingHandlers['numberMask'].init;

    ko.bindingHandlers['numberMask'].init = function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

        initNumber(element, valueAccessor, allBindingsAccessor);

        return ko.bindingHandlers['validationCore'].init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
    };

    var inittinymce = ko.bindingHandlers['tinymce'].init;

    ko.bindingHandlers['tinymce'].init = function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

        inittinymce(element, valueAccessor, allBindingsAccessor);

        return ko.bindingHandlers['validationCore'].init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
    };

    var initdatepicker = ko.bindingHandlers['datepicker'].init;

    ko.bindingHandlers['datepicker'].init = function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

        initdatepicker(element, valueAccessor, allBindingsAccessor);

        return ko.bindingHandlers['validationCore'].init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
    };

    //ko.validation.rules['areSame'] = {
    //    getValue: function (o) {
    //        return (typeof o === 'function' ? o() : o);
    //    },
    //    validator: function (val, otherField) {
    //        if (val != undefined && otherField != undefined)
    //            return val === this.getValue(otherField);
    //        else return true;
    //    },
    //    message: 'The fields must have the same value'
    //};
})(ko);

