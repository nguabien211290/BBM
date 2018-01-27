var PrintProductBarcode = PrintProductBarcode || {}
PrintProductBarcode.mvBarcode = function () {
    var self = this;
    self.FilterProduct = ko.observable(new Filter.mvFilter_Search_Control('Product_Control'));
    self.mlstBarcode = ko.observableArray();
    self.mConfig = ko.observable(new PrintProductBarcode.mConfig);
    self.FilterProduct().classNameTab('mvOBarcode');
    self.FilterProduct().isCheckProduct.subscribe(function (val) {
        ko.utils.arrayForEach(val, function (o) {
            var hasExits = ko.utils.arrayFirst(self.mlstBarcode(), function (b) {
                return b.id() == o
            })
            if (!hasExits)
                self.GetProduct(o);
        })
    });
    self.GetProduct = function (val) {
        CommonUtils.showWait(true, "BarcodeViewId");
        $.ajax({
            type: "GET",
            url: CommonUtils.url("/Partial/GetProductbyId"),
            cache: false,
            data: { productId: val },
        }).done(function (data) {
            if (data.isError) {
                CommonUtils.notify("Thông báo", data.Data.result, 'error');
            }
            else {
                var obj = ko.mapping.fromJS(data.Data.result.product, {}, new PrintProductBarcode.mProduct);
                if (data.Data.result.product_price)
                    obj.price(data.Data.result.product_price.Price);
                self.mlstBarcode.push(obj);
            }
        }).always(function () {
            CommonUtils.showWait(false,"BarcodeViewId");
        });

    };
    self.RemoveItem = function (val) {
        self.mOrderInput().Detail.remove(val)
    };
    self.SaveConfig = function () {
        CommonUtils.showWait(true,"BarcodeViewId");
        var objectData = ko.toJSON({ model: self.mConfig() });

        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Barcode/SaveConfig"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(self.mConfig())
        }).done(function (data) {
            self.GetConfig();
            CommonUtils.notify("Thông báo", data.messaging, !data.isError ? 'success' : 'error');
        }).always(function () {
            CommonUtils.showWait(false,"BarcodeViewId");
        });
    };
    self.GetConfig = function () {
        CommonUtils.showWait(true,"BarcodeViewId");
        $.ajax({
            type: "GET",
            url: CommonUtils.url("/Barcode/GetConfig"),
            cache: false,
        }).done(function (data) {
            if (data.Data) {
                self.mConfig(ko.mapping.fromJS(data.Data, {}, new PrintProductBarcode.mConfig));
            }
        }).always(function () {
            CommonUtils.showWait(false,"BarcodeViewId");
        });
    }
    self.printBarcode = function () {
        self.PrintIt();
    };

    self.ShowType = ko.observable("Setting");
    self.NeedPrint = ko.observableArray();
    self.PrintIt = function () {
        debugger
        self.NeedPrint([]);
        var barcode = [];
        ko.utils.arrayForEach(self.mlstBarcode(), function (xitem) {
            for (var i = 0; i < xitem.totalTem() ; i++)
                barcode.push(xitem);
        })
        if (barcode.length > 0) {

            self.NeedPrint(CommonUtils.chunkArray(barcode, self.mConfig().ItemInbreak()));

            self.ShowType("Standard");
        }
    };
    self.removeItem = function (obj) {
        self.mlstBarcode.remove(function (hasThis) { return hasThis.id() == obj.id() });
    };
    self.renderStyleStandard = function () {
        $(".cpage").css({ "width": self.mConfig().PageSize() + "mm" });
        $(".cpage").css({ "padding-top": self.mConfig().PaddingTopPage() + "mm" });
        $(".cpage").css({ "padding-bottom": self.mConfig().PaddingBotPage() + "mm" });
        $(".cpage").css({ "padding-left": self.mConfig().PaddingLeftPage() + "mm" });
        $(".cpage").css({ "padding-right": self.mConfig().PaddingRightPage() + "mm" });

        $(".barcode-group").css({ "margin-left": self.mConfig().MarginLeftPage() + "mm" });
        $(".barcode-group").css({ "margin-bottom": self.mConfig().MarginBottomItem() + "mm" });
        $(".print_barcode_img").css({ "margin-left": self.mConfig().MarginLeftItem() + "mm" });
        if (self.mConfig().FontSize() > 0)
            $(".barcode-group").css({
                "font-size": self.mConfig().FontSize() + "px"
            });
        $(".barcode-group").css({
            "padding": self.mConfig().PaddingItem() + "mm"
        });
        $(".review .barcode-group").css({
            "width": self.mConfig().WidthFlag() + "mm", "height": self.mConfig().HeightFlag() + "mm"
        });

        $(".barcode-group").find('.headerbarcode > div').css({ "text-align": "center" });
        $(".barcode-group").find('.headerbarcode > div').css({ "line-height": self.mConfig().FontSize() + "px" });
    }
    self.afterRenderPrintReviewPage = function () {
        self.renderStyleStandard();
    };
    self.settingsBarcode = function () {
        return {
            output: "css",
            barWidth: self.mConfig().WidthItem(),
            barHeight: self.mConfig().HeightItem(),
        };
    };
    self.PrintBarcode = function () {
        CommonUtils.PrintDiv("printbarcode");
    };
    self.Start = function () {
        ko.applyBindings(self, document.getElementById('BarcodeViewId'));
        self.GetConfig();
    };
}