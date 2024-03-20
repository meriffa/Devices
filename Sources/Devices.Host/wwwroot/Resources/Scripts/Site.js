var Devices = Devices || {};
Devices.Host = Devices.Host || {};
(function (namespace, $, undefined) {

    // Format number
    namespace.formatNumber = function (value, digits) {
        return new Intl.NumberFormat("en-US", { maximumFractionDigits: digits, minimumFractionDigits: digits }).format(value);
    }

    // Format date & time
    namespace.formatDateTime = function (value) {
        return convertToLocalDateTime(new Date(value)).toISOString().slice(0, 19).replace("T", " ");
    }

    // Format boolean
    namespace.formatBoolean = function (value) {
        return value == true ? "Yes" : "No";
    }

    // Convert UTC to local date & time
    function convertToLocalDateTime(value) {
        return new Date(value.getTime() - value.getTimezoneOffset() * 60 * 1000);
    }

    // Validate form
    namespace.validateForm = function () {
        if ($(".needs-validation")[0].checkValidity()) {
            $(".needs-validation").removeClass("was-validated");
            return true;
        }
        else {
            $(".needs-validation").addClass("was-validated");
            return false;
        }
    }

    // Display error
    namespace.displayError = function (jqXHR, textStatus, errorThrown) {
        alert("ERROR: " + errorThrown);
    }

}(Devices.Host.Site = Devices.Host.Site || {}, jQuery));