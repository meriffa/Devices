var Devices = Devices || {};
Devices.Web = Devices.Web || {};
(function (namespace, $, undefined) {

    // Initialization
    Devices.Host.Solutions.Site.initContentPage = function () {
        new DataTable("#grdData", {
            ajax: {
                url: "/Service/Security/GetUsers",
                dataSrc: ""
            },
            columns: [
                {
                    title: "User ID",
                    data: "id"
                },
                {
                    title: "Tenant Name",
                    data: "tenant.name"
                },
                {
                    title: "User Name",
                    data: "name"
                },
                {
                    title: "User Full Name",
                    data: "fullName"
                },
                {
                    title: "User Email",
                    data: "email"
                },
                {
                    title: "User Roles",
                    data: "roles"
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

}(Devices.Web.Users = Devices.Web.Users || {}, jQuery));