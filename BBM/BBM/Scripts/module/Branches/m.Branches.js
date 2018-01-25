var Branches = Branches || {};
Branches.mBranches = function () {
    var self = this;
    self.BranchesId = ko.observable();
    self.Code = ko.observable();
    self.BranchesName = ko.observable();
    self.Phone = ko.observable();
    self.Address = ko.observable();
    self.IsViewDetail = ko.observable(false);
    self.soft_Channel = ko.observableArray();
    self.IsPrimary = ko.observable(false);
};

Branches.mChannel = function () {
    var self = this;
    self.Id = ko.observable();
    self.Type = ko.observable();
    self.Code = ko.observable();
    self.Channel = ko.observable();
    self.BranchesId = ko.observable();
    self.Note = ko.observable();
    self.IsUpdate = ko.observable(false);
};


