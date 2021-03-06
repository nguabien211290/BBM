﻿var Order = Order || {};
Order.mvOrderSale = function (OrderId) {
    var self = this;
    debugger
    self.isChannelOnline = ko.observable(false);
    self.OrderCloneId = ko.observable(OrderCloneId);
    self.OrderSaleId = ko.observable(OrderId);
    self.OrderCloneId.subscribe(function (val) {
        if (val.length > 0)
            self.OrderSaleId(val);
    });
    self.StatusOrder = ko.observable();
    self.StatusOrder.subscribe(function (val) {
        if (self.mOrderSale())
            self.mOrderSale().Status(val);
    });
    self.KeywordSearch = ko.observable();
    self.SearchType = ko.observable("Code");
    self.ListProductSearch = ko.observableArray();
    self.valueDiscountForMember = ko.observable(valueDiscountFormember);
    self.lstEmployShip = ko.observableArray();
    self.lstOrder_Output = ko.observableArray();
    self.DisscountType = ko.observable('Money');
    self.InputDisscount = ko.observable(0);
    CommonUtils.loadLstLoadEmployes(function (data) {
        self.lstEmployShip(CommonUtils.MapArray(data, Order.mCustomer));
    });
    self.DisabledStatus = ko.observable(false);
    self.SetValueDisscount = function (value, type) {
        switch (type) {
            case 'Money':
                self.mOrderSale().DisscountValue(parseFloat(value));
                break;
            case 'Percent':
                var per = (self.mOrderSale().TotalMoney() * parseFloat(value)) / 100;
                self.mOrderSale().DisscountValue(per);
                break;
            case 'Code':
                value = value.toString();
                //self.mOrderSale().DisscountCode(undefined);
                //self.mOrderSale().DisscountCode(value);
                if (value.length <= 1) {
                    self.mOrderSale().DisscountValue("");
                } else {
                    CommonUtils.showWait(true);
                    $.ajax({
                        type: "POST",
                        url: CommonUtils.url("/OrderChannel/GetDisscount"),
                        cache: false,
                        dataType: 'json',
                        data: { code: value },
                    }).done(function (data) {
                        if (data == null)
                            return
                        if (!data.isError) {
                            self.mOrderSale().DisscountType(data.Data.type);
                            self.mOrderSale().DisscountValue(data.Data.value);
                        } else
                            CommonUtils.notify("Thông báo", data.messaging, 'error');
                    }).always(function () {
                        CommonUtils.showWait(false);
                    });
                }
                break;
        }
    }
    self.InputDisscount.subscribe(function (value) {
        self.SetValueDisscount(value, self.DisscountType());
    });
    self.DisscountType.subscribe(function (val) {
        self.SetValueDisscount(self.InputDisscount(), val);
    });
    self.mOrderSale = ko.observable(new Order.mOrder);
    self.EventChangePhoneCustomer = function (val) {
        self.Research_Customer(self.mOrderSale().Customer().Phone());
    };
    self.tmpItemBind = ko.observable();

    self.KeywordSearchFunction = ko.computed(function () {
        if (self.KeywordSearch() && self.KeywordSearch().length > 0) {
            self.Research_Product();
        }
    }).extend({ throttle: 600 });

    self.Total = ko.computed(function () {
        var sum = 0;
        ko.utils.arrayForEach(self.mOrderSale().Detail(), function (val) {
            sum += parseFloat(val.Total());
        });
        self.mOrderSale().Total(sum);
    });

    self.TotalMoenySumary = ko.computed(function () {
        var sum = 0;
        ko.utils.arrayForEach(self.mOrderSale().Detail(), function (val) {
            sum += parseFloat(val.TotalMoney());
        });
        self.mOrderSale().TotalMoney(sum);
        self.SetValueDisscount(self.InputDisscount(), self.DisscountType());

        var hasDiscount = ko.utils.arrayFirst(self.mOrderSale().Detail(), function (pro) {
            return pro.isDiscountForMember();
        })
        if (hasDiscount)
            self.mOrderSale().Customer().MarkTmp(parseInt(parseInt(self.mOrderSale().Customer().Mark()) - 1000 + (self.CustommerMoneyTake() / 1000)));
        else
            self.mOrderSale().Customer().MarkTmp(parseInt(parseInt(self.mOrderSale().Customer().Mark()) + (sum / 1000)));
    });

    //self.DisscountbyMark = function () {
    //    self.DisscountType('Percent');
    //    self.InputDisscount(10);
    //};

    self.IsDisscountbyMark = ko.observable(false);
    self.IsDisscountbyMark.subscribe(function (o) {
        if (!o) {
            ko.utils.arrayForEach(self.mOrderSale().Detail(), function (detail) {
                detail.isDiscountForMember(false);
            });
        }
    });

    self.CustommerMoneyTake = ko.computed(function () {
        var sum = (self.mOrderSale().TotalMoney() + self.mOrderSale().TotalOrther() + (self.mOrderSale().phithuho() ? self.mOrderSale().phithuho() : 0));

        if (self.DisscountType() == 'Code') {
            if (self.mOrderSale().DisscountType() == 1)
                return sum - self.mOrderSale().DisscountValue();
            if (self.mOrderSale().DisscountType() == 2) {
                var disscount = ((self.mOrderSale().TotalMoney() * self.mOrderSale().DisscountValue()) / 100);
                return sum - disscount;
            }
        } else {
            return sum - self.mOrderSale().DisscountValue();
        }
        return 0;
    });
    self.CustommerofMoney = ko.observable(0);
    self.CustommerMoneyGive = ko.computed(function () {
        return self.CustommerofMoney() - self.CustommerMoneyTake();
    });
    self.Customer = ko.observable();
    self.Research_Customer = function (val) {
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Customer/Research"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({ keyword: val }),
        }).done(function (data) {
            if (data == null) {
                return;
            }
            if (!data.isError && data.Data != null) {
                self.mOrderSale().Customer(ko.mapping.fromJS(data.Data, { 'ignore': ['DistrictName', 'ProvinceName'] }, new Order.mCustomer));
            } else {
                var phone = self.mOrderSale().Customer().Phone();
                self.mOrderSale().Customer(new Order.mCustomer);
                self.mOrderSale().Customer().Phone(phone);
            }
        }).always(function () {
        });
    };

    self.selectedProductSearch = ko.observable();
    self.frist_selectedProductSearch = ko.observable(false);
    self.selectedProductSearch.subscribe(function (obj) {
        if (self.frist_selectedProductSearch()) {
            self.GetProduct(obj);
        }
    });
    self.EventChangeKeyword = function () {
    }
    self.Research_Product = function () {
        if (self.KeywordSearch().length > 0) {
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/Product/Research"),
                cache: false,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: ko.toJSON({ keyword: self.KeywordSearch(), searchType: self.SearchType() }),
            }).done(function (data) {
                if (data == null)
                    return
                if (!data.isError) {
                    if (data.Data.length == 1) {
                        var obj = ko.observable(ko.mapping.fromJS(data.Data[0], {}, new Order.mProduct));
                        self.GetProduct(obj());
                    }
                    else {
                        self.ListProductSearch(CommonUtils.MapArray(data.Data, Order.mProduct));
                        $("#edit_user_details").focus();

                    }
                } else
                    CommonUtils.notify("Thông báo", data.messaging, 'error');
            }).always(function () {z
            });
        }

    }

    self.GetProduct = function (val) {
        self.ListProductSearch([]);
        var hasIt = ko.utils.arrayFirst(self.mOrderSale().Detail(), function (o) {
            return val.id() == o.ProductId();
        });
        if (hasIt != null)
        {    hasIt.Total(hasIt.Total() + 1);
			self.KeywordSearch('');
		}
        else {
            CommonUtils.showWait(true);
            $.ajax({
                type: "GET",
                url: CommonUtils.url("/Product/GetProductbyIdForOrder"),
                cache: false,
                data: { productId: val.id() },
            }).done(function (data) {
                if (data.isError) {
                    CommonUtils.notify("Thông báo", data.Data.result, 'error');
                }
                else {
                    var newObj = new Order.mOrderSaleDetail();
                    newObj.ProductName(data.Data.result.product.tensp);
                    newObj.ProductId(data.Data.result.product.id);
                    newObj.Code(data.Data.result.product.masp);
                    newObj.Price(data.Data.result.product.PriceChannel);
                    newObj.PriceFix(data.Data.result.product.Price);
                    newObj.Keyword(self.KeywordSearch());

                    newObj.isFristLoad(true);
                    self.mOrderSale().Detail.push(newObj);
                    self.KeywordSearch('');
                    self.ListProductSearch([]);
                }

            }).always(function () {
                CommonUtils.showWait(false);
            });
        }

    };
    self.RemoveItem = function (val) {
        self.mOrderSale().Detail.remove(val)
    };
    self.AddNewObjItemCart = function () {
        var item = new Order.mOrderSaleDetail();
        self.mOrderSale().Detail.push(item);
    };

    self.GetInfoOrder = function () {
        if (self.OrderSaleId() && self.OrderSaleId() > 0) {
            CommonUtils.showWait(true);
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/OrderChannel/GetInfoOrder"),
                cache: false,
                data: { Id: self.OrderSaleId() }
            }).done(function (data) {
                if (data == null)
                    return
                if (!data.isError) {
                    self.mOrderSale(ko.mapping.fromJS(data.Data, { 'ignore': ['Detail', 'Customer'] }, new Order.mOrder));
                    self.StatusOrder(data.Data.Status);
                    if (self.mOrderSale().Status() == 3 || self.mOrderSale().Status() == 4)
                        self.DisabledStatus(true);

                    if (IsClone == 'True')
                        self.mOrderSale().Id(0);
                    ko.utils.arrayForEach(data.Data.Detail, function (pro) {
                        var newObj = new Order.mOrderSaleDetail();
                        newObj.Id(pro.Id);
                        newObj.ProductName(pro.Product.tensp);
                        newObj.ProductId(pro.ProductId);
                        newObj.Code(pro.Product.masp);
                        newObj.Price(pro.Price);
                        //newObj.PriceFix(pro.Price);
                        newObj.Total(pro.Total);
                        newObj.Discount(pro.Discount);
                        self.mOrderSale().Detail.push(newObj);
                        if (pro.Discount > 0)
                            self.IsDisscountbyMark(true);
                    });
                    self.mOrderSale().Customer(ko.mapping.fromJS(data.Data.Customer, { 'ignore': ['DistrictName', 'ProvinceName'] }, new Order.mCustomer));
                    self.CustommerofMoney(self.CustommerMoneyTake());
                    self.InputDisscount(data.Data.DisscountValue);
                } else
                    CommonUtils.notify("Thông báo", data.messaging, 'error');
            }).always(function () {
                self.DisscountType('Money');
                //self.InputDisscount(0);
                CommonUtils.showWait(false);
            });
        }
        else {
            CommonUtils.showWait(true);
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/OrderChannel/RenewInfoOrder"),
                cache: false,
            }).done(function (data) {
                if (data == null)
                    return
                if (!data.isError) {
                    self.mOrderSale(new Order.mOrder);
                    self.mOrderSale().Code(data.Data.CodeOrder);
                    self.mOrderSale().ChannelName(data.Data.ChannelName);
                    self.mOrderSale().EmployeeCreate(data.Data.EmployeeNameCreate);
                    self.mOrderSale().isChannelOnline(data.Data.isChannelOnline);
                    self.CustommerofMoney(0);
                } else
                    CommonUtils.notify("Thông báo", data.messaging, 'error');
            }).always(function () {
                self.DisscountType('Money');
                self.InputDisscount(0);
                CommonUtils.showWait(false);
            });
        }

    };
    self.CloneOrder = function () {
        CommonUtils.confirm("Thông báo", "Đơn hàng này sẽ được hủy trước khi đặt lại ?", function () {
            self.mOrderSale().Status(4);
            self.DisabledStatus(true);
            CommonUtils.addTabDynamic('Bán hàng', '/OrderChannel/RenderViewOrder', '#contentX', true, { OrderId: self.mOrderSale().Id(), IsClone: true })
        });
    }
    self.mPrintModel = ko.observable();
    self.CustommerMoneyTake_Print = ko.observable();
    self.CustommerofMoney_Print = ko.observable();
    self.CustommerMoneyGive_Print = ko.observable();
    self.Start = function () {
        var el = IsClone == 'False' ? 'OrderSaleViewId-' + self.OrderSaleId() : 'OrderSaleViewId-0';
        self.OrderCloneId(OrderCloneId);

        ko.applyBindings(self, document.getElementById(el));
        self.GetInfoOrder();
    };

    self.CreatOrderSale = function (isDone) {
        self.mOrderSale().Customer().Mark(self.mOrderSale().Customer().MarkTmp());
        CommonUtils.showWait(true);
        self.mOrderSale().Total(self.CustommerMoneyTake());
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/OrderChannel/CreatOrderSale"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({ model: self.mOrderSale(), isDone: isDone }),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.mOrderSale().Code(data.Data.Code);
                if (data.Data.isPrint)
                    CommonUtils.Print("printordersales_" + OrderId);
                debugger
                if (OrderId == 0)
                    self.OrderSaleId(OrderId);
                self.GetInfoOrder();
            }
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };

    self.UpdateOrderSale = function () {
        if (self.mOrderSale().Id() > 0) {
            CommonUtils.showWait(true);
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/OrderChannel/UpdateStatus_Order_Sale"),
                cache: false,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: ko.toJSON({ model: self.mOrderSale() }),
            }).done(function (data) {
                if (data == null)
                    return

                //self.mOrderSale().Status(self.StatusOrder());
                //if (self.mOrderSale().Status() == 3 || self.mOrderSale().Status() == 4)
                //    self.DisabledStatus(true);

                CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');

                self.GetInfoOrder();

            }).always(function () {
                CommonUtils.showWait(false);
            });
        }
    };


    self.Print_Order_Sale = function () {
        if (self.mOrderSale().Id() > 0) {
            CommonUtils.showWait(true);
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/OrderChannel/Print_Order_Sale"),
                cache: false,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: ko.toJSON({ model: self.mOrderSale() }),
            }).done(function (data) {
                if (data == null)
                    return
                if (!data.isError) {
                    CommonUtils.Print("printordersales_" + OrderId);
                }
                CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
            }).always(function () {
                CommonUtils.showWait(false);
            });
        }
    };
};