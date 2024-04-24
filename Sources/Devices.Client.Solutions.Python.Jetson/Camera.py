#!/usr/bin/python3

import cv2


# Camera source
class Camera:

    # Initialization
    def __init__(self, width, height, fps):
        self.__pipeline = f"v4l2src device=/dev/video0 ! image/jpeg, width={width}, height={height}, framerate={fps}/1, format=MJPG ! nvv4l2decoder mjpeg=1 ! nvvidconv flip-method=4 ! videoconvert ! video/x-raw, format=(string)BGR ! appsink sync=false"
        print(f"Camera pipeline configured (Display = {width}x{height}, FPS = {fps}).")

    # Start camera
    def Start(self):
        self.__videoCapture = cv2.VideoCapture(self.__pipeline, cv2.CAP_GSTREAMER)
        if not self.__videoCapture.isOpened():
            raise Exception("Camera pipeline start failed.")
        print("Camera pipeline started.")

    # Return camera frame
    def GetFrame(self):
        _, frame = self.__videoCapture.read()
        return frame

    # Stop camera
    def Stop(self):
        self.__videoCapture.release()
        print("Camera pipeline stopped.")
