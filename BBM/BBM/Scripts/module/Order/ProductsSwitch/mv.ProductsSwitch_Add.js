var Order = Order || {};
Order.mvProductsSwitchAdd = function () {
    var self = this;

    self.FilterProduct = ko.observable(new Filter.mvFilter_Search_Control('Product_Control'));
    self.FilterProduct().classNameTab('mvProductsSwitchAdd');
    self.FilterProduct().isCheckProduct.subscribe(function (val) {
        ko.utils.arrayForEach(val, function (o) {
            var hasExits = ko.utils.arrayFirst(self.mProductsSwitch().Detail(), function (b) {
                return b.ProductId() == o
            })
            if (!hasExits)
                self.GetProduct(o);
            else {
                hasExits.Total(parseInt(hasExits.Total()) + 1);
            }
        })
    });



    self.lstOrder_Output = ko.observableArray();
    self.mProductsSwitch = ko.observable(new Order.mOrder);
    self.Total = ko.computed(function () {
        var sum = 0;
        ko.utils.arrayForEach(self.mProductsSwitch().Detail(), function (val) {
            sum += parseInt(val.Total());
        });
        self.mProductsSwitch().Total(sum);
    });
    self.CreatProductSwitch = function () {
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/ProductsSwitch/CreateProductsSwitch"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(self.mProductsSwitch()),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.mProductsSwitch(new Order.mOrder)
            }
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.RemoveItem = function (val) {
        self.mProductsSwitch().Detail.remove(val)
    };
    self.GetProduct = function (val) {
        var hasIt = ko.utils.arrayFirst(self.mProductsSwitch().Detail(), function (o) {
            return val == o.ProductId();
        });
        if (hasIt != null)
            hasIt.Total(hasIt.Total() + 1);
        else {
            CommonUtils.showWait(true);
            $.ajax({
                type: "GET",
                url: CommonUtils.url("/Product/GetProductbyId"),
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
                    self.mProductsSwitch().Detail.push(newObj);
                }

            }).always(function () {
                CommonUtils.showWait(false);
            });
        }

    };
    self.Start = function () {
        ko.applyBindings(self, document.getElementById('ProductsSwitchAddViewId'));
        self.mProductsSwitch(new Order.mOrder);
    };
};