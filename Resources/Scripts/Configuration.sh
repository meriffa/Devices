#!/bin/bash

# DietPi Configuration
dietpi-config 1 -> RPi Camera = On -> Reboot                                                    # Enable Camera
ls /dev/video*
dietpi-config 3 -> I2C State = On, I2C Frequency = 100kHz -> Reboot                             # Enable I2C
i2cdetect -y 1
dietpi-config 3 -> SPI State = On -> Reboot                                                     # Enable SPI
ls /sys/dev/spi*
sudo echo "dtoverlay=w1-gpio,gpiopin=4" | sudo tee -a /boot/config.txt                          # Enable 1-Wire
reboot now
ls /sys/bus/w1/devices/