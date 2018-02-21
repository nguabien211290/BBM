var Order = Order || {};
Order.mvOrderInputList = function () {
    var self = this;
    self.CountFilter = ko.observable(0);
    self.SearchReLoad = ko.computed(function () {
        if (self.CountFilter() > 0) {
            self.GetListOrderInput()
        }
    }).extend({ throttle: 1000 });
    self.FilterOrder = ko.observable(new Filter.mvFilter_Search_Control('Order_Input'));
    self.FilterOrder().Fiterby.subscribe(function (val) {
        if (val && val.length > 0)
            self.CountFilter(self.CountFilter() + 1);
    });
    self.FilterOrder().KeywordSearch.subscribe(function () {
        self.CountFilter(self.CountFilter() + 1);
    });

    self.lstOrder_Input = ko.observableArray();
    self.type_Output = ko.observable();

    self.GetListOrderInput = function () {
        self.lstOrder_Input([]);
        self.type_Output(ko.utils.arrayFirst(lstTypeOrder, function (ob) {
            return ob.Value == 'Output'
        }));
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
            url: CommonUtils.url("/Order_Input/GetOrder_Inside"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(model),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.lstOrder_Input(CommonUtils.MapArray(data.Data.listTable, Order.mOrder));
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
        self.GetListOrderInput();
    });
    self.TmpTable().ItemPerPage.subscribe(function () {
        self.TmpTable().CurrentPage(1);
        self.GetListOrderInput();
    });
    self.TmpTable().Sortby.subscribe(function (val) {
        if (val)
            self.GetListOrderInput();
    });
    self.lstOrder_Input.subscribe(function (val) {
        self.TmpTable().listData(val);
    });
    self.TmpTable().nameTemplate('table_OrderInput');
    //----------------------Filter-------------------
    self.GetDetail = function (val) {
        ko.utils.arrayForEach(self.TmpTable().listData(), function (v) {
            v.IsViewDetail(false);
        })
        val.IsViewDetail(true);
    };
    self.UpdateOrder = function (val) {
        var rs = val;
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Order_Input/UpdateDone_Order_Inside"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({ id: val.Id() }),
        }).done(function (data) {
           if (data.isError)
                CommonUtils.notify("Thông báo", data.messaging, 'error');
            else
                ko.utils.arrayForEach(self.lstOrder_Input(), function (o) {
                    if (o.Id() == rs.Id())
                        o.Status(CommonUtils.GetAttrStatus(statusOrder_Input, 'Done', 'Key'))
                });

        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.ReturnViewBarcode = function (val) {
        var data = { products: val.Detail() }
        CommonUtils.addTabDynamic('In Tem', CommonUtils.url('/Barcode/RenderView'), '#contentX', true, data);
    };

    self.Start = function () {
        ko.applyBindings(self, document.getElementById('OrderInputListViewId'));
        self.GetListOrderInput();
    };
};
