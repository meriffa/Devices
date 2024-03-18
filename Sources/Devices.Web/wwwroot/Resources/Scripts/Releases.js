var Devices = Devices || {};
Devices.Web = Devices.Web || {};
(function (namespace, $, undefined) {

    // Initialization
    Devices.Host.Site.initContentPage = function () {
        new DataTable("#grdData", {
            ajax: {
                url: "/Service/Configuration/GetReleases",
                dataSrc: ""
            },
            columns: [
                {
                    title: "Release ID",
                    data: "id"
                },
                {
                    title: "Release Service Date & Time",
                    data: "serviceDate",
                    render: function (data, type) {
                        return Devices.Host.Site.formatDateTime(data);
                    }
                },
                {
                    title: "Application",
                    data: "application.name"
                },
                {
                    title: "Package",
                    data: "package"
                },
                {
                    title: "Version",
                    data: "version"
                },
                {
                    title: "Action",
                    data: "action",
                    render: function (data, type) {
                        return data.type == 1 ? "Script" : "Unknown";
                    }
                },
                {
                    title: "Parameters",
                    data: "action.parameters"
                },
                {
                    title: "Arguments",
                    data: "action.arguments"
                },
                {
                    title: "Enabled",
                    data: "enabled",
                    render: function (data, type) {
                        return Devices.Host.Site.formatBoolean(data);
                    }
                }
            ],
            order: [[0, "asc"]]
        });
    }

}(Devices.Web.Releases = Devices.Web.Releases || {}, jQuery));