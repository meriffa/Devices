var Devices = Devices || {};
Devices.Web = Devices.Web || {};
Devices.Web.Solutions = Devices.Web.Solutions || {};
(function (namespace, $, undefined) {

    // Initialization
    Devices.Host.Site.initContentPage = function () {
        new DataTable("#grdData", {
            ajax: {
                url: "/Service/Solutions/Garden/GetWeatherConditions",
                dataSrc: ""
            },
            columns: [
                {
                    title: "Date & Time",
                    data: "date",
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
            order: [[0, "desc"]]
        });
    }

}(Devices.Web.Solutions.Weather = Devices.Web.Solutions.Weather || {}, jQuery));