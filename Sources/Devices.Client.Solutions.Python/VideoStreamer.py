#!/usr/bin/python3

import cv2
import flask
import logging
import threading
import waitress


# Video streamer
class VideoStreamer:

    # Initialization
    def __init__(self, port):
        self.__port = port
        self.__app = flask.Flask(__name__, template_folder="Templates")
        if self.__port is not None:
            self.__frame = None
            self.__lock = threading.Lock()
            logging.info(f"Video streamer initialized (Port = {port}).")

        # Page endpoint
        @self.__app.route("/")
        def index():
            return flask.render_template("Index.html")

        # Video endpoint
        @self.__app.route("/video")
        def video():
            return flask.Response(self.GenerateFrames(), mimetype="multipart/x-mixed-replace; boundary=frame")

    # Start video streamer
    def Start(self):
        if self.__port is not None:
            self.__thread = threading.Thread(target=self.StartApplication)
            self.__thread.daemon = True
            self.__thread.start()

    # Start video streamer thread
    def StartApplication(self):
        waitress.serve(self.__app, host="0.0.0.0", port=self.__port, ident=None)

    # Display video frame
    def Display(self, frame):
        if self.__port is not None:
            with self.__lock:
                self.__frame = frame.copy()

    # Generate video frames
    def GenerateFrames(self):
        while True:
            with self.__lock:
                if self.__frame is None:
                    continue
                success, image = cv2.imencode(".jpg", self.__frame)
                if not success:
                    continue
            yield (b"--frame\r\n" b"Content-Type: image/jpeg\r\n\r\n" + bytearray(image) + b"\r\n")
