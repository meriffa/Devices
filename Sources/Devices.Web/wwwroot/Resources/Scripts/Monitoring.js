var Devices = Devices || {};
Devices.Web = Devices.Web || {};
(function (namespace, $, undefined) {

    // Initialization
    Devices.Host.Site.initContentPage = function () {
        new DataTable("#grdData", {
            ajax: {
                url: "/Service/Monitoring/GetMonitoringMetrics",
                dataSrc: ""
            },
            columns: [
                {
                    title: "Device",
                    data: "device.name"
                },
                {
                    title: "Service Date & Time",
                    data: "serviceDate",
                    render: function (data, type) {
                        return Devices.Host.Site.formatDateTime(data);
                    }
                },
                {
                    title: "Device Date & Time Offset",
                    data: "deviceDateOffset"
                },
                {
                    title: "Last Reboot",
                    data: "deviceMetrics.lastRebootDate",
                    render: function (data, type) {
                        return Devices.Host.Site.formatDateTime(data);
                    }
                },
                {
                    title: "CPU User [%]",
                    data: "deviceMetrics.cpu.user",
                    render: DataTable.render.number(",", ".", 1, "", "")
                },
                {
                    title: "CPU System [%]",
                    data: "deviceMetrics.cpu.system",
                    render: DataTable.render.number(",", ".", 1, "", "")
                },
                {
                    title: "RAM Usage [%]",
                    data: "deviceMetrics.memory",
                    render: function (data, type) {
                        return Devices.Host.Site.formatNumber(100 * data.used / data.total, 2);
                    }
                },
                {
                    title: "Disk Usage [%]",
                    data: "deviceMetrics.disk",
                    render: function (data, type) {
                        return Devices.Host.Site.formatNumber(100 * data.used / data.total, 2);
                    }
                },
                {
                    title: "Kernel",
                    data: "deviceMetrics.kernelVersion"
                }
            ],
            order: [[1, "desc"]]
        });
    }

}(Devices.Web.Monitoring = Devices.Web.Monitoring || {}, jQuery));