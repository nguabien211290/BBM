var Catalog = Catalog || {};
Catalog.mvCatalog = function () {
    var self = this;
    self.TmpTable = ko.observable(new Paging_TmpControltool());
    self.TmpTable().CurrentPage.subscribe(function () {
        self.LoadListCatalog();
    });
    self.TmpTable().ItemPerPage.subscribe(function () {
        self.TmpTable().CurrentPage(1);
        self.LoadListCatalog();
    });
    self.TmpTable().KeywordSearch.subscribe(function () {
        self.LoadListCatalog();
    });
    self.TmpTable().Sortby.subscribe(function (val) {
        if (val)
            self.LoadListCatalog();
    });
    self.TmpTable().nameTemplate('table_Catalog');
    self.LoadListCatalog = function () {
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
            url: CommonUtils.url("/Catalog/GetCatalogby"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({ pageinfo: model }),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.TmpTable().listData(CommonUtils.MapArray(data.Data.listTable, Catalog.mCatalog));
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
    self.mCatalog = ko.observable(new Catalog.mCatalog);
    self.AddCatalog = function () {
        CommonUtils.showModal('#AddCatalog', function () {
            self.UpdateCatalog(self.mCatalog());
        });
    }
    self.UpdateCatalog = function (val) {
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Catalog/UpdateCatalog"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(val),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.LoadListCatalog();
                self.mCatalog(new Catalog.mCatalog);
            }
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).error(function () {
            CommonUtils.notify("Thông báo", 'Bạn không có quyền cập nhật nhóm hàng.',  'error');         
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.DeleteCatalog = function (val) {
        CommonUtils.confirm("Thông báo", "Bạn có chắc xóa nhóm sản phẩm này không? ", function () {
            CommonUtils.showWait(true);
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/Catalog/DeleteCatalog"),
                cache: false,
                data: { id: val.Id() },
            }).done(function (data) {
                if (data == null)
                    return
                self.LoadListCatalog();
                CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
            }).always(function () {
                CommonUtils.showWait(false);
            });
        });

    };

    self.Start = function () {
        ko.applyBindings(self, document.getElementById('CatalogViewId'));
        self.LoadListCatalog();
    };
};