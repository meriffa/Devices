#!/usr/bin/python3

import argparse


# Application arguments
class ApplicationArguments:

    # Initialization
    def __init__(self):
        self.__parser = argparse.ArgumentParser(prog="Devices.Client.Solutions.Python.Jetson", add_help=False)
        self.__parser.add_argument("-w", "--width", help="Video width.", type=int, required=False, default=640)
        self.__parser.add_argument("-h", "--height", help="Video height.", type=int, required=False, default=480)
        self.__parser.add_argument("-f", "--fps", help="Video FPS.", type=int, required=False, default=30)
        self.__parser.add_argument("--displayFPS", help="Display FPS.", action="store_true", default=False)

    # Parse arguments
    def Parse(self):
        return self.__parser.parse_args()
