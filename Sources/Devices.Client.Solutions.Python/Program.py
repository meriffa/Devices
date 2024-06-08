#!/usr/bin/python3

import configparser
import cv2
import logging
import logging.config
from ApplicationArguments import ApplicationArguments
from CameraDateTime import CameraDateTime
from CameraFPS import CameraFPS
from VideoPublisher import VideoPublisher


# Initialization
def Initialize():
    global camera, cameraDateTime, cameraFPS, displayPreview, videoPublisher
    arguments = ApplicationArguments().Parse()
    logging.config.fileConfig(f"{__file__[:-3]}.conf")
    configuration = configparser.ConfigParser()
    configuration.read(f"{__file__[:-3]}.ini")
    if arguments.source == "PiCSI":
        import CameraPicamera2
        camera = CameraPicamera2.CameraPicamera2(arguments.width, arguments.height, arguments.fps)
    else:
        import CameraOpenCV
        camera = CameraOpenCV.CameraOpenCV(arguments.source, arguments.width, arguments.height, arguments.fps, arguments.focus)
    cameraDateTime = CameraDateTime(arguments.displayDateTime)
    cameraFPS = CameraFPS(arguments.displayFPS)
    displayPreview = arguments.displayPreview
    videoPublisher = VideoPublisher(arguments.width, arguments.height, arguments.fps, arguments.location)


# Process frame
def ProcessFrame(camera, cameraDateTime, cameraFPS):
    frame = camera.GetFrame()
    if frame is not None:
        cameraDateTime.Display(frame)
        cameraFPS.Display(frame)
        videoPublisher.Display(frame)
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
    if displayPreview:
        cv2.destroyAllWindows()


# Camera feed processing
def Main():
    windowTitle = "Devices.Client.Solutions.Python"
    Initialize()
    camera.Start()
    videoPublisher.Start()
    try:
        if displayPreview:
            _ = cv2.namedWindow(windowTitle, cv2.WINDOW_AUTOSIZE)
        while True:
            cameraFPS.Start()
            if displayPreview:
                if StopRequested(windowTitle):
                    break
            frame = ProcessFrame(camera, cameraDateTime, cameraFPS)
            if frame is not None and displayPreview:
                cv2.imshow(windowTitle, frame)
            cameraFPS.Stop()
    except KeyboardInterrupt:
        logging.info("Program interrupted.")
    finally:
        Finalize(camera)


if __name__ == "__main__":
    Main()
