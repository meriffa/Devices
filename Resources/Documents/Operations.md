## Device Operations

### Setup New Device
- [DietPi](DietPi.md#setup-new-device)
- [Raspberry Pi OS](RaspberryPi.md#setup-new-device)

### Device Troubleshooting
- Get Devices.Host Logs: `./AWS.sh DownloadDevicesHostLogs`
- Get Device Log Files: `./AWS.sh DownloadDeviceLogs`
- View Health Checks Alarms: CloudWatch -> Alarms -> All Alarms

### Device Maintenance
- Backup Database: `./AWS.sh BackupDatabase`
- Restore Database: `./AWS.sh RestoreDatabase`
- Change Database User Password: `./AWS.sh ChangeDatabaseUserPassword`

# Change Device WiFi Network
- Connect device to portable router (Ethernet).
- Get device IP from router.
- Connect to device (ssh).
- Change device WiFi network.
- Disconnect device from router (Ethernet) & restart device.