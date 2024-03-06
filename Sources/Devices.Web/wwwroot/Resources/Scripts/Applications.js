var Devices = Devices || {};
Devices.Web = Devices.Web || {};
(function (namespace, $, undefined) {

    // Initialization
    Devices.Host.Site.initContentPage = function () {
        new DataTable("#grdData", {
            ajax: {
                url: "/Service/Configuration/GetApplications",
                dataSrc: ""
            },
            columns: [
                {
                    title: "Application ID",
                    data: "id"
                },
                {
                    title: "Application Name",
                    data: "name"
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

}(Devices.Web.Applications = Devices.Web.Applications || {}, jQuery));