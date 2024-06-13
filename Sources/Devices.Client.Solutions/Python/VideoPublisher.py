#!/usr/bin/python3

import cv2
import logging


# Video publisher
class VideoPublisher:

    # Initialization
    def __init__(self, width, height, fps, bitrate, location):
        self.__width = width
        self.__height = height
        self.__fps = fps
        self.__bitrate = int(bitrate / 1000)
        self.__location = location
        if self.__location is not None:
            logging.info(f"Video publisher initialized (Bitrate = {self.__bitrate}k, Location = '{self.__location})'.")

    # Start video publisher
    def Start(self):
        if self.__location is not None:
            target = f"appsrc ! videoconvert ! video/x-raw, format=I420 ! x264enc tune=zerolatency speed-preset=ultrafast bitrate={self.__bitrate} key-int-max={self.__fps * 2} ! video/x-h264, profile=baseline ! rtspclientsink location={self.__location}"
            self.__writer = cv2.VideoWriter(target, cv2.CAP_GSTREAMER, 0, self.__fps, (self.__width, self.__height), True)
            if not self.__writer.isOpened():
                raise Exception("Video publisher start failed.")
            logging.info("Video publisher started.")

    # Publish video frame
    def Display(self, frame):
        if self.__location is not None:
            self.__writer.write(frame)
