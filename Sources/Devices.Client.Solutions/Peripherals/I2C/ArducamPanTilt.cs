using Devices.Common.Extensions;
using Iot.Device.Pwm;
using System.Device.I2c;

namespace Devices.Client.Solutions.Peripherals.I2C;

/// <summary>
/// Arducam Camera Pan Tilt Platform
/// </summary>
public sealed class ArducamPanTilt : IDisposable
{

    #region Enums
    /// <summary>
    /// PCA9685 channels
    /// </summary>
    private enum Channels : byte
    {
        PAN = 0,
        TILT = 1
    }

    /// <summary>
    /// PCA9685 registers
    /// </summary>
    private enum Registers : byte
    {
        MODE1 = 0x00, // Mode register 1
        MODE2 = 0x01, // Mode register 2
        SUBADDRESS1 = 0x02, // I2C-bus sub-address 1
        SUBADDRESS2 = 0x03, // I2C-bus sub-address 2
        SUBADDRESS3 = 0x04, // I2C-bus sub-address 3
        ALL_CALL_ADDRESS = 0x05, // LED All Call I2C-bus address
        LED0_ON_LOW = 0x06, // LED0 output and brightness control byte 0
        LED0_ON_HIGH = 0x07, // LED0 output and brightness control byte 1
        LED0_OFF_LOW = 0x08, // LED0 output and brightness control byte 2
        LED0_OFF_HIGH = 0x09, // LED0 output and brightness control byte 3
        ALL_LED_ON_LOW = 0xFA, // Load all LEDn_ON registers byte 0
        ALL_LED_ON_HIGH = 0xFB, // Load all LEDn_ON registers byte 1
        ALL_LED_OFF_LOW = 0xFC, // Load all LEDn_OFF registers byte 0
        ALL_LED_OFF_HIGH = 0xFD, // Load all LEDn_OFF registers byte 1
        PRESCALE = 0xFE // Prescaler for PWM output frequency
    }

    /// <summary>
    /// PCA9685 MODE1 register flags
    /// </summary>
    private enum RegisterMode1 : byte
    {
        RESTART = 0x80,
        EXTERNAL_CLOCK = 0x40,
        AUTO_INCREMENT = 0x20,
        SLEEP = 0x10,
        SUB1 = 0x08,
        SUB2 = 0x04,
        SUB3 = 0x02,
        ALL_CALL = 0x01
    }
    #endregion

    #region Private Fields
    private I2cDevice? device;
    private readonly float frequency;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="busId"></param>
    /// <param name="frequency"></param>
    public ArducamPanTilt(int busId, float frequency = 60f)
    {
        try
        {
            this.frequency = frequency;
            device = I2cDevice.Create(new(busId: busId, Pca9685.I2cAddressBase));
            WriteByte(Registers.MODE1, (byte)RegisterMode1.RESTART);
            DelayExtension.DelayMicroseconds(10_000, allowThreadYield: true);
            SetPrescale(frequency);
        }
        catch
        {
            throw new("Arducam Pan Tilt device not found.");
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Set pan degree
    /// </summary>
    /// <param name="degree"></param>
    public void SetPan(int degree) => SetDegree(Channels.PAN, degree);

    /// <summary>
    /// Set tilt degree
    /// </summary>
    /// <param name="degree"></param>
    public void SetTilt(int degree) => SetDegree(Channels.TILT, degree);
    #endregion

    #region Private Methods
    /// <summary>
    /// Read register value
    /// </summary>
    /// <param name="register"></param>
    /// <returns></returns>
    private byte ReadByte(Registers register)
    {
        device!.WriteByte((byte)register);
        return device.ReadByte();
    }

    /// <summary>
    /// Write register value
    /// </summary>
    /// <param name="register"></param>
    /// <param name="value"></param>
    private void WriteByte(Registers register, byte value)
    {
        Span<byte> bytes = [(byte)register, value];
        device!.Write(bytes);
    }

    /// <summary>
    /// Set prescale based on PWM frequency
    /// </summary>
    /// <param name="frequency"></param>
    private void SetPrescale(float frequency)
    {
        var prescaleValue = 25_000_000f;    // 25MHz
        prescaleValue /= 4096;              // 4096 PWM cycles
        prescaleValue /= frequency;         // Frequency division value
        prescaleValue -= 1;
        var prescale = prescaleValue + 0.5f;
        var mode = ReadByte(Registers.MODE1);
        WriteByte(Registers.MODE1, (byte)((mode & 0x7F) | (byte)RegisterMode1.SLEEP));
        WriteByte(Registers.PRESCALE, (byte)prescale);
        WriteByte(Registers.MODE1, mode);
        DelayExtension.DelayMicroseconds(5_000, allowThreadYield: true);
        WriteByte(Registers.MODE1, (byte)(mode | (byte)RegisterMode1.RESTART | (byte)RegisterMode1.AUTO_INCREMENT));
    }

    /// <summary>
    /// Set servo degree
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="degree"></param>
    private void SetDegree(Channels channel, int degree)
    {
        if (degree >= 180)
            degree = 180;
        else if (degree <= 0)
            degree = 0;
        double pulse = (degree + 45.0d) / (90.0d * 1000.0d);
        SetDutyCycle(channel, pulse);
    }

    /// <summary>
    /// Set servo duty cycle
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="pulse"></param>
    private void SetDutyCycle(Channels channel, double pulse)
    {
        double pulseLength = 1000.0d;
        pulseLength /= frequency;
        pulseLength /= 4096.0d;
        pulse *= 1000.0d;
        pulse /= pulseLength;
        SetDutyCycle(channel, 0, (ushort)pulse);
    }

    /// <summary>
    /// Set servo duty cycle
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="on"></param>
    /// <param name="off"></param>
    private void SetDutyCycle(Channels channel, ushort on, ushort off)
    {
        WriteByte((Registers)((byte)Registers.LED0_ON_LOW + (byte)channel * 4), (byte)(on & 0xFF));
        WriteByte((Registers)((byte)Registers.LED0_ON_HIGH + (byte)channel * 4), (byte)(on >> 8));
        WriteByte((Registers)((byte)Registers.LED0_OFF_LOW + (byte)channel * 4), (byte)(off & 0xFF));
        WriteByte((Registers)((byte)Registers.LED0_OFF_HIGH + (byte)channel * 4), (byte)(off >> 8));
    }
    #endregion

    #region Finalization
    /// <summary>
    /// Finalization
    /// </summary>
    public void Dispose()
    {
        device?.Dispose();
        device = null;
    }
    #endregion

}