## Reference Solutions

### Garden Management System

- Hardware
  - BME280 Module (VCC = 3V3 [1], GND = GND [9], SCL = SCL [5], SDA = SDA [3])
  - MAX44009 Module (VCC = 3V3 [1], GND = GND [9], SCL = SCL [5], SDA = SDA [3])
- Software: Enable I2C, dotnet Devices.Client.Solutions.dll Garden

### Peripherals

#### Inputs

- Joystick (ADS7830)
  - Hardware: Joystick.png, Joystick Module, A/D Converter Module (ADS7830), Resistor (10kΩ)
  - Software: Enable I2C, dotnet Devices.Client.Solutions.dll "Inputs-Joystick" -t "ADS7830"
- Joystick (PCF8591)
  - Hardware: Joystick Module (X = AIN0, Y = AIN1, BTN = GPIO18), A/D Converter Module (PCF8591, SCL = SCL1, SDA = SD1), Resistor (10kΩ)
  - Software: Enable I2C, dotnet Devices.Client.Solutions.dll "Inputs-Joystick" -t "PCF8591"
- Keypad
  - Hardware: Keypad.png, Key Matrix (4x4), Resistor (4x, 10kΩ), Replace 3V3 Connection With GND Connection (Pull Up -> Pull Down)
  - Software: dotnet Devices.Client.Solutions.dll "Inputs-Keypad"
- Potentiometer
  - Hardware: Potentiometer.png, Potentiometer Module, A/D Converter Module (ADS7830)
  - Software: Enable I2C, dotnet Devices.Client.Solutions.dll "Inputs-Potentiometer"
- Real Time Clock (DS1302)
  - Hardware: Real Time Clock (DS1302).png, RTC DS1302 Module
  - Software: dotnet Devices.Client.Solutions.dll "Inputs-RealTimeClockDS1302"
- Real Time Clock (DS1307)
  - Hardware: Adafruit RGB Matrix HAT + RTC
  - Software: Enable I2C, dotnet Devices.Client.Solutions.dll "Inputs-RealTimeClockDS1307"
- Rotary Encoder
  - Hardware: Rotary Encoder Module
  - Software: dotnet Devices.Client.Solutions.dll "Inputs-RotaryEncoder"
- On / Off Input Device
  - Hardware
    - Button.png, Button Module (SIG = GPIO17, VCC = 3V3, GND = GND)
    - Hall Switch.png, Hall Switch Module (SIG = GPIO17, VCC = 3V3, GND = GND)
    - Infrared Proximity Switch (Tracking).png, IR Tracking Module (SIG = GPIO17, VCC = 3V3, GND = GND)
    - Infrared Proximity Switch (IR Obstacle).png, IR Obstacle Module (SIG = GPIO17, VCC = 3V3, GND = GND)
    - IR Receiver.png, IR Receiver Module (SIG = GPIO17, VCC = 3V3, GND = GND), IR Remote Control
    - Photo Interrupter.png, Photo Interrupter Module (SIG = GPIO17, VCC = 3V3, GND = GND)
    - Reed Switch.png, Reed Switch Module (SIG = GPIO17, VCC = 3V3, GND = GND)
    - Tilt Switch.png, Tilt Switch Module (SIG = GPIO17, VCC = 3V3, GND = GND)
    - Touch Switch.png, Touch Switch Module (SIG = GPIO17, VCC = 3V3, GND = GND)
    - Vibration Switch.png, Vibration Switch Module (SIG = GPIO17, VCC = 3V3, GND = GND)
  - Software: dotnet Devices.Client.Solutions.dll "Inputs-Switch"

#### Inputs: Sensors

- Analog Temperature Sensor
  - Hardware: Thermistor.png, Thermistor Module, A/D Converter Module (PCF8591)
  - Hardware: Analog Temperature Sensor.png, Analog Temperature (LM393) Module, A/D Converter Module (PCF8591)
  - Software: Enable I2C, dotnet Devices.Client.Solutions.dll "Sensors-AnalogTemperatureSensor"
- Barometric Sensor
  - Hardware: Barometric Sensor.png, Barometric Sensor Module
  - Software: Enable I2C, dotnet Devices.Client.Solutions.dll "Sensors-BarometricSensor"
- Digital Temperature Sensor
  - Hardware: Digital Temperature Sensor.png, Temperature Sensor (DS18B20) Module
  - Software: Enable 1-Wire, dotnet Devices.Client.Solutions.dll "Sensors-DigitalTemperatureSensor"
- Analog Hall Sensor
  - Hardware: Analog Hall Sensor.png, Analog Hall Switch Module, A/D Converter Module (PCF8591)
  - Software: Enable I2C, dotnet Devices.Client.Solutions.dll "Sensors-GenericAnalogSensor" -l "Magnetic Field"
