## Jetson Orin Nano Configuration

### Setup New Device
- Flash Image: Jetson Linux Developer Guide -> Quick Start -> To Flash the Jetson Developer Kit Operating Software
- Update Device OS: `./Jetson.sh SystemUpdate`
- Setup Firewall: `./Jetson.sh SetupFirewall`
- Install .NET Runtime: `./Jetson.sh InstallNETRuntime`
- Register Device: `./AWS.sh RegisterDevice <DeviceID> "<DeviceName>" "<DeviceToken>" "<MACAddress>" "<Location>"`
- Deploy Devices.Client: `./Jetson.sh DownloadClient "<MACAddress>" <ReleaseID>`
- Setup Scheduled Jobs: `sudo su - root -c "cd ~/Devices.Client && dotnet Devices.Client.dll execute --tasks Configuration"`

### References
- [JetPack SDK](https://developer.nvidia.com/embedded/jetpack)
- [Jetson Linux](https://developer.nvidia.com/embedded/jetson-linux)