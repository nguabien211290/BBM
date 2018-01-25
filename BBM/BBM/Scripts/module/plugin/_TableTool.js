var Paging_TmpControltool = function () {
    var self = this;
    self.OptionShowItem = ko.observableArray([5, 10, 15]);
    self.KeywordSearch = ko.observable();
    self.Sortby = ko.observable();
    self.SortbyDesc = ko.observable(true);   
    self.ItemPerPage = ko.observable(15);
    self.listData = ko.observableArray();
    self.StartItem = ko.observable(1);
    self.Totalitems = ko.observable(0);
    self.CurrentPage = ko.observable(1);
    self.NumberOfPage = ko.computed(function () {
        var num = Math.ceil(self.Totalitems() / self.ItemPerPage());
        return num > 1 ? num : 1;
    }, self).extend({ notify: 'always' });;
    self.HasPrevious = ko.observable(false);
    self.HasNext = ko.observable(false);
    self.nameTemplate = ko.observable();
    self.ChangePage = function (page) {
        if (page > 0 && page <= self.NumberOfPage()) {
            self.CurrentPage(page);
            if (self.CurrentPage() > 1) {
                self.HasPrevious(true);
            }
            else {
                self.HasPrevious(false);
            }
            if (self.CurrentPage() >= self.NumberOfPage()) {
                self.HasNext(false);
            }
            else {
                self.HasNext(true);
            }
        }
    }
};
