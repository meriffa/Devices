namespace Devices.Service.Models.Monitoring;

/// <summary>
/// Outage filter
/// </summary>
public enum OutageFilter
{
    All = 0,
    LastHour = 1,
    LastDay = 2,
    LastWeek = 3,
    LastMonth = 4
}