## DietPi Configuration

### Setup New Device
- Download Image
  - Raspberry Pi Zero 2 W: `./DietPi.sh DownloadImageArm32`
  - Raspberry Pi 4 Model B: `./DietPi.sh DownloadImageArm64`
  - Raspberry Pi 5: `./DietPi.sh DownloadImageArm64Pi5`
- Write Image To SD Card: `./DietPi.sh WriteImage`
- Boot Device: Insert SD card into device, power on device & wait for initial configuration to complete.
- Configure Device: `./DietPi.sh SetupDevice "<HostName>"`
- Update Device OS: `./DietPi.sh SystemUpdate`
- Setup Firewall: `./DietPi.sh SetupFirewall`
- Update Banner: `dietpi-banner` -> Device Model = On, Uptime = On, CPU Temp = On, \<Remaining> = Off
- Change Timezone: `dietpi-config` -> Language/Regional Options -> Geographic Area = \<Area>, Timezone = \<Timezone>
- Enable I2C Interface: `dietpi-config` -> Advanced Options -> I2C State = On -> Reboot
- Setup WiFi Connection: `dietpi-config` -> Network Options: Adapters -> Onboard WiFi = On, WiFi -> Enable, \<SSID>, \<Password> -> Done -> Reboot
- Install .NET Runtime: `./DietPi.sh InstallNETRuntime`
- Register Device: `./AWS.sh RegisterDevice <DeviceID> "<DeviceName>" "<DeviceToken>" "<MACAddress>" "<Location>"`
- Deploy Devices.Client: `./DietPi.sh DownloadClient "<MACAddress>" <ReleaseID>`
- Setup Scheduled Jobs: `cd ~/Devices.Client && dotnet Devices.Client.dll execute --tasks Configuration`