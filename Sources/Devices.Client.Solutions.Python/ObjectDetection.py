#!/usr/bin/python3

import cv2
import logging
import pathlib
import urllib.request


# Object detection model
class ObjectDetection:

    # Initialization
    def __init__(self):
        self.__COLOR = (0, 0, 255)
        path = self.DownloadResources()
        self.__classNames = self.LoadClassNames(path)
        self.__model = self.LoadModel(path)

    # Download resources
    def DownloadResources(self):
        path = f"{pathlib.Path(__file__).parent.resolve()}/Resources"
        pathlib.Path(path).mkdir(parents=True, exist_ok=True)
        files = [
            ("Classes_COCO.txt", "https://raw.githubusercontent.com/ChiekoN/OpenCV_SSD_MobileNet/master/model/object_detection_classes_coco.txt"),
            ("frozen_inference_graph.pb", "https://github.com/ChiekoN/OpenCV_SSD_MobileNet/raw/master/model/frozen_inference_graph.pb"),
            ("ssd_mobilenet_v2_coco_2018_03_29.pbtxt", "https://raw.githubusercontent.com/ChiekoN/OpenCV_SSD_MobileNet/master/model/ssd_mobilenet_v2_coco_2018_03_29.pbtxt"),
        ]
        for name, url in files:
            file = f"{path}/{name}"
            if not pathlib.Path(file).is_file():
                urllib.request.urlretrieve(url, file)
        logging.info("Object detection model resources loaded.")
        return path

    # Load classes
    def LoadClassNames(self, path):
        with open(f"{path}/Classes_COCO.txt", "r") as file:
            classNames = [n.title() for n in file.read().split("\n")]
            logging.info("Object detection model classes loaded.")
            return classNames

    # Load model
    def LoadModel(self, path):
        model = cv2.dnn.readNet(model=f"{path}/frozen_inference_graph.pb", config=f"{path}/ssd_mobilenet_v2_coco_2018_03_29.pbtxt", framework="TensorFlow")
        model.setPreferableBackend(cv2.dnn.DNN_BACKEND_CUDA)
        model.setPreferableTarget(cv2.dnn.DNN_TARGET_CUDA)
        logging.info("Object detection model loaded.")
        return model

    # Run object detection model
    def Run(self, image):
        blob = cv2.dnn.blobFromImage(image=image, size=(300, 300), mean=(104, 117, 123), swapRB=True)
        self.__model.setInput(blob)
        return self.__model.forward()

    # Display object detections
    def Display(self, image):
        detections = self.Run(image)
        image_height, image_width, _ = image.shape
        for detection in detections[0, 0, :, :]:
            if detection[2] > 0.3:
                class_name = f"{self.__classNames[int(detection[1]) - 1]} ({round(detection[2]*100, 1)}%)"
                x = detection[3] * image_width
                y = detection[4] * image_height
                width = detection[5] * image_width
                height = detection[6] * image_height
                cv2.rectangle(image, (int(x), int(y)), (int(width), int(height)), self.__COLOR, thickness=1)
                cv2.putText(image, class_name, (int(x), int(y - 5)), fontFace=cv2.FONT_HERSHEY_SIMPLEX, fontScale=0.5, color=self.__COLOR, thickness=1)
