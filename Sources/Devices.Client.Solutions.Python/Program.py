#!/usr/bin/python3

import configparser
import cv2
import logging
import logging.config
from ApplicationArguments import ApplicationArguments
from Camera import Camera
from StaticDisplay import StaticDisplay
from VideoStreamer import VideoStreamer


# Initialization
def Initialize():
    global arguments, configuration, camera, videoStreamer
    arguments = ApplicationArguments().Parse()
    logging.config.fileConfig(f"{__file__[:-3]}.conf")
    configuration = configparser.ConfigParser()
    configuration.read(f"{__file__[:-3]}.ini")
    camera = Camera(arguments.width, arguments.height, arguments.fps, UpdateFrame)
    videoStreamer = VideoStreamer(camera, arguments.port)


# Camera processing
def Main():
    Initialize()
    camera.Start()
    videoStreamer.Start()
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
