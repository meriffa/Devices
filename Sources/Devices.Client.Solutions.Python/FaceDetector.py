#!/usr/bin/python3

import cv2
import pathlib

# Face detector
class FaceDetector:

  # Initialization
  def __init__(self, width, height, widthLowResolution, heightLowResolution):
    self.__width = width
    self.__height = height
    self.__widthLowResolution = widthLowResolution
    self.__heightLowResolution = heightLowResolution
    path = pathlib.Path(__file__).parent.resolve()
    self.__faceClassifier = cv2.CascadeClassifier(f"{path}/Resources/haarcascade_frontalface_default.xml")
    self.__faces = []

  # Detect faces
  def Detect(self, frame):
    self.__faces = self.__faceClassifier.detectMultiScale(frame, scaleFactor = 1.1, minNeighbors = 3, minSize = (5, 5))
    return len(self.__faces)

  # Display faces
  def Display(self, frame):
    for face in self.__faces:
      (x, y, w, h) = [c * n // d for c, n, d in zip(face, (self.__width, self.__height) * 2, (self.__widthLowResolution, self.__heightLowResolution) * 2)]
      cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)