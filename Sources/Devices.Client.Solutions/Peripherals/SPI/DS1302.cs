using Iot.Device.Rtc;
using System.Device.Gpio;

namespace Devices.Client.Solutions.Peripherals.SPI;

/// <summary>
/// Real Time Clock (DS1302)
/// </summary>
public sealed class DS1302 : RtcBase
{

    #region Constants
    private const byte RTC_SECOND = 0;
    private const byte RTC_MINUTE = 1;
    private const byte RTC_HOUR = 2;
    private const byte RTC_DAY_OF_MONTH = 3;
    private const byte RTC_MONTH = 4;
    private const byte RTC_DAY_OF_WEEK = 5;
    private const byte RTC_YEAR = 6;
    private const byte RTC_WRITE_PROTECT = 7;
    private const byte RTC_BURST_MODE = 0x1F;
    private const byte COMMAND_READ = 0x81;
    private const byte COMMAND_WRITE = 0x80;
    #endregion

    #region Private Fields
    private readonly int dataPin;
    private readonly int clockPin;
    private readonly int clockSelectPin;
    private GpioController? controller;
    #endregion

    #region Properties
    /// <summary>
    /// Enabled flag
    /// </summary>
    public bool Enabled
    {
        get => (ReadRegister(RTC_SECOND) & 0x80) == 0;
        set => WriteRegister(RTC_SECOND, (byte)((ReadRegister(RTC_SECOND) & 0x7F) | (value ? 0x00 : 0x80)));
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="clockPin"></param>
    /// <param name="dataPin"></param>
    /// <param name="clockSelectPin"></param>
    public DS1302(int clockPin, int dataPin, int clockSelectPin)
    {
        this.dataPin = dataPin;
        this.clockPin = clockPin;
        this.clockSelectPin = clockSelectPin;
        controller = new(PinNumberingScheme.Logical);
        controller.OpenPin(dataPin, PinMode.Output);
        controller.OpenPin(clockPin, PinMode.Output);
        controller.OpenPin(clockSelectPin, PinMode.Output);
        controller.Write(dataPin, PinValue.Low);
        controller.Write(clockPin, PinValue.Low);
        controller.Write(clockSelectPin, PinValue.Low);
        WriteRegister(RTC_WRITE_PROTECT, 0);
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Return current date & time
    /// </summary>
    /// <returns></returns>
    protected override DateTime ReadTime()
    {
        var data = ReadAll();
        return new DateTime(
            2000 + Bcd2Dec((byte)(data[RTC_YEAR] & 0xFF)),
            Bcd2Dec((byte)(data[RTC_MONTH] & 0x1F)),
            Bcd2Dec((byte)(data[RTC_DAY_OF_MONTH] & 0x3F)),
            GetHour(data[RTC_HOUR]),
            Bcd2Dec((byte)(data[RTC_MINUTE] & 0x7F)),
            Bcd2Dec((byte)(data[RTC_SECOND] & 0x7F)));
    }

    /// <summary>
    /// Set current date & time
    /// </summary>
    /// <param name="time"></param>
    protected override void SetTime(DateTime time)
    {
        var data = new byte[8];
        data[RTC_SECOND] = Dec2Bcd(time.Second);
        data[RTC_MINUTE] = Dec2Bcd(time.Minute);
        data[RTC_HOUR] = EncodeHour(time.Hour);
        data[RTC_DAY_OF_MONTH] = Dec2Bcd(time.Day);
        data[RTC_MONTH] = Dec2Bcd(time.Month);
        data[RTC_DAY_OF_WEEK] = Dec2Bcd((int)time.DayOfWeek);
        data[RTC_YEAR] = Dec2Bcd(time.Year - 2000);
        WriteAll(data);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// BCD To decimal
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static int Bcd2Dec(byte value) => (value >> 4) * 10 + value % 16;

    /// <summary>
    /// Decimal To BCD
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static byte Dec2Bcd(int value)
    {
        if (value > 99 || value < 0)
            throw new ArgumentException("Value must be between 0-99.");
        return (byte)((value / 10 << 4) + value % 10);
    }

    /// <summary>
    /// Return hour value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static int GetHour(byte value)
    {
        if ((value & 0x80) != 0)
            return Bcd2Dec((byte)(value & 0x1F)) + 12 * ((value & 0x20) != 0 ? 1 : 0); // 12-hour mode
        else
            return Bcd2Dec((byte)(value & 0x1F)) + 10 * ((value & 0x20) != 0 ? 1 : 0); // 24-hour mode
    }

    /// <summary>
    /// Encode hour (12-hour mode)
    /// </summary>
    /// <param name="hour"></param>
    /// <returns></returns>
    private static byte EncodeHour(int hour)
    {
        if (hour <= 12)
            return (byte)(0x80 + Dec2Bcd(hour));
        else
            return (byte)(0xA0 + Dec2Bcd(hour - 12));
    }

    /// <summary>
    /// Shift in (LSB first)
    /// </summary>
    /// <returns></returns>
    private byte ShiftIn()
    {
        byte value = 0;
        controller!.OpenPin(dataPin, PinMode.Input);
        for (int i = 0; i < 8; ++i)
        {
            value |= (byte)((byte)controller.Read(dataPin) << i);
            controller.Write(clockPin, PinValue.High);
            controller.Write(clockPin, PinValue.Low);
        }
        return value;
    }

    /// <summary>
    /// Shift out (LSB first)
    /// </summary>
    /// <param name="data"></param>
    private void ShiftOut(byte data)
    {
        controller!.OpenPin(dataPin, PinMode.Output);
        for (int i = 0; i < 8; ++i)
        {
            controller.Write(dataPin, (data & (1 << i)) != 0 ? PinValue.High : PinValue.Low);
            controller.Write(clockPin, PinValue.High);
            controller.Write(clockPin, PinValue.Low);
        }
    }

    /// <summary>
    /// Read all 8 bytes of the clock in a single operation
    /// </summary>
    /// <returns></returns>
    private byte[] ReadAll()
    {
        var data = new byte[8];
        controller!.Write(clockSelectPin, PinValue.High);
        ShiftOut(COMMAND_READ | ((RTC_BURST_MODE & 0x1F) << 1));
        for (int i = 0; i < 8; ++i)
            data[i] = ShiftIn();
        controller.Write(clockSelectPin, PinValue.Low);
        return data;
    }

    /// <summary>
    /// Write all 8 bytes of the clock in a single operation
    /// </summary>
    /// <param name="data"></param>
    private void WriteAll(byte[] data)
    {
        controller!.Write(clockSelectPin, PinValue.High);
        ShiftOut(COMMAND_WRITE | ((RTC_BURST_MODE & 0x1F) << 1));
        for (int i = 0; i < 8; ++i)
            ShiftOut(data[i]);
        controller.Write(clockSelectPin, PinValue.Low);
    }

    /// <summary>
    /// Read data from RTC register
    /// </summary>
    /// <param name="register"></param>
    /// <returns></returns>
    private byte ReadRegister(byte register)
    {
        controller!.Write(clockSelectPin, PinValue.High);
        ShiftOut((byte)(COMMAND_READ | ((register & 0x1F) << 1)));
        var data = ShiftIn();
        controller.Write(clockSelectPin, PinValue.Low);
        return data;
    }

    /// <summary>
    /// Write data to RTC register
    /// </summary>
    /// <param name="register"></param>
    /// <param name="data"></param>
    private void WriteRegister(byte register, byte data)
    {
        controller!.Write(clockSelectPin, PinValue.High);
        ShiftOut((byte)(COMMAND_WRITE | ((register & 0x1F) << 1)));
        ShiftOut(data);
        controller.Write(clockSelectPin, PinValue.Low);
    }
    #endregion

    #region Finalization
    /// <summary>
    /// Finalization
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        controller?.Dispose();
        controller = null;
        base.Dispose(disposing);
    }
    #endregion

}