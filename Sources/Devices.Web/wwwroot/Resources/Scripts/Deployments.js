var Devices = Devices || {};
Devices.Web = Devices.Web || {};
(function (namespace, $, undefined) {

    // Initialization
    Devices.Host.Site.initContentPage = function () {
        const table = new DataTable("#grdData", {
            ajax: {
                url: "/Service/Configuration/GetDeployments",
                dataSrc: ""
            },
            columns: [
                {
                    title: "Deployment ID",
                    data: "id"
                },
                {
                    title: "Deployment Date & Time",
                    data: "date",
                    render: function (data, type) {
                        return Devices.Host.Site.formatDateTime(data);
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
                        return Devices.Host.Site.formatBoolean(data);
                    }
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
            order: [[2, "asc"], [1, "desc"]]
        });
        $("#btnToggleDetails").click(function () {
            var column = table.column(6);
            column.visible(!column.visible());
            $("#btnToggleDetails").text(column.visible() ? "Hide Details" : "Show Details");
        });
    }

}(Devices.Web.Deployments = Devices.Web.Deployments || {}, jQuery));