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
                    title: "Enabled",
                    data: "enabled",
                    render: function (data, type) {
                        return Devices.Host.Site.formatBoolean(data);
                    }
                },
                {
                    title: "Required Applications",
                    data: "requiredApplications",
                    render: function (data, type) {
                        return data.map(function (application) {
                            if (application.minimumVersion == null)
                                return application.application.name;
                            else
                                return `${application.application.name} (${application.minimumVersion})`;
                        }).join(", ");
                    }
                }
            ],
            order: [[0, "asc"]]
        });
    }

}(Devices.Web.Applications = Devices.Web.Applications || {}, jQuery));