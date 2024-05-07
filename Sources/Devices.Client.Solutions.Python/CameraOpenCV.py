#!/usr/bin/python3

import cv2
import logging


# Camera source (OpenCV)
class CameraOpenCV:

    # Initialization
    def __init__(self, source, width, height, fps):
        if source == "USB":
            self.__pipeline = 0
            self.__width = width
            self.__height = height
            self.__fps = fps
        elif source == "JetsonUSB":
            self.__pipeline = f"v4l2src device=/dev/video0 ! image/jpeg, width={width}, height={height}, framerate={fps}/1, format=MJPG ! nvv4l2decoder mjpeg=1 enable-max-performance=1 ! nvvidconv flip-method=4 ! videoconvert ! video/x-raw, format=(string)BGR ! appsink sync=false"
        elif source == "JetsonCSI":
            self.__pipeline = f"nvarguscamerasrc sensor-id=0 ! video/x-raw(memory:NVMM), width=(int){height}, height=(int){height}, framerate=(fraction){fps}/1 ! nvvidconv flip-method=0 ! video/x-raw, width=(int){height}, height=(int){height}, format=(string)BGRx ! videoconvert ! video/x-raw, format=(string)BGR ! appsink sync=false"
        else:
            raise Exception(f"Camera source '{source}' is not supported.")
        logging.info(f"Camera configured (Display = {width}x{height}, FPS = {fps}).")

    # Start camera
    def Start(self):
        if isinstance(self.__pipeline, str):
            self.__videoCapture = cv2.VideoCapture(self.__pipeline, cv2.CAP_GSTREAMER)
        else:
            self.__videoCapture = cv2.VideoCapture(self.__pipeline, cv2.CAP_V4L2)
            self.__videoCapture.set(cv2.CAP_PROP_FRAME_WIDTH, self.__width)
            self.__videoCapture.set(cv2.CAP_PROP_FRAME_HEIGHT, self.__height)
            self.__videoCapture.set(cv2.CAP_PROP_FPS, self.__fps)
        if not self.__videoCapture.isOpened():
            raise Exception("Camera start failed.")
        logging.info("Camera started.")

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
        logging.info("Camera stopped.")
