#!/usr/bin/python3

import cv2
import datetime
from ApplicationArguments import ApplicationArguments
from Camera import Camera
from CameraFPS import CameraFPS
from FaceDetector import FaceDetector
from MotionDetector import MotionDetector
from StaticDisplay import StaticDisplay
from VideoRecorder import VideoRecorder
from VideoStreamer import VideoStreamer

# Initialization
def Initialize():
  global arguments, camera, cameraFPS, faceDetector, motionDetector, videoStreamer, videoRecorder
  arguments = ApplicationArguments().Parse()
  camera = Camera(arguments.width, arguments.height, arguments.widthLR, arguments.heightLR, arguments.fps, UpdateFrame)
  cameraFPS = CameraFPS(arguments.displayFPS)
  faceDetector = FaceDetector(arguments.width, arguments.height, arguments.widthLR, arguments.heightLR)
  motionDetector = MotionDetector(arguments.width, arguments.height, arguments.widthLR, arguments.heightLR)
  videoStreamer = VideoStreamer(camera, arguments.port)
  videoRecorder = VideoRecorder(camera, arguments.videoFolder, arguments.videoDuration, arguments.videoTimeout)

# Camera processing
def Main():
  Initialize()  
  camera.Start()
  videoStreamer.Start()
  try:
    while True:
      cameraFPS.Start()
      frame = camera.GetFrame("main")
      frameLowResolution = cv2.cvtColor(camera.GetFrame("lores"), cv2.COLOR_YUV2GRAY_420)
      if faces := faceDetector.Detect(frameLowResolution):
        print(f'[{datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S.%f")}] Faces detected (Count = {faces}).', flush = True)
      if regions := motionDetector.Detect(frameLowResolution):
        print(f'[{datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S.%f")}] Movement regions detected (Count = {regions}).', flush = True)
      if faces or regions:
        videoRecorder.Start()
      videoRecorder.StopWhenCompleted()
      if arguments.displayPreview:
        cameraFPS.Display(frame)
        UpdateFrame(frame)
        cv2.imshow("Camera", frame)
        if cv2.waitKey(1) == ord('q'):
          break
      cameraFPS.Stop()
  except KeyboardInterrupt:
    print("Program stopped.", flush = True)
  finally:
    Finalize()

# Update frame
def UpdateFrame(frame):
  faceDetector.Display(frame)
  motionDetector.Display(frame)
  StaticDisplay.Display(frame)

# Finalization
def Finalize():
  camera.Stop()
  cv2.destroyAllWindows()

Main()