import cv2
import pathlib

# Face classifier
class FaceClassifier:

  # Initialization
  def __init__(self):
    path = pathlib.Path(__file__).parent.resolve()
    self.__faceClassifier = cv2.CascadeClassifier(f'{path}/Resources/haarcascade_frontalface_default.xml')

  # Detect faces
  def Detect(self, frame):
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    return self.__faceClassifier.detectMultiScale(gray, scaleFactor = 1.2, minNeighbors = 5, minSize = (20, 20))

  # Display faces
  def Display(self, frame, faces):
    for (x, y, w, h) in faces:
      cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)
    return len(faces) > 0