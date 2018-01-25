var Order = Order || {};
Order.mvOrderSuppliersList = function () {
    var self = this;
    self.lstOrder_Suppliers = ko.observableArray();
    self.CountFilter = ko.observable(0);
    self.SearchReLoad = ko.computed(function () {
        if (self.CountFilter() > 0) {
            self.GetListOrderSuppliers()
        }
    }).extend({ throttle: 1000 });
    self.FilterOrder = ko.observable(new Filter.mvFilter_Search_Control('Order_Suppliers'));
    self.FilterOrder().Fiterby.subscribe(function (val) {
        if (val && val.length > 0)
            self.CountFilter(self.CountFilter() + 1);
    });
    self.FilterOrder().KeywordSearch.subscribe(function () {
        self.CountFilter(self.CountFilter() + 1);
    });

    self.GetListOrderSuppliers = function () {
        self.lstOrder_Suppliers([]);
        var model = {
            pageindex: self.TmpTable().CurrentPage(),
            pagesize: self.TmpTable().ItemPerPage(),
            sortby: self.TmpTable().Sortby(),
            sortbydesc: self.TmpTable().SortbyDesc(),
            filterby: self.FilterOrder().Fiterby()
        };
        self.TmpTable().Sortby(undefined);
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Order_Suppliers/GetOrder_Suppliers"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(model),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.lstOrder_Suppliers(CommonUtils.MapArray(data.Data.listTable, Order.mOrder));
                self.TmpTable().Totalitems(data.Data.totalItems);
                self.TmpTable().StartItem(data.Data.startItem);
            }
            else
                CommonUtils.notify("Thông báo", data.messaging, 'error');

        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    //--------------Table--------------------------------
    self.TmpTable = ko.observable(new Paging_TmpControltool());
    self.TmpTable().CurrentPage.subscribe(function () {
        self.GetListOrderSuppliers();
    });
    self.TmpTable().ItemPerPage.subscribe(function () {
        self.TmpTable().CurrentPage(1);
        self.GetListOrderSuppliers();
    });
    self.TmpTable().Sortby.subscribe(function (val) {
        if (val)
            self.GetListOrderSuppliers();
    });
    self.lstOrder_Suppliers.subscribe(function (val) {
        self.TmpTable().listData(val);
    });
    self.TmpTable().nameTemplate('table_OrderSuppliers');
    //----------------------Filter-------------------
    self.GetDetail = function (val) {
        ko.utils.arrayForEach(self.TmpTable().listData(), function (v) {
            v.IsViewDetail(false);
        })
        val.IsViewDetail(true);
    }
    self.CreateOrderInput = function (val) {
        var data = { products: val.Detail(), orderSuppliersId: val.Id() }
        CommonUtils.addTabDynamic('Nhập hàng', CommonUtils.url('/Order_Input/RenderViewCreate'), '#contentX', true, data);
    };

    self.UpdateOrderDone = function (val) {
        val.Status(CommonUtils.GetAttrStatus(statusOrder_Input, 'Done', 'Key'))
        ko.utils.arrayForEach(val.Detail(), function (o) { o.Status(CommonUtils.GetAttrStatus(statusOrder_Input, 'Done', 'Key')); })
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Order_Suppliers/UpdateDone_Order_Suppliers"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({ id: val.Id() }),
        }).done(function (data) {
            if (data.isError)
                CommonUtils.notify("Thông báo", data.messaging, 'error');

        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.Start = function () {
        ko.applyBindings(self, document.getElementById('OrderSuppliersListViewId'));
        self.GetListOrderSuppliers();
    };
};
