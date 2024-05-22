## Frequently Asked Questions

- [What is the purpose of this solution?](#what-is-the-purpose-of-this-solution)
- [What devices is the solution intended for?](#what-devices-is-the-solution-intended-for)
- [What problems does the solution solve?](#what-problems-does-the-solution-solve)
- [What are the benefits of using this solution?](#what-are-the-benefits-of-using-this-solution)
- [What are potential applications of this solution?](#what-are-potential-applications-of-this-solution)
- [What are the solution design principles?](#what-are-the-solution-design-principles)
- [What are the advantages of this solution?](#what-are-the-advantages-of-this-solution)
- [What are the disadvantages of this solution?](#what-are-the-disadvantages-of-this-solution)
- [Why should I use this solution instead of building my own?](#why-should-i-use-this-solution-instead-of-building-my-own)
- [How much can the solution scale?](#how-much-can-the-solution-scale)
- [How much does this solution cost?](#how-much-does-this-solution-cost)
- [How does the solution work?](#how-does-the-solution-work)

### What is the purpose of this solution?
The purpose of this solution is to allow companies to manage their Edge Computing / Edge AI / IoT devices. Device management includes security, monitoring, configuration and application deployment. 

### What devices is the solution intended for?
Devices include Single Board Computers (e.g. Raspberry Pi, NVIDIA Jetson), Mini PCs (AMD Ryzen Embedded, Intel NUC) and other Systems on Chip (SoC) connected to input devices (sensors, cameras) and output devices (relays, motors, pumps) running on full operating system (Linux, Windows). See the [Managed Devices](/Resources/Images/Devices.png) diagram as an example.

### What problems does the solution solve?
Any number of connected devices require some level of management. Such device management capabilities include - inventory (how many devices do I have, what devices do I have, where are my devices), security (what is the device allowed to do, is the device lost/stolen), monitoring (are my devices up and running, are there any device outages), configuration (reconfigure settings, upgrade OS, deploy applications). The solution provides all of these device management capabilities.

### What are the benefits of using this solution?
The solution provides ready to use highly customizable device management services at no cost. This allows a company to focus most of its resources in building business applications and systems instead of infrastructure services.

### What are potential applications of this solution?
The solution is applicable in a wide range of settings - Manufacturing, Retail, Digital Signage, Warehouse Automation, Transportation & Logistics, Agriculture, Chemicals, Oil & Gas, Healthcare, Housing, Robotics. There are several reference implementation (Weather Station, Watering System and Intrusion Detection System) that provide a starting point for various automation systems (sensor data collection, real-time device control, live video processing). The Weather Station collects weather data (temperature, humidity and light) from input sensors. The Watering System allows for remote pump control and plant watering. The Intrusion Detection System monitors for movement in specific areas and provides notifications.

### What are the solution design principles?
Simplicity, Reliability, Security, Scalability.

### What are the advantages of this solution?
The advantages of this solution are - free to use and modify, transparency, robustness, scalability, full control over cost, no vendor lock-in. The solution is published under [MIT License](/LICENSE). The open source allows for full visibility of what the solution does and how it works. The solution is a result of decades of development expertise across various industries and multiple large scale global software deployments.

### What are the disadvantages of this solution?
The disadvantage of this solution is that it requires certain level of customization and addition hosting infrastructure before it can be launched. This is not a turn key solution. This approach allows the solution to focus on implementations where the objective is to achieve the best outcome while having full control over the costs.

### Why should I use this solution instead of building my own?
The advantages of using this solution over building an in-house solution are - leverage existing & future investments in solution capabilities, leverage expertise and industry best practices built into the solution. The solution can be used as a basis for home grown solution at a later stage by providing a solid foundation for a specific device management solution.

### How much can the solution scale?
The solution is build with scalability as a key design principle. It can support hundreds of devices using a minimal server footprint (single AWS EC2 instance) as is. Further performance enhancements, component scale out and server upgrade can allow the solution to support tens of thousands of devices.

### How much does this solution cost?
There are several components that contribute to the overall solution cost. The solution itself is free to use. The solution deployment requires development effort to customize it to a particular environment and potentially integrate it with existing systems. The deployment also requires additional infrastructure (shared / dedicated, physical / virtualized) where the solution will be hosted. The overall customization and infrastructure cost depends on a particular implementation.

### How does the solution work?
See the [README](/README.md) for solution architecture, components and technical details.