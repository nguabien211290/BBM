var Order = Order || {};
Order.mvOrderSaleList = function () {
    var self = this;
    self.CountFilter = ko.observable(0);
    self.SearchReLoad = ko.computed(function () {
        if (self.CountFilter() > 0) {
            self.GetListOrderSale()
        }
    }).extend({ throttle: 1000 });
    self.FilterOrder = ko.observable(new Filter.mvFilter_Search_Control('Order_Channel'));
    self.FilterOrder().Fiterby.subscribe(function (val) {
        if (val && val.length > 0)
            self.CountFilter(self.CountFilter() + 1);
    });
    self.FilterOrder().KeywordSearch.subscribe(function () {
        self.CountFilter(self.CountFilter() + 1);
    });

    self.lstOrder_Sale = ko.observableArray();
    self.OptionStatusOrder = ko.observableArray([
        { Value: 1, Name: 'Chờ xử lý' },
        { Value: 2, Name: 'Đang giao hàng' },
        { Value: 3, Name: 'Hoàn thành' },
        { Value: 4, Name: 'Hủy' },
        { Value: 5, Name: 'Hoàn trả' }
    ]);
    self.GetListOrderSale = function () {
        self.lstOrder_Sale([]);
        var model = {
            pageindex: self.TmpTable().CurrentPage(),
            pagesize: self.TmpTable().ItemPerPage(),
            sortby: self.TmpTable().Sortby(),
            sortbydesc: self.TmpTable().SortbyDesc(),
            keyword: self.FilterOrder().KeywordSearch(),
            filterby: self.FilterOrder().Fiterby()
        };
        self.TmpTable().Sortby(undefined);
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/OrderChannel/GetOrder"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(model),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.lstOrder_Sale(CommonUtils.MapArray(data.Data.listTable, Order.mTableOrder));
                self.TmpTable().TotalMoney(data.Data.Total);
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
        self.GetListOrderSale();
    });
    self.TmpTable().ItemPerPage.subscribe(function () {
        self.TmpTable().CurrentPage(1);
        self.GetListOrderSale();
    });
    self.TmpTable().Sortby.subscribe(function (val) {
        if (val)
            self.GetListOrderSale();
    });
    self.lstOrder_Sale.subscribe(function (val) {
        self.TmpTable().listData(val);
    });
    self.TmpTable().nameTemplate('table_OrderSale');
    //----------------------Filter-------------------
    self.GetDetail = function (val) {
        ko.utils.arrayForEach(self.TmpTable().listData(), function (v) {
            v.IsViewDetail(false);
        })
        val.IsViewDetail(true);
    };
    self.UpdateOrder = function (val) {
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/OrderChannel/UpdateDone_Order_Sale"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({ model: val }),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                CommonUtils.notify("Thông báo", 'Cập nhật đơn hàng thành công', 'success');
            } else
                CommonUtils.notify("Thông báo", data.messaging, 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.Start = function () {
        ko.applyBindings(self, document.getElementById('OrderSaleListViewId'));
        self.GetListOrderSale();
    };

    self.GetDetailOrderSale = function (val) {
        var data = { order: val.Detail() }
        CommonUtils.addTabDynamic('Nhập hàng', CommonUtils.url('/Order_Input/RenderViewCreate'), '#contentX', true, data);
    };
};
