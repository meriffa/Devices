#!/usr/bin/python3

import cv2
import datetime


# Static display
class StaticDisplay:

    # Display static information
    def Display(frame):
        cv2.putText(frame, datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S"), (2, frame.shape[0] - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.75, (255, 255, 255), 1, cv2.LINE_AA)
