var PrintProductBarcode = PrintProductBarcode || {}
PrintProductBarcode.mvPrintProductBarcode = function () {
    var self = this;
    self.ListVariant = ko.observable();
    self.Product = ko.observable(new PrintProductBarcode.mProduct);
    self.Orders = ko.observableArray();
    self.Config = ko.observable(new PrintProductBarcode.mConfig);

    self.objpurchaseOrder = ko.observable(new PrintProductBarcode.m_purchaseOrder());
    self.listReceived = ko.observableArray();
    self.listNotReceived = ko.observableArray();
    self.isHasSelected = ko.observable(false);
    self.CheckAll = ko.observable(false);
    self.GroupedResults = ko.observableArray();
    self.mBarcodeConfig_Displayname = ko.observable(true);
    self.mBarcodeConfig_Displaycode = ko.observable(true);
    self.mBarcodeConfig_Displayprice = ko.observable(true);
    self.selectedPrintType = ko.observable(3);
    self.MarginLeftPage = ko.observable(0);
    self.MarginLeftItem = ko.observable(0);
    self.WidthItem = ko.observable(1);
    self.HeightItem = ko.observable(25);
    self.WidthFlag = ko.observable(35);
    self.HeightFlag = ko.observable(22);
    self.pageSize = ko.observable(105);
    self.itemInLine = ko.observable(3);
    self.fontSize = ko.observable(9);
    self.paddingItem = ko.observable(2);
    self.paddingTopPage = ko.observable(0);
    self.paddingBotPage = ko.observable(0);
    self.paddingLeftPage = ko.observable(0);
    self.paddingRightPage = ko.observable(0);
    self.marginBottomItem = ko.observable(3);

    self.listStandard = ko.observableArray([
          { id: 1, name: 'A4', width: 210 },//210mm
          { id: 2, name: 'A5', width: 148 },//148mm
          { id: 3, name: 'A6', width: 105 },//105mm
          { id: 4, name: 'A7', width: 74 },//74mm
    ]);
    self.NeedPrint = ko.observableArray();
    self.IsReload = ko.observable(false);
    self.LoadData = function () {
        if (typeof (productId) != typeof (undefined))
            self.LoadDataProduct();
        else if (typeof (orderIds) != typeof (undefined))
            self.LoadDataOrderIds();
        else if (typeof (purchaeseOrderId) != typeof (undefined))
            self.GetPurchaseOrderInfo();
    };
    self.LoadDataProduct = function () {
        CommonUtils.showWait(true, "BarcodeViewId");
        PrintProductBarcode.rPrintProductBarcode().GetProductModel({ productId: productId }, function (result) {
            if (!result.HasError) {
                var data = result.Data;
                ko.mapping.fromJS(data.product, {}, self.Product());
                self.ListVariant(CommonUtils.MapArray(data.product.variants, PrintProductBarcode.mProductVariant));
                ko.utils.arrayForEach(self.ListVariant(), function (variant) {
                    variant.productname(self.Product().title());
                })
                self.BindingConfig(data.config);
            } else {
            }
        }, function () {
            CommonUtils.showWait(false, "BarcodeViewId");
            self.IsReload(true);
        });
    };

    self.BindingConfig = function (config) {
        ko.mapping.fromJS(config, {}, self.Config());
        if (self.Config().barcodeType() <= 0) {
            self.Config().barcodeType(1);
        }
        //if (self.Config().MarginLeftPage() > 0)
        //    self.MarginLeftPage(self.Config().MarginLeftPage());
        //else
        //    self.Config().MarginLeftPage(self.MarginLeftPage());

        //if (self.Config().MarginLeftItem() > 0)
        //    self.MarginLeftItem(self.Config().MarginLeftItem());
        //else
        //    self.Config().MarginLeftItem(self.MarginLeftItem());

        if (self.Config().pageSize() > 0) {
            self.pageSize(self.Config().pageSize());
        }
        else
            self.Config().pageSize(self.pageSize());

        if (self.Config().itemInLine() > 0)
            self.itemInLine(self.Config().itemInLine());
        else
            self.Config().itemInLine(self.itemInLine());

        if (self.Config().fontSize() > 0)
            self.fontSize(self.Config().fontSize());
        else
            self.Config().fontSize(self.fontSize());

        //if (self.Config().paddingItem() > 0)
        //    self.paddingItem(self.Config().paddingItem());
        //else
        //    self.Config().paddingItem(self.paddingItem());

        if (self.Config().HeightFlag() > 0)
            self.HeightFlag(self.Config().HeightFlag());
        else
            self.Config().HeightFlag(self.HeightFlag());

        if (self.Config().WidthFlag() > 0)
            self.WidthFlag(self.Config().WidthFlag());
        else
            self.Config().WidthFlag(self.WidthFlag());

        if (self.Config().WidthItem() > 0)
            self.WidthItem(self.Config().WidthItem());
        else
            self.Config().WidthItem(self.WidthItem());

        if (self.Config().HeightItem() > 0)
            self.HeightItem(self.Config().HeightItem());
        else
            self.Config().HeightItem(self.HeightItem());

        //if (self.Config().paddingTopPage() > 0)
        //    self.paddingTopPage(self.Config().paddingTopPage());
        //else
        //    self.Config().paddingTopPage(self.paddingTopPage());

        //if (self.Config().paddingBotPage() > 0)
        //    self.paddingBotPage(self.Config().paddingBotPage());
        //else
        //    self.Config().paddingBotPage(self.paddingBotPage());

        //if (self.Config().paddingLeftPage() > 0)
        //    self.paddingLeftPage(self.Config().paddingLeftPage());
        //else
        //    self.Config().paddingLeftPage(self.paddingLeftPage());

        //if (self.Config().paddingRightPage() > 0)
        //    self.paddingRightPage(self.Config().paddingRightPage());
        //else
        //    self.Config().paddingRightPage(self.paddingRightPage());

        //if (self.Config().marginBottomItem() > 0)
        //    self.marginBottomItem(self.Config().marginBottomItem());
        //else
        //    self.Config().marginBottomItem(self.marginBottomItem());
        self.MarginLeftPage(self.Config().MarginLeftPage());
        self.MarginLeftItem(self.Config().MarginLeftItem());
        // self.pageSize(self.Config().pageSize());
        // self.itemInLine(self.Config().itemInLine());
        // self.fontSize(self.Config().fontSize());
        self.paddingItem(self.Config().paddingItem());
        // self.HeightFlag(self.Config().HeightFlag());
        // self.WidthFlag(self.Config().WidthFlag());
        // self.WidthItem(self.Config().WidthItem());
        // self.HeightItem(self.Config().HeightItem());
        self.paddingTopPage(self.Config().paddingTopPage());
        self.paddingBotPage(self.Config().paddingBotPage());
        self.paddingLeftPage(self.Config().paddingLeftPage());
        self.paddingRightPage(self.Config().paddingRightPage());
        self.marginBottomItem(self.Config().marginBottomItem());

        self.selectedPrintType(self.Config().printType());
        self.renderStyleStandard();
    };
    self.Config().barcodeType.subscribe(function (val) {
        if (val) {
            if (typeof (productId) != typeof (undefined))
                ko.utils.arrayForEach(self.ListVariant(), function (key) {
                    if (key.barcode() == null)
                        key.isBarcodeValid(false);
                    else {
                        if (key.barcode().length < 12 && self.Config().barcodeType() == 2)
                            key.isBarcodeValid(false);
                        else if (key.barcode().length < 11 && self.Config().barcodeType() == 3)
                            key.isBarcodeValid(false);
                        else
                            key.isBarcodeValid(true);
                    }

                    if (key.isBarcodeValid() == true && key.countPrint() > 0)
                        key.errorPrintBarcode(false);
                    else
                        key.errorPrintBarcode(true);
                });
            else if (typeof (orderIds) != typeof (undefined))
                ko.utils.arrayForEach(self.Orders(), function (key) {
                    if (key.order_number() == null)
                        key.isBarcodeValid(false);
                    else
                        key.isBarcodeValid(true);

                    if (key.isBarcodeValid() == true && key.countPrint() > 0)
                        key.errorPrintBarcode(false);
                    else
                        key.errorPrintBarcode(true);
                });
            self.GenerateBarcodeSetting();
        }
    });
    self.PrintAllVariant = function () {
        window.location.href = "#/standard";
    };

    self.BindHandler = function () {
        ko.bindingHandlers.iChecked = {
            init: function (element, valueAccessor) {
            },
            update: function (element, valueAccessor) {
                var value = ko.unwrap(valueAccessor());
                var total = 0;
                var iserror = false;
                self.NeedPrint.removeAll();
                if (typeof (productId) != typeof (undefined))
                    ko.utils.arrayForEach(self.ListVariant(), function (variant) {
                        if (variant.countPrint() == 0 || variant.errorPrintBarcode() == true) {
                            iserror = true;
                        }
                        else {
                            total += variant.countPrint();
                            var lop = variant.countPrint();
                            for (var x = 0; x < lop; x++) {
                                self.NeedPrint.push(variant);
                            }
                        }
                    });
                else if (typeof (orderIds) != typeof (undefined))
                    ko.utils.arrayForEach(self.Orders(), function (order) {
                        if (order.countPrint() == 0 || order.errorPrintBarcode() == true) {
                            iserror = true;
                        }
                        else {
                            total += order.countPrint();
                            var lop = order.countPrint();
                            for (var x = 0; x < lop; x++) {
                                self.NeedPrint.push(order);
                            }
                        }
                    });
            }
        }
    }
    self.selectedPrintType.subscribe(function (obj) {
        var prStyle = ko.utils.arrayFirst(self.listStandard(), function (val) {
            return val.id == obj;
        });

        self.Config().printType(obj);

        if (prStyle && self.IsReload())
            self.pageSize(prStyle.width);
    });
    self.pageSize.subscribe(function (value) {
        self.Config().pageSize(value);
    });
    self.SavePrintType = function () {
        CommonUtils.showWait_v2(true, "BarcodeViewId");
        PrintProductBarcode.rPrintProductBarcode().SavePrintType(ko.toJSON(self.Config()), function (result) {
            if (result == true) {
                self.Config().printType(self.selectedPrintType());
                toastr.info("Đã lưu khổ in");
            }
            else
                toastr.error("Không thể lưu khổ in, thử lại sau.");
        }, function () {
            CommonUtils.showWait_v2(false, "BarcodeViewId");
        });
    };

    self.GenerateBarcode = function () {
        var tmplst = $('.print_barcode');

        $.each(tmplst, function (key, value) {
            var id = '#' + value.getAttribute('id');
            self.Genbarcode(id);
        });
    };

    self.GenerateBarcodeSetting = function () {
        setTimeout(function () {
            var tmplst = $('.print_barcode');
            $.each(tmplst, function (key, value) {
                var id = '#' + value.getAttribute('id');
                self.Genbarcode(id);
            });
        }, 300);
    };

    self.Genbarcode = function (item) {
        $(item).empty();
        var ni = document.getElementById(item.replace("#", ""));
        var div = document.createElement('div');
        div.setAttribute('style', 'margin:auto');
        var barcode = $(item).attr('val');
        var obj = {
            isShowCode: false,
            code: barcode,
            id: div,
            config: self.Config()
        }
        var result = GenerateHrvBarcode2(obj, self.Config().barcodeType());
        ni.appendChild(div);
        if (!result) {
            $(div).append("<div class='text-danger bold-light'>Barcode không hợp lệ</div>");
        }
    };
    self.MarginLeftPage.subscribe(function (val) {
        if (!self.IsReload())
            return;
        self.Config().MarginLeftPage(parseInt(val));
        self.GenerateBarcodeSetting();
    });
    self.MarginLeftItem.subscribe(function (val) {
        if (!self.IsReload())
            return;
        self.Config().MarginLeftItem(parseInt(val));
        self.GenerateBarcodeSetting();
    });
    self.WidthItem.subscribe(function (val) {
        if (!self.IsReload())
            return;
        self.Config().WidthItem(parseInt(val));
        self.WidthFlag(parseInt(val) * 35);
        self.GenerateBarcodeSetting();
    });
    self.HeightItem.subscribe(function (val) {
        if (!self.IsReload())
            return;
        self.Config().HeightItem(parseInt(val));
        var conver_mm = (parseInt(val) * 0.264583);
        self.HeightFlag(conver_mm + 15);
        self.GenerateBarcodeSetting();
    });
    self.fontSize.subscribe(function (val) {
        if (!self.IsReload())
            return;
        self.Config().fontSize(parseInt(val));
        self.renderStyleStandard();
    });
    self.itemInLine.subscribe(function (val) {
        self.Config().itemInLine(parseInt(val));
        if (!self.IsReload())
            return;
    });
    self.paddingItem.subscribe(function (val) {
        if (!self.IsReload())
            return;
        self.Config().paddingItem(parseInt(val));
        self.renderStyleStandard();
    });
    self.WidthFlag.subscribe(function (val) {
        if (!self.IsReload())
            return;
        self.Config().WidthFlag(parseInt(val));
    });
    self.HeightFlag.subscribe(function (val) {
        if (!self.IsReload())
            return;
        self.Config().HeightFlag(parseInt(val));
    });
    self.paddingTopPage.subscribe(function (val) {
        if (!self.IsReload())
            return;
        self.Config().paddingTopPage(parseInt(val));
    });
    self.paddingBotPage.subscribe(function (val) {
        if (!self.IsReload())
            return;
        self.Config().paddingBotPage(parseInt(val));
    });
    self.paddingLeftPage.subscribe(function (val) {
        if (!self.IsReload())
            return;
        self.Config().paddingLeftPage(parseInt(val));
    });
    self.paddingRightPage.subscribe(function (val) {
        if (!self.IsReload())
            return;
        self.Config().paddingRightPage(parseInt(val));
    });
    self.marginBottomItem.subscribe(function (val) {
        if (!self.IsReload())
            return;
        self.Config().marginBottomItem(parseInt(val));
    });
    self.renderStyleStandard = function () {
        $(".cpage").css({ "width": self.Config().pageSize() + "mm" });
        $(".cpage").css({ "padding-top": self.Config().paddingTopPage() + "mm" });
        $(".cpage").css({ "padding-bottom": self.Config().paddingBotPage() + "mm" });
        $(".cpage").css({ "padding-left": self.Config().paddingLeftPage() + "mm" });
        $(".cpage").css({ "padding-right": self.Config().paddingRightPage() + "mm" });
        $(".barcode-group").css({ "margin-left": self.Config().MarginLeftPage() + "mm" });
        $(".barcode-group").css({ "margin-bottom": self.Config().marginBottomItem() + "mm" });
        $(".print_barcode_img").css({ "margin-left": self.Config().MarginLeftItem() + "mm" });
        if (self.Config().fontSize() > 0)
            $(".barcode-group").css({
                "font-size": self.Config().fontSize() + "px"
            });
        $(".barcode-group").css({
            "padding": self.Config().paddingItem() + "mm"
        });
        $(".review .barcode-group").css({
            "width": self.Config().WidthFlag() + "mm", "height": self.Config().HeightFlag() + "mm"
        });
        $(".barcode-group").find('.header > div').css({ "line-height": self.Config().fontSize() + "px" });
    }
    self.fixPrintCode = function () {
        setTimeout(function () {
            $('.print_barcode_img').each(function () {
                var width = 0;
                if ($(this).find('.print_barcode > div').width() > $(this).width()) {
                    $(this).children().children().children().each(function (i, v) {
                        if (parseFloat($(this).css("border-left-width").replace("px", "")) > 0) {
                            $(this).css('border-left-width', parseFloat($(this).css("border-left-width").replace("px", "")) / 2);
                        } else {
                            $(this).css('width', parseFloat($(this).css("width").replace("px", "")) / 2);
                        }
                        width += parseFloat($(this).css("width").replace("px", "")) / 2;
                    });
                    $(this).find('.print_barcode > div').css('width', width * 2);
                }

            });
        }, 300)
    };
    self.afterRenderPrintReviewPage = function () {
        self.renderStyleStandard();
    };
    /********************PurchasesOrder***********************/
    self.PrintIt = function () {
        self.NeedPrint([]);
        ko.utils.arrayForEach(self.GroupedResults(), function (item) {
            if (item.Selected()) {
                ko.utils.arrayForEach(item.Data(), function (xitem) {
                    if (xitem.barcode() && xitem.is_barcode_valid()) {
                        for (var i = 0; i < xitem.quantity() ; i++)
                            self.NeedPrint.push(xitem);
                    }
                })
            }
        });
        if (self.NeedPrint().length > 0) {
            window.location.href = "#/standard";
        }
        else {
            toastr.error("Không có thông tin barcode hợp lệ");
        }
    };
    self.GroupReceive = function () {
        var data = self.listReceived();
        var returnData = [];

        if (self.listNotReceived().length > 0) {
            var group = new PrintProductBarcode.m_po_detail_receive_group();
            group.ReceiveId(999999);
            group.ReceiveDate();
            group.IsNotReceived(true);
            ko.utils.arrayFirst(self.listNotReceived(), function (item) {
                group.Data.push(item);
            })
            returnData.push(group);
        }
        if (data == null || data == undefined)
            return;

        var rs = ko.utils.arrayFilter(data, function (item) {
            return item.receive_id() != null;
        });

        //group

        ko.utils.arrayForEach(rs, function (item) {
            var existgroup = ko.utils.arrayFirst(returnData, function (item_in_return) {
                return item_in_return.ReceiveId() == item.receive_id();
            });

            if (existgroup != null)
                existgroup.Data.push(item);
            else {
                var group = new PrintProductBarcode.m_po_detail_receive_group();
                group.ReceiveId(item.receive_id());
                group.ReceiveDate(item.receive_date());

                group.Data.push(item);
                returnData.push(group);
            }
        });

        self.GroupedResults(returnData);
    };
    self.SelectAll = ko.computed({
        read: function () {
            var cnt = 0;

            var list = self.GroupedResults();

            $("#listReceivedOrder").val("");

            if (list == undefined || list.length == 0) {
                //$(".dvListAction").hide();
                return false;
            }

            for (var i = 0; i < list.length; i++)
                if (list[i].Selected()) {
                    if ($("#listReceivedOrder").val() == "") {
                        $("#listReceivedOrder").val(list[i].ReceiveId());
                    }
                    else {
                        $("#listReceivedOrder").val($("#listReceivedOrder").val() + "," + list[i].ReceiveId());
                    }
                    cnt += 1;
                }


            if (cnt > 0) {
                self.isHasSelected(true);
            }
            else {
                self.isHasSelected(false);
            }

            if (cnt == (list.length)) {
                return true;
            }
            else
                return false;
        },
        write: function (value) {
            ko.utils.arrayForEach(self.GroupedResults(), function (item) {
                item.Selected(value);
            });
        }
    });
    self.GetPurchaseOrderInfo = function (POId) {
        CommonUtils.showWait_v2(true);
        PrintProductBarcode.rPrintProductBarcode().GetPurchaseOrderInfo({ 'id': purchaeseOrderId }, function (result) {
            if (!result.HasError) {
                if (result.Data.purchase_order != null) {
                    self.objpurchaseOrder(ko.mapping.fromJS(result.Data.purchase_order));
                    if (self.objpurchaseOrder()) {
                        self.listNotReceived(CommonUtils.MapArray(self.objpurchaseOrder().line_item.not_received_items(), PrintProductBarcode.m_receivedOrder));
                        self.listReceived(CommonUtils.MapArray(self.objpurchaseOrder().line_item.received_items(), PrintProductBarcode.m_receivedOrder));

                        self.GroupReceive();
                        self.BindingConfig(result.Data.config);
                    }
                }
            }
            else {
                toastr.error(result.ErrorMessage);
            }
        }, function () {
            CommonUtils.showWait_v2(false);
            self.IsReload(true);
        });
    };
    self.SetConfigDefault = function () {
        self.selectedPrintType(3);
        self.MarginLeftPage(0);
        self.MarginLeftItem(0);
        self.WidthItem(1);
        self.HeightItem(25);
        self.WidthFlag(35);
        self.HeightFlag(22);
        self.pageSize(105);
        self.itemInLine(3);
        self.fontSize(9);
        self.paddingItem(2);
        self.paddingTopPage(0);
        self.paddingBotPage(0);
        self.paddingLeftPage(0);
        self.paddingRightPage(0);
        self.marginBottomItem(3);
    };
    self.VisibleConfigBarCode = function () {
        if ($(".detail").hasClass('active')) {
            $(".box-recommend").find('.box-detail').slideUp();
            $(".detail").removeClass('active')
        }
        else {
            $(".box-recommend").find('.box-detail').slideDown();
            $(".detail").addClass('active');
        }
    }
    self.PrintOnePur = function (obj) {
        self.NeedPrint([]);
        for (var i = 0; i < obj.quantity() ; i++)
            self.NeedPrint.push(obj);
        if (self.NeedPrint().length > 0) {
            window.location.href = "#/standard";
        }
    };

    self.listIdProduct = ko.observableArray();
    self.LoadProducts = function () {
        if (self.listIdProduct.length > 0) {
            CommonUtils.showWait(true,"BarcodeViewId");
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/Branches/GetBranches"),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: ko.toJSON(self.listIdProduct())
            }).done(function (data) {
                if (data == null)
                    return
                if (!data.isError) {

                } else
                    CommonUtils.notify("Thông báo", data.messaging, 'error');

            }).always(function () {
                CommonUtils.showWait(false, "BarcodeViewId");
            });
        }
    };

    self.Start = function () {
        ko.applyBindings(self, document.getElementById('BarcodeViewId'));
        self.LoadProducts();
    };
};

