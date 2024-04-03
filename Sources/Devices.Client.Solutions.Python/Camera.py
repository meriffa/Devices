#!/usr/bin/python3

import libcamera
import picamera2
import picamera2.encoders

# Camera source
class Camera:

  # Initialization
  def __init__(self, width, height, widthLowResolution, heightLowResolution, fps, updateFrame, bitrate = 12000000):
    self.__camera = picamera2.Picamera2()
    main = {"size": (width, height), "format": "RGB888"}
    lores = {"size": (widthLowResolution, heightLowResolution), "format": "YUV420"}
    controls = {"FrameDurationLimits": (int(1000000 / fps), int(1000000 / fps))}
    transform = libcamera.Transform(hflip = True, vflip = False)
    configuration = self.__camera.create_preview_configuration(main = main, lores = lores, controls = controls, transform = transform)
    self.__camera.align_configuration(configuration)
    self.__camera.configure(configuration)
    self.__encoder = picamera2.encoders.H264Encoder(bitrate)
    self.__camera.encoder = self.__encoder
    self.__updateFrame = updateFrame
    self.__camera.post_callback = self.UpdateFrame
    self.__output = []
    print(f"Camera configured (Display: {width}x{height}, Detection: {widthLowResolution}x{heightLowResolution}, FPS: {fps}).", flush = True)

  # Start camera
  def Start(self):
    self.__camera.start(show_preview = False)
    print("Camera started.", flush = True)
    self.__camera.start_encoder(self.__encoder)
    print("Camera encoder started.", flush = True)

  # Stop camera
  def Stop(self):
    self.__camera.stop_encoder(self.__encoder)
    print("Camera encoder stopped.", flush = True)    
    self.__camera.stop()
    print("Camera stopped.", flush = True)

  # Return frame
  def GetFrame(self, stream):
    return self.__camera.capture_array(stream)

  # Update frame
  def UpdateFrame(self, request):
    with picamera2.MappedArray(request, "main") as buffer:
      frame = buffer.array
      if self.__updateFrame:
        self.__updateFrame(frame)

  # Add permanent encoder
  def AddPermanentEncoder(self, output):
    self.__output = [output]
    self.__encoder.output = self.__output

  # Add transient encoder
  def AddTransientEncoder(self, output):
    self.__encoder.output = self.__output + [output]