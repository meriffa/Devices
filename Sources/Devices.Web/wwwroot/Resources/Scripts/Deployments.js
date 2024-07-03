var Devices = Devices || {};
Devices.Web = Devices.Web || {};
(function (namespace, $, undefined) {

    // Current table
    namespace.table = null;

    // Initialization
    Devices.Host.Solutions.Site.initContentPage = function () {
        $("#cmbDevice").change(displayViewData);
        $("#btnToggleDetails").click(function () {
            var column = namespace.table.column(7);
            column.visible(!column.visible());
            $("#btnToggleDetails").text(column.visible() ? "Hide Details" : "Show Details");
        });
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
                url: `/Service/Configuration/GetCompletedDeployments?deviceId=${$("#cmbDevice").val()}`,
                dataSrc: ""
            },
            columns: [
                {
                    title: "Deployment ID",
                    data: "id"
                },
                {
                    title: "Deployment Device Date & Time",
                    data: "deviceDate",
                    render: function (data, type) {
                        return Devices.Host.Solutions.Site.formatDateTime(data);
                    }
                },
                {
                    title: "Device",
                    data: "device.name"
                },
                {
                    title: "Application",
                    data: "release.application.name"
                },
                {
                    title: "Release",
                    data: "release.version"
                },
                {
                    title: "Success",
                    data: "success",
                    render: function (data, type) {
                        return Devices.Host.Solutions.Site.formatBoolean(data);
                    }
                },
                {
                    title: "Arguments",
                    data: "release.action.arguments"
                },
                {
                    title: "Details",
                    data: "details",
                    visible: false,
                    render: function (data, type) {
                        return `<span>${data.replace("\n", "<br />")}</span>`;
                    }
                }
            ],
            order: [[1, "desc"]]
        });
    }

}(Devices.Web.Deployments = Devices.Web.Deployments || {}, jQuery));