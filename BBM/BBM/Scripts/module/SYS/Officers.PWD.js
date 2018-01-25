Officers.mPWD = function () {
    var self = this;
    self.PwdCurent = ko.observable();
    self.PwdRelace = ko.observable();
    self.Pwd = ko.observable();
    self.Email = ko.observable();
    self.PwdCheck = ko.observable();
};
Officers.mvPWD = function () {
    var self = this;
    self.mOfficers = ko.observable(new Officers.mPWD);
    self.GetAcc = function () {
        CommonUtils.showWait(true);
        $.ajax({
            type: "GET",
            url: "/SYS_Officers/Remove/"
        }).done(function (data) {
            ko.mapping.fromJS(data, {}, self.mOfficers);
            if (data.success == false)
                CommonUtils.notify("Thông báo", data.messaging, data.success ? '' : 'error');
        }).always(function () {
            CommonUtils.showWait(false);
        });
    };
    self.Start = function () {
        self.GetAcc();
    };
}