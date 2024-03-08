var Devices = Devices || {};
Devices.Host = Devices.Host || {};
(function (namespace, $, undefined) {

    // Format number
    Devices.Host.Site.formatNumber = function (value, digits) {
        return new Intl.NumberFormat("en-US", { maximumFractionDigits: digits, minimumFractionDigits: digits }).format(value);
    }

    // Format date & time
    Devices.Host.Site.formatDateTime = function (value) {
        return convertToLocalDateTime(new Date(value)).toISOString().slice(0, 19).replace("T", " ");
    }

    // Format boolean
    Devices.Host.Site.formatBoolean = function (value) {
        return value == true ? "Yes" : "No";
    }

    // Convert UTC to local date & time
    function convertToLocalDateTime(value) {
        return new Date(value.getTime() - value.getTimezoneOffset() * 60 * 1000);
    }

}(Devices.Host.Site = Devices.Host.Site || {}, jQuery));