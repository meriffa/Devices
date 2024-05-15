## Capability Roadmap

### Framework

- Real-Time Communication: Provide Alternative Management Protocol (SignalR, WebRTC), Configuration, Monitoring
- Live Video: Streaming, Streaming Security, Image Processing, Dedicated Media Server
- Image Processing: OpenCV DNN (Caffe, TensorFlow, PyTorch, Darknet, ONNX), Model Fine Tuning, Track Object Marker Movement
- Provide additional ML/AI support ([TensorFlow Hub](https://tfhub.dev/) / [Kaggle Models](https://www.kaggle.com/models), [PyTorch Hub](https://pytorch.org/hub/), [Hugging Face Models](https://huggingface.co/docs/transformers/), Hugging Face Transformers, [ONNX Runtime](https://onnxruntime.ai/)).
- Implement specialized device policies & roles (Weather Device, Camera Device).
- Improve connectivity resiliency. Save data locally on connection error & resend on successful subsequent transmission. Apply to weather data & camera notifications.
- Review potential proprietary IoT framework integration ([AWS IoT Device Management](https://aws.amazon.com/iot-device-management/), [Azure IoT Hub](https://azure.microsoft.com/products/iot-hub/), [IBM Watson IoT Platform](https://internetofthings.ibmcloud.com/)).
- Improve performance and resilience by implementing messaging support (Kafka).
- Improve programming language support (Python).
- Provide additional programming language support (C/C++, Java).
- Provide additional connectivity support (Bluetooth, BLE, Zigbee).

### Garden Management System

- Watering System 2.0: Water Level Sensor, Reverse Pumps, Relay & Terminal Strip Block Housing (3D Print)
- Implement direct device control (buttons, LCD display).