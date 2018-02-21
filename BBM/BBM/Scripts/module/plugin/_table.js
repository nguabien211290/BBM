var Paging_TmpControltool = function (nameView, IsFiter) {
    var self = this;
    self.IsFiter = ko.observable(IsFiter);
    self.CountFilter = ko.observable(0);
    self.Filter_Search = ko.observable(new Filter.mvFilter_Search_Control(nameView));
    self.OptionShowItem = ko.observableArray([20, 50, 100]);
    self.KeywordSearch = ko.observable();
    self.Sortby = ko.observable();
    self.SortbyDesc = ko.observable(true);
    self.ItemPerPage = ko.observable();
    self.listData = ko.observableArray();
    self.StartItem = ko.observable(1);
    self.Totalitems = ko.observable(0);
    self.CurrentPage = ko.observable(1);
    self.TotalMoney = ko.observable(0);

    self.nameView = ko.observable(nameView);
    self.NumberOfPage = ko.computed(function () {
        var num = Math.ceil(self.Totalitems() / self.ItemPerPage());
        return num > 1 ? num : 1;
    }, self).extend({ notify: 'always' });;
    self.HasPrevious = ko.observable(false);
    self.HasNext = ko.observable(false);
    self.nameTemplate = ko.observable(nameView);
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
    self.CurrentPage.subscribe(function () {
        self.CountFilter(self.CountFilter() + 1);
    });
    self.ItemPerPage.subscribe(function () {
        self.CurrentPage(1);
        self.CountFilter(self.CountFilter() + 1);
    });
    self.Sortby.subscribe(function (val) {
        if (val)
            self.CountFilter(self.CountFilter() + 1);
    });
    self.Filter_Search().Fiterby.subscribe(function (val) {
        if (val && val.length > 0)
            self.CountFilter(self.CountFilter() + 1);
        else
            self.listData([]);
    });
    self.Filter_Search().KeywordSearch.subscribe(function () {
        self.CountFilter(self.CountFilter() + 1);
    });

    self.Model = ko.computed(function () {
        return {
            pageindex: self.CurrentPage(),
            pagesize: self.ItemPerPage(),
            sortby: self.Sortby(),
            sortbydesc: self.SortbyDesc(),
            filterby: self.Filter_Search().Fiterby()
        };
    });
};
