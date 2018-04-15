(function (ko) {
    ko.bindingHandlers.selectPicker = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            if ($(element).is('select')) {
                if (ko.isObservable(valueAccessor())) {
                    if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                        // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                        ko.bindingHandlers.selectedOptions.init(element, valueAccessor, allBindingsAccessor);
                    } else {
                        // regular select and observable so call the default value binding
                        ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor);

                    }
                }
                $(element).addClass('selectpicker').selectpicker();
            }
        },
        update: function (element, valueAccessor, allBindingsAccessor) {
            if ($(element).is('select')) {
                var selectPickerOptions = allBindingsAccessor().selectPickerOptions;
                if (typeof selectPickerOptions !== 'undefined' && selectPickerOptions !== null) {
                    var options = selectPickerOptions.optionsArray,
                        optionsText = selectPickerOptions.optionsText,
                        optionsValue = selectPickerOptions.optionsValue,
                        optionsCaption = selectPickerOptions.optionsCaption,
                        isDisabled = selectPickerOptions.disabledCondition || false,
                        resetOnDisabled = selectPickerOptions.resetOnDisabled || false;
                    if (ko.utils.unwrapObservable(options).length > 0) {
                        // call the default Knockout options binding
                        ko.bindingHandlers.options.update(element, options, allBindingsAccessor);
                    }
                    if (isDisabled && resetOnDisabled) {
                        // the dropdown is disabled and we need to reset it to its first option
                        $(element).selectpicker('val', $(element).children('option:first').val());
                    }
                    $(element).prop('disabled', isDisabled);
                }
                if (ko.isObservable(valueAccessor())) {
                    if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                        // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                        ko.bindingHandlers.selectedOptions.update(element, valueAccessor);
                    } else {
                        // call the default Knockout value binding
                        ko.bindingHandlers.value.update(element, valueAccessor);

                    }
                }

                $(element).selectpicker('refresh');
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


    ko.bindingHandlers.datelongfull = {
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            if (value) {
                var date;
                if (value.constructor === Date)
                    date = value;
                else
                    date = new Date(parseInt(value.substr(6)));

                $(element).text(CommonUtils.SetDateFormat(date, 'f'));

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
                    date = new Date(parseInt(value.substr(6)));

                $(element).text(CommonUtils.SetDayFormat(date, 'f'));

            }
            else
                $(element).text(null);
        }
    };
    ko.bindingHandlers.numericTotal = {
        init: function (element, valueAccessor) {
            $(element).on("keydown", function (event) {
                // Allow: backspace, delete, tab, escape, and enter
                if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||
                    // Allow: Ctrl+A
                    (event.keyCode == 65 && event.ctrlKey === true) ||
                    // Allow: . ,
                    (event.keyCode == 190 || event.keyCode == 110) ||
                    // Allow: home, end, left, right
                    (event.keyCode >= 35 && event.keyCode <= 39)) {
                    // let it happen, don't do anything
                    return;
                }
                else {
                    // Ensure that it is a number and stop the keypress
                    if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                        event.preventDefault();
                    }
                }
            });
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
    ko.bindingHandlers.textProvince = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);

            var provinces = new Common.mvProvince();

            var province = ko.utils.arrayFirst(provinces.Provinces(), function (v) {
                return v.value == valueUnwrapped
            });
            if (province)
                $(element).text(province.name);
            else
                $(element).text("---");
        }
    };
    ko.bindingHandlers.textDistrict = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);

            var provinces = new Common.mvProvince();

            var district = ko.utils.arrayFirst(provinces.District(), function (v) {
                return v.value == valueUnwrapped
            });
            if (district)
                $(element).text(district.name);
            else
                $(element).text("---");
        }
    };

    ko.bindingHandlers.textEmployeeName = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);
            var employee = ko.utils.arrayFirst(common.lstEmployess(), function (v) {
                return v.Id == valueUnwrapped
            });
            if (employee)
                $(element).text(employee.Name);
            else
                $(element).text("---");
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

    ko.bindingHandlers.ko_autocomplete = {
        init: function (element, params) {
            $(element).autocomplete(params());
        },
        update: function (element, params) {
            $(element).autocomplete("option", "source", params().source);
        }
    };
    ko.bindingHandlers.barcode = {
        update: function (element, valueAccessor, allBindings) {
            var opts = allBindings().settings || {};

            var value = ko.utils.unwrapObservable(valueAccessor());
            $(element).barcode(value.trim(), "code128", opts);
        }
    };
    ko.bindingHandlers.renderStatus = {
        update: function (element, valueAccessor, allBindings) {
            debugger
            var typeStatus = allBindings().typeStatus || {};

            var value = ko.utils.unwrapObservable(valueAccessor());
            var html = "";
            var listStt = [];
            switch (typeStatus) {
                case "Order_Output":
                    listStt = statusOrder_Output;
                    break;
                case "Order_Input":
                    listStt = statusOrder_Input;
                    break;
                case "Order_Suppliers":
                    listStt = statusOrder_Suppliers;
                    break;
                case "Order_Channel":
                    listStt = statusOrder_Sale;
                    break;
                case "Order_Braches":
                    listStt = statusOrder_Braches;
                    break;
                case "Order_Switch":
                    listStt = statusOrder_Switch;
                    break;
            }
            var status = ko.utils.arrayFirst(listStt, function (stt) {
                return stt.Key == value;
            });

            if (status) {
                switch (status.Code) {
                    case "Process":
                        html = '<span class="label label-important arrowed">' + status.Value + '</span>';
                        break;
                    case "Done":
                    case "Exported":
                        html = '<span class="label label-success arrowed-in arrowed-in-right">' + status.Value + '</span>';
                        break;
                    case "Shipped":
                        html = '<span class="label label-info arrowed">' + status.Value + '</span>';
                        break;
                    case "Cancel":
                        html = '<span class="label label-gray arrowed">' + status.Value + '</span>';
                        break;
                    case "Refund":
                        html = '<span class="label label-important arrowed">' + status.Value + '</span>';
                        break;
                    case "ShipCancel":
                        html = '<span class="label label-important arrowed">' + status.Value + '</span>';
                        break;
                }
            }
            $(element).html(html);
        }
    };
    ko.bindingHandlers.fileUpload = {
        update: function (element, valueAccessor, allBindingsAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor())
            if (element.files.length && value) {
                var file = element.files[0];
                var callback = allBindingsAccessor().callback

                var formData = new FormData();
                formData.append("file", file);
                formData.append("name", file.name);

                callback(formData);
            }
        }
    };
    ko.bindingHandlers.img = {
        update: function (element, valueAccessor) {
            //grab the value of the parameters, making sure to unwrap anything that could be observable
            var value = ko.utils.unwrapObservable(valueAccessor()),
                src = ko.utils.unwrapObservable(value.src),
                $element = $(element);

            //now set the src attribute to either the bound or the fallback value
            if (src) {
                $element.attr("src", src);
            } else {
                $element.attr("src", CommonUtils.url("/Images/no-img.jpg"));
            }
        },
        init: function (element, valueAccessor) {
            var $element = $(element);

            //hook up error handling that will unwrap and set the fallback value
            $element.error(function () {
                $element.attr("src", CommonUtils.url("/Images/no-img.jpg"));
            });
        },
    }
})(ko);