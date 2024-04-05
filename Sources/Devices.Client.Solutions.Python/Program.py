#!/usr/bin/python3

import configparser
import cv2
import datetime
import logging
import logging.config
from ApplicationArguments import ApplicationArguments
from Camera import Camera
from CameraFPS import CameraFPS
from FaceDetector import FaceDetector
from GardenService import GardenService
from MotionDetector import MotionDetector
from StaticDisplay import StaticDisplay
from VideoRecorder import VideoRecorder
from VideoStreamer import VideoStreamer

# Initialization
def Initialize():
  global arguments, configuration, camera, cameraFPS, faceDetector, motionDetector, videoStreamer, videoRecorder, gardenService
  arguments = ApplicationArguments().Parse()
  logging.config.fileConfig(f"{__file__[:-3]}.conf")
  configuration = configparser.ConfigParser()
  configuration.read(f"{__file__[:-3]}.ini")
  camera = Camera(arguments.width, arguments.height, arguments.widthLR, arguments.heightLR, arguments.fps, UpdateFrame)
  cameraFPS = CameraFPS(arguments.displayFPS)
  faceDetector = FaceDetector(arguments.width, arguments.height, arguments.widthLR, arguments.heightLR)
  motionDetector = MotionDetector(arguments.width, arguments.height, arguments.widthLR, arguments.heightLR)
  videoStreamer = VideoStreamer(camera, arguments.port)
  videoRecorder = VideoRecorder(camera, arguments.videoFolder, arguments.videoDuration, arguments.videoTimeout)
  gardenService = GardenService(configuration["Application"]["Host"], configuration["Application"]["DevicesClientPath"])

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
        logging.info(f'Faces detected (Count = {faces}).')
      if regions := motionDetector.Detect(frameLowResolution):
        logging.info(f'Movement regions detected (Count = {regions}).')
      if faces or regions:
        gardenService.SaveCameraNotification(faces, regions, videoRecorder.Start())
      videoRecorder.StopWhenCompleted()
      if arguments.displayPreview:
        cameraFPS.Display(frame)
        UpdateFrame(frame)
        cv2.imshow("Camera", frame)
        if cv2.waitKey(1) == ord('q'):
          break
      cameraFPS.Stop()
  except KeyboardInterrupt:
    logging.warning("Program stopped.")
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