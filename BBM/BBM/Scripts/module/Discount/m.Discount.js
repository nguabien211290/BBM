var Discount = Discount || {};
Discount.mDiscount = function () {
    var self = this;   
    self.Id = ko.observable();  
    self.Code = ko.observable();
    self.Name = ko.observable();
    self.Startdate = ko.observable(new Date());
    self.Enddate = ko.observable(new Date());
    self.Type = ko.observable();
    self.Total = ko.observable();
    self.IsNotExp = ko.observable(false);
    self.Value = ko.observable();
    self.Disable = ko.observable();
    self.EmployeeCreate = ko.observable();
    self.DateCreate = ko.observable();
    self.EmployeeUpdate = ko.observable();
    self.DateUpdate = ko.observable();
};

