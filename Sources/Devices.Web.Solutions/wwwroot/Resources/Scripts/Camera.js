var Devices = Devices || {};
Devices.Web = Devices.Web || {};
Devices.Web.Solutions = Devices.Web.Solutions || {};
(function (namespace, $, undefined) {

    // Current table
    namespace.table = null;

    // Initialization
    Devices.Host.Site.initContentPage = function () {
        $("#cmbDevice").change(displayViewData);
        loadDevices();
    }

    // Load devices
    function loadDevices() {
        $.ajax({
            method: "GET",
            contentType: "application/json",
            url: "/Service/Solutions/Garden/GetDevices",
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
                url: `/Service/Solutions/Garden/GetDeviceCameraNotifications?deviceId=${$("#cmbDevice").val()}`,
                dataSrc: ""
            },
            columns: [
                {
                    title: "Device",
                    data: "device.name"
                },
                {
                    title: "Device Date & Time",
                    data: "deviceDate",
                    render: function (data, type) {
                        return Devices.Host.Site.formatDateTime(data);
                    }
                },
                {
                    title: "Faces Detected",
                    data: "faceCount"
                },
                {
                    title: "Movement Regions Detected",
                    data: "motionRegionCount"
                },
                {
                    title: "Video File",
                    data: "videoFileName"
                }
            ],
            order: [[1, "desc"]]
        });
    }

}(Devices.Web.Solutions.Camera = Devices.Web.Solutions.Camera || {}, jQuery));