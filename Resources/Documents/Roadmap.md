## Capability Roadmap

### Framework

- Implement specialized device policies & roles (Weather Device, Camera Device, Watering Device).
- Implement user tenant separation (access specific devices & services, limit all devices to tenant devices).
- Handle Running Application Redeployment (Devices.Client.Solutions Watering, Pre/Post Deployment Action).
- Real-Time Communication: Provide Alternative Management Protocol (SignalR, WebRTC, SSH over SignalR / WebRTC), Configuration, Monitoring.
- Provide MQTT support.
- Video: Streaming Security.
- Implement device configuration groups.
- Implement SignalR Redis Backplane.
- Implement web application cache (Redis, Memcached).
- Implement load balancer (HAProxy, AWS ELB).
- Image Processing: OpenCV Deep Learning / OpenCV DNN (Caffe, TensorFlow, PyTorch, Darknet, ONNX), Model Fine Tuning, Track Object Marker Movement.
- Provide additional ML/AI support ([TensorFlow Hub](https://tfhub.dev/) / [Kaggle Models](https://www.kaggle.com/models), [PyTorch Hub](https://pytorch.org/hub/), [Hugging Face Models](https://huggingface.co/docs/transformers/), Hugging Face Transformers, [ONNX Runtime](https://onnxruntime.ai/)).
- Improve connectivity resiliency. Save data locally on connection error & resend on successful subsequent transmission. Apply to weather data & camera notifications.
- Improve performance and resilience by implementing messaging support (Kafka).
- Provide additional programming language support (C/C++, Java).
- Provide additional connectivity support (Bluetooth, BLE, Zigbee, Cellular).

### Reference Solutions

- Watering System Enhancements: Drip Watering Line, Water Level Sensor, Reverse Pumps, Relay & Terminal Strip Block Housing (3D Print).
- Intrusion Detection System: Entrance Sensors, Visual Notification, Video.
- Camera Enhancements: Smooth Pan & Tilt.