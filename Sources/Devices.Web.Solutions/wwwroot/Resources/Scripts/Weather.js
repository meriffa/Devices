var Devices = Devices || {};
Devices.Web = Devices.Web || {};
Devices.Web.Solutions = Devices.Web.Solutions || {};
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
        if ($("#cmbView").val() == "")
            displayViewDataAll();
        else
            displayViewDataAggregate();
    }

    // Display all view data
    function displayViewDataAll() {
        if (namespace.table != null) {
            namespace.table.destroy();
            $("#grdData").empty();
        }
        namespace.table = new DataTable("#grdData", {
            ajax: {
                url: `/Service/Solutions/Garden/GetDeviceWeatherConditions?deviceId=${$("#cmbDevice").val()}`,
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
                    title: "Temperature [℃]",
                    data: "temperature",
                    render: DataTable.render.number(",", ".", 2, "", "")
                },
                {
                    title: "Humidity [%]",
                    data: "humidity",
                    render: DataTable.render.number(",", ".", 2, "", "")
                },
                {
                    title: "Pressure [hPa]",
                    data: "pressure",
                    render: DataTable.render.number(",", ".", 2, "", "")
                },
                {
                    title: "Illuminance [Lux]",
                    data: "illuminance",
                    render: DataTable.render.number(",", ".", 2, "", "")
                }
            ],
            order: [[1, "desc"]]
        });
    }

    // Display aggregate view data
    function displayViewDataAggregate() {
        if (namespace.table != null) {
            namespace.table.destroy();
            $("#grdData").empty();
        }
        namespace.table = new DataTable("#grdData", {
            ajax: {
                url: `/Service/Solutions/Garden/GetAggregateWeatherConditions?deviceId=${$("#cmbDevice").val()}&aggregationType=${$("#cmbView").val()}`,
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
                    title: "Temperature (Min) [℃]",
                    data: "temperature.minimum",
                    render: DataTable.render.number(",", ".", 2, "", "")
                },
                {
                    title: "Temperature (Max) [℃]",
                    data: "temperature.maximum",
                    render: DataTable.render.number(",", ".", 2, "", "")
                },
                {
                    title: "Temperature (Avg) [℃]",
                    data: "temperature.average",
                    render: DataTable.render.number(",", ".", 2, "", "")
                },
                {
                    title: "Humidity (Min) [%]",
                    data: "humidity.minimum",
                    render: DataTable.render.number(",", ".", 2, "", "")
                },
                {
                    title: "Humidity (Max) [%]",
                    data: "humidity.maximum",
                    render: DataTable.render.number(",", ".", 2, "", "")
                },
                {
                    title: "Humidity (Avg) [%]",
                    data: "humidity.average",
                    render: DataTable.render.number(",", ".", 2, "", "")
                },
                {
                    title: "Pressure (Avg) [hPa]",
                    data: "pressure.average",
                    render: DataTable.render.number(",", ".", 2, "", "")
                },
                {
                    title: "Illuminance (Avg) [Lux]",
                    data: "illuminance.average",
                    render: DataTable.render.number(",", ".", 2, "", "")
                }
            ],
            order: [[1, "desc"]]
        });
    }

}(Devices.Web.Solutions.Weather = Devices.Web.Solutions.Weather || {}, jQuery));