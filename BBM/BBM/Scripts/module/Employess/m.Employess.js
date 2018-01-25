var Employess = Employess || {};
Employess.mEmployess = function () {
    var self = this;
    self.Id = ko.observable();
    self.Name = ko.observable();
    self.Titles = ko.observable();
    self.Email = ko.observable();
    self.Pwd = ko.observable();
    self.Phone = ko.observable();
    self.Address = ko.observable();
    self.EmployeeCreate = ko.observable();
    self.DateCreate = ko.observable();
    self.EmployeeUpdate = ko.observable();
    self.DateUpdate = ko.observable();
    self.IsViewDetail = ko.observable(false);
    self.PwdNew = ko.observable();
    self.PwdRep = ko.observable();
    self.Roles = ko.observable();
    self.GroupRoles = ko.observable();
    self.IsDisable = ko.observable();
    self.isUpdatePwd = ko.observable();
};

Employess.mEmployessTitle = function () {
    var self = this;
    self.Id = ko.observable();
    self.Name = ko.observable();
};
Employess.mRoleBrand = function () {
    var self = this;
    self.BrandId = ko.observable();
    self.BrandName = ko.observable();
    self.mGroupRoles = ko.observable();
};
Employess.mGroupRoles = function () {
    var self = this;
    self.Id = ko.observable();
    self.Name = ko.observable();
    self.Roles = ko.observableArray();
    self.isSelectGroup = ko.observable();
    self.isSelectGroup.subscribe(function (val) {
        if (self.Roles().length > 0) {
            ko.utils.arrayForEach(self.Roles(), function (val) {
                val.isSelect(val);
            });
        }
    })
};
Employess.mRoles = function () {
    var self = this;
    self.Id = ko.observable();
    self.Name = ko.observable();
    self.isSelect = ko.observable();
};

