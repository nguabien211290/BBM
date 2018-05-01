var Employess = Employess || {};
Employess.mvEmployess = function () {
    var self = this;
    self.Table = ko.observable(new Paging_TmpControltool("Employess", true, true));
    self.SearchReLoad = ko.computed(function () {
        if (self.Table().CountFilter() > 0) {
            self.LoadListEmployess()
        }
    }).extend({ throttle: 500 });
    self.IsReCreatePwd = ko.observable(false);
    self.LoadListEmployess = function () {
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Employess/GetEmployessby"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(self.Table().Model()),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.Table().listData(CommonUtils.MapArray(data.Data.listTable, Employess.mEmployess));
                self.Table().Totalitems(data.Data.totalItems);
                self.Table().StartItem(data.Data.startItem);
            } else
                CommonUtils.notify("Thông báo", data.messaging, 'error');

        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.GetDetail = function (employ) {
        ko.utils.arrayForEach(self.lstRoles(), function (obj) {
            ko.utils.arrayForEach(obj.mGroupRoles(), function (obj2) {
                ko.utils.arrayForEach(obj2.Roles(), function (obj3) {
                    obj3.isSelect(false);
                });
            });
        });

        ko.utils.arrayForEach(self.Table().listData(), function (val) {
            val.IsViewDetail(false);
        })

        employ.IsViewDetail(true);

        if (employ.GroupRoles()) {
            ko.utils.arrayForEach(employ.GroupRoles(), function (objGroupRoles) {

                var rolebyBrand = ko.utils.arrayFirst(self.lstRoles(), function (o) {
                    return o.BrandId() == objGroupRoles.BrandId();
                });

                if (rolebyBrand) {
                    ko.utils.arrayForEach(objGroupRoles.Roles(), function (item_objobjGroupRoles) {

                        ko.utils.arrayForEach(rolebyBrand.mGroupRoles(), function (item_rolebyBrand) {

                            var role = ko.utils.arrayFirst(item_rolebyBrand.Roles(), function (o) {
                                return o.Id() == item_objobjGroupRoles;
                            });

                            if (role)
                                role.isSelect(true);

                        });

                    });
                }

            })
        }
    };
    self.mNewCustomer = ko.observable(new Employess.mEmployess);
    self.AddEmployee = function () {
        CommonUtils.showModal('#AddEmployee', function () {
            if (self.mNewCustomer().PwdRep() !== self.mNewCustomer().Pwd()) {
                CommonUtils.notify("Thông báo", "Mật khẩu nhập lại không đúng", 'error');
            }
            else {
                self.UpdateEmloyess(self.mNewCustomer());
            }
        });
    }

    self.UpdateEmloyess = function (val) {
        if (!val.Id() || self.IsReCreatePwd())
            if (val.Pwd() !== val.PwdRep()) {
                CommonUtils.notify("Thông báo", "Mật khẩu nhập lại không đúng.", 'error');
                return;
            }

        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Employess/UpdateEmployess"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(val),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.Table().CountFilter(self.Table().CountFilter() + 1);
                self.mNewCustomer(new Employess.mEmployess)
            }
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.lstRoles = ko.observableArray();
    self.LoadRoles = function () {
        CommonUtils.showWait(true);
        $.ajax({
            type: "GET",
            url: CommonUtils.url("/Partial/GroupRole"),
            cache: false,
            dataType: 'json',
        }).done(function (data) {
            if (data != null) {
                var brand = common.mvBranches().Option();
                ko.utils.arrayForEach(brand, function (vl) {
                    var roles = CommonUtils.MapArray(data, Employess.mGroupRoles);
                    ko.utils.arrayForEach(roles, function (vl) {
                        var ak = CommonUtils.MapArray(vl.Roles(), Employess.mRoles);
                        vl.Roles(ak);
                    });
                    var objnew = new Employess.mRoleBrand();
                    objnew.BrandId(vl.BranchesId);
                    objnew.BrandName(vl.BranchesName);
                    objnew.mGroupRoles(roles);
                    self.lstRoles.push(objnew);
                });
            }
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.UpdateRoles = function (val) {
        var rolesFull = ko.observableArray();
        ko.utils.arrayForEach(self.lstRoles(), function (obj) {
            var ls = "";
            ko.utils.arrayForEach(obj.mGroupRoles(), function (group) {
                ko.utils.arrayForEach(group.Roles(), function (roles) {
                    if (roles.isSelect())
                        if (ls.length <= 0) {
                            ls += roles.Id();
                        } else {
                            ls += ',' + roles.Id();
                        }

                });
            });
            rolesFull.push({ BrandId: obj.BrandId(), Roles: ls });
        });
        val.Roles(ko.toJSON(rolesFull));
        self.UpdateEmloyess(val);
    };
   

    self.mEmployTitle = ko.observable(new Employess.mEmployessTitle);
    self.lstEmployTitle = ko.observableArray();
    self.mEmployInfo = ko.observable();

    self.LoadEmployessInfo = function () {
        CommonUtils.showWait(true);
        $.ajax({
            type: "GET",
            url: CommonUtils.url("/Employess/LoadEmployessInfo"),
            cache: false,
            dataType: 'json',
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.mEmployInfo(ko.mapping.fromJS(data.Data, {}, new Employess.mEmployess));
                self.mEmployInfo().Pwd("");
            }
            else
                CommonUtils.notify("Thông báo", data.messaging, 'error');

        }).always(function () {
            CommonUtils.showWait(false);
        });
    };

    self.LoadEmployessTitles = function () {
        CommonUtils.showWait(true);
        $.ajax({
            type: "GET",
            url: CommonUtils.url("/Employess/LoadEmployessTitles"),
            cache: false,
            dataType: 'json',
        }).done(function (data) {
            if (data == null)
                return
            self.lstEmployTitle(CommonUtils.MapArray(data, Employess.mEmployessTitle));

        }).always(function () {
            CommonUtils.showWait(false);
        });
    };

    self.OpenModalEmloyeeTitle = function () {
        CommonUtils.showModal('#ModalEmloyeeTitle', function () {
            CommonUtils.showWait(true);
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/Employess/UpdateEmloyeeTitle"),
                cache: false,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: ko.toJSON(self.mEmployTitle()),
            }).done(function (data) {
                if (data == null)
                    return
                self.LoadEmployessTitles();
            }).always(function () {
                CommonUtils.showWait(false);
            });
        });
    };

    self.UpdateEmloyessInfo = function () {
        if (self.mEmployInfo().isUpdatePwd()) {
            if (self.mEmployInfo().PwdNew() !== self.mEmployInfo().PwdRep()) {
                CommonUtils.notify("Thông báo", "Mật khẩu nhập lại không đúng.", 'error');
                return;
            }
        }

        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Employess/UpdateMyInfo"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(self.mEmployInfo()),
        }).done(function (data) {
            if (data == null)
                return
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };

    self.DeleteEmployess = function (val) {
        CommonUtils.confirm("Thông báo", "Bạn chắc muốn xóa User <b>" + val.Email() + "</b>", function () {
            CommonUtils.showWait(true);
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/Employess/DeleteEmployess"),
                cache: false,
                data: { id: val.Id() },
            }).done(function (data) {
                if (data == null)
                    return
                CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
                self.Table().CountFilter(self.Table().CountFilter() + 1);
            }).always(function () {
                CommonUtils.showWait(false);
            });
        });
    };

    self.Refesh = function () {
        self.CountFilter(self.CountFilter() + 1);
    };

    self.StartEmpinfo = function () {
        ko.applyBindings(self, document.getElementById('EmployessInfoViewId'));
        self.LoadEmployessInfo();
    };

    self.Start = function () {
        ko.applyBindings(self, document.getElementById('EmployessViewId'));
        self.LoadRoles();
        self.LoadEmployessTitles();
    };

};