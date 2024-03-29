## Raspberry Pi OS Configuration

### Setup New Device
- Download Image: `./RaspberryPi.sh DownloadImage`
- Write Image To SD Card: `./RaspberryPi.sh WriteImage`
- Boot Device: Insert SD card into device, power on device & wait for initial configuration to complete.
- Configure OpenSSH: `./RaspberryPi.sh SetupSSH`
- Configure Device: `./RaspberryPi.sh SetupDevice "<HostName>"`
- Update Device OS: `./RaspberryPi.sh SystemUpdate`
- Setup Firewall: `./RaspberryPi.sh SetupFirewall`
- Change Timezone: `sudo raspi-config` -> Localisation Options -> Timezone -> Geographic Area = \<Area>, Timezone = \<Timezone>
- Enable I2C Interface: `sudo raspi-config nonint do_i2c 0`
- Setup WiFi Connection: `sudo raspi-config` -> System Options -> Wireless LAN -> Country = \<Country>, SSID = \<SSID>, Password = \<Password>
- Install .NET Runtime: `./RaspberryPi.sh InstallNETRuntime`
- Register Device: `./AWS.sh RegisterDevice <DeviceID> "<DeviceName>" "<DeviceToken>" "<MACAddress>" "<Location>"`
- Deploy Devices.Client: `./RaspberryPi.sh DownloadClient "<DeviceToken>" <ReleaseID>`
- Setup Scheduled Jobs: `sudo su - root -c "cd ~/Devices.Client && dotnet Devices.Client.dll execute --tasks Configuration"`

### References
- [Camera Modules](https://www.raspberrypi.com/documentation/accessories/camera.html) 
- [Camera Software](https://www.raspberrypi.com/documentation/computers/camera_software.html)
- [Picamera2 Library](https://datasheets.raspberrypi.com/camera/picamera2-manual.pdf)