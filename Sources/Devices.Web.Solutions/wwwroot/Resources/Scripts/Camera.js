var Devices = Devices || {};
Devices.Web = Devices.Web || {};
Devices.Web.Solutions = Devices.Web.Solutions || {};
(function (namespace, $, undefined) {

    // Initialization
    Devices.Host.Site.initContentPage = function () {
        $("#cmbDevice").change(verifyDevicePresence);
        $("#btnShutdown").click(sendShutdownRequest);
        $("#cameraPan").change(function () {
            $("#cameraPanValue").val($("#cameraPan").val());
            sendPanRequest(parseInt($("#cameraPan").val(), 10));
        });
        $("#cameraPanValue").change(function () {
            var value = parseInt($("#cameraPanValue").val(), 10);
            if (value < 0) {
                value = 0;
                $("#cameraPanValue").val(value);
            } else if (value > 180) {
                value = 180;
                $("#cameraPanValue").val(value);
            }
            $("#cameraPan").val(value);
            sendPanRequest(value);
        });
        $("#cameraTilt").change(function () {
            $("#cameraTiltValue").val($("#cameraTilt").val());
            sendTiltRequest(parseInt($("#cameraTilt").val(), 10));
        });
        $("#cameraTiltValue").change(function () {
            var value = parseInt($("#cameraTiltValue").val(), 10);
            if (value < 0) {
                value = 0;
                $("#cameraTiltValue").val(value);
            } else if (value > 180) {
                value = 180;
                $("#cameraTiltValue").val(value);
            }
            $("#cameraTilt").val(value);
            sendTiltRequest(value);
        });
        connectToHub();
    }

    // Connect to hub
    function connectToHub() {
        namespace.connection = new signalR.HubConnectionBuilder().withUrl("/Hub/Solutions/Camera").withAutomaticReconnect().build();
        namespace.connection.on("DevicePresenceConfirmationResponse", handleDevicePresenceConfirmationResponse);
        namespace.connection.on("ShutdownResponse", handleShutdownResponse);
        namespace.connection.start().then(function () {
            loadDevices();
        }).catch(function (ex) {
            logMessage(`ERROR: ${ex.toString()}`);
        });
    }

    // Load devices
    function loadDevices() {
        $.ajax({
            method: "GET",
            contentType: "application/json",
            url: "/Service/Solutions/Garden/GetCameraDevices",
            success: function (devices) {
                $.each(devices, function (key, device) {
                    $("#cmbDevice").append(`<option value="${device.id}">${device.name} (${device.location})</option>`);
                });
                if ($("#cmbDevice > option").length > 0)
                    verifyDevicePresence();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                Devices.Host.Site.displayError(jqXHR, textStatus, errorThrown);
            }
        });
    }

    // Verify device presence
    function verifyDevicePresence() {
        setupCameraControls(false);
        logMessage(`Device '${$("#cmbDevice option:selected").text()}' presence verification requested.`);
        namespace.connection.invoke("SendDevicePresenceConfirmationRequest", $("#cmbDevice").val()).catch(function (ex) {
            logMessage(`ERROR: ${ex.toString()}`);
        });
    }

    // Handle device presence confirmation response
    function handleDevicePresenceConfirmationResponse(state) {
        setupCameraControls(true);
        logMessage(`Device '${$("#cmbDevice option:selected").text()}' presence verification completed.`);
        $("#cameraPan").val(state.pan);
        $("#cameraPanValue").val(state.pan);
        $("#cameraTilt").val(state.tilt);
        $("#cameraTiltValue").val(state.tilt);
    }

    // Send shutdown response
    function sendShutdownRequest() {
        namespace.connection.invoke("SendShutdownRequest", $("#cmbDevice").val()).then(function () {
            logMessage("Controller shutdown requested.");
        }).catch(function (ex) {
            logMessage(`ERROR: ${ex.toString()}`);
        });
    }

    // Handle shutdown response
    function handleShutdownResponse() {
        setupCameraControls(false);
        logMessage("Controller shutdown completed.");
    }

    // Send pan request
    function sendPanRequest(value) {
        namespace.connection.invoke("SendPanRequest", $("#cmbDevice").val(), value).then(function () {
            logMessage(`Camera pan ${value}° requested.`);
        }).catch(function (ex) {
            logMessage(`ERROR: ${ex.toString()}`);
        });
    }

    // Send tilt request
    function sendTiltRequest(value) {
        namespace.connection.invoke("SendTiltRequest", $("#cmbDevice").val(), value).then(function () {
            logMessage(`Camera tilt ${value}° requested.`);
        }).catch(function (ex) {
            logMessage(`ERROR: ${ex.toString()}`);
        });
    }

    // Setup pump controls
    function setupCameraControls(enabled) {
        if (enabled) {
            $("#btnShutdown").removeAttr("disabled");
            $("#cameraPan").removeAttr("disabled");
            $("#cameraPanValue").removeAttr("disabled");
            $("#cameraTilt").removeAttr("disabled");
            $("#cameraTiltValue").removeAttr("disabled");
            $("#cameraFocus").removeAttr("disabled");
            $("#cameraFocusValue").removeAttr("disabled");
            $("#cameraZoom").removeAttr("disabled");
            $("#cameraZoomValue").removeAttr("disabled");
        }
        else {
            $("#btnShutdown").attr("disabled", true);
            $("#cameraPan").attr("disabled", true);
            $("#cameraPanValue").attr("disabled", true);
            $("#cameraTilt").attr("disabled", true);
            $("#cameraTiltValue").attr("disabled", true);
            $("#cameraFocus").attr("disabled", true);
            $("#cameraFocusValue").attr("disabled", true);
            $("#cameraZoom").attr("disabled", true);
            $("#cameraZoomValue").attr("disabled", true);
        }
    }

    // Log message
    function logMessage(text) {
        console.log(`[${Devices.Host.Site.formatDateTimeMilliseconds(new Date())}] ${text}`);
    }

}(Devices.Web.Solutions.Camera = Devices.Web.Solutions.Camera || {}, jQuery));