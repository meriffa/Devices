#!/usr/bin/python3

import argparse

# Application arguments
class ApplicationArguments:

  # Initialization
  def __init__(self):
    self.__parser = argparse.ArgumentParser(prog = "Devices.Client.Solutions.Python", add_help = False)
    self.__parser.add_argument("-w", "--width", help = "Video width.", type = int, required = True)
    self.__parser.add_argument("-h", "--height", help = "Video height.", type = int, required = True)
    self.__parser.add_argument("--widthLR", help = "Video low resolution width.", type = int, required = False, default = 320)
    self.__parser.add_argument("--heightLR", help = "Video low resolution height.", type = int, required = False, default = 240)
    self.__parser.add_argument("-f", "--fps", help = "Video FPS.", type = int, required = False, default = 30)
    self.__parser.add_argument("-p", "--port", help = "Streaming port.", type = int, required = False)
    self.__parser.add_argument("-v", "--videoFolder", help = "Video folder.", required = True)
    self.__parser.add_argument("-d", "--videoDuration", help = "Video duration.", type = float, required = False, default = 5.0)
    self.__parser.add_argument("-t", "--videoTimeout", help = "Video timeout.", type = float, required = False, default = 10.0)
    self.__parser.add_argument("--displayPreview", help = "Display preview.", action = argparse.BooleanOptionalAction, default = False)
    self.__parser.add_argument("--displayFPS", help = "Display FPS.", action = argparse.BooleanOptionalAction, default = False)

  # Parse arguments
  def Parse(self):
    return self.__parser.parse_args()