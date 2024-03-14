var Devices = Devices || {};
Devices.Web = Devices.Web || {};
(function (namespace, $, undefined) {

    // Initialization
    Devices.Host.Site.initContentPage = function () {
        new DataTable("#grdData", {
            ajax: {
                url: "/Service/Identity/GetDevices",
                dataSrc: ""
            },
            columns: [
                {
                    title: "Device ID",
                    data: "id"
                },
                {
                    title: "Device Token",
                    data: "token"
                },
                {
                    title: "Device Name",
                    data: "name"
                },
                {
                    title: "Device Location",
                    data: "location"
                },
                {
                    title: "Enabled",
                    data: "enabled",
                    render: function (data, type) {
                        return Devices.Host.Site.formatBoolean(data);
                    }
                }
            ],
            order: [[0, "asc"]]
        });
    }

}(Devices.Web.Devices = Devices.Web.Devices || {}, jQuery));