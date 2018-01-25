var Suppliers = Suppliers || {};
Suppliers.mSuppliers = function () {
    var self = this;
    self.SuppliersId = ko.observable();
    self.Name = ko.observable();
    self.Address = ko.observable();
    self.Phone = ko.observable();
    self.Email = ko.observable();
    self.AccBank = ko.observable();
    self.EmployeeCreate = ko.observable();
    self.DateCreate = ko.observable();
    self.EmployeeUpdate = ko.observable();
    self.DateUpdate = ko.observable();
    self.IsViewDetail = ko.observable(false);
};
