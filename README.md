# IoT Device Management Framework

IoT Device Management Framework is an open-source and cross-platform framework for managing IoT devices.

## Services

The framework provides the following services:

- Device Identity: The service uses fingerprinting services (host name, NIC MAC address, SSH public key) to identify the device using backend database and store the device identity locally.
- Device Monitoring: The service captures basic device metrics (CPU, memory, last reboot time) into a backend database.
- Device Configuration: The service allows for centralized device configuration and application deployment.

## Repository

The repository contains the following components:

- Framework
- Reference Solutions
  - Garden Management System: Provides an end-to-end reference implementation of the framework and its features. The system collects current weather conditions (temperature, humidity, pressure & light) data from device sensors and displays the information on a web site.
  - Peripherals: Provides reference implementations for using various hardware components such as sensors, LEDs, motors, switches, etc.

## Components

Solution consists of the following components:

![Architecture Diagram](./Resources/Images/Architecture.png)

- Devices.Service: Provides framework services to be deployed to cloud infrastructure.
- Devices.Service.Solutions: Provides reference solution services to be deployed to cloud infrastructure.
- Devices.Web: Provides framework web UI components.
- Devices.Web.Solutions: Provides reference solution web UI components.
- Devices.Host: Provides hosting environment for Devices.Service, Devices.Service.Solutions, Devices.Web and Devices.Web.Solutions components.
- Devices.Client: Provides framework device client.
- Devices.Client.Solutions: Provides reference solution device client.

## Platforms

The framework has been tested with the following platforms:

- Devices
  - Raspberry Pi Zero 2 W
  - Raspberry Pi 4 Model B
- Device Operating Systems
  - DietPi 9 (Debian 12, Arm32, ARMv6)
  - DietPi 9 (Debian 12, Arm64, ARMv8)
  - Raspberry Pi OS 2024-03-15 (Debian 12, Arm64, ARMv8)
- Software
  - .NET Core 8.0