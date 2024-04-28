#!/usr/bin/python3

import libcamera
import logging
import picamera2
import picamera2.encoders


# Camera source
class Camera:

    # Initialization
    def __init__(self, width, height, fps, updateFrame, bitrate=12000000):
        self.__camera = picamera2.Picamera2()
        main = {"size": (width, height), "format": "RGB888"}
        controls = {"FrameDurationLimits": (int(1000000 / fps), int(1000000 / fps))}
        transform = libcamera.Transform(hflip=True, vflip=False)
        configuration = self.__camera.create_preview_configuration(main=main, controls=controls, transform=transform)
        self.__camera.align_configuration(configuration)
        self.__camera.configure(configuration)
        self.__encoder = picamera2.encoders.MJPEGEncoder(bitrate)
        self.__camera.encoder = self.__encoder
        self.__updateFrame = updateFrame
        self.__camera.post_callback = self.UpdateFrame
        self.__output = []
        logging.info(f"Camera configured (Display: {width}x{height}, FPS: {fps}).")

    # Start camera
    def Start(self):
        self.__camera.start(show_preview=False)
        logging.info("Camera started.")
        self.__camera.start_encoder(self.__encoder)
        logging.info("Camera encoder started.")

    # Stop camera
    def Stop(self):
        self.__camera.stop_encoder(self.__encoder)
        logging.info("Camera encoder stopped.")
        self.__camera.stop()
        logging.info("Camera stopped.")

    # Update frame
    def UpdateFrame(self, request):
        with picamera2.MappedArray(request, "main") as buffer:
            frame = buffer.array
            if self.__updateFrame:
                self.__updateFrame(frame)

    # Add transient encoder
    def AddTransientEncoder(self, output):
        self.__encoder.output = self.__output + [output]
