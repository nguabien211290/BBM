var Order = Order || {};
Order.mvOrderBranchList = function () {
    var self = this;
    self.Table = ko.observable(new Paging_TmpControltool("Order_Branch", true));
    self.isRequestlistOrder = ko.observable();
    self.isRequestlistOrder.subscribe(function (val) {
        var filterRequestOrder = ko.utils.arrayFirst(self.Table().Filter_Search().Fiterby(), function (filter) {
            return filter.Fiter == "RequestOrder";
        });

        if (filterRequestOrder != null) {
            ko.utils.arrayForEach(self.Table().Filter_Search().Fiterby(), function (filter) {
                if (filter.Fiter == "RequestOrder") {
                    filter.Value = val ? "Request" : "List";
                    self.Table().CountFilter(self.Table().CountFilter() + 1);
                }
            });
        } else {
            var newobject = new Filter.mFilterModel();
            newobject.Fiter = "RequestOrder";
            newobject.Value = val ? "Request" : "List";
            newobject.IsDisplay = false;
            self.Table().Filter_Search().Fiterby.push(newobject);
        }
    })
    self.SearchReLoad = ko.computed(function () {
        if (self.Table().CountFilter() > 0) {
            self.GetListOrderBranch()
        }
    }).extend({ throttle: 1000 });

    self.GetListOrderBranch = function () {
        CommonUtils.showWait(true);
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Order_Branch/GetOrder_Branches"),
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

    self.Resfesh = function () {
        self.Table().CountFilter(self.Table().CountFilter() + 1);
    };

    //----------------------Filter-------------------
    self.GetDetail = function (val) {
        ko.utils.arrayForEach(self.Table().listData(), function (v) {
            v.IsViewDetail(false);
        })
        val.IsViewDetail(true);
    };
    self.ReturnViewOutOrder = function (val) {
        var data = { orderId: val.Id()}
        CommonUtils.addTabDynamic('Xuất hàng', CommonUtils.url('/Order_Output/RenderViewCreate'), '#contentX', true, data);
    };
    self.Start = function () {
        ko.applyBindings(self, document.getElementById('OrderBranchListViewId'));
        self.isRequestlistOrder(true);
    };

};
