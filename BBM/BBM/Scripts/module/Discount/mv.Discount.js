var Discount = Discount || {};
Discount.mvDiscount = function () {
    var self = this;
    self.mDisscount = ko.observable(new Discount.mDiscount);
    self.lstTypeDiscount = ko.observableArray([{ 'value': 1, 'name': 'vnd' }, { 'value': 2, 'name': '%' }])
    //---------Table Filter-----------------------
    self.TmpTable = ko.observable(new Paging_TmpControltool("Chương trình Khuyễn mãi"));
    self.FilterProduct = ko.observable(new Filter.mvFilter_Search_Control('Discount', true));
    self.FilterProduct().classNameTab('mvDiscount');
    self.CountFilter = ko.observable(0);
    self.FilterProduct().KeywordSearch.subscribe(function () {
        self.CountFilter(self.CountFilter() + 1);
    });
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
    self.TmpTable().nameTemplate('table_Discount');
    self.SearchReLoad = ko.computed(function () {
        if (self.CountFilter() > 0) {
            self.LoadListDiscount()
        }
    }).extend({ throttle: 500 });
    //---------Table Filter-----------------------


    self.LoadListDiscount = function () {
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
            url: CommonUtils.url("/Discount/GetDiscountby"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({ pageinfo: model }),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.TmpTable().listData(CommonUtils.MapArray(data.Data.listTable, Discount.mDiscount));
                self.TmpTable().Totalitems(data.Data.totalItems);
                self.TmpTable().StartItem(data.Data.startItem);
            } else
                CommonUtils.notify("Thông báo", data.messaging, 'error');

        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.Dissable_Discount = function (obj) {
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Discount/DisableDiscount"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(obj),
        }).done(function (data) {
            self.CountFilter(self.CountFilter() + 1);
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.AddDiscount = function () {

    };

    self.Refesh = function () {
        self.CountFilter(self.CountFilter() + 1);
    };

    self.OpentModalAddDiscount = function () {
        self.mDisscount(new Discount.mDiscount);
        CommonUtils.showModal('#AddDiscount', function () {
            CommonUtils.showWait(true);
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/Discount/CreateDiscount"),
                cache: false,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: ko.toJSON(self.mDisscount()),
            }).done(function (data) {
                if (data == null)
                    return
                self.mDisscount(new Discount.mDiscount);
                $('#AddDiscount').modal('hide');
                CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');

                self.CountFilter(self.CountFilter() + 1);
            }).always(function () {
                CommonUtils.showWait(false);
            });
        });
    };

    self.Start = function () {
        ko.applyBindings(self, document.getElementById('DiscountViewId'));
    };
};