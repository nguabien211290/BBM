var Customer = Customer || {};
Customer.mvCustomer = function () {
    var self = this;
    self.provinces = ko.observable(new Common.mvProvince());
    self.TmpTable = ko.observable(new Paging_TmpControltool());
    self.TmpTable().CurrentPage.subscribe(function () {
        self.LoadListCustomer();
    });
    self.TmpTable().ItemPerPage.subscribe(function () {
        self.TmpTable().CurrentPage(1);
        self.LoadListCustomer();
    });
    self.TmpTable().KeywordSearch.subscribe(function () {
        self.LoadListCustomer();
    });
    self.TmpTable().Sortby.subscribe(function (val) {
        if (val)
            self.LoadListCustomer();
    });
    self.TmpTable().nameTemplate('table_Customer');
    self.LoadListCustomer = function () {
        self.TmpTable().listData([]);
        var model = {
            pageindex: self.TmpTable().CurrentPage(),
            pagesize: self.TmpTable().ItemPerPage(),
            keyword: self.TmpTable().KeywordSearch(),
            sortby: self.TmpTable().Sortby(),
            sortbydesc: self.TmpTable().SortbyDesc()
        };
        self.TmpTable().Sortby(undefined);
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Customer/GetCustomerby"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({ pageinfo: model }),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.TmpTable().listData(CommonUtils.MapArray(data.Data.listTable, Customer.mCustomer));
                self.TmpTable().Totalitems(data.Data.totalItems);
                self.TmpTable().StartItem(data.Data.startItem);
            } else
                CommonUtils.notify("Thông báo", data.messaging, 'error');

        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.GetDetail = function (val) {
        ko.utils.arrayForEach(self.TmpTable().listData(), function (v) {
            v.IsViewDetail(false);
        })
        val.IsViewDetail(true);
    };
    self.mDetailHistoryCart = ko.observable();
    self.GetDetailHistoryCart = function (val) {
        self.mDetailHistoryCart(val);
        CommonUtils.showModal('#historyCartModal');
    };
    self.mCustomerUpdate = ko.observable(new Customer.mCustomer);
    self.OpentModalUpdateCustomer = function (val, isNew) {
        if (!isNew)
            self.mCustomerUpdate(val);
        CommonUtils.showModal('#updateCustomerModal', function () {
            CommonUtils.showWait(true);
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/Customer/UpdateCustomer"),
                cache: false,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: ko.toJSON(self.mCustomerUpdate()),
            }).done(function (data) {
                if (data == null)
                    return
                if (!data.isError) {
                    self.LoadListCustomer();
                    self.mCustomerUpdate(new Customer.mCustomer)
                }
                CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
            }).always(function () {
                CommonUtils.showWait(false);
            });
        });
    };
    self.AddCustomer = function () {
        self.OpentModalUpdateCustomer(self.mCustomerUpdate(new Customer.mCustomer), true);
    };


    self.Start = function () {
        ko.applyBindings(self, document.getElementById('CustomerViewId'));
        self.LoadListCustomer();
    };
};