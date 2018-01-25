var Customer = Customer || {};
Customer.mCustomer = function () {
    var self = this;
    self.Code = ko.observable();
    self.Id = ko.observable();
    self.Name = ko.observable();
    self.Email = ko.observable();
    self.Phone = ko.observable();
    self.Address = ko.observable();
    self.DistrictId = ko.observable();
    self.ProvinceId = ko.observable();
    self.Orders = ko.observableArray();
    self.IsViewDetail = ko.observable(false);
    self.User = ko.observable();
    self.Mark = ko.observable(0);
};

