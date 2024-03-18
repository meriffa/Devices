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
                    title: "CPU Idle [%]",
                    data: "deviceMetrics.cpu.idle",
                    render: DataTable.render.number(",", ".", 1, "", "")
                },
                {
                    title: "Total [MB]",
                    data: "deviceMetrics.memory.total",
                    render: DataTable.render.number(",", ".", 0, "", "")
                },
                {
                    title: "Used [MB]",
                    data: "deviceMetrics.memory.used",
                    render: DataTable.render.number(",", ".", 0, "", "")
                },
                {
                    title: "Free [MB]",
                    data: "deviceMetrics.memory.free",
                    render: DataTable.render.number(",", ".", 0, "", "")
                }
            ],
            order: [[1, "desc"]]
        });
    }

}(Devices.Web.Monitoring = Devices.Web.Monitoring || {}, jQuery));