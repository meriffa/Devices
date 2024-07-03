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
                url: `/Service/Configuration/GetPendingDeployments?deviceId=${$("#cmbDevice").val()}`,
                dataSrc: ""
            },
            columns: [
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
                    title: "Arguments",
                    data: "release.action.arguments"
                }
            ],
            order: [[0, "asc"], [2, "asc"]]
        });
    }

}(Devices.Web.PendingDeployments = Devices.Web.PendingDeployments || {}, jQuery));