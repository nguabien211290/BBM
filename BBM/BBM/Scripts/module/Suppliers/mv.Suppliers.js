var Suppliers = Suppliers || {};
Suppliers.mvSuppliers = function () {
    var self = this;
    self.provinces = ko.observable(new Common.mvProvince());
    self.TmpTable = ko.observable(new Paging_TmpControltool());
    self.TmpTable().CurrentPage.subscribe(function () {
        self.LoadListSuppliers();
    });
    self.TmpTable().ItemPerPage.subscribe(function () {
        self.TmpTable().CurrentPage(1);
        self.LoadListSuppliers();
    });
    self.TmpTable().KeywordSearch.subscribe(function () {
        self.LoadListSuppliers();
    });
    self.TmpTable().Sortby.subscribe(function (val) {
        if (val)
            self.LoadListSuppliers();
    });
    self.TmpTable().nameTemplate('table_Suppliers');
    self.LoadListSuppliers = function () {
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
            url: CommonUtils.url("/Suppliers/GetSuppliersby"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({ pageinfo: model }),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.TmpTable().listData(CommonUtils.MapArray(data.Data.listTable, Suppliers.mSuppliers));
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
    self.mSuppliers = ko.observable(new Suppliers.mSuppliers);
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
                self.LoadListSuppliers();
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
                self.LoadListSuppliers();
                CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
            }).always(function () {
                CommonUtils.showWait(false);
            });
        });

    };
    self.Start = function () {
        ko.applyBindings(self, document.getElementById('SuppliersViewId'));
        self.LoadListSuppliers();
    };
};