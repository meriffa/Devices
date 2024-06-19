## Frequently Asked Questions

- [What is the purpose of this software?](#what-is-the-purpose-of-this-software)
- [What is a Mini Computer?](#what-is-a-mini-computer)
- [What problems does the software solve?](#what-problems-does-the-software-solve)
- [What are the benefits of using this software?](#what-are-the-benefits-of-using-this-software)
- [What are potential applications of this software?](#what-are-potential-applications-of-this-software)
- [What are the software design principles?](#what-are-the-software-design-principles)
- [What are the advantages of using this software?](#what-are-the-advantages-of-using-this-software)
- [What are the cases not suitable for this software?](#what-are-the-cases-not-suitable-for-this-software)
- [Why should I use this software instead of building my own?](#why-should-i-use-this-software-instead-of-building-my-own)
- [How much can the software scale?](#how-much-can-the-software-scale)
- [How much does this software cost?](#how-much-does-this-software-cost)
- [How does the software work?](#how-does-the-software-work)

### What is the purpose of this software?
The purpose of this software is to allow companies to manage their Mini Computers. Management includes identity & security, monitoring, application deployment & configuration services.

### What is a Mini Computer?
Mini Computer is a Single Board Computer (e.g. Raspberry Pi, NVIDIA Jetson), Mini PC (AMD Ryzen Embedded, Intel NUC) or System on Chip (SoC) Internet connected device, running full operating system (Linux, Windows). The device has attached inputs (sensors, cameras) and/or outputs (relays, motors, servos). See the [Managed Devices](/Resources/Images/Devices.png) diagram as an example.

### What problems does the software solve?
Any number of connected Mini Computers require some level of management. Such management includes - device identity (unique device identity), inventory (how many devices do I have, what devices do I have, where are my devices), security (what is the device allowed to do, is the device lost/stolen), monitoring (are my devices up and running, are there any device outages), configuration (reconfigure settings, upgrade OS, deploy applications).

### What are the benefits of using this software?
The software provides ready to use highly customizable device management services at no cost. This allows a company to focus its resources on building business applications and systems instead of infrastructure services.

### What are potential applications of this software?
The software is applicable in a wide range of settings - Manufacturing, Retail, Digital Signage, Warehouse Automation, Real Estate (Property Management, Residential / Commercial Buildings), Transportation & Logistics, Agriculture, Chemicals, Oil & Gas, Healthcare, Housing, Robotics, Food & Beverage (Breweries, Wineries, Distilleries, Bakeries, Meat Processing), Sports & Entertainment (Golf Courses, Sports Arenas), Religious Organizations. There are several reference implementation (Weather Station, Watering System and Intrusion Detection System) that provide a starting point for various automation tasks (sensor data collection, real-time device control, live video processing). The Weather Station collects weather data (temperature, humidity and light) from input sensors. The Watering System allows for remote pump control and plant watering. The Intrusion Detection System monitors for movement in specific areas and provides notifications.

### What are the software design principles?
Simplicity, Reliability, Security, Scalability.

### What are the advantages of using this software?
The advantages of this software are - free to use and modify, transparency, robustness, scalability, full control over cost, no vendor lock-in. The software is published under [MIT License](/LICENSE). The open source allows for full visibility of what the software does and how it works. The software is a result of decades of development and infrastructure expertise across various industries and multiple large scale global software deployments.

### What are the cases not suitable for this software?
The software is not suitable for cases where turn-key solution is required. The software framework requires certain level of customization and addition hosting infrastructure before lunch. The software targets implementations where the objective is to achieve the best outcome and most optimal cost.

### Why should I use this software instead of building my own?
The advantages of using this software over building an in-house one are - leverage existing & future investments in framework capabilities, leverage expertise and industry best practices built into the software. Use as a basis for home grown platform at a later stage.

### How much can the software scale?
The software solution is build with scalability as a key design principle. It can support hundreds of devices using a minimal server footprint (single AWS EC2 instance) as is. Further performance enhancements, component scale out and server upgrade can allow the platform to support tens of thousands of devices.

### How much does this software cost?
The software is free to use and modify. The deployment requires development effort to customize it to a particular environment and integrate it with existing systems. The deployment also requires its own hosting infrastructure (shared / dedicated, virtualized / physical). The overall customization and infrastructure costs depends on a particular implementation.

### How does the software work?
See the [README](/README.md) for solution architecture and components.