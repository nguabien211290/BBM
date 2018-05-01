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
Order.mTableCustomer = function () {
    var self = this;
    self.Id = ko.observable();
    self.Name = ko.observable();
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
    self.District_lst = ko.observableArray();
    self.ProvinceName = ko.observable();
    self.DistrictName = ko.observable();
    self.ProvinceId.subscribe(function (val) {
        self.District_lst([]);
        var city = ko.utils.arrayFirst(common.mvCity().City(), function (x) { return x.id == val })
        if (city)
            self.ProvinceName(city.tentp);
        ko.utils.arrayForEach(common.mvCity().Districts(), function (x) {
            if (x.idtp == val)
                self.District_lst.push(x);
        })
    });
    self.DistrictId.subscribe(function (val) {
        var districts = ko.utils.arrayFirst(common.mvCity().Districts(), function (x) { return x.id == val })
        if (districts)
            self.DistrictName(districts.tentinh);
    })

    self.Mark = ko.observable(0);
    self.MarkTmp = ko.observable(0);
    self.NameShip = ko.observable();
    self.AddressShip = ko.observable();
    self.PhoneShip = ko.observable();
    self.DistrictIdShip = ko.observable();
    self.ProvinceIdShip = ko.observable();
};
Order.mTableOrder = function () {
    var self = this;
    var index = 0;

    self.Code = ko.observable();
    self.Id = ko.observable(0);
    self.Status = ko.observable();

    self.Total = ko.observable(0);
    self.Note = ko.observable();
    self.TypeOrder = ko.observable();
    self.IsViewDetail = ko.observable(false);
    self.Customer = ko.observable(new Order.mTableCustomer);
    self.EmployeeNameShip = ko.observable();
    self.EmployeeNameCreate = ko.observable();
    self.DateCreate = ko.observable(new Date());
    self.SelectId = ko.observable(false);
    self.SelectId.subscribe(function (v) {
        debugger
    })
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
    self.StatusPrint = ko.observable();
    self.phithuho = ko.observable();
    self.StatusPrint = ko.observable();

    self.pttt = ko.observable();
    self.tenptgh = ko.observable();
    self.tinhtrang = ko.observable();
    self.idgiogiao = ko.observable();
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
    self.Discount = ko.observable(0);
    self.PriceFix = ko.observable();
    self.Product = ko.observable();
    self.TotalMoney = ko.observable();
    self.TotalMoney_computed = ko.computed(function () {
        if (self.Total() && self.Price())
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
            self.Discount(self.Price() - ((self.Price() * valueDiscountFormember) / 100));
        } else {
            self.TotalMoney(self.Total() * self.Price());
            self.Discount(0);
        }
    });

    self.Discount.subscribe(function (val) {
        if (val > 0 && self.Id() > 0)
            self.isDiscountForMember(true)
    })

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
