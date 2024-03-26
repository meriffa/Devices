## Device Configuration

### Setup New Device
- [DietPi](DietPi.md#setup-new-device)
- [Raspberry Pi OS](RaspberryPi.md#setup-new-device)

## Troubleshooting & Maintenance

### Troubleshooting
- Get Devices.Host Logs: `./AWS.sh DownloadDevicesHostLogs`
- Get Device Log Files: `./AWS.sh DownloadDeviceLogs`
- View Health Checks Alarms: CloudWatch -> Alarms -> All Alarms

### Maintenance
- Backup Database: `./AWS.sh BackupDatabase`
- Restore Database: `./AWS.sh RestoreDatabase`
- Change Database User Password: `./AWS.sh ChangeDatabaseUserPassword`