- Photoresistor
  - Hardware: Photoresistor.png, Photoresistor Module, A/D Converter Module (PCF8591)
  - Software: Enable I2C, dotnet Devices.Client.Solutions.dll "Sensors-GenericAnalogSensor" -l "Light"
- Sound Sensor
  - Hardware: Sound Sensor.png, Sound Sensor Module, A/D Converter Module (PCF8591)
  - Software: Enable I2C, dotnet Devices.Client.Solutions.dll "Sensors-GenericAnalogSensor" -l "Sound"
- Flame Sensor
  - Hardware: Flame Sensor.png, Flame Sensor Module, A/D Converter Module (PCF8591)
  - Software: Enable I2C, dotnet Devices.Client.Solutions.dll "Sensors-GenericAnalogSensor" -l "Flame"
- Gas Sensor
  - Hardware: Gas Sensor.png, Gas Sensor Module, A/D Converter Module (PCF8591)
  - Software: Enable I2C, dotnet Devices.Client.Solutions.dll "Sensors-GenericAnalogSensor" -l "Gas"
- Rain Sensor
  - Hardware: Rain Sensor.png, Raindrop Sensor Module, A/D Converter Module (PCF8591)
  - Software: Enable I2C, dotnet Devices.Client.Solutions.dll "Sensors-GenericAnalogSensor" -l "Rain"
- Gyroscope Accelerometer Sensor
  - Hardware: Gyroscope Accelerometer Sensor.png, MPU6050 Module
  - Software: dotnet Devices.Client.Solutions.dll "Sensors-GyroscopeAccelerometerSensor"
- Laser Tripwire
  - Hardware: Laser Tripwire.png, Laser Transmitter Module (KY-008) (S = GPIO17, 5V = 5V, - = GND), Laser Receiver Sensor Module (DS18B20) (VCC = 5V, OUT = GPIO21, GND = GND), Resistor (2x, 10kΩ)
  - Software: dotnet Devices.Client.Solutions.dll "Sensors-LaserTripwire"
- LIDAR Lite Sensor
  - Hardware: LIDAR-Lite V3HP Sensor (5V (Red) = 5V, Ground (Black) = GND, Power Enable (Orange) = GPIO17, I2C SCL (Green) = SCL, I2C SDA (Blue) = SDA)
  - Software: Enable I2C, dotnet Devices.Client.Solutions.dll "Sensors-LIDARLiteSensor"
- Temperature & Humidity Sensor
  - Hardware: Temperature & Humidity Sensor.png, Humiture Module
  - Software: Enable 1-Wire, dotnet Devices.Client.Solutions.dll "Sensors-TemperatureAndHumiditySensor"
- Ultrasonic Distance Sensor
  - Hardware: Ultrasonic Distance Sensor.png, Ultrasonic Sensor Module (HC-SR04) Module
  - Software: dotnet Devices.Client.Solutions.dll "Sensors-UltrasonicDistanceSensor"

#### Inputs: Camera

- Raspberry Pi Camera
  - Hardware: Raspberry Pi Camera Module V2 (RPI-CAM-V2) 
  - Software: Enable Camera, Install & Configure MediaMTX / v4l2rtspserver / Motion Daemon

#### Outputs

- On / Off Output Device
  - Hardware: Active Buzzer.png, Active Buzzer Module
  - Hardware: Laser Emitter.png, Laser Emitter Module
  - Hardware: Relay.png, Relay Module
  - Software: dotnet Devices.Client.Solutions.dll "Outputs-GenericOutput"
- Passive Buzzer
  - Hardware: Passive Buzzer.png, Passive Buzzer Module
  - Software: dotnet Devices.Client.Solutions.dll "Outputs-PassiveBuzzer"
- LCD Display
  - Hardware: LCD Display.png, I2C LCD1602 (PCF8574T) Module
  - Software: dotnet Devices.Client.Solutions.dll "Outputs-LCDDisplay"
- ePaper Display
  - Hardware: Waveshare 7.5inch e-Paper (B) Display, e-Paper Driver HAT (HAT Board Alignment = Inside Relative To Raspberry Pi, Ribbon Cable Orientation = Pins Up Both Sides)
  - Software: Enable SPI, dotnet Devices.Client.Solutions.dll "Outputs-EPaperDisplay" -i Image.png
- Pump Relay Module
  - Hardware: GND = GND, IN1 = GPIO17, IN2 = GPIO27, IN3 = GPIO22, IN4 = GPIO5, VCC = 5V
- Arducam Camera Pan Tilt Platform
  - Hardware: VCC = 5V, SDA = SDA, SCL = SCL, GND = GND

#### Outputs: LEDs

- Single Color LED
  - Hardware: Single Color LED.png, Single Color LED (Red), Resistor (220Ω)
  - Software: dotnet Devices.Client.Solutions.dll "LED-SingleColorLED"
