#!/usr/bin/python3

import libcamera
import logging
import picamera2
import picamera2.encoders


# Camera source (Picamera2)
class CameraPicamera2:

    # Initialization
    def __init__(self, width, height, fps):
        self.__camera = picamera2.Picamera2()
        main = {"format": "RGB888", "size": (width, height)}
        controls = {"FrameDurationLimits": (int(1000000 / fps), int(1000000 / fps))}
        transform = libcamera.Transform(hflip=False, vflip=True)
        configuration = self.__camera.create_preview_configuration(main=main, controls=controls, transform=transform)
        self.__camera.align_configuration(configuration)
        self.__camera.configure(configuration)
        logging.info(f"Camera configured (Display: {width}x{height}, FPS: {fps}).")

    # Start camera
    def Start(self):
        self.__camera.start(show_preview=False)
        logging.info("Camera started.")

    # Return frame
    def GetFrame(self):
        return self.__camera.capture_array("main")

    # Stop camera
    def Stop(self):
        self.__camera.stop()
        logging.info("Camera stopped.")
