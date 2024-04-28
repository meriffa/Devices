var Devices = Devices || {};
Devices.Web = Devices.Web || {};
Devices.Web.Solutions = Devices.Web.Solutions || {};
(function (namespace, $, undefined) {

    // Hub connection
    namespace.connection = null;

    // Initialization
    Devices.Host.Site.initContentPage = function () {
        $("#btnTurnAllPumpsOn").click(function () {
            $("[id^=chkPump]").prop("checked", true).trigger("change");
        });
        $("#btnTurnAllPumpsOff").click(function () {
            $("[id^=chkPump]").prop("checked", false).trigger("change");
        });
        $("#btnShutdown").click(function () {
            namespace.connection.invoke("SendShutdownRequest", parseInt($("#cmbDevice").val())).then(function () {
                logMessage("Controller shutdown requested.");
            }).catch(function (ex) {
                logMessage(`ERROR: ${ex.toString()}`);
            });
        });
        $("#btnClearDeviceLog").click(function () {
            $("#txtDeviceLog").val("");
        });
        $("[id^=chkPump]").change(sendPumpRequest);
        loadDevices();
    }

    // Load devices
    function loadDevices() {
        $.ajax({
            method: "GET",
            contentType: "application/json",
            url: "/Service/Solutions/Garden/GetDevices",
            success: function (devices) {
                $.each(devices, function (key, device) {
                    $("#cmbDevice").append(`<option value="${device.id}">${device.name} (${device.location})</option>`);
                });
                if ($("#cmbDevice").val())
                    connectToHub();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                Devices.Host.Site.displayError(jqXHR, textStatus, errorThrown);
            }
        });
    }

    // Connect to hub
    function connectToHub() {
        namespace.connection = new signalR.HubConnectionBuilder().withUrl("/Hub/Solutions/Garden").withAutomaticReconnect().build();
        namespace.connection.on("PumpResponse", receivePumpResponse);
        namespace.connection.start().then(function () {
            setupPumpControls(true);
        }).catch(function (ex) {
            logMessage(`ERROR: ${ex.toString()}`);
        });
    }

    // Send pump request
    function sendPumpRequest() {
        element = $(this)
        var deviceId = parseInt($("#cmbDevice").val());
        var pumpId = element.data("pump");
        var pumpState = element.prop("checked");
        namespace.connection.invoke("SendPumpRequest", deviceId, pumpId, pumpState).then(function () {
            logMessage(`Water Pump #${pumpId} = ${pumpState ? "On" : "Off"} requested.`);
        }).catch(function (ex) {
            logMessage(`ERROR: ${ex.toString()}`);
        });
    }

    // Receive pump response
    function receivePumpResponse(deviceId, pumpId, pumpState, error) {
        if (error == null)
            logMessage(`Water Pump #${pumpId} = ${pumpState ? "On" : "Off"} completed.`);
        else
            logMessage(`Water Pump #${pumpId} = ${pumpState ? "On" : "Off"} completed (Error = '${error}').`);
    }

    // Log message
    function logMessage(text) {
        $("#txtDeviceLog").val(`[${Devices.Host.Site.formatDateTime(new Date())}] ${text}\n${$("#txtDeviceLog").val()}`);
    }

    // Setup pump controls
    function setupPumpControls(enabled) {
        if (enabled) {
            $("#btnTurnAllPumpsOn").removeAttr("disabled");
            $("#btnTurnAllPumpsOff").removeAttr("disabled");
            $("#btnShutdown").removeAttr("disabled");
            $("[id^=chkPump]").removeAttr("disabled");
        }
        else {
            $("#btnTurnAllPumpsOn").attr("disabled", true);
            $("#btnTurnAllPumpsOff").attr("disabled", true);
            $("#btnShutdown").attr("disabled", true);
            $("[id^=chkPump]").attr("disabled", true);
        }
    }

}(Devices.Web.Solutions.Watering = Devices.Web.Solutions.Watering || {}, jQuery));