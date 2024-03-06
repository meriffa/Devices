## Roadmap

### Framework

- Device Configuration Service: Download Applications, Download Scripts, Run Commands, Pending Deployments, Devices.Client Self Update, Restart, Sync Clock
- Device Data Exchange Service: Identification, Retry, Fallback (store locally), Zip & Transfer Files
- Device OS Support: Raspberry Pi OS, Ubuntu Core
- Device Support: Raspberry Pi Zero Model 2 W, Raspberry Pi Pico, Nvidia Jetson Nano, Orange Pi, Banana Pi, Asus Tinker Board
- Resilience: Random Client Request Delay (DDoS Mitigation), Server / Client Outages (persists & retry), Configurable Deployment Retries
- Programming Support: C/C++/Python/Java (file, web proxy)
- Message Queue Integration: Kafka
- Edge ML Integration: Google Coral Accelerator, [TensorFlow Lite](https://www.tensorflow.org/lite)
- Proprietary IoT Framework Integration: [AWS IoT Device Management](https://aws.amazon.com/iot-device-management/), [Azure IoT Hub](https://azure.microsoft.com/products/iot-hub), [IBM Watson IoT Platform](https://internetofthings.ibmcloud.com/)
- Device Identity Service: Add device attributes.
- Device Configuration Service: Provide targeted deployments using device attributes.
- Connectivity: Implement additional connection protocols (Bluetooth, BLE, Zigbee).

### Garden Management System

- Enhancements: Add Sensor Device (Rain), Display Device (Button, Display), Watering Device (Motors), Security Device (Motion, Camera)