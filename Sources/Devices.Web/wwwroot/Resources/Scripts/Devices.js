var Devices = Devices || {};
Devices.Web = Devices.Web || {};
(function (namespace, $, undefined) {

    // Initialization
    Devices.Host.Solutions.Site.initContentPage = function () {
        new DataTable("#grdData", {
            ajax: {
                url: "/Service/Identity/GetDeviceStatuses",
                dataSrc: ""
            },
            columns: [
                {
                    title: "Device ID",
                    data: "device.id"
                },
                {
                    title: "Device Token",
                    data: "token"
                },
                {
                    title: "Device Name",
                    data: "device.name"
                },
                {
                    title: "Device Location",
                    data: "device.location"
                },
                {
                    title: "Enabled",
                    data: "enabled",
                    render: function (data, type) {
                        return Devices.Host.Solutions.Site.formatBoolean(data);
                    }
                },
                {
                    title: "Last Update",
                    data: "deviceDate",
                    render: function (data, type, row) {
                        var date = Devices.Host.Solutions.Site.formatDateTime(data);
                        if (type === 'display') {
                            if (date == null)
                                date = "N/A";
                            return `<div class="${row.level.toLowerCase()}">${date}</div>`;
                        }
                        return date;
                    }
                },
                {
                    title: "Uptime",
                    data: "uptime"
                },
                {
                    title: "Applications",
                    data: "deployments",
                    render: function (data, type) {
                        var deployments = "";
                        $.each(data, function (key, deployment) {
                            deployments += `<div class="${deployment.success ? "green" : "red"}">${deployment.application} (${deployment.version})<div>`;
                        });
                        return deployments;
                    }
                }
            ],
            order: [[0, "asc"]]
        });
    }

}(Devices.Web.Devices = Devices.Web.Devices || {}, jQuery));