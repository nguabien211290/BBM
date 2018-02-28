var Order = Order || {};
Order.mvOrderSuppliersList = function () {
    var self = this;
    self.Table = ko.observable(new Paging_TmpControltool("Order_Suppliers", true));
    self.SearchReLoad = ko.computed(function () {
        if (self.Table().CountFilter() > 0) {
            self.GetListOrderSuppliers()
        }
    }).extend({ throttle: 1000 });
    self.GetListOrderSuppliers = function () {
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Order_Suppliers/GetOrder_Suppliers"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON(self.Table().Model()),
        }).done(function (data) {
            if (data == null)
                return
            if (!data.isError) {
                self.Table().listData(CommonUtils.MapArray(data.Data.listTable, Order.mOrder));
                self.Table().Totalitems(data.Data.totalItems);
                self.Table().StartItem(data.Data.startItem);
            }
            else
                CommonUtils.notify("Thông báo", data.messaging, 'error');

        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.GetDetail = function (val) {
        ko.utils.arrayForEach(self.Table().listData(), function (v) {
            v.IsViewDetail(false);
        })
        val.IsViewDetail(true);
    }
    self.CreateOrderInput = function (val) {
        var data = { orderId: val.Id(), orderSuppliersId: val.Id() }
        CommonUtils.addTabDynamic('Nhập hàng', CommonUtils.url('/Order_Input/RenderViewCreate'), '#contentX', true, data);
    };

    self.ReturnViewBarcode = function (val) {
        var data = { products: val.Detail() }
        CommonUtils.addTabDynamic('In Tem', CommonUtils.url('/Barcode/RenderView'), '#contentX', true, data);
    };

    self.UpdateOrderDone = function (val) {
        val.Status(CommonUtils.GetAttrStatus(statusOrder_Input, 'Done', 'Key'))
        ko.utils.arrayForEach(val.Detail(), function (o) { o.Status(CommonUtils.GetAttrStatus(statusOrder_Input, 'Done', 'Key')); })
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Order_Suppliers/UpdateDone_Order_Suppliers"),
            cache: false,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: ko.toJSON({ id: val.Id() }),
        }).done(function (data) {
            if (data.isError)
                CommonUtils.notify("Thông báo", data.messaging, 'error');

        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.Start = function () {
        ko.applyBindings(self, document.getElementById('OrderSuppliersListViewId'));
    };
};
