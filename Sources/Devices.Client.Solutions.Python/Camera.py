#!/usr/bin/python3

import cv2
import logging


# Camera source
class Camera:

    # Initialization
    def __init__(self, source, width, height, fps):
        if source == "JetsonUSB":
            self.__pipeline = f"v4l2src device=/dev/video0 ! image/jpeg, width={width}, height={height}, framerate={fps}/1, format=MJPG ! nvv4l2decoder mjpeg=1 enable-max-performance=1 ! nvvidconv flip-method=4 ! videoconvert ! video/x-raw, format=(string)BGR ! appsink sync=false"
        elif source == "PiCSI":
            raise Exception(f"Under construction.")
        else:
            raise Exception(f"Camera source '{source}' is not supported.")
        logging.info(f"Camera pipeline configured (Display = {width}x{height}, FPS = {fps}).")

    # Start camera
    def Start(self):
        self.__videoCapture = cv2.VideoCapture(self.__pipeline, cv2.CAP_GSTREAMER)
        if not self.__videoCapture.isOpened():
            raise Exception("Camera pipeline start failed.")
        logging.info("Camera pipeline started.")

    # Return camera frame
    def GetFrame(self):
        success, frame = self.__videoCapture.read()
        if success:
            return frame
        else:
            return None

    # Stop camera
    def Stop(self):
        self.__videoCapture.release()
        logging.info("Camera pipeline stopped.")
