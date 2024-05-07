#!/usr/bin/python3

import argparse


# Application arguments
class ApplicationArguments:

    # Initialization
    def __init__(self):
        self.__parser = argparse.ArgumentParser(prog="Devices.Client.Solutions.Python", add_help=False)
        self.__parser.add_argument("-s", "--source", help="Video camera source.", type=str, required=True, choices=["Picamera2", "USB", "JetsonUSB", "JetsonCSI"])
        self.__parser.add_argument("-w", "--width", help="Video width.", type=int, required=True)
        self.__parser.add_argument("-h", "--height", help="Video height.", type=int, required=True)
        self.__parser.add_argument("-f", "--fps", help="Video FPS.", type=int, required=True)
        self.__parser.add_argument("-p", "--port", help="Streaming port.", type=int, required=False)
        self.__parser.add_argument("--displayDateTime", help="Display date & time.", action=argparse.BooleanOptionalAction, default=True)
        self.__parser.add_argument("--displayFPS", help="Display FPS.", action=argparse.BooleanOptionalAction, default=False)
        self.__parser.add_argument("--displayPreview", help="Display preview window.", action=argparse.BooleanOptionalAction, default=False)
        self.__parser.add_argument("--objectDetection", help="Perform object detection.", action=argparse.BooleanOptionalAction, default=False)

    # Parse arguments
    def Parse(self):
        return self.__parser.parse_args()