- Rolling Color LED
  - Hardware: Rolling Color LED.png, Auto Flash LED Module (VCC = GPIO17, GND = GND)
  - Software: dotnet Devices.Client.Solutions.dll "LED-RollingColorLED"
- Dual Color LED
  - Hardware: Dual Color LED.png, Dual Color LED Module (R = GPIO17, G = GPIO18, GND = GND)
  - Software: dotnet Devices.Client.Solutions.dll "LED-DualColorLED"
- RGB LED
  - Hardware: RGB LED.png, RGB LED Module (VCC = 3V3, R = GPIO17, G = GPIO18, B = GPIO27))
  - Software: dotnet Devices.Client.Solutions.dll "LED-RGBLED"
- LED Bar Graph
  - Hardware: LED Bar Graph.png, LED Bar Graph, SN74HC595N, Resistor (8x, 220Ω)
  - Software: dotnet Devices.Client.Solutions.dll "LED-LEDBarGraph"
- 7-Segment 1-Digit LED Display
  - Hardware: 7-Segment 1-Digit LED Display.png, 7-Segment 1-Digit LED Display, SN74HC595N, Resistor (8x, 220Ω)
  - Software: dotnet Devices.Client.Solutions.dll "LED-LEDDisplay7Segment1Digit"
- 7-Segment 4-Digit LED Display
  - Hardware: 7-Segment 4-Digit LED Display.png, 7-Segment 4-Digit LED Display, SN74HC595N, Resistor (8x, 220Ω), Resistor (4x, 1kΩ), PNP Transistor (4x, S8550)
  - Software: dotnet Devices.Client.Solutions.dll "LED-LEDDisplay7Segment4Digit"
- LED Matrix
  - Hardware: LED Matrix.png, LED Matrix (8x8), SN74HC595N (2x), Resistor (8x, 220Ω)
  - Software: dotnet Devices.Client.Solutions.dll "LED-LEDMatrix"

#### Outputs: RBG LED Matrix

- Hardware: Adafruit RGB Matrix HAT + RTC, Waveshare 64x64 RGB LED Matrix (2x) (Panel 1 JIN = Panel 2 JOUT, Panel 2 JIN = RGB Matrix HAT)
- Software
  - dotnet Devices.Client.Solutions.dll "RBGLEDMatrix-Ant" -h 2
  - dotnet Devices.Client.Solutions.dll "RBGLEDMatrix-Equalizer" -h 2
  - dotnet Devices.Client.Solutions.dll "RBGLEDMatrix-Genetics" -h 2
  - dotnet Devices.Client.Solutions.dll "RBGLEDMatrix-Image" -h 2 -f "Image.png"
  - dotnet Devices.Client.Solutions.dll "RBGLEDMatrix-Panel" -h 2
  - dotnet Devices.Client.Solutions.dll "RBGLEDMatrix-Rain" -h 2
  - dotnet Devices.Client.Solutions.dll "RBGLEDMatrix-Sandpile" -h 2
  - dotnet Devices.Client.Solutions.dll "RBGLEDMatrix-Snowball" -h 2
  - dotnet Devices.Client.Solutions.dll "RBGLEDMatrix-Text" -h 2 -t "Hello World!"
  - dotnet Devices.Client.Solutions.dll "RBGLEDMatrix-Video" -h 2 -f "Video.mp4"

#### Outputs: Motors

- DC Motor (L293D)
  - Hardware: DC Motor (L293D).png, DC Motor, Motor Driver Chip (L293D), Breadboard Power Module, L293D (+, -) = Breadboard Power Module (+, -)
  - Software: dotnet Devices.Client.Solutions.dll "Motors-DCMotorL293D" -s 50
- DC Motor (L298N)
  - Hardware: DC Motor (L298N).png, DC Motor, L298N Motor Drive Controller ((OUT1, OUT2) = DC Motor (+, -), (VS, GND) = Power Module (+, -), ENA = GPIO6, IN1 = GPI27, IN2 = GPI22), Breadboard Power Module (GND = Raspberry Pi GND)
  - Software: dotnet Devices.Client.Solutions.dll "Motors-DCMotorL298N" -s 50
- DC Motor (Motor HAT)
  - Hardware: DC Motor, Motor HAT (MA1 = DC Motor +, MA2 = DC Motor -, VIN = Power Module 5V, GND = Power Module GND), Breadboard Power Module
  - Software: dotnet Devices.Client.Solutions.dll "Motors-DCMotorHAT" -s 50
- Servo Motor
  - Hardware: Servo Motor.png, Servo Motor
  - Software: dotnet Devices.Client.Solutions.dll "Motors-ServoMotor"
- Stepper Motor
  - Hardware: Stepper Motor.png, Stepper Motor, Stepper Motor Driver (ULN2003APG), Breadboard Power Module, Raspberry Pi (-) = Breadboard Power Module (-)
  - Software: dotnet Devices.Client.Solutions.dll "Motors-StepperMotor"