var Product = Product || {};
Product.mProduct = function () {
    var self = this;
    self.product = ko.observable();
    self.product_price = ko.observable();
    self.product_stock = ko.observable();
    self.product_stocks = ko.observableArray();
};
Product.mProductSample = function () {
    var self = this;
    self.Id = ko.observable();

    self.CatalogId = ko.observable();
    self.SuppliersId = ko.observable();
    self.UnitId = ko.observable();
    self.Barcode = ko.observable();
    self.masp = ko.observable();
    self.tensp = ko.observable();
    self.Detail_Info = ko.observable();
    self.PriceBase = ko.observable();
    self.PriceBase_Old = ko.observable();
    self.PriceCompare = ko.observable();
    self.PriceInput = ko.observable();
    self.PriceWholesale = ko.observable();
    self.PriceMainStore = ko.observable();

    self.HasInChannel = ko.observable(false);
    self.IsEdit = ko.observable(false);
    self.Img = ko.observable();
    self.Note = ko.observable();
    self.Status = ko.observable();
    self.StatusVAT = ko.observable();

    self.Stock_Sum = ko.observable(0);
};
Product.mProduct_Price = function () {
    var self = this;
    self.Id = ko.observable();
    self.ProductId = ko.observable();
    self.IdKenh = ko.observable();
    self.Count_sale = ko.observable();
    self.Price = ko.observable(0);
    self.PriceChange = ko.observable(0);
    self.IsEdit = ko.observable(false);
    self.Code = ko.observable();
    self.ProductName = ko.observable();
    self.Barcode = ko.observable();
    self.IsChangePrice = ko.observable(false);

    self.IsChangePrice_Discount = ko.observable(false);
    self.shop_sanpham = ko.observable();
    self.OptionPrice = ko.observableArray();
    self.Price_Discount = ko.observable(0);
    self.StartDate_Discount = ko.observable(new Date());
    self.Enddate_Discount = ko.observable(new Date());
    self.Channels = ko.observableArray();
}
Product.mProduct_Stock = function () {
    var self = this;
    self.BranchesId = ko.observable();
    self.ProductId = ko.observable();
    self.Stock_Total = ko.observable(0);
    self.Note = ko.observable();
}
Product.mChannel = function () {
    var self = this;
    self.Id = ko.observable();
}
Product.mDisplayProductSample = function () {
    var self = this;
    self.IsId = ko.observable(false);
    self.IsBarcode = ko.observable(true);
    self.Ismasp = ko.observable(true);
    self.Istensp = ko.observable(true);
    self.IsPriceBase = ko.observable(true);
    self.IsPriceBase_Old = ko.observable(true);
    self.IsPriceCompare = ko.observable(true);
    self.IsPriceWholesale = ko.observable(true);
    self.IsPriceInput = ko.observable(true);
    self.IsPriceMainStore = ko.observable(true);
    self.IsImg = ko.observable(true);
    self.IsStatus = ko.observable(true);
    self.IsStock = ko.observable(true);
    self.IsStock_Sum = ko.observable(true);
    self.IsPrice_Discount = ko.observable(true);
    self.IsPrice_Channel = ko.observable(true);
    self.IsVAT = ko.observable(true);
};