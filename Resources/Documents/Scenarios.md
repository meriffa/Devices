## Development Scenarios

### Client-Only Update (Non-Breaking Changes)
- Implement changes in the 'Devices.Client' project.
- Disable device application updates ('DeviceApplication' table).
- Create new 'Devices.Client' / 'Devices.Client.Solutions' release ('Release' table, 'Devices.Client.zip' / 'Devices.Client.Solutions.zip' package).
- Enable device application updates ('DeviceApplication' table).
- Deactivate previous 'Devices.Client' / 'Devices.Client.Solutions' release ('Release' table).

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
- Create new 'Devices.Client' & 'Devices.Client.Solutions' releases ('Release' table, 'Devices.Client.zip' & 'Devices.Client.Solutions.zip' packages).
- Enable device application updates ('DeviceApplication' table).
- Deploy new 'Devices.Client' & 'Devices.Client.Solutions' releases.
- Remove backward compatibility changes in the 'Devices.Service' project.
- Increment AssemblyFileVersion in the 'AssemblyInfo.cs' file.
- Redeploy 'Devices.Host' project (update configuration).
- Deactivate previous 'Devices.Client' & 'Devices.Client.Solutions' releases ('Release' table).