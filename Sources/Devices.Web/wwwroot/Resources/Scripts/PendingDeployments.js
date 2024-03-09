var Devices = Devices || {};
Devices.Web = Devices.Web || {};
(function (namespace, $, undefined) {

    // Initialization
    Devices.Host.Site.initContentPage = function () {
        new DataTable("#grdData", {
            ajax: {
                url: "/Service/Configuration/GetPendingDeployments",
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