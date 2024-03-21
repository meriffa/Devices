## DietPi Configuration

### Setup New Device
- Download Image
  - Raspberry Pi Zero/1: `./DietPi.sh DownloadImagePi0`
  - Raspberry Pi 2/3/4: `./DietPi.sh DownloadImagePi4`
- Write Image To SD Card: `./DietPi.sh WriteImage`
- Boot Device: Insert SD card into device, power on device and wait for initial configuration to complete.
- Configure Device: `./DietPi.sh SetupDevice "<HostName>"`
- Update Device OS: `./DietPi.sh SystemUpdate`
- Setup Firewall: `./DietPi.sh SetupFirewall`
- Install .NET Runtime
  - Raspberry Pi Zero/1: `./DietPi.sh InstallNETRuntimeManualArm32`
  - Raspberry Pi 2/3/4: `./DietPi.sh InstallNETRuntimeManualArm64`
- Register Device: `./AWS.sh RegisterDevice <DeviceID> "<DeviceName>" "<DeviceToken>" "<MACAddress>" "<Location>"`
- Deploy Devices.Client: `./DietPi.sh DownloadClient "<DeviceToken>" <ReleaseID>`
- Update Banner: `dietpi-banner` -> Device Model = On, Uptime = On, CPU Temp = On, <Remaining> = Off
- Change Timezone: `dietpi-config` -> Language/Regional Options -> Geographic Area = <Area>, Timezone = <Timezone>
- Enable I2C Interface: `dietpi-config` -> Advanced Options -> I2C State = On -> Reboot
- Setup WiFi Connection: `dietpi-config` -> Network Options: Adapters -> Onboard WiFi = On, WiFi -> Enable, <SSID> (<Password>) -> Done -> Reboot
- Setup Scheduled Jobs: `dotnet Devices.Client.dll Configuration`