#!/usr/bin/python3

import argparse


# Application arguments
class ApplicationArguments:

    # Initialization
    def __init__(self):
        self.__parser = argparse.ArgumentParser(prog="Devices.Client.Solutions.Python", add_help=False)
        self.__parser.add_argument("-s", "--source", help="Camera source.", type=str, required=True, choices=["PiUSB", "JetsonUSB", "JetsonUVC", "PiCSI", "JetsonCSI"])
        self.__parser.add_argument("-w", "--width", help="Camera width.", type=int, required=True)
        self.__parser.add_argument("-h", "--height", help="Camera height.", type=int, required=True)
        self.__parser.add_argument("-f", "--fps", help="Camera FPS.", type=int, required=True)
        self.__parser.add_argument("-o", "--focus", help="Camera focus.", type=int, required=False)
        self.__parser.add_argument("-l", "--location", help="Media server publish location.", type=str, required=False)
        self.__parser.add_argument("--displayDateTime", help="Display date & time.", action=argparse.BooleanOptionalAction, default=True)
        self.__parser.add_argument("--displayFPS", help="Display FPS.", action=argparse.BooleanOptionalAction, default=False)
        self.__parser.add_argument("--displayPreview", help="Display preview window.", action=argparse.BooleanOptionalAction, default=False)

    # Parse arguments
    def Parse(self):
        return self.__parser.parse_args()
