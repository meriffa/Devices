#!/usr/bin/python3

import datetime
import logging
import pathlib
import picamera2.outputs
import shutil
import time

# Video recorder
class VideoRecorder:

  # Initialization
  def __init__(self, camera, videoFolder, videoDuration, videoTimeout):
    self.__camera = camera
    self.__output = picamera2.outputs.CircularOutput()
    self.__camera.AddPermanentEncoder(self.__output)
    self.__videoFolder = videoFolder
    self.__videoDuration = videoDuration
    self.__videoTimeout = videoTimeout
    pathlib.Path(self.__videoFolder).mkdir(parents = True, exist_ok = True)
    self.__recordingStart = None
    self.__recordingEnd = time.time()

  # Start recording
  def Start(self):
    if self.__recordingStart is None:
      now = time.time()
      if now - self.__recordingEnd >= self.__videoTimeout:
        self.__recordingStart = now
        filename = pathlib.Path(self.__videoFolder) / f'{datetime.datetime.now().strftime("%Y%m%d-%H%M%S")}.h264'
        self.__output.fileoutput = filename
        total, used, free = shutil.disk_usage(self.__videoFolder)
        if free / total > 0.1:
          self.__output.start()
          logging.info(f'Recording started (File: \'{filename}\').')
          return filename
        else:
          logging.warning(f"Insufficient disk space (Used = {used / total:.2%}, Free = {free / total:.2%}).")
          return None

  # Stop recording when completed
  def StopWhenCompleted(self):
    if self.__recordingStart is not None:
      now = time.time()
      if now - self.__recordingStart >= self.__videoDuration:
        self.__output.stop()
        self.__recordingStart = None
        self.__recordingEnd = now
        logging.info("Recording stopped.")