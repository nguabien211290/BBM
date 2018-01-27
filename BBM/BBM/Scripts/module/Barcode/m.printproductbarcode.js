var PrintProductBarcode = PrintProductBarcode || {}

PrintProductBarcode.mProduct = function () {
    var self = this;
    self.id = ko.observable();
    self.tensp = ko.observable();
    self.masp = ko.observable();
    self.Barcode = ko.observable();
    self.totalTem = ko.observable();
    self.price = ko.observable(0);
};

PrintProductBarcode.mConfig = function () {
    var self = this;
    self.Id = ko.observable();
    self.MarginLeftPage = ko.observable(0);
    self.MarginLeftItem = ko.observable(0);
    self.WidthItem = ko.observable(1);
    self.HeightItem = ko.observable(25);
    self.WidthFlag = ko.observable(35);
    self.HeightFlag = ko.observable(22);
    self.PageSize = ko.observable();
    self.ItemInLine = ko.observable(3);
    self.FontSize = ko.observable(9);
    self.PaddingItem = ko.observable(2);
    self.PaddingTopPage = ko.observable(0);
    self.PaddingBotPage = ko.observable(0);
    self.PaddingLeftPage = ko.observable(0);
    self.PaddingRightPage = ko.observable(0);
    self.MarginBottomItem = ko.observable(0);
    self.ListStandardPage = ko.observableArray([
         { id: 1, name: 'A4', width: 210 },//210mm
         { id: 2, name: 'A5', width: 148 },//148mm
         { id: 3, name: 'A6', width: 105 },//105mm
         { id: 4, name: 'A7', width: 74 },//74mm
         { id: 5, name: 'Tùy chỉnh', width: 0 },
    ]);
    self.SelectedStandard = ko.observable();
    self.PageSize = ko.observable();
    self.isInit = ko.observable(false);
    self.ItemInbreak = ko.observable(1);
    self.SelectedStandard.subscribe(function (val) {
        if (self.isInit())
            self.PageSize(val.width);
    });
    self.PageSize.subscribe(function (val) {
        if (!self.isInit()) {
            var page = ko.utils.arrayFirst(self.ListStandardPage(), function (o) {
                return val === o.width;
            });
            if (page)
                self.SelectedStandard(page);
            else {
                var pagecustom = ko.utils.arrayFirst(self.ListStandardPage(), function (o) {
                    return o.id == 5;
                });
                self.SelectedStandard(pagecustom);
            }
            self.isInit(true);
        }
    });
};
PrintProductBarcode.mConfig.prototype.toJSON = function () {
    return {
        Id: this.Id,
        MarginLeftPage: this.MarginLeftPage,
        MarginLeftItem: this.MarginLeftItem,
        WidthItem: this.WidthItem,
        HeightItem: this.HeightItem,
        WidthFlag: this.WidthFlag,
        HeightFlag: this.HeightFlag,
        PageSize: this.PageSize,
        ItemInLine: this.ItemInLine,
        FontSize: this.FontSize,
        PaddingItem: this.PaddingItem,
        PaddingTopPage: this.PaddingTopPage,
        PaddingBotPage: this.PaddingBotPage,
        PaddingLeftPage: this.PaddingLeftPage,
        PaddingRightPage: this.PaddingRightPage,
        MarginBottomItem: this.MarginBottomItem,
        ItemInbreak: this.ItemInbreak
    }
}
