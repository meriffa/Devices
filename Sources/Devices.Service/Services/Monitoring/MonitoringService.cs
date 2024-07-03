using Devices.Common.Models.Monitoring;
using Devices.Service.Interfaces.Identification;
using Devices.Service.Interfaces.Monitoring;
using Devices.Service.Models.Identification;
using Devices.Service.Models.Monitoring;
using Devices.Service.Models.Security;
using Devices.Service.Options;
using Devices.Service.Services.Identification;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;

namespace Devices.Service.Services.Monitoring;

/// <summary>
/// Monitoring service
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
public class MonitoringService(ILogger<MonitoringService> logger, IOptions<ServiceOptions> options) : DataService(options.Value.Database), IMonitoringService
{

    #region Private Fields
    private readonly ILogger<MonitoringService> logger = logger;
    private readonly string deviceLogsFolder = options.Value.DeviceLogsFolder;
    #endregion

    #region Public Methods
    /// <summary>
    /// Return monitoring metrics
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public List<MonitoringMetrics> GetMonitoringMetrics(User user)
    {
        try
        {
            var result = new List<MonitoringMetrics>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    m.""ServiceDate"",
                    m.""DeviceDate"",
                    m.""LastReboot"",
                    m.""KernelVersion"",
                    m.""CpuUser"",
                    m.""CpuSystem"",
                    m.""CpuIdle"",
                    m.""CpuTemperature"",
                    m.""MemoryTotal"",
                    m.""MemoryUsed"",
                    m.""MemoryFree"",
                    m.""DiskTotal"",
                    m.""DiskUsed"",
                    m.""DiskFree"",
                    d.""DeviceID"",
                    d.""DeviceName"",
                    d.""DeviceLocation""
                FROM
                    ""DeviceMetric"" m JOIN
                    ""Device"" d ON d.""DeviceID"" = m.""DeviceID"" JOIN
                    ""DeviceTenant"" dt ON dt.""DeviceID"" = d.""DeviceID""
                WHERE
                    dt.""TenantID"" = @TenantID;", cn);
            cmd.Parameters.Add("@TenantID", NpgsqlDbType.Integer).Value = user.Tenant.Id;
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(GetMonitoringMetrics(r));
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Save device metrics
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="serviceDate"></param>
    /// <param name="metrics"></param>
    public void SaveDeviceMetrics(int deviceId, DateTime serviceDate, DeviceMetrics metrics)
    {
        try
        {
            using var cn = GetConnection();
            CleanupMonitoringMetrics(cn, deviceId, serviceDate);
            using var cmd = GetCommand(
                @"INSERT INTO ""DeviceMetric""
                    (""DeviceID"",
                    ""ServiceDate"",
                    ""DeviceDate"",
                    ""LastReboot"",
                    ""KernelVersion"",
                    ""CpuUser"",
                    ""CpuSystem"",
                    ""CpuIdle"",
                    ""CpuTemperature"",
                    ""MemoryTotal"",
                    ""MemoryUsed"",
                    ""MemoryFree"",
                    ""DiskTotal"",
                    ""DiskUsed"",
                    ""DiskFree"")
                VALUES
                    (@DeviceID,
                    @ServiceDate,
                    @DeviceDate,
                    @LastReboot,
                    @KernelVersion,
                    @CpuUser,
                    @CpuSystem,
                    @CpuIdle,
                    @CpuTemperature,
                    @MemoryTotal,
                    @MemoryUsed,
                    @MemoryFree,
                    @DiskTotal,
                    @DiskUsed,
                    @DiskFree);", cn);
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Integer).Value = deviceId;
            cmd.Parameters.Add("@ServiceDate", NpgsqlDbType.TimestampTz).Value = serviceDate;
            cmd.Parameters.Add("@DeviceDate", NpgsqlDbType.TimestampTz).Value = metrics.DeviceDate;
            cmd.Parameters.Add("@LastReboot", NpgsqlDbType.TimestampTz).Value = metrics.LastRebootDate;
            cmd.Parameters.Add("@KernelVersion", NpgsqlDbType.Varchar, 1024).Value = metrics.KernelVersion;
            cmd.Parameters.Add("@CpuUser", NpgsqlDbType.Numeric).Value = metrics.Cpu.User;
            cmd.Parameters.Add("@CpuSystem", NpgsqlDbType.Numeric).Value = metrics.Cpu.System;
            cmd.Parameters.Add("@CpuIdle", NpgsqlDbType.Numeric).Value = metrics.Cpu.Idle;
            cmd.Parameters.Add("@CpuTemperature", NpgsqlDbType.Numeric).Value = metrics.Cpu.Temperature;
            cmd.Parameters.Add("@MemoryTotal", NpgsqlDbType.Integer).Value = metrics.Memory.Total;
            cmd.Parameters.Add("@MemoryUsed", NpgsqlDbType.Integer).Value = metrics.Memory.Used;
            cmd.Parameters.Add("@MemoryFree", NpgsqlDbType.Integer).Value = metrics.Memory.Free;
            cmd.Parameters.Add("@DiskTotal", NpgsqlDbType.Integer).Value = metrics.Disk.Total;
            cmd.Parameters.Add("@DiskUsed", NpgsqlDbType.Integer).Value = metrics.Disk.Used;
            cmd.Parameters.Add("@DiskFree", NpgsqlDbType.Integer).Value = metrics.Disk.Free;
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Return device outages
    /// </summary>
    /// <param name="identityService"></param>
    /// <param name="deviceId"></param>
    /// <param name="filter"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public List<DeviceOutage> GetDeviceOutages(IIdentityService identityService, int? deviceId, OutageFilter filter, User user)
    {
        try
        {
            var result = new List<DeviceOutage>();
            using var cn = GetConnection();
            if (deviceId != null)
                AddDeviceOutages(cn, identityService.GetDevice(deviceId.Value, user), result);
            else
                foreach (var device in identityService.GetDevices(user))
                    AddDeviceOutages(cn, device, result);
            return FilterDeviceOutages(result, filter);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Upload device logs
    /// </summary>
    /// <param name="file"></param>
    public void UploadDeviceLogs(IFormFile file)
    {
        string fileName = Path.GetFileName(file.FileName);
        if (file.Length > 0 && fileName.EndsWith(".zip"))
        {
            if (!Directory.Exists(deviceLogsFolder))
                Directory.CreateDirectory(deviceLogsFolder);
            using var stream = File.Create(Path.Combine(deviceLogsFolder, fileName));
            file.CopyTo(stream);
            logger.LogInformation("Device logs uploaded (FileName = '{FileName}').", fileName);
        }
        else
            logger.LogWarning("Device logs upload skipped (FileName = '{FileName}').", fileName);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return monitoring metrics instance
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static MonitoringMetrics GetMonitoringMetrics(NpgsqlDataReader reader) => new()
    {
        ServiceDate = (DateTime)reader["ServiceDate"],
        DeviceDateOffset = ((DateTime)reader["DeviceDate"]).Subtract((DateTime)reader["ServiceDate"]),
        Device = IdentityService.GetDevice(reader),
        DeviceMetrics = new()
        {
            DeviceDate = (DateTime)reader["DeviceDate"],
            LastRebootDate = (DateTime)reader["LastReboot"],
            KernelVersion = (string)reader["KernelVersion"],
            Cpu = new()
            {
                User = (double)(decimal)reader["CpuUser"],
                System = (double)(decimal)reader["CpuSystem"],
                Idle = (double)(decimal)reader["CpuIdle"],
                Temperature = (double)(decimal)reader["CpuTemperature"]
            },
            Memory = new()
            {
                Total = (int)reader["MemoryTotal"],
                Used = (int)reader["MemoryUsed"],
                Free = (int)reader["MemoryFree"]
            },
            Disk = new()
            {
                Total = (int)reader["DiskTotal"],
                Used = (int)reader["DiskUsed"],
                Free = (int)reader["DiskFree"]
            }
        }
    };

    /// <summary>
    /// Cleanup monitoring metrics
    /// </summary>
    /// <param name="cn"></param>
    /// <param name="deviceId"></param>
    /// <param name="serviceDate"></param>
    private void CleanupMonitoringMetrics(NpgsqlConnection cn, int deviceId, DateTime serviceDate)
    {
        using var cmd = GetCommand(@"DELETE FROM ""DeviceMetric"" WHERE ""DeviceID"" = @DeviceID AND ""ServiceDate"" < @ServiceDate;", cn);
        cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Integer).Value = deviceId;
        cmd.Parameters.Add("@ServiceDate", NpgsqlDbType.TimestampTz).Value = serviceDate.AddMonths(-1);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Return device monitoring start date
    /// </summary>
    /// <param name="cn"></param>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    private DateTime GetDeviceMonitoringStartDate(NpgsqlConnection cn, int deviceId)
    {
        using var cmd = GetCommand(@"SELECT MIN(""ServiceDate"") FROM ""DeviceMetric"" WHERE ""DeviceID"" = @DeviceID;", cn);
        cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Integer).Value = deviceId;
        return AdjustDateUp(cmd.ExecuteScalar() as DateTime? ?? DateTime.UtcNow);
    }

    /// <summary>
    /// Adjust date down
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static DateTime AdjustDateDown(DateTime value)
    {
        var date = value.AddMinutes((int)Math.Floor(value.Minute / 5.0d) * 5 - value.Minute);
        return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0, DateTimeKind.Utc);
    }

    /// <summary>
    /// Adjust date up
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static DateTime AdjustDateUp(DateTime value)
    {
        var date = value.AddMinutes((int)Math.Ceiling(value.Minute / 5.0d) * 5 - value.Minute);
        return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0, DateTimeKind.Utc);
    }

    /// <summary>
    /// Add device outages
    /// </summary>
    /// <param name="cn"></param>
    /// <param name="device"></param>
    /// <param name="outages"></param>
    private void AddDeviceOutages(NpgsqlConnection cn, Device device, List<DeviceOutage> outages)
    {
        using var cmd = GetCommand(
            @"WITH ""cteReferenceDates"" AS (
                    SELECT
                        ""ReferenceDate""
                    FROM
                        GENERATE_SERIES(@StartDate, @EndDate, '5 minutes'::interval) ""ReferenceDate""),
                    ""cteDeviceMetric"" AS (
                    SELECT
                        ""DeviceID"",
                        TO_TIMESTAMP(ROUND(EXTRACT(epoch FROM ""ServiceDate"") / 300) * 300) AT TIME ZONE 'UTC' ""ServiceDate""
                    FROM
                        ""DeviceMetric""
                    WHERE
                        ""DeviceID"" = @DeviceID)
                SELECT
                    r.""ReferenceDate""
                FROM
                    ""cteReferenceDates"" r LEFT JOIN
                    ""cteDeviceMetric"" m ON m.""ServiceDate"" = r.""ReferenceDate""
                WHERE
                    m.""ServiceDate"" IS NULL
                ORDER BY
                    r.""ReferenceDate"";", cn);
        cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Integer).Value = device.Id;
        cmd.Parameters.Add("@StartDate", NpgsqlDbType.TimestampTz).Value = GetDeviceMonitoringStartDate(cn, device.Id);
        cmd.Parameters.Add("@EndDate", NpgsqlDbType.TimestampTz).Value = AdjustDateDown(DateTime.UtcNow);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            AddDeviceOutage(outages, device, (DateTime)r["ReferenceDate"]);
    }

    /// <summary>
    /// Add device outage
    /// </summary>
    /// <param name="outages"></param>
    /// <param name="device"></param>
    /// <param name="outageDateTime"></param>
    private static void AddDeviceOutage(List<DeviceOutage> outages, Device device, DateTime outageDateTime)
    {
        if (outages.Count > 0)
        {
            var outage = outages[^1];
            if (outage.Outage.To == outageDateTime)
            {
                outage.Outage.To = outage.Outage.To.AddMinutes(5);
                outage.Outage.Duration = outage.Outage.Duration.Add(TimeSpan.FromMinutes(5));
            }
            else
                outages.Add(GetDeviceOutage(device, outageDateTime));
        }
        else
            outages.Add(GetDeviceOutage(device, outageDateTime));
    }

    /// <summary>
    /// Return device outage
    /// </summary>
    /// <param name="device"></param>
    /// <param name="outageDateTime"></param>
    /// <returns></returns>
    private static DeviceOutage GetDeviceOutage(Device device, DateTime outageDateTime) => new()
    {
        Device = device,
        Outage = new() { From = outageDateTime, To = outageDateTime.AddMinutes(5), Duration = TimeSpan.FromMinutes(5) }
    };

    /// <summary>
    /// Filter device outages
    /// </summary>
    /// <param name="outages"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    private static List<DeviceOutage> FilterDeviceOutages(List<DeviceOutage> outages, OutageFilter filter) => filter switch
    {
        OutageFilter.LastHour => FilterDeviceOutages(outages, DateTime.UtcNow.AddHours(-1)),
        OutageFilter.LastDay => FilterDeviceOutages(outages, DateTime.UtcNow.AddDays(-1)),
        OutageFilter.LastWeek => FilterDeviceOutages(outages, DateTime.UtcNow.AddDays(-7)),
        OutageFilter.LastMonth => FilterDeviceOutages(outages, DateTime.UtcNow.AddMonths(-1)),
        OutageFilter.All => outages,
        _ => throw new($"Outage filter '{filter}' is not supported.")
    };

    /// <summary>
    /// Filter device outages
    /// </summary>
    /// <param name="outages"></param>
    /// <param name="startDate"></param>
    /// <returns></returns>
    private static List<DeviceOutage> FilterDeviceOutages(List<DeviceOutage> outages, DateTime startDate) => outages.Where(i => startDate <= i.Outage.To).ToList();
    #endregion

}