var Employess = Employess || {};
Employess.mvEmployess = function () {
    var self = this;
    self.TmpTable = ko.observable(new Paging_TmpControltool());
    self.TmpTable().CurrentPage.subscribe(function () {
        self.LoadListEmployess();
    });
    self.TmpTable().ItemPerPage.subscribe(function () {
        self.TmpTable().CurrentPage(1);
        self.LoadListEmployess();
    });
    self.TmpTable().KeywordSearch.subscribe(function () {
        self.LoadListEmployess();
    });
    self.TmpTable().Sortby.subscribe(function (val) {
        if (val)
            self.LoadListEmployess();
    });
    self.IsReCreatePwd = ko.observable(false);

    self.TmpTable().nameTemplate('table_Employess');
    self.LoadListEmployess = function () {
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
            url: CommonUtils.url("/Employess/GetEmployessby"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({ pageinfo: model }),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.TmpTable().listData(CommonUtils.MapArray(data.Data.listTable, Employess.mEmployess));
                self.TmpTable().Totalitems(data.Data.totalItems);
                self.TmpTable().StartItem(data.Data.startItem);
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

        ko.utils.arrayForEach(self.TmpTable().listData(), function (val) {
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

        //if (val.Roles() != null) {

        //    ko.utils.arrayForEach(self.TmpTable().listData(), function (employ) {
        //        employ.IsViewDetail(false);
        //    })
        //    val.IsViewDetail(true);
        //}
        //else {
        //    ko.utils.arrayForEach(self.TmpTable().listData(), function (v) {
        //        v.IsViewDetail(false);
        //    });
        //    ko.utils.arrayForEach(self.lstRoles(), function (obj) {
        //        ko.utils.arrayForEach(obj.mGroupRoles(), function (obj2) {
        //            ko.utils.arrayForEach(obj2.Roles(), function (obj3) {
        //                obj3.isSelect(false);
        //            });
        //        });
        //    });
        //    val.IsViewDetail(true);
        //}

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
                self.LoadListEmployess();
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
    self.Start = function () {
        ko.applyBindings(self, document.getElementById('EmployessViewId'));
        self.LoadListEmployess();
        self.LoadRoles();
        self.LoadEmployessTitles();
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
                self.LoadListEmployess();
            }).always(function () {
                CommonUtils.showWait(false);
            });
        });
    };
    self.StartEmpinfo = function () {
        ko.applyBindings(self, document.getElementById('EmployessInfoViewId'));
        self.LoadEmployessInfo();
    };

};