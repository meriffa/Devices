#!/usr/bin/python3

import cv2
import PIL.Image
import transformers
from ApplicationArguments import ApplicationArguments
from Camera import Camera
from CameraFPS import CameraFPS


# Initialization
def Initialize():
    arguments = ApplicationArguments().Parse()
    camera = Camera(arguments.width, arguments.height, arguments.fps)
    cameraFPS = CameraFPS(arguments.displayFPS)
    pipeline = transformers.pipeline(model="facebook/detr-resnet-101")
    return camera, cameraFPS, pipeline


# Check if program stop is requested
def StopRequested(windowTitle):
    if cv2.getWindowProperty(windowTitle, cv2.WND_PROP_AUTOSIZE) < 0:
        return True
    keyCode = cv2.waitKey(1) & 0xFF
    if keyCode == 27 or keyCode == ord("q"):
        return True
    return False


# Image Segmentation
def ImageSegmentationDETRBox(pipeline, frame):
    image = PIL.Image.fromarray(cv2.cvtColor(frame, cv2.COLOR_BGR2RGB))
    predictions = pipeline(image)
    for prediction in predictions:
        print(f'Label = {prediction["label"]}, Score = {prediction["score"]}')


# Process frame
def ProcessFrame(camera, cameraFPS, pipeline):
    frame = camera.GetFrame()
    ImageSegmentationDETRBox(pipeline, frame)
    cameraFPS.Display(frame)
    return frame


# Finalization
def Finalize(camera):
    camera.Stop()
    cv2.destroyAllWindows()


# Camera feed processing
def Main():
    windowTitle = "Camera"
    camera, cameraFPS, pipeline = Initialize()
    camera.Start()
    try:
        _ = cv2.namedWindow(windowTitle, cv2.WINDOW_AUTOSIZE)
        while True:
            cameraFPS.Start()
            if StopRequested(windowTitle):
                break
            cv2.imshow(windowTitle, ProcessFrame(camera, cameraFPS, pipeline))
            cameraFPS.Stop()
    except KeyboardInterrupt:
        print("Program interrupted.")
    finally:
        Finalize(camera)


if __name__ == "__main__":
    Main()
