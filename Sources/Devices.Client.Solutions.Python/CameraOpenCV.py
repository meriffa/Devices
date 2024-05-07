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
        elif source == "JetsonV2":
            self.__pipeline = f"libcamerasrc camera-name=/base/axi/pcie@120000/rp1/i2c@88000/imx219@10 ! video/x-raw, format=RGBx, width={width}, height={height}, framerate={fps}/1 ! videoconvert ! video/x-raw, format=(string)BGR ! appsink sync=false"
        elif source == "JetsonHQ":
            self.__pipeline = f"libcamerasrc camera-name=/base/axi/pcie@120000/rp1/i2c@88000/imx477@1a ! video/x-raw, format=RGBx, width={width}, height={height}, framerate={fps}/1 ! videoconvert ! video/x-raw, format=(string)BGR ! appsink sync=false"
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
