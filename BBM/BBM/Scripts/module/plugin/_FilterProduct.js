var Filter = Filter || {};
Filter.mFilter_CatalogProduct = function () {
    var self = this;
    self.Id = ko.observable();
    self.RefId = ko.observable();
    self.ChannelId = ko.observable();
    self.Name = ko.observable();
    self.Checked = ko.observable();
};
Filter.mFilter_SuppliersProduct = function () {
    var self = this;
    self.SuppliersId = ko.observable();
    self.Address = ko.observable();
    self.Phone = ko.observable();
    self.Name = ko.observable();
    self.Email = ko.observable();
    self.AccBank = ko.observable();
    self.Thue = ko.observable();
    self.Checked = ko.observable();
};
Filter.mFilter_UnitProduct = function () {
    var self = this;
    self.Id = ko.observable();
    self.Name = ko.observable();
    self.Checked = ko.observable();
};
Filter.mvFilter_Product = function (Id) {
    var self = this;
    //********************************************
    self.listCatalog = ko.observableArray([]);
    self.listSuppliers = ko.observableArray([]);
    self.listUnit = ko.observableArray([]);
    //********************************************
    self.lstCatalog_Product = ko.observableArray([]);
    self.lstSuppliers_Product = ko.observableArray([]);
    self.lstUnit_Product = ko.observableArray([]);

    self.TmplstCatalog_Product = ko.observableArray([]);
    self.TmplstSuppliers_Product = ko.observableArray([]);
    self.TmplstUnit_Product = ko.observableArray([]);
    //********************************************
    CommonUtils.loadListCatalog(function (data) {

        var newobj = ko.observable(new Filter.mFilter_CatalogProduct());
        newobj().Id(0);
        newobj().Name('Tất cả');
        self.listCatalog.push(newobj());
        ko.utils.arrayForEach(data, function (item) {
            var newobj = ko.mapping.fromJS(item, {}, new  Filter.mFilter_CatalogProduct);
            self.listCatalog.push(newobj);
        });

        self.TmplstCatalog_Product.push(self.listCatalog()[0]);
    });
    CommonUtils.loadListSuppliers(function (data) {
        var newobj = ko.observable(new Filter.mFilter_SuppliersProduct());
        newobj().SuppliersId(0);
        newobj().Name('Tất cả');
        self.listSuppliers.push(newobj());
        ko.utils.arrayForEach(data, function (item) {
            var newobj = ko.mapping.fromJS(item, {}, new Filter.mFilter_SuppliersProduct);
            self.listSuppliers.push(newobj);
        });
        self.TmplstSuppliers_Product.push(self.listSuppliers()[0]);
    });

    //********************************************

    self.TmplstCatalog_Product.subscribe(function (obj) {
        var lstId = [];
        ko.utils.arrayForEach(obj, function (o) {
            lstId.push(o.Id());
        });
        self.lstCatalog_Product(lstId);
    })

    self.TmplstSuppliers_Product.subscribe(function (obj) {
        var lstId = [];
        ko.utils.arrayForEach(obj, function (o) {
            lstId.push(o.SuppliersId());
        });
        self.lstSuppliers_Product(lstId);
    })
    self.changeCatalog = function (val) {
        if (val.Checked())
            self.lstCatalog_Product.push(val.Id());
        else
            self.lstCatalog_Product.remove(val.Id());
    };
    self.changeSuppliers = function (val) {
        if (val.Checked())
            self.lstSuppliers_Product.push(val.SuppliersId());
        else
            self.lstSuppliers_Product.remove(val.SuppliersId());
    };
    self.changeUnit = function (val) {
        if (val.Checked())
            self.lstUnit_Product.push(val.Id());
        else
            self.lstUnit_Product.remove(val.Id());
    };
    //********************************************
};