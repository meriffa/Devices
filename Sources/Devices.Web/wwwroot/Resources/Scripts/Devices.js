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
                url: `/Service/Identity/GetDeviceStatuses?deviceId=${$("#cmbDevice").val()}`,
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