var Suppliers = Suppliers || {};
Suppliers.mvSuppliers = function () {
    var self = this;
    self.mSuppliers = ko.observable(new Suppliers.mSuppliers);
    self.provinces = ko.observable(new Common.mvProvince());
    self.Table = ko.observable(new Paging_TmpControltool("Suppliers", true, true));
    self.SearchReLoad = ko.computed(function () {
        if (self.Table().CountFilter() > 0) {
            self.LoadListSuppliers()
        }
    }).extend({ throttle: 1000 });
    self.LoadListSuppliers = function () {
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Suppliers/GetSuppliersby"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(self.Table().Model()),
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