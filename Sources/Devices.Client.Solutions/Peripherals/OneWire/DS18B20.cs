using Iot.Device.OneWire;
using UnitsNet;

namespace Devices.Client.Solutions.Peripherals.OneWire;

/// <summary>
/// Digital Temperature Sensor (DS18B20)
/// </summary>
public sealed class DS18B20
{

    #region Private Fields
    private readonly OneWireThermometerDevice device;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    public DS18B20()
    {
        foreach (var busId in OneWireBus.EnumerateBusIds())
        {
            var bus = new OneWireBus(busId);
            bus.ScanForDeviceChanges();
            foreach (var deviceId in bus.EnumerateDeviceIds())
            {
                var device = new OneWireDevice(busId, deviceId);
                if (device.Family == DeviceFamily.Ds18b20 && OneWireThermometerDevice.IsCompatible(busId, deviceId))
                {
                    this.device = new(busId, deviceId);
                    return;
                }
            }
        }
        throw new("DS18B20 device not found.");
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Read temperature
    /// </summary>
    /// <returns></returns>
    public Temperature ReadTemperature() => device.ReadTemperature();
    #endregion

}