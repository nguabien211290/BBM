var Discount = Discount || {};
Discount.mvDiscount = function () {
    var self = this;
    self.mDisscount = ko.observable(new Discount.mDiscount);
    self.lstTypeDiscount = ko.observableArray([{ 'value': 1, 'name': 'vnd' }, { 'value': 2, 'name': '%' }])
    self.TmpTable = ko.observable(new Paging_TmpControltool());
    self.TmpTable().CurrentPage.subscribe(function () {
        self.LoadListDiscount();
    });
    self.TmpTable().ItemPerPage.subscribe(function () {
        self.TmpTable().CurrentPage(1);
        self.LoadListDiscount();
    });
    self.TmpTable().KeywordSearch.subscribe(function () {
        self.LoadListDiscount();
    });
    self.TmpTable().Sortby.subscribe(function (val) {
        if (val)
            self.LoadListDiscount();
    });
    self.TmpTable().nameTemplate('table_Discount');
    self.LoadListDiscount = function () {
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
            self.LoadListDiscount();
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.AddDiscount = function () {
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
            self.LoadListDiscount();
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.Start = function () {
        ko.applyBindings(self, document.getElementById('DiscountViewId'));
        self.LoadListDiscount();
    };
};