var Devices = Devices || {};
Devices.Web = Devices.Web || {};
(function (namespace, $, undefined) {

    // Current table
    namespace.table = null;

    // Initialization
    Devices.Host.Solutions.Site.initContentPage = function () {
        $("#cmbDevice").change(displayViewData);
        loadDevices();
    }

    // Load devices
    function loadDevices() {
        $.ajax({
            method: "GET",
            contentType: "application/json",
            url: "/Service/Identity/GetDevices",
            success: function (devices) {
                $.each(devices, function (key, device) {
                    $("#cmbDevice").append(`<option value="${device.id}">${device.name} (${device.location})</option>`);
                });
                displayViewData();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                Devices.Host.Solutions.Site.displayError(jqXHR, textStatus, errorThrown);
            }
        });
    }

    // Display view data
    function displayViewData() {
        if (namespace.table != null) {
            namespace.table.destroy();
            $("#grdData").empty();
        }
        namespace.table = new DataTable("#grdData", {
            ajax: {
                url: `/Service/Monitoring/GetMonitoringMetrics?deviceId=${$("#cmbDevice").val()}`,
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
                        return Devices.Host.Solutions.Site.formatDateTime(data);
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
                        return Devices.Host.Solutions.Site.formatDateTime(data);
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
                    title: "CPU Temperature [℃]",
                    data: "deviceMetrics.cpu.temperature",
                    render: DataTable.render.number(",", ".", 1, "", "")
                },
                {
                    title: "RAM Usage [%]",
                    data: "deviceMetrics.memory",
                    render: function (data, type) {
                        return Devices.Host.Solutions.Site.formatNumber(100 * data.used / data.total, 2);
                    }
                },
                {
                    title: "Disk Usage [%]",
                    data: "deviceMetrics.disk",
                    render: function (data, type) {
                        return Devices.Host.Solutions.Site.formatNumber(100 * data.used / data.total, 2);
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