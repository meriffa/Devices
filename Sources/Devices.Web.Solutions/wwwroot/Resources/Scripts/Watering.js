var Devices = Devices || {};
Devices.Web = Devices.Web || {};
Devices.Web.Solutions = Devices.Web.Solutions || {};
(function (namespace, $, undefined) {

    // Private members
    namespace.connection = null;
    namespace.startTime = Array(10).fill(null);
    namespace.stopwatchInterval = Array(10).fill(null);
    namespace.elapsedPausedTime = Array(10).fill(0);

    // Initialization
    Devices.Host.Site.initContentPage = function () {
        $("#cmbDevice").change(verifyDevicePresence);
        $("#btnTurnAllPumpsOn").click(function () {
            $("[id^=chkPump]:not(:checked)").prop("checked", true).trigger("change");
        });
        $("#btnTurnAllPumpsOff").click(function () {
            $("[id^=chkPump]:checked").prop("checked", false).trigger("change");
        });
        $("#btnShutdown").click(sendShutdownRequest);
        $("#btnClearDeviceLog").click(function () {
            $("#txtDeviceLog").val("");
        });
        $("[id^=chkPump]").change(sendPumpRequest);
        $("[id^=btnStopwatchReset]").click(resetStopwatch);
        connectToHub();
    }

    // Connect to hub
    function connectToHub() {
        namespace.connection = new signalR.HubConnectionBuilder().withUrl("/Hub/Solutions/Watering").withAutomaticReconnect().build();
        namespace.connection.on("DevicePresenceConfirmationResponse", handleDevicePresenceConfirmationResponse);
        namespace.connection.on("ShutdownResponse", handleShutdownResponse);
        namespace.connection.on("PumpResponse", handlePumpResponse);
        namespace.connection.on("OperatorPresenceConfirmationRequest", handleOperatorPresenceConfirmationRequest);
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
            url: "/Service/Solutions/Garden/GetWateringDevices",
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
        setupPumpControls(false);
        logMessage(`Device '${$("#cmbDevice option:selected").text()}' presence verification requested.`);
        namespace.connection.invoke("SendDevicePresenceConfirmationRequest", $("#cmbDevice").val()).catch(function (ex) {
            logMessage(`ERROR: ${ex.toString()}`);
        });
    }

    // Handle device presence confirmation response
    function handleDevicePresenceConfirmationResponse() {
        setupPumpControls(true);
        logMessage(`Device '${$("#cmbDevice option:selected").text()}' presence verification completed.`);
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
        setupPumpControls(false);
        logMessage("Controller shutdown completed.");
    }

    // Send pump request
    function sendPumpRequest() {
        element = $(this)
        var pumpIndex = element.data("pump");
        var pumpState = element.prop("checked");
        namespace.connection.invoke("SendPumpRequest", $("#cmbDevice").val(), pumpIndex, pumpState).then(function () {
            logMessage(`Water Pump #${pumpIndex + 1} = ${pumpState ? "On" : "Off"} requested.`);
        }).catch(function (ex) {
            logMessage(`ERROR: ${ex.toString()}`);
        });
        $(`#btnStopwatchReset${pumpIndex}`).prop("disabled", pumpState);
    }

    // Handle pump response
    function handlePumpResponse(pumpIndex, pumpState, error) {
        if (error == null) {
            logMessage(`Water Pump #${pumpIndex + 1} = ${pumpState ? "On" : "Off"} completed.`);
            if (pumpState)
                startStopwatch(pumpIndex);
            else
                stopStopwatch(pumpIndex);
        }
        else
            logMessage(`Water Pump #${pumpIndex + 1} = ${pumpState ? "On" : "Off"} completed (Error = '${error}').`);
    }

    // Handle operator presence confirmation request
    function handleOperatorPresenceConfirmationRequest(deviceId) {
        namespace.connection.invoke("SendOperatorPresenceConfirmationResponse", deviceId).then(function () {
            logMessage("Watering in progress ...");
        }).catch(function (ex) {
            logMessage(`ERROR: ${ex.toString()}`);
        });
    }

    // Setup pump controls
    function setupPumpControls(enabled) {
        if (enabled) {
            $("#btnTurnAllPumpsOn").removeAttr("disabled");
            $("#btnTurnAllPumpsOff").removeAttr("disabled");
            $("#btnClearDeviceLog").removeAttr("disabled");
            $("#btnShutdown").removeAttr("disabled");
            $("[id^=chkPump]").removeAttr("disabled");
            $("[id^=btnStopwatchReset]").removeAttr("disabled");
        }
        else {
            $("#btnTurnAllPumpsOn").attr("disabled", true);
            $("#btnTurnAllPumpsOff").attr("disabled", true);
            $("#btnClearDeviceLog").attr("disabled", true);
            $("#btnShutdown").attr("disabled", true);
            $("[id^=chkPump]").attr("disabled", true);
            $("[id^=btnStopwatchReset]").attr("disabled", true);
        }
    }

    // Log message
    function logMessage(text) {
        $("#txtDeviceLog").val(`[${Devices.Host.Site.formatDateTimeMilliseconds(new Date())}] ${text}\n${$("#txtDeviceLog").val()}`);
    }

    // Start pump stopwatch
    function startStopwatch(pumpIndex) {
        if (!namespace.stopwatchInterval[pumpIndex]) {
            namespace.startTime[pumpIndex] = new Date().getTime() - namespace.elapsedPausedTime[pumpIndex];
            namespace.stopwatchInterval[pumpIndex] = setInterval(function () { updateStopwatch(pumpIndex); }, 1000);
        }
    }

    // Stop pump stopwatch
    function stopStopwatch(pumpIndex) {
        clearInterval(namespace.stopwatchInterval[pumpIndex]);
        namespace.elapsedPausedTime[pumpIndex] = new Date().getTime() - namespace.startTime[pumpIndex];
        namespace.stopwatchInterval[pumpIndex] = null;
    }

    // Reset pump stopwatch
    function resetStopwatch() {
        var pumpIndex = $(this).data("pump");
        stopStopwatch(pumpIndex);
        namespace.elapsedPausedTime[pumpIndex] = 0;
        document.getElementById(`swPump${pumpIndex}`).innerHTML = "00:00:00";
    }

    // Update pump stopwatch
    function updateStopwatch(pumpIndex) {
        var currentTime = new Date().getTime();
        var elapsedTime = currentTime - namespace.startTime[pumpIndex];
        var seconds = Math.floor(elapsedTime / 1000) % 60;
        var minutes = Math.floor(elapsedTime / 1000 / 60) % 60;
        var hours = Math.floor(elapsedTime / 1000 / 60 / 60);
        document.getElementById(`swPump${pumpIndex}`).innerHTML = `${zeroPad(hours, 2)}:${zeroPad(minutes, 2)}:${zeroPad(seconds, 2)}`;
    }

    // Zero pad number
    const zeroPad = (value, places) => String(value).padStart(places, "0");

}(Devices.Web.Solutions.Watering = Devices.Web.Solutions.Watering || {}, jQuery));