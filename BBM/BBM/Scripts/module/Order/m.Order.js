var Order = Order || {};
Order.mProduct = function () {
    var self = this;
    self.id = ko.observable();
    self.SuppliersId = ko.observable();
    self.UnitId = ko.observable();
    self.Barcode = ko.observable();
    self.masp = ko.observable();
    self.tensp = ko.observable();
    self.Detail_Info = ko.observable();
    self.Stop_Sale = ko.observable();
    self.PriceInput = ko.observable();
    self.PriceInput_Old = ko.observable();
    self.PriceInput_New = ko.observable();
    self.Stock_Total = ko.observable();
    self.HasInChannel = ko.observable(false);
    self.IsEditPro = ko.observable();
    self.soft_Channel = ko.observable();

    self.SuppliersName = ko.observable();
    self.PriceCompare = ko.observable();
    self.PriceBase = ko.observable();
};
Order.mSuppliers = function () {
    var self = this;
    self.SuppliersId = ko.observable();
    self.Name = ko.observable();
    self.Address = ko.observable();
    self.Phone = ko.observable();
    self.Email = ko.observable();
};
Order.mOrder = function () {
    var self = this;
    self.Id = ko.observable();
    self.Status = ko.observable();
    self.Total = ko.observable();
    self.Note = ko.observable('');
    self.TypeOrder = ko.observable();
    self.Detail = ko.observableArray();
    self.IsViewDetail = ko.observable(false);
    self.Id_From = ko.observable();
    self.Id_To = ko.observable();
    self.IsShip = ko.observable(false);
    self.Name_From = ko.observable();
    self.Name_To = ko.observable();
    self.EmployeeNameUpdate = ko.observable();
    self.EmployeeNameCreate = ko.observable();
    self.DisscountValue = ko.observable();
    self.DisscountCode = ko.observable();
    self.StatusPrint = ko.observable();
    self.OrderFromId = ko.observable();
};
Order.mOrderDetail = function () {
    var self = this;
    self.Id = ko.observable();
    self.Code = ko.observable();
    self.ProductName = ko.observable();
    self.OrderId = ko.observable();
    self.ProductId = ko.observable();
    self.Total = ko.observable(1);
    self.Price = ko.observable();
    self.Product = ko.observable();
    self.Status = ko.observable();
    self.TotalMoney = ko.computed(function () {
        if (self.Total() > 0 && self.Price() > 0)
            return self.Total() * self.Price();
        return 0;
    });


    self.SuppliersName = ko.observable();
    self.PriceCompare = ko.observable();
    self.PriceBase = ko.observable();
    self.Stock_Total = ko.observable();
    self.Stock_Totals = ko.observableArray();
    self.PriceChannels = ko.observableArray();
    self.Sale_Average = ko.observable();
    self.soft_Suppliers = ko.observable();
};
Order.mChannel = function () {
    var self = this;
    self.Id = ko.observable();
    self.Channel = ko.observable();
    self.Price = ko.observable();
};
Order.mBranch = function () {
    var self = this;
    self.BranchesId = ko.observable();
    self.BranchesName = ko.observableArray();
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
