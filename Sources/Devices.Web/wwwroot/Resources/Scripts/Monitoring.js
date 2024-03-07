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
                    data: "identity.id"
                },
                {
                    title: "Date & Time",
                    data: "device.date",
                    render: function (data, type) {
                        return Devices.Host.Site.formatDateTime(data);
                    }
                },
                {
                    title: "Last Reboot",
                    data: "device.lastRebootDate",
                    render: function (data, type) {
                        return Devices.Host.Site.formatDateTime(data);
                    }
                },
                {
                    title: "CPU User [%]",
                    data: "device.cpu.user",
                    render: DataTable.render.number(",", ".", 1, "", "")
                },
                {
                    title: "CPU System [%]",
                    data: "device.cpu.system",
                    render: DataTable.render.number(",", ".", 1, "", "")
                },
                {
                    title: "CPU Idle [%]",
                    data: "device.cpu.idle",
                    render: DataTable.render.number(",", ".", 1, "", "")
                },
                {
                    title: "Total [MB]",
                    data: "device.memory.total",
                    render: DataTable.render.number(",", ".", 0, "", "")
                },
                {
                    title: "Used [MB]",
                    data: "device.memory.used",
                    render: DataTable.render.number(",", ".", 0, "", "")
                },
                {
                    title: "Free [MB]",
                    data: "device.memory.free",
                    render: DataTable.render.number(",", ".", 0, "", "")
                }
            ],
            order: [[1, "asc"]]
        });
    }

}(Devices.Web.Monitoring = Devices.Web.Monitoring || {}, jQuery));