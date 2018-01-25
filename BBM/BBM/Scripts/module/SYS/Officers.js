var Officers = Officers || {};
Officers.mOfficers = function () {
    var self = this;
    self.Id = ko.observable();
    self.IdRole = ko.observable();
    self.IdKho = ko.observable();
    self.MaNV = ko.observable("");

    self.Kho = ko.observable("");
    self.TenNv = ko.observable("");
    self.Sdt = ko.observable("");
    self.Diachi = ko.observable("");
    self.Email = ko.observable("");
    self.Pwd = ko.observable();
    self.RoleNv = ko.observableArray();
};
Officers.mKho = function () {
    var self = this;
    self.Id = ko.observable();
    self.Kho = ko.observable();
    self.Diachi = ko.observable();
    self.Lienhe = ko.observable();
    self.Makho = ko.observable();
};
Officers.mRole = function () {
    var self = this;
    self.Id = ko.observable();
    self.Role = ko.observable();
    self.Note = ko.observable();
    self.IsHas = ko.observable(false);
    self.IsCheck = ko.observable(false);
};
Officers.mOfficers_Role = function () {
    var self = this;
    self.Id = ko.observable();
    self.IdRole = ko.observable();
    self.IdNv = ko.observable();
    self.DateUpdate = ko.observable();
    self.sys_nhanvien = ko.observable(new Officers.mOfficers);
    self.sys_group_role = ko.observable(new Officers.mRole);
};
Officers.mvOfficers = function () {
    var self = this;
    self.mOfficers = ko.observable(new Officers.mOfficers);
    self.lstOfficers = ko.observableArray();
    self.LoadListOfficers = function () {
        CommonUtils.showWait(true);
        $.ajax({
            type: "GET",
            url: CommonUtils.url("/SYS_Officers/GetListNV"),
        }).done(function (data) {
            if (data == null)
                return
            self.lstOfficers(CommonUtils.MapArray(data, Officers.mOfficers));
            self.mOfficers().RoleNv(self.lstRole());
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.Detail = function (val) {
        CommonUtils.showWait(true);
        $.ajax({
            type: "GET",
            url: CommonUtils.url("/SYS_Officers/GetNV"),
            data: { Id: val.Id() },
        }).done(function (data) {
            if (data == null)
                return
            ko.mapping.fromJS(data, {}, self.mOfficers);
            var tmpRole = self.mOfficers().RoleNv();
            ko.utils.arrayForEach(self.lstRole(), function (item) {
                item.IsCheck(false);
            });
            self.mOfficers().RoleNv(self.lstRole());
            ko.utils.arrayForEach(tmpRole, function (item) {
                var hasIt = ko.utils.arrayFirst(self.mOfficers().RoleNv(), function (obj) {
                    return obj.Id() == item.Id();
                });
                if (hasIt)
                    hasIt.IsCheck(true);
            });
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.selectChanged_KhoProduct = function (data, event) {
        var k = $(event.target).attr("index");
        var Id = parseInt(k);
        if (k) {
            var tmpIndex = ko.utils.arrayFirst(self.lstKHO(), function (obj) {
                return obj.Id === parseInt(k);
            });
            if (tmpIndex)
                data.Kho = tmpIndex.Kho;
        }
    };
    self.Cancel = function () {
        self.mOfficers(new Officers.mOfficers);
        ko.utils.arrayForEach(self.lstRole(), function (item) {
            item.IsCheck(false);
        });
        self.mOfficers().RoleNv(self.lstRole());

    };
    self.SaveChange = function (val) {
        CommonUtils.showWait(true);
        if (!val.Id())
            val.Pwd(CommonUtils.randomString(15));
        $.ajax({
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            url: "/SYS_Officers/AddOrUpdate",
            data: ko.toJSON({ model: val }),
        }).done(function (data) {
            self.LoadListOfficers();
            self.Cancel();
            CommonUtils.notify("Thông báo", data.messaging, data.success ? '' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.Remove = function (d) {
        CommonUtils.openConfirm('Xóa nhân viên này ?', function () {
            CommonUtils.showWait(true);
            $.ajax({
                type: "DELETE",
                url: "/SYS_Officers/Remove/" + d.Id()
            }).done(function (data) {
                self.LoadListOfficers();
                CommonUtils.notify("Thông báo", data.messaging, data.success ? '' : 'error');
            }).always(function () {
                CommonUtils.showWait(false);
            });
        });
    };
    self.Start = function () {
        ko.applyBindings(self, document.getElementById('officerViewId'));
        self.LoadListOfficers();
        self.LoadRole();
        self.LoadKHO();
    };
    self.lstRole = ko.observableArray();
    self.LoadRole = function () {
        CommonUtils.showWait(true);
        $.ajax({
            type: "GET",
            url: CommonUtils.url("/SYS_Role/GetListRole"),
        }).done(function (data) {
            if (data == null)
                return
            self.lstRole(CommonUtils.MapArray(data, Officers.mRole));
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.selectKHO = ko.observable();
    self.lstKHO = ko.observableArray();
    self.LoadKHO = function () {
        CommonUtils.showWait(true);
        $.ajax({
            type: "GET",
            url: CommonUtils.url("/KHO/GetList"),
        }).done(function (data) {
            if (data == null)
                return
            self.lstKHO(CommonUtils.MapArray(data, Officers.mKho));
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
};

