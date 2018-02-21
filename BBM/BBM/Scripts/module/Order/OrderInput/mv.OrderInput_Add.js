var Order = Order || {};
Order.mvOrderInputAdd = function (Products, OrderSuppliersId) {
    var self = this;

    self.mOrderInput = ko.observable(new Order.mOrder);
    if (Products != undefined) {
        if (Products != null && Products.length > 0) {
            var data = JSON.parse(Products);
            ko.utils.arrayForEach(data, function (o) {
                self.mOrderInput().Detail.push(ko.mapping.fromJS(o.product, {}, new Order.mOrderDetail));
            });
        }
    };

    self.NameChannels = ko.observableArray();
    self.FilterProduct = ko.observable(new Filter.mvFilter_Search_Control('Product_Control'));

    self.FilterProduct().classNameTab('mvOrderInputAdd');
    self.FilterProduct().isCheckProduct.subscribe(function (val) {
        ko.utils.arrayForEach(val, function (o) {
            var hasExits = ko.utils.arrayFirst(self.mOrderInput().Detail(), function (b) {
                return b.ProductId() == o
            })
            if (!hasExits)
                self.GetProduct(o);
            else {
                hasExits.Total(parseInt(hasExits.Total()) + 1);
            }
        })
    });



    self.Total = ko.computed(function () {
        var sum = 0;
        ko.utils.arrayForEach(self.mOrderInput().Detail(), function (val) {
            sum += val.TotalMoney();
        });
        self.mOrderInput().Total(sum);
    });

    self.GetProduct = function (val) {
        var hasIt = ko.utils.arrayFirst(self.mOrderInput().Detail(), function (o) {
            return val == o.ProductId();
        });
        if (hasIt != null)
            hasIt.Total(hasIt.Total() + 1);
        else {
            CommonUtils.showWait(true, self.idelement());
            $.ajax({
                type: "GET",
                url: CommonUtils.url("/Partial/GetProductbyId"),
                cache: false,
                data: { productId: val },
            }).done(function (data) {
                if (data.isError) {
                    CommonUtils.notify("Thông báo", data.Data.result, 'error');
                }
                else {

                    var newObj = new Order.mOrderDetail();
                    newObj.ProductName(data.Data.result.product.tensp);
                    newObj.ProductId(data.Data.result.product.id);
                    newObj.Code(data.Data.result.product.masp);
                    if (data.Data.result.product.SuppliersName)
                        newObj.SuppliersName(data.Data.result.product.SuppliersName);
                    newObj.Price(data.Data.result.product.PriceBase);
                    newObj.PriceCompare(data.Data.result.product.PriceCompare);
                    newObj.Stock_Total(data.Data.result.product.Stock_Total);
                    newObj.PriceBase(data.Data.result.product.PriceBase);
                    var PriceChannels = CommonUtils.MapArray(data.Data.result.product.PriceChannels, Order.mChannel);
                    newObj.PriceChannels(PriceChannels);
                    self.mOrderInput().Detail.push(newObj);
                }

            }).always(function () {
                CommonUtils.showWait(false, self.idelement());
            });
        }

    };

    self.CreatOrderInput = function (isDone) {
        CommonUtils.showWait(true, self.idelement());
        var objectData;
        if (OrderSuppliersId)
            objectData = ko.toJSON({ model: self.mOrderInput(), isDone: isDone, OrderSuppliersId: OrderSuppliersId });
        else
            objectData = ko.toJSON({ model: self.mOrderInput(), isDone: isDone });
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Order_Input/CreateOrder_Inside"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: objectData
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.mOrderInput(new Order.mOrder)
            }
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            OrderSuppliersId = undefined;
            CommonUtils.showWait(false, self.idelement());
        });
    };
    self.RemoveItem = function (val) {
        self.mOrderInput().Detail.remove(val)
    };
    self.mOrderInput().Detail.subscribe(function (val) {
        var lstNameChannel = [];
        ko.utils.arrayForEach(val, function (o) {
            ko.utils.arrayForEach(o.PriceChannels(), function (i) {
                lstNameChannel.push(i.Channel());
            })
        });
        var tmp = [];
        for (var i = 0; i < lstNameChannel.length; i++) {
            if (tmp.indexOf(lstNameChannel[i]) == -1) {
                tmp.push(lstNameChannel[i]);
            }
        }

        self.NameChannels(tmp);
    });
    self.idelement = ko.observable();
    self.Start = function () {
        self.idelement("OrderInputAddViewId");
        ko.applyBindings(self, document.getElementById("OrderInputAddViewId"));

    };
};