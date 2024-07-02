var Devices = Devices || {};
Devices.Web = Devices.Web || {};
(function (namespace, $, undefined) {

    // Initialization
    Devices.Host.Solutions.Site.initContentPage = function () {
        new DataTable("#grdData", {
            ajax: {
                url: "/Service/Security/GetTenants",
                dataSrc: ""
            },
            columns: [
                {
                    title: "Tenant ID",
                    data: "id"
                },
                {
                    title: "Tenant Name",
                    data: "name"
                },
                {
                    title: "Tenant Email",
                    data: "email"
                },
                {
                    title: "Enabled",
                    data: "enabled",
                    render: function (data, type) {
                        return Devices.Host.Solutions.Site.formatBoolean(data);
                    }
                }
            ],
            order: [[0, "asc"]]
        });
    }

}(Devices.Web.Tenants = Devices.Web.Tenants || {}, jQuery));