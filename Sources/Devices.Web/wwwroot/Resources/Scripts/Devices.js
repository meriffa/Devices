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
                    title: "Device Name",
                    data: "name"
                },
                {
                    title: "Active",
                    data: "active",
                    render: function (data, type) {
                        return Devices.Host.Site.formatBoolean(data);
                    }
                }
            ],
            order: [[1, "asc"]]
        });
    }

}(Devices.Web.Devices = Devices.Web.Devices || {}, jQuery));