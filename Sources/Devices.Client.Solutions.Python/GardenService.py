#!/usr/bin/python3

import datetime
import logging
import requests
import subprocess
import urllib3

# Camera FPS display
class GardenService:

  # Initialization
  def __init__(self, hostUrl, devicesClientPath):
    self.__hostUrl = hostUrl
    self.__devicesClientPath = devicesClientPath
    urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

  # Save camera notification
  def SaveCameraNotification(self, faces, regions, videoFile):
    if videoFile is not None:
      deviceDate = datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%S.%fZ")
      data = {"deviceDate": f"{deviceDate}", "faceCount": faces, "motionRegionCount": regions, "videoFileName": f"{videoFile}" }
      headers = {"Content-type": "application/json", "Authorization": f"Bearer {self.GetDeviceIdentity()}"}
      r = requests.post(f"{self.__hostUrl}/Service/Solutions/Garden/SaveCameraNotification", headers = headers, json = data, verify = False)
      if r.status_code == 200:
        logging.info("Camera notification created.")
      else:
        logging.error(f"Camera notification creation failed (Error = {r.status_code}).")

  # Return device identity
  def GetDeviceIdentity(self):
    result = subprocess.run(["dotnet", self.__devicesClientPath, "execute", "--tasks", "Identity"], stdout = subprocess.PIPE)
    if result.returncode:
      logging.error("Devices.Client execution failed.")
      return None
    return result.stdout.decode("utf-8").strip()