## Capability Roadmap

### Framework

- Implement outage display page.
- Implement SignalR Redis Backplane.
- Implement web application cache (Redis, Memcached).
- Implement load balancer (HAProxy, AWS ELB).
- Build Release Management REST API & web pages.
- Real-Time Communication: Provide Alternative Management Protocol (SignalR, WebRTC, SSH over SignalR / WebRTC), Configuration, Monitoring.
- Handle Running Application Redeployment (Devices.Client.Solutions Watering, Pre/Post Deployment Action).
- Video: Streaming Security, Image Processing.
- Image Processing: OpenCV Deep Learning / OpenCV DNN (Caffe, TensorFlow, PyTorch, Darknet, ONNX), Model Fine Tuning, Track Object Marker Movement.
- Provide additional ML/AI support ([TensorFlow Hub](https://tfhub.dev/) / [Kaggle Models](https://www.kaggle.com/models), [PyTorch Hub](https://pytorch.org/hub/), [Hugging Face Models](https://huggingface.co/docs/transformers/), Hugging Face Transformers, [ONNX Runtime](https://onnxruntime.ai/)).
- Implement specialized device policies & roles (Weather Device, Camera Device).
- Improve connectivity resiliency. Save data locally on connection error & resend on successful subsequent transmission. Apply to weather data & camera notifications.
- Improve performance and resilience by implementing messaging support (Kafka).
- Provide additional programming language support (C/C++, Java).
- Provide additional connectivity support (Bluetooth, BLE, Zigbee).
- Implement Device Groups (Configuration).
- Proprietary IoT Services: [AWS IoT Device Management](https://aws.amazon.com/iot-device-management/), [Azure IoT Hub](https://azure.microsoft.com/products/iot-hub/), [IBM Watson IoT Platform](https://internetofthings.ibmcloud.com/).
- IoT Conferences: [IoT Tech Expo](https://www.iottechexpo.com), [IoT Solutions World Congress](https://www.iotsworldcongress.com), [IoT Evolution Expo](https://www.iotevolutionexpo.com).

### Reference Solutions

- Intrusion Detection System: Entrance Sensors, Visual Notification, Video.
- Watering System 2.0: Drip Watering Line, Water Level Sensor, Reverse Pumps, Relay & Terminal Strip Block Housing (3D Print).
- Camera Enhancements: Camera Security (Filter Devices), Smooth Pan & Tilt.
- Implement Direct Device Control (Buttons, LCD Display).