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
        self.__originalSize = None
        self.__fullResolution = None
        logging.info(f"Camera configured (Display: {width}x{height}, FPS: {fps}).")

    # Start camera
    def Start(self):
        self.__camera.start(show_preview=False)
        logging.info("Camera started.")

    # Return frame
    def GetFrame(self):
        return self.__camera.capture_array("main")

    # Set camera zoom
    def GetFocusRange(self):
        return self.__camera.camera_controls["LensPosition"][:2]

    # Set camera focus
    def SetFocus(self, value):
        if value == 0:
            self.__camera.set_controls({"AfMode": libcamera.controls.AfModeEnum.Continuous})
            logging.info("Camera focus set to continuous.")
        else:
            self.__camera.set_controls({"AfMode": libcamera.controls.AfModeEnum.Manual, "LensPosition": value})
            logging.info(f"Camera focus set to {value}.")

    # Set camera zoom
    def SetZoom(self, value):
        self.__camera.capture_metadata()
        if self.__originalSize is None:
            self.__originalSize = self.__camera.capture_metadata()["ScalerCrop"][2:]
        if self.__fullResolution is None:
            self.__fullResolution = self.__camera.camera_properties["PixelArraySize"]
        scale = 1 - (value - 1) / 10
        size = [int(s * scale) for s in self.__originalSize]
        offset = [(r - s) // 2 for r, s in zip(self.__fullResolution, size)]
        self.__camera.set_controls({"ScalerCrop": offset + size})
        logging.info(f"Camera zoom level set to {value}x.")

    # Stop camera
    def Stop(self):
        self.__camera.stop()
        logging.info("Camera stopped.")
