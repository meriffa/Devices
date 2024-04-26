#!/usr/bin/python3

from ApplicationArguments import ApplicationArguments
from Camera import Camera
from CameraFPS import CameraFPS
from Model import Model
import cv2


# Initialization
def Initialize():
    arguments = ApplicationArguments().Parse()
    camera = Camera(arguments.width, arguments.height, arguments.fps)
    cameraFPS = CameraFPS(arguments.displayFPS)
    model = Model()
    return camera, cameraFPS, model


# Check if program stop is requested
def StopRequested(windowTitle):
    if cv2.getWindowProperty(windowTitle, cv2.WND_PROP_AUTOSIZE) < 0:
        return True
    keyCode = cv2.waitKey(1) & 0xFF
    if keyCode == 27 or keyCode == ord("q"):
        return True
    return False


# Process frame
def ProcessFrame(camera, cameraFPS, model):
    frame = camera.GetFrame()
    cameraFPS.Display(frame)
    model.Display(frame, model.Run(frame))
    return frame


# Finalization
def Finalize(camera):
    camera.Stop()
    cv2.destroyAllWindows()


# Camera feed processing
def Main():
    windowTitle = "Camera"
    camera, cameraFPS, model = Initialize()
    camera.Start()
    try:
        _ = cv2.namedWindow(windowTitle, cv2.WINDOW_AUTOSIZE)
        while True:
            cameraFPS.Start()
            if StopRequested(windowTitle):
                break
            cv2.imshow(windowTitle, ProcessFrame(camera, cameraFPS, model))
            cameraFPS.Stop()
    except KeyboardInterrupt:
        print("Program interrupted.")
    finally:
        Finalize(camera)


if __name__ == "__main__":
    Main()
