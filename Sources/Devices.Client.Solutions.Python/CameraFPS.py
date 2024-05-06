#!/usr/bin/python3

import cv2
import time


# Camera FPS display
class CameraFPS:

    # Initialization
    def __init__(self, enabled):
        self.__fps = 0.0
        self.__enabled = enabled

    # Start FPS measurement
    def Start(self):
        if self.__enabled:
            self.__start = time.time()

    # Display FPS value
    def Display(self, frame):
        if self.__enabled:
            text = f"{int(self.__fps)} fps"
            cv2.putText(frame, text, (frame.shape[1] - len(text) * 14, 25), cv2.FONT_HERSHEY_SIMPLEX, 0.75, (0, 0, 255), 1, cv2.LINE_AA)

    # Stop FPS measurement
    def Stop(self):
        if self.__enabled:
            self.__fps = 0.9 * self.__fps + 0.1 * (1 / (time.time() - self.__start))
