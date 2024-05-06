#!/usr/bin/python3

import cv2
import logging
from ApplicationArguments import ApplicationArguments
from Camera import Camera
from CameraDateTime import StaticDisplay
from VideoStreamer import VideoStreamer


# Initialization
def Initialize():
    videoStreamer = VideoStreamer(camera, arguments.port)


# Camera processing
def Main():
    Initialize()
    camera.Start()
    # videoStreamer.Start()
    try:
        while True:
            pass
    except KeyboardInterrupt:
        logging.warning("Program stopped.")
    finally:
        Finalize()


# Update frame
def UpdateFrame(frame):
    StaticDisplay.Display(frame)


# Finalization
def Finalize():
    camera.Stop()
    cv2.destroyAllWindows()


if __name__ == "__main__":
    Main()
