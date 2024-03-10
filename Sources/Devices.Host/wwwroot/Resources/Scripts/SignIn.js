var Devices = Devices || {};
Devices.Host = Devices.Host || {};
(function (namespace, $, undefined) {

    // Initialization
    Devices.Host.Site.initContentPage = function() {
        $("#btnSignIn").click(submitForm);
    }

    // Submit form
    function submitForm() {
        if (Devices.Host.Site.validateForm())
            $("form").submit();
        else
            return false;
    }

}(Devices.Host.SignIn = Devices.Host.SignIn || {}, jQuery));