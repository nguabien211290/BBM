var Branches = Branches || {};
Branches.mvBranches = function () {
    var self = this;
    self.listData = ko.observableArray();
    self.LoadListBranches = function () {
        self.listData([]);
        CommonUtils.showWait(true,"BranchesViewId");
        $.ajax({
            type: "GET",
            url: CommonUtils.url("/Branches/GetBranches"),
            cache: false,
            dataType: 'json',
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.listData(CommonUtils.MapArray(data.Data, Branches.mBranches));
                ko.utils.arrayForEach(self.listData(), function (val) {
                    val.soft_Channel(CommonUtils.MapArray(val.soft_Channel(), Branches.mChannel));
                });
            } else
                CommonUtils.notify("Thông báo", data.messaging, 'error');

        }).always(function () {
            CommonUtils.showWait(false,"BranchesViewId");
        });
    };
    self.GetDetail = function (val) {
        ko.utils.arrayForEach(self.listData(), function (v) {
            v.IsViewDetail(false);
        })
        val.IsViewDetail(true);
    };
    self.GetDetailChannel = function (val) {
        ko.utils.arrayForEach(self.listData(), function (vals) {
            ko.utils.arrayForEach(vals.soft_Channel(), function (it) {
                it.IsUpdate(false);
            })
        })
        val.IsUpdate(true);

    };
    self.SaveUpdateBranchs = function (val) {
        CommonUtils.showWait(true,"BranchesViewId");
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Branches/UpdateBranches"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(val),
        }).done(function (data) {
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false,"BranchesViewId");
        });
    };


    self.addBrandes = function (val) {
        ko.utils.arrayForEach(self.listData(), function (vals) {
            ko.utils.arrayForEach(vals.soft_Channel(), function (it) {
                it.IsUpdate(false);
            })
        });
        var obj = ko.observable(new Branches.mChannel);
        obj().Channel("Chi nhánh mới");
        obj().BranchesId(val.BranchesId());
        obj().IsUpdate(true);
        val.soft_Channel.push(obj());     
    };

    self.SaveUpdateChannel = function (val) {
        CommonUtils.showWait(true,"BranchesViewId");
        if (val.Id()) {
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/Branches/Update_Channel"),
                cache: false,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: ko.toJSON(val),
            }).done(function (data) {
                if (!data.isError) {
                    self.LoadListBranches();
                }
                CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
            }).always(function () {
                CommonUtils.showWait(false,"BranchesViewId");
            });
        } else {
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/Branches/Create_Channel"),
                cache: false,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: ko.toJSON(val),
            }).done(function (data) {
                if (!data.isError) {
                    self.LoadListBranches();
                }
                CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
            }).always(function () {
                CommonUtils.showWait(false,"BranchesViewId");
            });
        }
    };

    self.RemoveChannel = function (val) {
        CommonUtils.showWait(true,"BranchesViewId");
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Branches/RemoveChannel"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({model:val}),
        }).done(function (data) {
            if (!data.isError) {
                self.LoadListBranches();
            }
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false,"BranchesViewId");
        });
    };
    self.mBrachesNew = ko.observable();
    self.OpentModalAddBranches = function () {
        self.mBrachesNew(new Branches.mBranches);
        CommonUtils.showModal('#updateBranchesModal', function () {
            CommonUtils.showWait(true,"BranchesViewId");
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/Branches/CreateBranches"),
                cache: false,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: ko.toJSON(self.mBrachesNew()),
            }).done(function (data) {
                if (data == null)
                    return
                if (!data.isError) {
                    self.LoadListBranches();
                }
                CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
            }).always(function () {
                CommonUtils.showWait(false,"BranchesViewId");
            });
        });
    };
    self.RemoveBranchs = function (val) {
        CommonUtils.showWait(true,"BranchesViewId");
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Branches/RemoveBranchs"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({ model: val }),
        }).done(function (data) {
            if (!data.isError) {
                self.LoadListBranches();
            }
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false,"BranchesViewId");
        });
    };
    self.Start = function () {
        ko.applyBindings(self, document.getElementById('BranchesViewId'));
        self.LoadListBranches();
    };
};