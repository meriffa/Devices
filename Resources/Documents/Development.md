## Development

### Local Configuration
- Install .NET SDK 8.0: `./Development.sh InstallDotNetSDK`
- Install Visual Studio Code: `./Development.sh InstallVisualStudioCode`
- Install RSync: `./Development.sh InstallRSync`
- Configure SSH: Edit `/.ssh/config` and specify `Host`, `User`, `HostName`, `IdentityFile`

### Device Configuration
- Install Visual Studio Debugger: `./Development.sh InstallVisualStudioDebugger`
- Install RSync: `./Development.sh InstallRSync`

### Client-Only Update (Non-Breaking Changes)
- Implement changes in the 'Devices.Client' project.
- Disable device application updates ('DeviceApplication' table).
- Create new 'Devices.Client' release ('Release' table, 'Devices.Client.zip' package).
- Enable device application updates ('DeviceApplication' table).
- Deactivate previous 'Devices.Client' release ('Release' table).

### Service-Only Update (Non-Breaking Changes)
- Implement changes in the 'Devices.Service' project.
- Increment AssemblyFileVersion in the 'AssemblyInfo.cs' file.
- Redeploy 'Devices.Host' project (update configuration).

### Client & Service Update (Breaking Changes)
- Implement changes in the 'Devices.Service' project to support both existing and new clients.
- Implement changes in the 'Devices.Client' project.
- Increment AssemblyFileVersion in the 'AssemblyInfo.cs' file.
- Redeploy 'Devices.Host' project (update configuration).
- Disable device application updates ('DeviceApplication' table).
- Create new 'Devices.Client' releases ('Release' table, 'Devices.Client.zip' packages).
- Enable device application updates ('DeviceApplication' table).
- Deploy new 'Devices.Client' releases.
- Remove backward compatibility changes in the 'Devices.Service' project.
- Increment AssemblyFileVersion in the 'AssemblyInfo.cs' file.
- Redeploy 'Devices.Host' project (update configuration).
- Deactivate previous 'Devices.Client' releases ('Release' table).