var Filter = Filter || {};
Filter.mFilter_CatalogProduct = function () {
    var self = this;
    self.Id = ko.observable();
    self.RefId = ko.observable();
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

Filter.mFilterModel = function () {
    var self = this;
    self.Fiter = ko.observable();
    self.FiterDisplay = ko.observable();
    self.Name = ko.observable();
    self.Value = ko.observable();
    self.Value2 = ko.observable();
    self.StartDate = ko.observable();
    self.EndDate = ko.observable();
    self.Type = ko.observable();
    self.IsDisplay = ko.observable(true);
};

Filter.mvFilter_Search_Control = function (type, isNotFilter) {
    var self = this;
    //********************************************
    self.CountFilter = ko.observable(0);
    self.SearchReLoad = ko.computed(function () {
        if (self.CountFilter() > 0) {
            self.LoadListProduct()
        }
    }).extend({ throttle: 1000 });
    self.typeFilter = ko.observable(type);
    self.isNotFilter = ko.observable(isNotFilter);
    self.typeChildFilter = ko.observable();
    self.listCatalog = ko.observableArray([]);
    self.listSuppliers = ko.observableArray([]);
    self.listUnit = ko.observableArray([]);
    self.listStatus = ko.observableArray([]);
    self.listStatusVAT = ko.observableArray([]);
    self.LstBranches = ko.observableArray();
    self.listEmployeeShip = ko.observableArray();
    self.listCompare = ko.observableArray([{ Name: 'Bằng', Value: 'Equals' }, { Name: 'Lớn hơn', Value: 'MoreThan' }, { Name: 'Nhỏ hơn', Value: 'LessThan' }])
    self.selectCompare = ko.observable();

    self.Fiterby = ko.observableArray([]);
    self.StartdateFilter = ko.observable(new Date());
    self.EndDateFilter = ko.observable(new Date());
    self.ValueFilter = ko.observable();


    self.BranchesSelect = ko.observable();
    self.classNameTab = ko.observable(type);
    //********************************************
    if (self.typeFilter() == 'Product' || self.typeFilter() == 'Product_Control') {
        CommonUtils.loadListCatalog(function (data) {
            var newobj = ko.observable(new Filter.mFilter_CatalogProduct());
            newobj().Id(0);
            newobj().Name('Tất cả');
            self.listCatalog.push(newobj());
            ko.utils.arrayForEach(data, function (item) {
                var newobj = ko.mapping.fromJS(item, {}, new Filter.mFilter_CatalogProduct);
                self.listCatalog.push(newobj);
            });
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
        });
        CommonUtils.LoadLstBranches(function (data) {
            self.LstBranches(data);
        });
        self.listStatus(statusProducts);
        self.listStatusVAT(statusVAT);
        self.ValueFilter(0);
    }
    if (self.typeFilter() == 'Order_Output'
        || self.typeFilter() == 'Order_Suppliers'
        || self.typeFilter() == "Order_Input") {
        switch (self.typeFilter()) {
            case "Order_Output":
                self.listStatus(statusOrder_Output);
                break;
            case "Order_Input":
                self.listStatus(statusOrder_Input);
                break;
            case "Order_Suppliers":
                self.listStatus(statusOrder_Suppliers);
                break;
        }
        CommonUtils.LoadLstBranches(function (data) {
            self.LstBranches(data);
        });
        CommonUtils.loadListSuppliers(function (data) {
            self.listSuppliers(data);
        });
    }
    if (self.typeFilter() == 'Order_Channel') {
        self.listStatus(statusOrder_Sale);
        CommonUtils.loadLstLoadEmployes(function (data) {
            self.listEmployeeShip(data);
        });
    }

    //********************************************
    //**********************Paging **********************
    self.OptionShowItem = ko.observableArray([{ key: 5, label: '10 dòng' }, { key: 20, label: '20 dòng' }, { key: 40, label: '40 dòng' }, { key: 100, label: '100 dòng' }]);
    self.Sortby = ko.observable();
    self.SortbyDesc = ko.observable(true);
    self.ItemPerPage = ko.observable(5);
    self.listData = ko.observableArray();
    self.KeywordSearch = ko.observable();
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
    self.Fiterby.subscribe(function (val) {
        if (self.typeFilter() == 'Product_Control') {
            self.CountFilter(self.CountFilter() + 1);
        }
    });

    self.KeywordSearch.subscribe(function () {
        if (self.typeFilter() == 'Product_Control')
            self.CountFilter(self.CountFilter() + 1);
    });

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
    self.AddToList = function (val) {
        self.isCheckProduct([]);
        self.isCheckProduct.push(val.product().id());
    };
    self.isCheckProduct = ko.observableArray();
    self.LoadListProduct = function () {
        self.listData([]);
        self.isCheckProduct([]);
        var model = {
            pageindex: self.CurrentPage(),
            pagesize: self.ItemPerPage(),
            keyword: self.KeywordSearch(),
            sortby: self.Sortby(),
            sortbydesc: self.SortbyDesc(),
            filterby: self.Fiterby()
        };
        self.Sortby(undefined);
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Product/GetProductby"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({ pageinfo: model }),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                //var lstTmp = CommonUtils.MapArray(data.Data.listTable, Filter.mProductSample);
                //self.listData(lstTmp);
                //self.Totalitems(data.Data.totalItems);
                //self.StartItem(data.Data.startItem);

                var lstTmp = CommonUtils.MapArray(data.Data.listTable, Product.mProduct);
                ko.utils.arrayForEach(lstTmp, function (item) {
                    item.product(ko.mapping.fromJS(item.product(), {}, new Product.mProductSample));
                    item.product_price(ko.mapping.fromJS(item.product_price(), {}, new Product.mProduct_Price));
                    item.product_stock(ko.mapping.fromJS(item.product_stock(), {}, new Product.mProduct_Stock));
                    item.product_price().OptionPrice([
                        { "Name": "Giá Kênh", "Code": "PriceNow", "Value": item.product_price().Price(), "Id": item.product_price().ProductId() },
                        { "Name": "Giá cơ bản", "Code": "Price", "Value": item.product().PriceBase(), "Id": item.product_price().ProductId() },
                        { "Name": "Giá nhập cũ", "Code": "PriceOld", "Value": item.product().PriceBase_Old(), "Id": item.product_price().ProductId() },
                        { "Name": "Giá nhập mới", "Code": "PriceNew", "Value": item.product().PriceInput(), "Id": item.product_price().ProductId() },
                        { "Name": "Giá tham chiếu", "Code": "PriceCompare", "Value": item.product().PriceCompare(), "Id": item.product_price().ProductId() },
                        { "Name": "Giá cửa hàng", "Code": "PriceMainStore", "Value": item.product().PriceMainStore(), "Id": item.product_price().ProductId() }

                    ]);


                });
                self.listData(lstTmp);
                self.Totalitems(data.Data.totalItems);
                self.StartItem(data.Data.startItem);


            } else
                CommonUtils.notify("Thông báo", data.messaging, 'error');

        }).always(function () {
            CommonUtils.showWait(false);
        });
    };

    /****************************************************************************/
    self.PushTagFilter = function (val, type) {
        if (self.typeFilter() == "Product" || self.typeFilter() == "Product_Control") {
            if (type == "Catalog") {
                var hasFil = ko.utils.arrayFirst(self.Fiterby(), function (o) {
                    return o.Fiter == type && o.Value == val.Id();
                });
                if (!hasFil) {
                    var newobject = new Filter.mFilterModel();
                    newobject.Fiter = type;
                    newobject.FiterDisplay = "Nhóm" + ": " + val.Name();
                    newobject.Name = val.Name();
                    newobject.Value = val.Id();
                    self.Fiterby.push(newobject);
                }
            }
            if (type == "Suppliers") {
                var hasFil = ko.utils.arrayFirst(self.Fiterby(), function (o) {
                    return o.Fiter == type && o.Value == val.SuppliersId();
                });
                if (!hasFil) {
                    var newobject = new Filter.mFilterModel();
                    newobject.Fiter = type;
                    newobject.FiterDisplay = "NPP" + ": " + val.Name();
                    newobject.Name = val.Name();
                    newobject.Value = val.SuppliersId();
                    self.Fiterby.push(newobject);
                }
            }
            if (type == "Status") {
                var hasFil = ko.utils.arrayFirst(self.Fiterby(), function (o) {
                    return o.Fiter == type && o.Value == val.Key;
                });
                if (!hasFil) {
                    var newobject = new Filter.mFilterModel();
                    newobject.Fiter = type;
                    newobject.FiterDisplay = "Trạng thái" + ": " + val.Value;
                    newobject.Name = val.Value;
                    newobject.Value = val.Key;
                    self.Fiterby.push(newobject);
                }
            }
            if (type == "VAT") {
                var hasFil = ko.utils.arrayFirst(self.Fiterby(), function (o) {
                    return o.Fiter == type && o.Value == val.Key;
                });
                if (!hasFil) {
                    var newobject = new Filter.mFilterModel();
                    newobject.Fiter = type;
                    newobject.FiterDisplay = "VAT" + ": " + val.Value;
                    newobject.Name = val.Value;
                    newobject.Value = val.Key;
                    self.Fiterby.push(newobject);
                }
            }
            if (type == "Price") {
                var hasFil = ko.utils.arrayFirst(self.Fiterby(), function (o) {
                    return o.Fiter == type && o.Value == self.selectCompare().Value && o.Name == self.ValueFilter();
                });
                if (!hasFil) {
                    var newobject = new Filter.mFilterModel();
                    newobject.Fiter = type;
                    newobject.FiterDisplay = "Giá kênh " + self.selectCompare().Name + " " + CommonUtils.textMoneyDefaultSymbol(self.ValueFilter());
                    newobject.Name = self.ValueFilter();
                    newobject.Value = self.selectCompare().Value;
                    self.Fiterby.push(newobject);
                }
                self.ValueFilter(0);
                CommonUtils.closeModal('#Order_ChannelPriceFilteModal' + self.classNameTab());
            }
            if (type == "Stock") {
                var hasFil = ko.utils.arrayFirst(self.Fiterby(), function (o) {
                    return o.Fiter == type && o.Value == self.selectCompare().Value
                        && o.Name == self.ValueFilter()
                        && o.Value2 == self.BranchesSelect().BranchesId;
                });
                if (!hasFil) {
                    var newobject = new Filter.mFilterModel();
                    newobject.Fiter = type;
                    newobject.FiterDisplay = "Tồn kho của Kho " + self.BranchesSelect().BranchesName + " " + self.selectCompare().Name + " " + self.ValueFilter();
                    newobject.Name = self.ValueFilter();
                    newobject.Value = self.selectCompare().Value;
                    newobject.Value2 = self.BranchesSelect().BranchesId;
                    self.Fiterby.push(newobject);
                }
                self.ValueFilter(0);
                CommonUtils.closeModal('#Order_ChannelPriceFilteModal' + self.classNameTab());
            }
            if (type == "Order_Channel") {
                var hasFil = ko.utils.arrayFirst(self.Fiterby(), function (o) {
                    return o.Fiter == type;
                });
                if (hasFil) {
                    self.Fiterby.remove(hasFil);
                } else {
                    var braches = ko.utils.arrayFirst(self.LstBranches(), function (o) {
                        return o.BranchesId == self.BranchesSelect().BranchesId;
                    });
                    var newobject = new Filter.mFilterModel();
                    newobject.FiterDisplay = "Bán hàng theo";
                    newobject.Fiter = "Order_Channel";
                    newobject.Name = 'Đơn đặt hàng của kho ' + braches.BranchesName;
                    newobject.Value = self.BranchesSelect().BranchesId;
                    newobject.Type = 'Date';
                    newobject.StartDate = self.StartdateFilter();
                    newobject.EndDate = self.EndDateFilter();
                    self.Fiterby.push(newobject);
                }
                CommonUtils.closeModal('#Order_ChannelFilteModal' + self.classNameTab());
            }
        }
        if (self.typeFilter() == "Order_Output"
            || self.typeFilter() == "Order_ProductSwitch"
            || self.typeFilter() == "Order_Input"
            || self.typeFilter() == 'Order_Channel'
            || self.typeFilter() == 'Order_Suppliers'
        ) {
            if (type == "Suppliers") {
                var hasFil = ko.utils.arrayFirst(self.Fiterby(), function (o) {
                    return o.Fiter == type && o.Value == val.SuppliersId;
                });
                if (!hasFil) {
                    var newobject = new Filter.mFilterModel();
                    newobject.Fiter = type;
                    newobject.FiterDisplay = "NPP" + ": " + val.Name;
                    newobject.Name = val.Name;
                    newobject.Value = val.SuppliersId;
                    self.Fiterby.push(newobject);
                }
            }
            if (type == "EmployeeShip") {
                var hasFil = ko.utils.arrayFirst(self.Fiterby(), function (o) {
                    return o.Fiter == type && o.Value == val.Key;
                });
                if (!hasFil) {
                    var newobject = new Filter.mFilterModel();
                    newobject.Fiter = type;
                    newobject.FiterDisplay = "Nhân viên giao hàng" + ": " + val.Name;
                    newobject.Name = val.Name;
                    newobject.Value = val.Id;
                    self.Fiterby.push(newobject);
                }
            }
            if (type == "EmployeeCreate") {
                var hasFil = ko.utils.arrayFirst(self.Fiterby(), function (o) {
                    return o.Fiter == type && o.Value == val.Key;
                });
                if (!hasFil) {
                    var newobject = new Filter.mFilterModel();
                    newobject.Fiter = type;
                    newobject.FiterDisplay = "Nhân viên bán hàng" + ": " + val.Name;
                    newobject.Name = val.Name;
                    newobject.Value = val.Id;
                    self.Fiterby.push(newobject);
                }
            }
            if (type == "Status") {
                var hasFil = ko.utils.arrayFirst(self.Fiterby(), function (o) {
                    return o.Fiter == type && o.Value == val.Key;
                });
                if (!hasFil) {
                    var newobject = new Filter.mFilterModel();
                    newobject.Fiter = type;
                    newobject.FiterDisplay = "Trạng thái" + ": " + val.Value;
                    newobject.Name = val.Value;
                    newobject.Value = val.Key;
                    self.Fiterby.push(newobject);
                }
            }
            if (type == "Brach_To") {
                var hasFil = ko.utils.arrayFirst(self.Fiterby(), function (o) {
                    return o.Fiter == type && o.Value == val.Key;
                });
                if (!hasFil) {
                    var newobject = new Filter.mFilterModel();
                    newobject.Fiter = type;
                    newobject.FiterDisplay = "Kho đến" + ": " + val.BranchesName;
                    newobject.Name = val.BranchesName;
                    newobject.Value = val.BranchesId;
                    self.Fiterby.push(newobject);
                }
            }
            if (type == "Brach_From") {
                var hasFil = ko.utils.arrayFirst(self.Fiterby(), function (o) {
                    return o.Fiter == type && o.Value == val.Key;
                });
                if (!hasFil) {
                    var newobject = new Filter.mFilterModel();
                    newobject.Fiter = type;
                    newobject.FiterDisplay = "Từ kho" + ": " + val.BranchesName;
                    newobject.Name = val.BranchesName;
                    newobject.Value = val.BranchesId;
                    self.Fiterby.push(newobject);
                }
            }
            if (type == "Time_To_From") {
                var hasFil = ko.utils.arrayFirst(self.Fiterby(), function (o) {
                    return o.Fiter == type;
                });
                if (hasFil) {
                    self.Fiterby.remove(hasFil);
                } else {
                    var newobject = new Filter.mFilterModel();
                    newobject.Fiter = type;
                    newobject.Type = 'Date';
                    newobject.StartDate = self.StartdateFilter();
                    newobject.EndDate = self.EndDateFilter();
                    self.Fiterby.push(newobject);
                }
                CommonUtils.closeModal('#Order_ChannelFilteModal' + self.classNameTab());
            }
        }
    };
    self.RemoveTagFilter = function (val) {
        self.Fiterby.remove(val);
    };
    self.FilterChange = ko.observable(1).extend({ rateLimit: { timeout: 500, method: "notifyWhenChangesStop" } });
};