#!/usr/bin/python3

from ApplicationArguments import ApplicationArguments
from Camera import Camera
from CameraDateTime import CameraDateTime
from CameraFPS import CameraFPS
from ObjectDetection import ObjectDetection
from VideoStreamer import VideoStreamer
import configparser
import cv2
import logging
import logging.config


# Initialization
def Initialize():
    arguments = ApplicationArguments().Parse()
    logging.config.fileConfig(f"{__file__[:-3]}.conf")
    configuration = configparser.ConfigParser()
    configuration.read(f"{__file__[:-3]}.ini")
    camera = Camera(arguments.source, arguments.width, arguments.height, arguments.fps)
    cameraDateTime = CameraDateTime(arguments.displayDateTime)
    cameraFPS = CameraFPS(arguments.displayFPS)
    objectDetection = ObjectDetection()
    videoStreamer = VideoStreamer(arguments.port)
    return camera, cameraDateTime, cameraFPS, objectDetection, videoStreamer, arguments.displayPreview


# Process frame
def ProcessFrame(camera, cameraDateTime, cameraFPS, objectDetection, videoStreamer):
    frame = camera.GetFrame()
    if frame is not None:
        cameraDateTime.Display(frame)
        cameraFPS.Display(frame)
        objectDetection.Display(frame)
        videoStreamer.Display(frame)
    return frame


# Check if program stop is requested
def StopRequested(windowTitle):
    if cv2.getWindowProperty(windowTitle, cv2.WND_PROP_AUTOSIZE) < 0:
        return True
    keyCode = cv2.waitKey(1) & 0xFF
    if keyCode == 27 or keyCode == ord("q"):
        return True
    return False


# Finalization
def Finalize(camera):
    camera.Stop()
    cv2.destroyAllWindows()


# Camera feed processing
def Main():
    windowTitle = "Devices.Client.Solutions.Python"
    camera, cameraDateTime, cameraFPS, objectDetection, videoStreamer, displayPreview = Initialize()
    camera.Start()
    videoStreamer.Start()
    try:
        if displayPreview:
            _ = cv2.namedWindow(windowTitle, cv2.WINDOW_AUTOSIZE)
        while True:
            cameraFPS.Start()
            if displayPreview:
                if StopRequested(windowTitle):
                    break
            frame = ProcessFrame(camera, cameraDateTime, cameraFPS, objectDetection, videoStreamer)
            if frame is not None and displayPreview:
                cv2.imshow(windowTitle, frame)
            cameraFPS.Stop()
    except KeyboardInterrupt:
        logging.info("Program interrupted.")
    finally:
        Finalize(camera)


if __name__ == "__main__":
    Main()
