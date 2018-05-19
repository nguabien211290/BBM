var Product = Product || {};
Product.mvProduct = function () {
    var self = this;
    self.mNewProduct = ko.observable(new Product.mProductSample);
    self.LstError = ko.observable();
    self.statusVAT = ko.observableArray(statusVAT);
    self.mDisplayProductSample = ko.observable(new Product.mDisplayProductSample());
    /*********************************Filter********************************/
    self.FilterProduct = ko.observable(new Filter.mvFilter_Search_Control('Product'));
    self.FilterProduct().classNameTab('mvProduct');
    self.FilterProduct().Fiterby.subscribe(function (val) {
        if (val && val.length > 0)
            self.CountFilter(self.CountFilter() + 1);
        else
            self.listData([]);
    });
    self.FilterProduct().KeywordSearch.subscribe(function () {
        self.CountFilter(self.CountFilter() + 1);
    });
    self.CountFilter = ko.observable(0);
    self.CurrentPage = ko.observable();
    self.ItemPerPage = ko.observable();
    self.Sortby = ko.observable();
    self.Totalitems = ko.observable();
    self.ItemPerPage.subscribe(function () {
        self.CurrentPage(1);
        self.CountFilter(self.CountFilter() + 1);
    });
    self.CurrentPage.subscribe(function () {
        self.CountFilter(self.CountFilter() + 1);
    });
    self.Sortby.subscribe(function (val) {
        self.CountFilter(self.CountFilter() + 1);
    });
    self.SearchReLoad = ko.computed(function () {
        if (self.CountFilter() > 0) {
            self.LoadListProduct()
        }
    }).extend({ throttle: 500 });
    self.LstStatusProduct = ko.observableArray(statusProducts);
    self.LoadListProduct = function () {
        self.listData([]);
        var model = {
            pageindex: self.CurrentPage(),
            pagesize: self.ItemPerPage(),
            sortby: self.Sortby(),
            sortbydesc: self.SortbyDesc(),
            keyword: self.FilterProduct().KeywordSearch(),
            filterby: self.FilterProduct().Fiterby()
        };
        //self.Sortby(undefined);
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

    /*********************************End Filter********************************/
    /*********************************Paging***********************************/

    self.isShowOptionDisplayColum = ko.observable(false);
    self.StartItem = ko.observable();
    self.listData = ko.observableArray();
    self.SortbyDesc = ko.observable();
    self.OptionShowItem = ko.observableArray([5, 10, 20, 40, 50, 100]);
    self.NumberOfPage = ko.computed(function () {
        var num = Math.ceil(self.Totalitems() / self.ItemPerPage());
        return num > 1 ? num : 1;
    }, self).extend({ notify: 'always' });
    self.HasPrevious = ko.observable(false);
    self.HasNext = ko.observable(false);
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
    /*********************************End Paging***********************************/

    /*------Start Update Price -------*/
    self.EventClick_Price = function (val) {
        ko.utils.arrayForEach(self.listData(), function (o) {
            o.product_price().IsChangePrice(false);
            o.product_price().IsChangePrice_Discount(false);
        })
        val.product_price().IsChangePrice(true);
    };

    self.EventClick_Price_Discount = function (val) {
        ko.utils.arrayForEach(self.listData(), function (o) {
            o.product_price().IsChangePrice_Discount(false);
            o.product_price().IsChangePrice(false);
        });
        debugger
        val.product_price().StartDate_Discount(val.product_price().StartDate_Discount() ? CommonUtils.ConverToDate(val.product_price().StartDate_Discount()) : new Date());
        val.product_price().Enddate_Discount(val.product_price().Enddate_Discount() ? CommonUtils.ConverToDate(val.product_price().Enddate_Discount()) : new Date());
        val.product_price().IsChangePrice_Discount(true);
    };

    self.selectedPrice = ko.observable({ "Name": "", "Code": "", "Value": 0, "Id": 0 });
    self.selectedPrice.subscribe(function (val) {
        var product = ko.utils.arrayFirst(self.listData(), function (o) {
            return o.product().id() == val.Id;
        });
        debugger
        if (product != undefined) {
            if (product.product_price().IsChangePrice())
                product.product_price().Price(val.Value);
            if (product.product_price().IsChangePrice_Discount())
                product.product_price().Price_Discount(val.Value);
        }        
    });
    self.calculator = ko.observable('+');
    self.money_percent = ko.observable('vnd');
    self.intCalculator = ko.observable(0);

    self.CalculatorPrice = function (val, type) {
        if (self.intCalculator().length <= 0 || !CommonUtils.IsNumeric(self.intCalculator()))
            return

        var disscountValue = 0;

        var intCalculator = parseInt(self.intCalculator());

        var selectedPrice = parseInt(self.selectedPrice().Value);

        if (self.calculator() == "+") {
            if (self.money_percent() == "percent") {
                disscountValue = self.selectedPrice().Value + ((intCalculator / 100) * selectedPrice);
            }
            else
                disscountValue = selectedPrice + intCalculator
        }

        if (self.calculator() == "-") {
            if (self.money_percent() == "percent") {
                disscountValue = selectedPrice - ((intCalculator / 100) * selectedPrice);
            }
            else
                disscountValue = selectedPrice - intCalculator;
        }
        if (type == 'PriceChannel')
            val.product_price().Price(disscountValue);
        if (type == 'PriceChannel_Disscount')
            val.product_price().Price_Discount(disscountValue);
    };

    self.EventChange_InputPrice = function (val) {
        self.CalculatorPrice(val, 'PriceChannel');
    };
    self.EventChange_InputPriceDisscount = function (val) {
        debugger
        self.CalculatorPrice(val, 'PriceChannel_Disscount');
    };
    self.SubmitPrice = function (val) {
        CommonUtils.showWait(true);
        val.product_price().ProductId(val.product().id());
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Product/ChangePrice"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(val.product_price()),
        }).done(function (data) {

            self.CountFilter(self.CountFilter() + 1);

            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };

    self.SubmitPrice_Discount = function (val) {
        CommonUtils.showWait(true);
        val.product_price().ProductId(val.product().id());
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Product/ChangePrice_Discount"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(val.product_price()),
        }).done(function (data) {

            self.CountFilter(self.CountFilter() + 1);

            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };

    /*------End Update Price ------- */
    /*------Start Get, Update Add Product-------*/
    self.GetDetail = function (obj) {
        ko.utils.arrayForEach(self.listData(), function (v) {
            v.product().IsEdit(false)
        })
        obj.product().IsEdit(true);
    };
    self.UpdateProduct = function (obj) {
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Product/UpdateProduct"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(obj.product()),
        }).done(function (data) {
            self.LoadListProduct();
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.AddProductModal = function () {
        self.AddProduct(self.mNewProduct());
    }
    self.AddProduct = function (obj) {
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Product/CreateProduct"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(self.mNewProduct()),
        }).done(function (data) {
            if (!data.isError) {
                self.Sortby("DateCreate");
                self.SortbyDesc(true);
                CommonUtils.closeModal('#AddProductModal')
                self.mNewProduct(new Product.mProductSample);
                self.LstError(undefined);
            } else {
                self.LstError(data.messaging);
            }
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.DeleteProduct = function (val) {
        CommonUtils.confirm("Thông báo", "Bạn có muốn xóa sản phẩm " + val.product().tensp(), function () {
            CommonUtils.showWait(true);
            $.ajax({
                type: "POST",
                url: CommonUtils.url("/Product/DeleteProduct"),
                cache: false,
                data: { id: val.product().id() },
            }).done(function (data) {
                self.LoadListProduct();
                CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
            }).always(function () {
                CommonUtils.showWait(false);
            });
        })

    };
    /*------End Update Add Product-------*/
    /*---------------Start Import-----------------------*/
    self.fileName = ko.observable();
    self.fileError = ko.observableArray([]);
    self.Percent_Process = ko.observable('0');
    self.Import_Excel = function (formData) {
        self.fileError([]);
        CheckData();
        self.IsImport(true);
        CommonUtils.showWait(true);
        $.ajax({
            url: CommonUtils.url("/Import/Import_Product"),
            type: "POST",
            data: formData,
            dataType: "json",
            cache: false,
            contentType: false,
            processData: false
        }).done(function (data) {
            if (!data.isError) {
                self.LoadListProduct();
            } else {
                if (data && data.Data && data.Data.length > 0) {
                    self.fileError(data.Data);
                }
            }
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        })
            .always(function (result) {
                self.fileName(undefined);
                CommonUtils.showWait(false);
                self.IsImport(false);
            });
    };

    var inteval_sync = null;
    function CheckData(val) {
        if (inteval_sync == null)
            inteval_sync = setInterval(function () {
                self.CheckSyncData();
            }, 1000);

        if (val) {
            clearInterval(inteval_sync);
            inteval_sync = null;
        }
    };

    self.CheckSyncData = function () {
        $.ajax({
            type: "GET",
            url: CommonUtils.url("/Common/GetConfigSys"),
            cache: false,
            dataType: 'json',
        }).done(function (data) {
            if (!data.isError) {
                common.mConfig(ko.mapping.fromJS(data.Data.Config, {}, new Common.mConfig));
                if (common.mConfig().Error_Import() != null) {
                    var error = JSON.parse(common.mConfig().Error_Import());
                    self.fileError(error);
                }
                self.Percent_Process(common.mConfig().Percent_Process() + '%');
            } else
                CommonUtils.notify("Thông báo", data.messaging, 'error');
        }).always(function () {
            if (!common.mConfig().Is_import()) {
                self.IsImport(false);
                CheckData(true);
            }
        });;
    };
    self.IsImport = ko.observable(false);
    /*---------------End Import-----------------------*/

    self.Resfesh = function () {
        self.CountFilter(self.CountFilter() + 1);
    };

    self.Start = function () {
        ko.applyBindings(self, document.getElementById('ProductViewId'));
        if (common.mConfig().Is_import()) {
            self.IsImport(true);
            CheckData();
        }
    };
};