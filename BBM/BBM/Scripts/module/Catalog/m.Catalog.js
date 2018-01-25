var Catalog = Catalog || {};
Catalog.mCatalog = function () {
    var self = this;
    self.Id = ko.observable();
    self.Name = ko.observable();
    self.IsViewDetail = ko.observable(false);
};
