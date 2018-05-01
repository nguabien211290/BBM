var Suppliers = Suppliers || {};
Suppliers.mvSuppliers = function () {
    var self = this;
    self.mSuppliers = ko.observable(new Suppliers.mSuppliers);

    //---------Table Filter-----------------------
    self.Table = ko.observable(new Paging_TmpControltool("Nhà phân phối"));
    self.FilterProduct = ko.observable(new Filter.mvFilter_Search_Control('Suppliers', true));
    self.FilterProduct().classNameTab('mvSuppliers');
    self.CountFilter = ko.observable(0);
    self.FilterProduct().KeywordSearch.subscribe(function () {
        self.CountFilter(self.CountFilter() + 1);
    });
    self.Table().CurrentPage.subscribe(function () {
        self.CountFilter(self.CountFilter() + 1);
    });
    self.Table().ItemPerPage.subscribe(function () {
        self.Table().CurrentPage(1);
        self.CountFilter(self.CountFilter() + 1);
    });

    self.Table().Sortby.subscribe(function (val) {
        if (val)
            self.CountFilter(self.CountFilter() + 1);
    });
    self.Table().nameTemplate('table_Suppliers');
    self.SearchReLoad = ko.computed(function () {
        if (self.CountFilter() > 0) {
            self.LoadListSuppliers()
        }
    }).extend({ throttle: 500 });
    //---------Table Filter-----------------------

    self.LoadListSuppliers = function () {
        CommonUtils.showWait(true);

        self.Table().listData([]);
        var model = {
            pageindex: self.Table().CurrentPage(),
            pagesize: self.Table().ItemPerPage(),
            keyword: self.FilterProduct().KeywordSearch(),
            sortby: self.Table().Sortby(),
            sortbydesc: self.Table().SortbyDesc()
        };

        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Suppliers/GetSuppliersby"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({ pageinfo: model }),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.Table().listData(CommonUtils.MapArray(data.Data.listTable, Suppliers.mSuppliers));
                self.Table().Totalitems(data.Data.totalItems);
                self.Table().StartItem(data.Data.startItem);
            } else
                CommonUtils.notify("Thông báo", data.messaging, 'error');

        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.GetDetail = function (val) {
        ko.utils.arrayForEach(self.Table().listData(), function (v) {
            v.IsViewDetail(false);
        })
        val.IsViewDetail(true);
    };
    self.AddSuppliers = function () {
        CommonUtils.showModal('#AddSuppliers', function () {
            self.UpdateSuppliers(self.mSuppliers());
        });
    }
    self.UpdateSuppliers = function (val) {
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Suppliers/UpdateSuppliers"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(val),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.Table().CountFilter(self.Table().CountFilter() + 1);
                self.mSuppliers(new Suppliers.mSuppliers);
            }
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.DeleteSuppliers = function (val) {
        CommonUtils.confirm("Thông báo", "Bạn có chắc xóa nhà phân phối này không? ", function () {
            CommonUtils.showWait(true);
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/Suppliers/DeleteSuppliers"),
                cache: false,
                data: { id: val.SuppliersId() },
            }).done(function (data) {
                if (data == null)
                    return
                self.Table().CountFilter(self.Table().CountFilter() + 1);
                CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
            }).always(function () {
                CommonUtils.showWait(false);
            });
        });

    };
    self.Refesh = function () {
        self.CountFilter(self.CountFilter() + 1);
    };

    self.Start = function () {
        ko.applyBindings(self, document.getElementById('SuppliersViewId'));
    };
};