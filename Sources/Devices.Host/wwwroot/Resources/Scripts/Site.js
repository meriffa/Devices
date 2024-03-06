var Devices = Devices || {};
Devices.Host = Devices.Host || {};
(function (namespace, $, undefined) {

    // Format number
    Devices.Host.Site.formatNumber = function (value, digits) {
        return new Intl.NumberFormat("en-US", { maximumFractionDigits: digits, minimumFractionDigits: digits }).format(value);
    }

    // Format date & time
    Devices.Host.Site.formatDateTime = function (value) {
        return new Date(value).toISOString().slice(0, 19).replace("T", " ");
    }

}(Devices.Host.Site = Devices.Host.Site || {}, jQuery));