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
                    title: "Release Date & Time",
                    data: "date",
                    render: function (data, type) {
                        return Devices.Host.Site.formatDateTime(data);
                    }
                },
                {
                    title: "Application Name",
                    data: "application.name"
                },
                {
                    title: "Package",
                    data: "package"
                },
                {
                    title: "Release Version",
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
                    title: "Active",
                    data: "active",
                    render: function (data, type) {
                        return Devices.Host.Site.formatBoolean(data);
                    }
                }
            ],
            order: [[0, "asc"]]
        });
    }

}(Devices.Web.Releases = Devices.Web.Releases || {}, jQuery));