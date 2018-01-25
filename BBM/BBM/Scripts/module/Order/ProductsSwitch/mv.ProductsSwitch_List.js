var Order = Order || {};
Order.mvProductsSwitchList = function () {
    var self = this;
    self.CountFilter = ko.observable(0);
    self.SearchReLoad = ko.computed(function () {
        if (self.CountFilter() > 0) {
            self.GetListOrderOutput()
        }
    }).extend({ throttle: 1000 });
    self.FilterOrder = ko.observable(new Filter.mvFilter_Search_Control('Order_ProductSwitch'));
    self.FilterOrder().Fiterby.subscribe(function (val) {
        if (val && val.length > 0)
            self.CountFilter(self.CountFilter() + 1);
    });
    self.FilterOrder().KeywordSearch.subscribe(function () {
        self.CountFilter(self.CountFilter() + 1);
    });

    self.lstProduct_Switch = ko.observableArray();
    self.GetListOrderOutput = function () {
        self.lstProduct_Switch([]);
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
            url: CommonUtils.url("/ProductsSwitch/Get_ProductsSwitch"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(model),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.lstProduct_Switch(CommonUtils.MapArray(data.Data.listTable, Order.mOrder));
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
        self.GetListOrderOutput();
    });
    self.TmpTable().ItemPerPage.subscribe(function () {
        self.TmpTable().CurrentPage(1);
        self.GetListOrderOutput();
    });
    self.TmpTable().Sortby.subscribe(function (val) {
        if (val)
            self.GetListOrderOutput();
    });
    self.lstProduct_Switch.subscribe(function (val) {
        self.TmpTable().listData(val);
    });
    self.TmpTable().nameTemplate('table_ProdcutsSwitch');
    //----------------------Filter-------------------
    self.GetDetail = function (val) {
        ko.utils.arrayForEach(self.TmpTable().listData(), function (v) {
            v.IsViewDetail(false);
        })
        val.IsViewDetail(true);
    }
    self.Start = function () {
        ko.applyBindings(self, document.getElementById('ProdcutsSwitchListViewId'));
        self.GetListOrderOutput();
    };

};
