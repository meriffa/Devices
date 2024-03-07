using Devices.Common.Models.Monitoring;
using Devices.Service.Interfaces.Monitoring;
using Devices.Service.Options;
using Devices.Service.Services.Identification;
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
                    m.""Date"",
                    m.""LastReboot"",
                    m.""CpuUser"",
                    m.""CpuSystem"",
                    m.""CpuIdle"",
                    m.""MemoryTotal"",
                    m.""MemoryUsed"",
                    m.""MemoryFree"",
                    d.""DeviceID"",
                    d.""DeviceName"",
                    d.""DeviceActive""
                FROM
                    ""DeviceMetric"" m JOIN
                    ""Device"" d ON d.""DeviceID"" = m.""DeviceID""
                ORDER BY
                    d.""DeviceName"",
                    m.""Date"" DESC;", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(new()
                {
                    Device = IdentityService.GetDevice(r),
                    DeviceMetrics = new()
                    {
                        Date = (DateTime)r["Date"],
                        LastRebootDate = (DateTime)r["LastReboot"],
                        Cpu = new()
                        {
                            User = (float)r["CpuUser"],
                            System = (float)r["CpuSystem"],
                            Idle = (float)r["CpuIdle"]
                        },
                        Memory = new()
                        {
                            Total = (int)r["MemoryTotal"],
                            Used = (int)r["MemoryUsed"],
                            Free = (int)r["MemoryFree"]
                        }
                    }
                });
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Save monitoring metrics
    /// </summary>
    /// <param name="metrics"></param>
    public void SaveMonitoringMetrics(MonitoringMetrics metrics)
    {
        try
        {
            using var cn = GetConnection();
            CleanupMonitoringMetrics(cn, metrics);
            using var cmd = GetCommand(
                @"INSERT INTO ""DeviceMetric""
                    (""DeviceID"",
                    ""Date"",
                    ""LastReboot"",
                    ""CpuUser"",
                    ""CpuSystem"",
                    ""CpuIdle"",
                    ""MemoryTotal"",
                    ""MemoryUsed"",
                    ""MemoryFree"")
                VALUES
                    (@DeviceID,
                    @Date,
                    @LastReboot,
                    @CpuUser,
                    @CpuSystem,
                    @CpuIdle,
                    @MemoryTotal,
                    @MemoryUsed,
                    @MemoryFree);", cn);
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Varchar, 64).Value = metrics.Device.Id;
            cmd.Parameters.Add("@Date", NpgsqlDbType.TimestampTz).Value = metrics.DeviceMetrics.Date;
            cmd.Parameters.Add("@LastReboot", NpgsqlDbType.TimestampTz).Value = metrics.DeviceMetrics.LastRebootDate;
            cmd.Parameters.Add("@CpuUser", NpgsqlDbType.Real).Value = metrics.DeviceMetrics.Cpu.User;
            cmd.Parameters.Add("@CpuSystem", NpgsqlDbType.Real).Value = metrics.DeviceMetrics.Cpu.System;
            cmd.Parameters.Add("@CpuIdle", NpgsqlDbType.Real).Value = metrics.DeviceMetrics.Cpu.Idle;
            cmd.Parameters.Add("@MemoryTotal", NpgsqlDbType.Integer).Value = metrics.DeviceMetrics.Memory.Total;
            cmd.Parameters.Add("@MemoryUsed", NpgsqlDbType.Integer).Value = metrics.DeviceMetrics.Memory.Used;
            cmd.Parameters.Add("@MemoryFree", NpgsqlDbType.Integer).Value = metrics.DeviceMetrics.Memory.Free;
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Cleanup monitoring metrics (1 month or older)
    /// </summary>
    /// <param name="cn"></param>
    /// <param name="metrics"></param>
    private void CleanupMonitoringMetrics(NpgsqlConnection cn, MonitoringMetrics metrics)
    {
        using var cmd = GetCommand(@"DELETE FROM ""DeviceMetric"" WHERE ""DeviceID"" = @DeviceID AND ""Date"" < @Date;", cn);
        cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Varchar, 64).Value = metrics.Device.Id;
        cmd.Parameters.Add("@Date", NpgsqlDbType.TimestampTz).Value = metrics.DeviceMetrics.Date.AddMonths(-1);
        cmd.ExecuteNonQuery();
    }
    #endregion

}