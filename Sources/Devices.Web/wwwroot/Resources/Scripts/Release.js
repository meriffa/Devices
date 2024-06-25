var Devices = Devices || {};
Devices.Web = Devices.Web || {};
(function (namespace, $, undefined) {

    // Initialization
    Devices.Host.Site.initContentPage = function () {
        $("#btnSave").click(submitForm);
    }

    // Submit form
    function submitForm() {
        if (Devices.Host.Site.validateForm()) {
            $("#btnSave").attr("disabled", true);
            $("form").submit();
        }
        else
            return false;
    }

}(Devices.Web.Release = Devices.Web.Release || {}, jQuery));