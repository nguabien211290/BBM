var Order = Order || {};
Order.mProduct = function () {
    var self = this;
    self.id = ko.observable();
    self.SuppliersId = ko.observable();
    self.UnitId = ko.observable();
    self.Barcode = ko.observable();
    self.masp = ko.observable();
    self.CodeType = ko.observable()
    self.tensp = ko.observable();
    self.Detail_Info = ko.observable();
    self.Stop_Sale = ko.observable();
    self.PriceBase = ko.observable();
    self.PriceBase_Old = ko.observable();
    self.PriceInput = ko.observable();
    self.HasInChannel = ko.observable(false);
    self.IsEditPro = ko.observable();
    self.soft_Channel = ko.observable();
    self.PriceChannel = ko.observable();
};


Order.mCustomer = function () {
    var self = this;
    self.Id = ko.observable();
    self.Name = ko.observable();
    self.Address = ko.observable();
    self.Phone = ko.observable();
    self.Email = ko.observable();
    self.Code = ko.observable();
    self.DistrictId = ko.observable();
    self.ProvinceId = ko.observable();

    self.Mark = ko.observable(0);
    self.MarkTmp = ko.observable(0);
    self.NameShip = ko.observable();
    self.AddressShip = ko.observable();
    self.PhoneShip = ko.observable();
    self.DistrictIdShip = ko.observable();
    self.ProvinceIdShip = ko.observable();
};
Order.mOrder = function () {
    var self = this;
    var index = 0;

    self.Code = ko.observable();
    self.Id = ko.observable(0);
    self.IdTmp = ko.observable();
    self.IsShip = ko.observable();
    self.Status = ko.observable();
    self.DisscountCode = ko.observable();
    self.DisscountValue = ko.observable(0);
    self.DisscountType = ko.observable();

    self.Total = ko.observable(0);
    self.TotalOrther = ko.observable(0);
    self.TotalMoney = ko.observable();
    self.Note = ko.observable();
    self.TypeOrder = ko.observable();
    self.Detail = ko.observableArray();
    self.IsViewDetail = ko.observable(false);
    self.Id_From = ko.observable();
    self.Id_To = ko.observable();
    self.Customer = ko.observable(new Order.mCustomer);


    self.EmployeeCreate = ko.observable();
    self.EmployeeUpdate = ko.observable();
    self.EmployeeNameUpdate = ko.observable();
    self.EmployeeNameShip = ko.observable();
    self.EmployeeNameCreate = ko.observable();

    self.DateCreate = ko.observable(new Date());
    self.BranchesName = ko.observable();
    self.ChannelName = ko.observable();
    self.DateUpdate = ko.observable();
    self.EmployeeShip = ko.observable();
    self.isChannelOnline = ko.observable(false);
};
Order.mOrderSaleDetail = function () {
    var self = this;
    self.Id = ko.observable(0);
    self.Keyword = ko.observable();
    self.ListProductSearch = ko.observableArray();
    self.Code = ko.observable();
    self.ProductName = ko.observable();
    self.OrderId = ko.observable();
    self.ProductId = ko.observable();
    self.Total = ko.observable(1);
    self.Price = ko.observable();
    self.PriceFix = ko.observable();
    self.Product = ko.observable();
    self.TotalMoney = ko.observable();
    self.TotalMoney_computed = ko.computed(function () {
        if (self.Total() > 0 && self.Price() > 0)
            self.TotalMoney(self.Total() * self.Price());
        else
            self.TotalMoney(0);
    });
    self.isDiscountForMember = ko.observable(false);
    self.isDiscountForMember.subscribe(function (o) {
        if (o) {
            var sum = self.Total() * self.Price();
            var discount = sum - ((sum * valueDiscountFormember) / 100)
            self.TotalMoney(discount);
        } else {
            self.TotalMoney(self.Total() * self.Price());
        }
    });

    self.isFristLoad = ko.observable(false);
    self.binded = ko.observable(false);
    $("body").on("click", function () {
        self.ListProductSearch([]);
    });
};
Order.mEmployee = function () {
    var self = this;
    self.Id = ko.observable();
    self.Name = ko.observable();
};
Order.mChannel = function () {
    var self = this;
    self.Id = ko.observable();
    self.Channel = ko.observable();
};
Order.mOrderPrint = function () {
    var self = this;
    self.suppliers = ko.observable();
    self.products = ko.observableArray();
};
Order.mProductsPrint = function () {
    var self = this;
    self.Total = ko.observable();
    self.Product = ko.observable(new Order.mProduct);
};
