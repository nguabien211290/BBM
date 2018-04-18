var Common = Common || {};
Common.mConfig = function () {
    var self = this;
    self.Is_import = ko.observable(false);
    self.Error_Import = ko.observable();
    self.ChannelId = ko.observable();
    self.UserName = ko.observable();
    self.Percent_Process = ko.observable(0);
};
Common.mvBranches = function () {
    var self = this;
    self.Option = ko.observableArray();
    self.Channel = ko.observableArray();
    self.SelectBranches = ko.observable(0);
    self.SelectChannel = ko.observable(0);
    self.SelectBranches.subscribe(function (val) {
        if (val)
            self.Channel(val.soft_Channel);
    });

};

Common.mvCommon = function () {
    var self = this;
    self.mvBranches = ko.observable(new Common.mvBranches);
    self.mvCity = ko.observable(new Common.mvCity);

    self.lstChannel = ko.observableArray();
    self.lstEmployess = ko.observableArray();
    self.lstNotification = ko.observableArray();
    self.isFistLoad = ko.observable(true);
    self.mConfig = ko.observable();
    self.NotifiCount = ko.observable(0);
    self.ReturnLink = function (val) {
        switch (val.Type) {
            case 1:
                if (val.Href)
                    CommonUtils.addTabDynamic('Danh sách nhập hàng', val.Href, '#contentX')
                break;
            case 2:
                break;
        }
    };

    self.ConfigSystem = function () {
        $.ajax({
            type: "GET",
            url: CommonUtils.url("/Common/GetConfigSys"),
            cache: false,
            dataType: 'json',
        }).done(function (data) {
            if (!data.isError) {
                self.mvBranches().Option(data.Data.User.Branches);
                var braches = ko.utils.arrayFirst(self.mvBranches().Option(), function (o) {
                    return o.BranchesId == data.Data.User.BranchesId;
                });

                self.mvBranches().SelectBranches(braches);

                self.mvBranches().SelectChannel.subscribe(function (val) {
                    if (!self.isFistLoad())
                        if (val) {
                            $.ajax({
                                type: "GET",
                                url: CommonUtils.url("/Partial/SetChannel"),
                                cache: false,
                                data: { ChannelId: val.Id }
                            }).always(function () {
                                location.reload(true);
                            });
                        }
                });

                var channels = ko.utils.arrayFirst(self.mvBranches().Channel(), function (o) {
                    return o.Id == data.Data.User.ChannelId;
                });

                self.mvBranches().SelectChannel(channels);
                self.lstEmployess(data.Data.Employee);
                self.lstChannel(data.Data.User.Channel);
                self.mConfig(ko.mapping.fromJS(data.Data.Config, {}, new Common.mConfig));
            } else
                CommonUtils.notify("Thông báo", data.messaging, 'error');
        }).always(function () {
            self.isFistLoad(false);
        });
    };
    self.Notification = function () {
        $.ajax({
            type: "GET",
            url: CommonUtils.url("/Notificaiton/LoadNotification"),
            cache: false,
            dataType: 'json',
        }).done(function (data) {
            if (data) {
                self.lstNotification(data);
                ko.utils.arrayForEach(data, function (o) {
                    if (!o.IsReview)
                        self.NotifiCount(self.NotifiCount() + 1);
                })
            } else
                CommonUtils.notify("Thông báo", 'Không load được Notification', 'error');
        });
    };
    self.ClickReview = function () {
        $.ajax({
            type: "POST",
            url: CommonUtils.url("/Notificaiton/IsReview"),
            cache: false
        }).always(function () {
            self.NotifiCount(0);
        });
    };
    setInterval(function () {
        //self.Notification();
    }, 100000);

    self.Start = function () {
        ko.applyBindings(self);
        self.ConfigSystem();
        self.Notification();
        self.mvCity().GetCity();
    };
};
Common.mvCity = function () {
    var self = this;
    self.City = ko.observableArray();
    self.Districts = ko.observableArray();

    self.GetCity = function () {
        $.ajax({
            type: "GET",
            url: CommonUtils.url("/Common/GetCity_District"),
            cache: false,
            dataType: 'json',
        }).done(function (data) {
            self.City(data.city);
            self.Districts(data.district);
        })
    };
};
Common.mvProvince = function () {
    var self = this;
    self.District = ko.observableArray([
        { value: 1, name: 'Quận 1' },
        { value: 2, name: 'Quận 2' }
    ]);
    self.Provinceslst = ko.observableArray([
    { value: 1, name: 'Phường tân thới hòa', province: 1 },
    { value: 2, name: 'Phường tân thới hòa 12', province: 1 },
    { value: 3, name: 'Phường tân an', province: 2 }
    ]);
    self.Provinces = function (districtId) {
        var rs = [];
        if (districtId) {
            ko.utils.arrayForEach(self.Provinceslst(), function (o) {
                if (o.province == districtId)
                    rs.push(o);
            });
        }
        return rs;
    }
};