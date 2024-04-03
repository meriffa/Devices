#!/usr/bin/python3

import picamera2.outputs
import socket
import threading

# Video streamer
class VideoStreamer:

  # Initialization
  def __init__(self, camera, port):
    self.__camera = camera
    self.__port = port

  # Start streaming
  def Start(self):
    if self.__port is not None:
      thread = threading.Thread(target = self.Stream)
      thread.daemon = True
      thread.start()

  # Stream content
  def Stream(self):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as endpoint:
      endpoint.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
      endpoint.bind(("0.0.0.0", self.__port))
      endpoint.listen()
      print(f"Streaming started (IP: 0.0.0.0:{self.__port}).", flush = True)
      while True:
        clientConnection, clientAddress = endpoint.accept()
        print(f"Streaming client connected (IP: {clientAddress[0]}:{clientAddress[1]}).", flush = True)
        event = threading.Event()
        stream = clientConnection.makefile("wb")
        output = picamera2.outputs.FileOutput(stream)
        output.start()
        self.__camera.AddTransientEncoder(output)
        output.connectiondead = lambda _: event.set()
        event.wait()