# Device Management Framework

Device Management Framework is an open-source software framework for managing Mini Computers.

## Architecture

![Architecture Diagram](/Resources/Images/Architecture.png)

- Core Tier: The core tier has web interface, web services and real-time communication endpoints and structured data store.
  - The web site allows end users to monitor and manage Mini Computers remotely.
  - The Mini Computers are connected to centralized web services and both implement the framework management services (Identity & Security, Monitoring, Deployment & Configuration).
  - The real-time communication endpoints provide direct message relay channel between Mini Computers and browser based or backend system clients.
  - The device metadata and configuration is stored in a transactional database.
  - The core tier is hosed either on premise or on a cloud infrastructure.
- Device Tier: The device tier hosts the device agent and business applications.
  - The device agent application manages the device configuration and business application deployments.
  - The business applications are interacting with the sensors and peripherals to provide a specific business capability.

## Components

- [Devices.Common](/Sources/Devices.Common/): Class library shared between core and device tiers.
- [Devices.Service](/Sources/Devices.Service/): Core tier web services.
- [Devices.Web](/Sources/Devices.Web/): Core tier web site pages & assets.
- [Devices.Client](/Sources/Devices.Client/): Device agent application.

## References

- [FAQ](/Resources/Documents/FAQ.md)
- [Development](/Resources/Documents/Development.md)
- [Platforms](/Resources/Documents/Platforms.md)
- [Glossary](/Resources/Documents/Glossary.md)