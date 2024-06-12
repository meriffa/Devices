#!/usr/bin/python3

import inspect
import logging.config
import multiprocessing.shared_memory
from CameraDateTime import CameraDateTime
from CameraFPS import CameraFPS
from VideoPublisher import VideoPublisher


# Camera controller
class CameraController:

    # Initialization
    def __init__(self, size, source, width, height, fps, location, focus=None, displayDateTime=False, displayFPS=False):
        self.__source = source
        self.__width = width
        self.__height = height
        self.__fps = fps
        self.__location = location
        self.__displayDateTime = displayDateTime
        self.__displayFPS = displayFPS
        logging.config.fileConfig(f"{inspect.getfile(lambda: None)[:-3]}.conf")
        if self.__source == "PiCSI":
            import CameraPicamera2
            self.__camera = CameraPicamera2.CameraPicamera2(self.__width, self.__height, self.__fps)
        else:
            import CameraOpenCV
            self.__camera = CameraOpenCV.CameraOpenCV(self.__source, self.__width, self.__height, self.__fps, self.__focus)
        self.__cameraDateTime = CameraDateTime(self.__displayDateTime)
        self.__cameraFPS = CameraFPS(self.__displayFPS)
        self.__videoPublisher = VideoPublisher(self.__width, self.__height, self.__fps, self.__location)
        self.__sharedMemory = multiprocessing.shared_memory.SharedMemory(create=True, size=size)

    # Return shared memory name
    def GetSharedMemoryName(self):
        return self.__sharedMemory.name

    # Check if stop is requested
    def StopRequested(self):
        return self.__sharedMemory.buf[0] != 0

    # Process commands (zoom, focus)
    def ProcessCommands(self):
        buffer = self.__sharedMemory.buf
        if buffer[4] != 0:
            range = self.__camera.GetFocusRange()
            buffer[8:12] = int(range[0] * 100).to_bytes(4, "little")
            buffer[12:16] = int(range[1] * 100).to_bytes(4, "little")
            logging.info(f"Camera focus range (min = {range[0]}, max = {range[1]}).")
            buffer[4] = 0
        if buffer[16] != 0:
            self.__camera.SetFocus(int.from_bytes(buffer[20:24], "little") / 100)
            buffer[16] = 0
        if buffer[24] != 0:
            self.__camera.SetZoom(int.from_bytes(buffer[28:32], "little") / 100)
            buffer[24] = 0

    # Release shared memory
    def ReleaseSharedMemory(self):
        self.__sharedMemory.close()
        self.__sharedMemory.unlink()

    # Process frame
    def ProcessFrame(self):
        frame = self.__camera.GetFrame()
        if frame is not None:
            self.__cameraDateTime.Display(frame)
            self.__cameraFPS.Display(frame)
            self.__videoPublisher.Display(frame)

    # Start camera loop
    def Start(self):
        self.__camera.Start()
        self.__videoPublisher.Start()
        try:
            while not self.StopRequested():
                self.ProcessCommands()
                self.__cameraFPS.Start()
                self.ProcessFrame()
                self.__cameraFPS.Stop()
        except KeyboardInterrupt:
            logging.info("Controller interrupted.")
        finally:
            self.Finalize()

    # Finalization
    def Finalize(self):
        self.__camera.Stop()
        self.ReleaseSharedMemory()
