using Devices.Common.Models.Monitoring;
using Devices.Service.Interfaces.Monitoring;
using Devices.Service.Models.Monitoring;
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
    /// <returns></returns>
    public List<MonitoringMetrics> GetMonitoringMetrics()
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
                    ""Device"" d ON d.""DeviceID"" = m.""DeviceID"";", cn);
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
    #endregion

}