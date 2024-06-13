# Device Management Framework

Device Management Framework is an open-source solution for managing Edge Computing, Edge AI & IoT devices.

## Architecture

![Architecture Diagram](/Resources/Images/Architecture.png)

- Core Tier: The core tier has web interface, web services and real-time communication endpoints and structured data store.
  - The web site allows end users to monitor and manage devices remotely.
  - The web services allow both devices and backend systems to connect and implement specific management services (Security, Monitoring, Configuration).
  - The real-time communication endpoints provide direct control message relay channel between devices and browser based or backed system clients.
  - The device metadata and configuration is stored in a transactional database.
  - The core tier is hosed either on premise or on a cloud infrastructure.
- Device Tier: The device tier hosts the device agent and business applications.
  - The device agent application manages the device configuration and business application deployments.
  - The business applications are interacting with the sensors and peripherals to provide a specific business capability.

## Components

- [Devices.Common](/Sources/Devices.Common/): Class library shared between core and device tiers.
- [Devices.Common.Solutions](/Sources/Devices.Common.Solutions/): Reference class library shared between core and device tiers.
- [Devices.Host](/Sources/Devices.Host/): Core tier host (web site, web services) application.
- [Devices.Service](/Sources/Devices.Service/): Core tier web services.
- [Devices.Service.Solutions](/Sources/Devices.Service.Solutions/): Core tier reference web services.
- [Devices.Web](/Sources/Devices.Web/): Core tier web site pages & assets.
- [Devices.Web.Solutions](/Sources/Devices.Web.Solutions/): Core tier reference web site pages & assets.
- [Devices.Client](/Sources/Devices.Client/): Device agent application.
- [Devices.Client.Solutions](/Sources/Devices.Client.Solutions/): Device reference business application (C#, Python).

## References

- [FAQ](/Resources/Documents/FAQ.md)
- [Development](/Resources/Documents/Development.md)
- [AWS](/Resources/Documents/AWS.md)
- [Operations](/Resources/Documents/Operations.md)
- [Examples](/Resources/Documents/Solutions.md)
- [Platforms](/Resources/Documents/Platforms.md)
- [Roadmap](/Resources/Documents/Roadmap.md#capability-roadmap)
- [Glossary](/Resources/Documents/Glossary.md)