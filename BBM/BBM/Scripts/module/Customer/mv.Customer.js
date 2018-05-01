var Customer = Customer || {};
Customer.mvCustomer = function () {
    var self = this;
    self.provinces = ko.observable(new Common.mvProvince());
    self.TmpTable = ko.observable(new Paging_TmpControltool("Khách hàng"));
    self.FilterProduct = ko.observable(new Filter.mvFilter_Search_Control('Customer', true));
    self.FilterProduct().classNameTab('mvCustomer');
    self.CountFilter = ko.observable(0);
    self.FilterProduct().KeywordSearch.subscribe(function () {
        self.CountFilter(self.CountFilter() + 1);
    });
    self.SearchReLoad = ko.computed(function () {
        if (self.CountFilter() > 0) {
            self.LoadListCustomer()
        }
    }).extend({ throttle: 500 });

    self.TmpTable().CurrentPage.subscribe(function () {
        self.CountFilter(self.CountFilter() + 1);
    });
    self.TmpTable().ItemPerPage.subscribe(function () {
        self.TmpTable().CurrentPage(1);
        self.CountFilter(self.CountFilter() + 1);
    });

    self.TmpTable().Sortby.subscribe(function (val) {
        if (val)
            self.CountFilter(self.CountFilter() + 1);
    });
    self.TmpTable().nameTemplate('table_Customer');

    self.LoadListCustomer = function () {
        self.TmpTable().listData([]);
        var model = {
            pageindex: self.TmpTable().CurrentPage(),
            pagesize: self.TmpTable().ItemPerPage(),
            keyword: self.FilterProduct().KeywordSearch(),
            sortby: self.TmpTable().Sortby(),
            sortbydesc: self.TmpTable().SortbyDesc()
        };
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
                    self.CountFilter(self.CountFilter() + 1);
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

    self.Refesh = function () {
        self.CountFilter(self.CountFilter() + 1);
    };

    self.Start = function () {
        ko.applyBindings(self, document.getElementById('CustomerViewId'));
    };
};