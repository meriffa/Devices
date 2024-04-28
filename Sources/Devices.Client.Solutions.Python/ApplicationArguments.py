#!/usr/bin/python3

import argparse


# Application arguments
class ApplicationArguments:

    # Initialization
    def __init__(self):
        self.__parser = argparse.ArgumentParser(prog="Devices.Client.Solutions.Python", add_help=False)
        self.__parser.add_argument("-w", "--width", help="Video width.", type=int, required=False, default=640)
        self.__parser.add_argument("-h", "--height", help="Video height.", type=int, required=False, default=480)
        self.__parser.add_argument("-f", "--fps", help="Video FPS.", type=int, required=False, default=30)
        self.__parser.add_argument("-p", "--port", help="Streaming port.", type=int, required=False, default=8443)

    # Parse arguments
    def Parse(self):
        return self.__parser.parse_args()
