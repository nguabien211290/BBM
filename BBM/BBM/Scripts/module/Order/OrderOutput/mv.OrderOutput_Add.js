var Order = Order || {};
Order.mvOrderOutputAdd = function (Products) {
    var self = this;

    self.lstOrder_Output = ko.observableArray();
    self.mOrderOutput = ko.observable(new Order.mOrder);
    if (Products != undefined) {
        if (Products != null && Products.length > 0) {
            var data = JSON.parse(Products);

            self.mOrderOutput().OrderFromId(data[0].OrderFromId);
            ko.utils.arrayForEach(data, function (result) {
                var newObj = new Order.mOrderDetail();
               

                newObj.ProductName(result.product.tensp);
                newObj.ProductId(result.product.id);
                newObj.Code(result.product.masp);
                newObj.Total(result.Total);
                
                if (result.product.SuppliersName)
                    newObj.SuppliersName(result.product.SuppliersName);
                if (result.product_stock)
                    newObj.Stock_Total(result.product_stock.Stock_Total);
                if (result.product_stocks)
                    newObj.Stock_Totals(result.product_stocks);
                self.mOrderOutput().Detail.push(newObj);
            });
        }
    };

    self.LstBranches = ko.observableArray();

    self.FilterProduct = ko.observable(new Filter.mvFilter_Search_Control('Product_Control'));
    self.FilterProduct().classNameTab('mvOrderOutputAdd');
    self.FilterProduct().isCheckProduct.subscribe(function (val) {
        ko.utils.arrayForEach(val, function (o) {
            var hasExits = ko.utils.arrayFirst(self.mOrderOutput().Detail(), function (b) {
                return b.ProductId() == o
            })
            if (!hasExits)
                self.GetProduct(o);
            else {
                hasExits.Total(parseInt(hasExits.Total()) + 1);
            }
        });
    });

    self.Total = ko.computed(function () {
        var sum = 0;
        ko.utils.arrayForEach(self.mOrderOutput().Detail(), function (val) {
            sum += parseInt(val.Total());
        });
        self.mOrderOutput().Total(sum);
    });
    self.GetProduct = function (val) {
        var hasIt = ko.utils.arrayFirst(self.mOrderOutput().Detail(), function (o) {
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
                    if (data.Data.result.product.SuppliersName)
                        newObj.SuppliersName(data.Data.result.product.SuppliersName);
                    if (data.Data.result.product_stock)
                        newObj.Stock_Total(data.Data.result.product_stock.Stock_Total);
                    if (data.Data.result.product_stocks)
                        newObj.Stock_Totals(data.Data.result.product_stocks);
                    self.mOrderOutput().Detail.push(newObj);
                }

            }).always(function () {
                CommonUtils.showWait(false);
            });
        }

    };
    self.CreatOrderOutput = function () {
        CommonUtils.showWait(true);
        self.mOrderOutput().Id_From(UserBranchId);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Order_Output/CreateOrder_Output"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(self.mOrderOutput()),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.PrintModel(self.mOrderOutput());
                CommonUtils.Print("printorderoutput");
                self.mOrderOutput(new Order.mOrder);
            }
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    CommonUtils.LoadLstBranches(function (data) {
        self.LstBranches(data);
    });
    self.RemoveItem = function (val) {
        self.mOrderOutput().Detail.remove(val)
    };
    self.Calculate_TotalSumAdvSalebyChannel = function () {
        CommonUtils.showWait(true);
        var lstId = ko.observableArray();
        ko.utils.arrayForEach(self.mOrderOutput().Detail(), function (product) {
            lstId.push(product.ProductId());
        });
        var model = ko.toJSON({ productIds: lstId(), brancheId: self.mOrderOutput().Id_To(), totaldateSum: 10 });
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Order_Output/TotalSumAdvSalebyChannel"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: model
        }).done(function (data) {
            ko.utils.arrayForEach(self.mOrderOutput().Detail(), function (pro) {
                var obj = ko.utils.arrayFirst(data, function (o) {
                    return o.ProductId = pro.ProductId()
                });
                if (obj)
                    pro.Sale_Average(obj.Sale_Average)

            });
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };

    self.mOrderOutput().Detail.subscribe(function () {
        if (self.mOrderOutput().Detail().length <= 0)
            return;
        self.Calculate_TotalSumAdvSalebyChannel();
    });

    self.mOrderOutput().Id_To.subscribe(function (val) {
        if (self.mOrderOutput().Detail().length <= 0)
            return;
        self.Calculate_TotalSumAdvSalebyChannel();
    });

    self.Start = function () {
        ko.applyBindings(self, document.getElementById('OrderOutputAddViewId'));
        if (Products == undefined)
            self.mOrderOutput(new Order.mOrder);
    };

    self.PrintModel = ko.observable();
};