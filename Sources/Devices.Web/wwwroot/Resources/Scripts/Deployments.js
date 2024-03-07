var Devices = Devices || {};
Devices.Web = Devices.Web || {};
(function (namespace, $, undefined) {

    // Initialization
    Devices.Host.Site.initContentPage = function () {
        new DataTable("#grdData", {
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
                    data: "device.id"
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
                    data: "details"
                }
            ],
            order: [[0, "asc"]]
        });
    }

}(Devices.Web.Deployments = Devices.Web.Deployments || {}, jQuery));