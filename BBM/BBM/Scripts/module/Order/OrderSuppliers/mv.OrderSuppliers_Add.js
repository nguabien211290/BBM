var Order = Order || {};
Order.mvOrderSuppliersAdd = function () {
    var self = this;
    self.FilterProduct = ko.observable(new Filter.mvFilter_Search_Control('Product_Control'));
    self.FilterProduct().classNameTab('mvOrderSuppliersAdd');
    self.FilterProduct().isCheckProduct.subscribe(function (val) {
        ko.utils.arrayForEach(val, function (o) {
            var hasExits = ko.utils.arrayFirst(self.mOrderSuppliers().Detail(), function (b) {
                return b.ProductId() == o
            })
            if (!hasExits)
                self.GetProduct(o);
            else {
                hasExits.Total(parseInt(hasExits.Total()) + 1);
            }
        })
    });

    self.mOrderSuppliers = ko.observable(new Order.mOrder);
    //self.ComputechSearch = ko.computed(function () {
    //    if (self.Keyword() && self.Keyword().length > 0) {
    //        self.Research_Product();
    //    }
    //}).extend({ throttle: 400 });
    self.Total = ko.computed(function () {
        var sum = 0;
        ko.utils.arrayForEach(self.mOrderSuppliers().Detail(), function (val) {
            sum += parseInt(val.TotalMoney());
        });
        self.mOrderSuppliers().Total(sum);
    });
    /********************************************************************/
    self.GetProduct = function (val) {
        var hasIt = ko.utils.arrayFirst(self.mOrderSuppliers().Detail(), function (o) {
            return val == o.ProductId();
        });
        if (hasIt != null)
            hasIt.Total(hasIt.Total() + 1);
        else {
            CommonUtils.showWait(true);
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

                    newObj.SuppliersName(data.Data.result.product.SuppliersName);
                    newObj.Price(data.Data.result.product.PriceBase);
                    newObj.PriceCompare(data.Data.result.product.PriceCompare);
                    newObj.Stock_Total(data.Data.result.product.Stock_Total);
                    newObj.PriceBase(data.Data.result.product.PriceBase);
                    newObj.soft_Suppliers(data.Data.result.product.soft_Suppliers);
                    var PriceChannels = CommonUtils.MapArray(data.Data.result.product.PriceChannels, Order.mChannel);
                    newObj.PriceChannels(PriceChannels);
                    self.mOrderSuppliers().Detail.push(newObj);

                    self.mOrderSuppliers().Note(self.mOrderSuppliers().Note() + data.Data.note);
                }

            }).always(function () {
                CommonUtils.showWait(false);
            });
        }

    };
    /********************************************************************/
    self.mOrderSuppliers_Print = ko.observableArray();
    self.CreatOrder_PrintSuppliers = function () {

        self.mOrderSuppliers_Print([]);

        var groupby = CommonUtils.Groupbyobject(self.mOrderSuppliers().Detail(), 'SuppliersName', 'SuppliersName');
        var result = ko.observableArray();
        ko.utils.arrayForEach(groupby, function (item) {
            var obj = ko.observable(new Order.mOrderPrint);
            obj().suppliers(new Order.mSuppliers);
            obj().suppliers().Name(item.suppliers);
            obj().suppliers(ko.mapping.fromJS(item.products[0].soft_Suppliers(), {}, new Order.mSuppliers));
            obj().products(item.products);
            result.push(obj());
        });
        self.mOrderSuppliers_Print(CommonUtils.Groupbycol(result, 2));


    };
    self.Print = function (val) {
        var el = "printorder_" + val.suppliers().SuppliersId();
        CommonUtils.Print("printorder_" + val.suppliers().SuppliersId());
    };
    /********************************************************************/
    self.RemoveItem = function (val) {
        self.mOrderSuppliers().Detail.remove(val)
    };

    self.CreatOrderSuppliers = function () {
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Order_Suppliers/CreateOrder_Suppliers"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(self.mOrderSuppliers()),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.mOrderSuppliers(new Order.mOrder)
                self.mOrderSuppliers_Print([]);
            }
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };

    self.Start = function () {
        ko.applyBindings(self, document.getElementById('OrderSuppliersAddViewId'));
        self.mOrderSuppliers(new Order.mOrder);
    };
};