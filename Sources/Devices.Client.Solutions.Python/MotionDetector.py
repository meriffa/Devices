#!/usr/bin/python3

import cv2

# Motion detector
class MotionDetector:

  # Initialization
  def __init__(self, width, height, widthLowResolution, heightLowResolution, blurSize = 21, deltaThreshold = 10, minArea = 30 * 30):
    self.__width = width
    self.__height = height
    self.__widthLowResolution = widthLowResolution
    self.__heightLowResolution = heightLowResolution
    self.__blurSize = (blurSize, blurSize)
    self.__deltaThreshold = deltaThreshold
    self.__minArea = minArea
    self.__average = None
    self.__contours = []

  # Detect motion
  def Detect(self, frame):
    gray = cv2.GaussianBlur(frame, self.__blurSize, 0)
    if self.__average is None:
      self.__average = gray.copy().astype("float")
      return None
    cv2.accumulateWeighted(gray, self.__average, 0.5)
    delta = cv2.absdiff(gray, cv2.convertScaleAbs(self.__average))
    threshold = cv2.threshold(delta, self.__deltaThreshold, 255, cv2.THRESH_BINARY)[1]
    threshold = cv2.dilate(threshold, None, iterations = 2)
    contours, _ = cv2.findContours(threshold.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    self.__contours = [contour for contour in contours if cv2.contourArea(contour) >= self.__minArea]
    return len(self.__contours)

  # Display motion regions
  def Display(self, frame):
    for contour in self.__contours:
      face = cv2.boundingRect(contour)
      (x, y, w, h) = [c * n // d for c, n, d in zip(face, (self.__width, self.__height) * 2, (self.__widthLowResolution, self.__heightLowResolution) * 2)]
      cv2.rectangle(frame, (x, y), (x + w, y + h), (255, 0, 0), 2)