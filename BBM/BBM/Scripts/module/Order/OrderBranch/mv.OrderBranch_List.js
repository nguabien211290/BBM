var Order = Order || {};
Order.mvOrderBranchList = function () {
    var self = this;
    self.Table = ko.observable(new Paging_TmpControltool("Order_Branch", true));
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
    
    //----------------------Filter-------------------
    self.GetDetail = function (val) {
        ko.utils.arrayForEach(self.Table().listData(), function (v) {
            v.IsViewDetail(false);
        })
        val.IsViewDetail(true);
    }
    self.Start = function () {
        ko.applyBindings(self, document.getElementById('OrderBranchListViewId'));
    };

};
