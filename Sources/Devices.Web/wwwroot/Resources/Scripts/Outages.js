var Devices = Devices || {};
Devices.Web = Devices.Web || {};
(function (namespace, $, undefined) {

    // Current table
    namespace.table = null;

    // Initialization
    Devices.Host.Site.initContentPage = function () {
        $("#cmbDevice").change(displayViewData);
        $("#cmbView").change(displayViewData);
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
                Devices.Host.Site.displayError(jqXHR, textStatus, errorThrown);
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
                url: `/Service/Monitoring/GetDeviceOutages?deviceId=${$("#cmbDevice").val()}&filter=${$("#cmbView").val()}`,
                dataSrc: ""
            },
            columns: [
                {
                    title: "Device ID",
                    data: "device.id"
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
                    title: "Outage Start",
                    data: "outage.from",
                    render: function (data, type) {
                        return Devices.Host.Site.formatDateTime(data);
                    }
                },
                {
                    title: "Outage End",
                    data: "outage.to",
                    render: function (data, type) {
                        return Devices.Host.Site.formatDateTime(data);
                    }
                },
                {
                    title: "Outage Duration",
                    data: "outage.duration"
                }
            ],
            order: [[3, "desc"], [1, "asc"]]
        });
    }

}(Devices.Web.Outages = Devices.Web.Outages || {}, jQuery));