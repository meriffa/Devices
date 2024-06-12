#!/usr/bin/python3

import cv2
import datetime


# Camera date & time display
class CameraDateTime:

    # Initialization
    def __init__(self, enabled):
        self.__enabled = enabled

    # Display date & time
    def Display(self, frame):
        if self.__enabled:
            cv2.putText(frame, datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S"), (2, frame.shape[0] - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.75, (255, 255, 255), 1, cv2.LINE_AA)
