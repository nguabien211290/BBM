var Order = Order || {};
Order.mvOrderOutputAdd = function () {
    var self = this;

    self.lstOrder_Output = ko.observableArray();
    self.mOrderOutput_Branches = ko.observable(new Order.mOrder);
    
    
    self.LstBranches = ko.observableArray();

    self.FilterProduct = ko.observable(new Filter.mvFilter_Search_Control('Product_Control'));
    self.FilterProduct().classNameTab('mvOrderOutputAdd');
    self.FilterProduct().isCheckProduct.subscribe(function (val) {
        ko.utils.arrayForEach(val, function (o) {
            var hasExits = ko.utils.arrayFirst(self.mOrderOutput_Branches().Detail(), function (b) {
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
        ko.utils.arrayForEach(self.mOrderOutput_Branches().Detail(), function (val) {
            sum += parseInt(val.Total());
        });
        self.mOrderOutput_Branches().Total(sum);
    });
    self.GetProduct = function (val) {
        var hasIt = ko.utils.arrayFirst(self.mOrderOutput_Branches().Detail(), function (o) {
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
                    if (data.Data.result.product.SuppliersName)
                        newObj.SuppliersName(data.Data.result.product.SuppliersName);
                    if (data.Data.result.product_stock)
                        newObj.Stock_Total(data.Data.result.product_stock.Stock_Total);
                    if (data.Data.result.product_stocks)
                        newObj.Stock_Totals(data.Data.result.product_stocks);
                    self.mOrderOutput_Branches().Detail.push(newObj);
                }

            }).always(function () {
                CommonUtils.showWait(false);
            });
        }

    };
    self.CreatOrderOutput = function () {
        CommonUtils.showWait(true);
        self.mOrderOutput_Branches().Id_From(UserBranchId);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Order_Branch/CreateOrder_Branches"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(self.mOrderOutput_Branches()),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.PrintModel(self.mOrderOutput_Branches());
                CommonUtils.Print("printorderoutput");
                self.mOrderOutput_Branches(new Order.mOrder);
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
        self.mOrderOutput_Branches().Detail.remove(val)
    };
    self.Calculate_TotalSumAdvSalebyChannel = function () {
        CommonUtils.showWait(true);
        var lstId = ko.observableArray();
        ko.utils.arrayForEach(self.mOrderOutput_Branches().Detail(), function (product) {
            lstId.push(product.ProductId());
        });
        var model = ko.toJSON({ productIds: lstId(), brancheId: self.mOrderOutput_Branches().Id_To(), totaldateSum: 10 });
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Order_Output/TotalSumAdvSalebyChannel"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: model
        }).done(function (data) {
            ko.utils.arrayForEach(self.mOrderOutput_Branches().Detail(), function (pro) {
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

    self.mOrderOutput_Branches().Detail.subscribe(function () {
        if (self.mOrderOutput_Branches().Detail().length <= 0)
            return;
        self.Calculate_TotalSumAdvSalebyChannel();
    });

    self.mOrderOutput_Branches().Id_To.subscribe(function (val) {
        if (self.mOrderOutput_Branches().Detail().length <= 0)
            return;
        self.Calculate_TotalSumAdvSalebyChannel();
    });

    self.Start = function () {
        ko.applyBindings(self, document.getElementById('OrderBranchAddViewId'));
        self.mOrderOutput_Branches(new Order.mOrder);
    };

    self.PrintModel = ko.observable();
};