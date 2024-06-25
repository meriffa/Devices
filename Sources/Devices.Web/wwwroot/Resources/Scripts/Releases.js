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
                },
                {
                    title: "Concurrency",
                    data: "allowConcurrency",
                    render: function (data, type) {
                        return Devices.Host.Site.formatBoolean(data);
                    }
                },
                {
                    data: "id",
                    orderable: false,
                    render: function (data, type) {
                        return `<a role="button" class="btn btn-outline-dark" href="/Framework/Release?releaseId=${data}">Edit</a>`;
                    }
                },
                {
                    data: null,
                    orderable: false,
                    render: function (data, type) {
                        if (data.enabled)
                            return `<button type="button" class="btn btn-outline-dark" title="Disable release" onclick="Devices.Web.Releases.enableDisableRelease(${data.id}, false)">Disable</button>`;
                        else
                            return `<button type="button" class="btn btn-outline-dark" title="Enable release" onclick="Devices.Web.Releases.enableDisableRelease(${data.id}, true)">Enable</button>`;
                    }
                }
            ],
            order: [[0, "desc"]]
        });
    }

    // Enable / disable release
    namespace.enableDisableRelease = function (releaseId, enabled) {
        $.ajax({
            method: "POST",
            contentType: "application/json",
            url: `/Service/Configuration/EnableDisableRelease?releaseId=${releaseId}&enabled=${enabled}`,
            success: function () {
                location.reload();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                Devices.Host.Site.displayError(jqXHR, textStatus, errorThrown);
            }
        });
    }


}(Devices.Web.Releases = Devices.Web.Releases || {}, jQuery));