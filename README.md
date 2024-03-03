# IoT Device Management Framework

IoT Device Management Framework is an open-source and cross-platform framework for managing IoT devices.

## Features

The framework provides the following features:

- Device Identity
- Device Monitoring
- Device Configuration
- Device Data Exchange

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

- Device
  - Raspberry Pi 4 Model B
- Device OS
  - DietPi 9.1.1