#!/bin/bash

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}";
  exit 1;
}

# Install Packages
InstallPackages() {
  local packages=("$@")
  for package in "${packages[@]}"; do
    if apt-cache policy $package | grep -q "Installed: (none)" ; then
      echo "Package '$package' installation started."
      apt-get install $package -y -qq
      [ $? != 0 ] && DisplayErrorAndStop "Package '$package' installation failed."
      echo "Package '$package' installation completed."
    else
      echo "Package '$package' installation skipped."
    fi
  done
}

# Install client project
InstallClient() {
  echo "'$1' installation started."
  TARGET_FOLDER=$HOME/$1
  rm -rf $TARGET_FOLDER/
  [ $? != 0 ] && DisplayErrorAndStop "'$1' installation failed.";
  echo "Target folder '$TARGET_FOLDER' deleted."
  mkdir -p $TARGET_FOLDER/
  [ $? != 0 ] && DisplayErrorAndStop "'$1' installation failed.";
  echo "Target folder '$TARGET_FOLDER' created."
  cp -r * $TARGET_FOLDER/
  [ $? != 0 ] && DisplayErrorAndStop "'$1' installation failed.";
  echo "Package content copied from '$(pwd)' to '$TARGET_FOLDER'."
  rm $TARGET_FOLDER/Install.sh
  [ $? != 0 ] && DisplayErrorAndStop "'$1' installation failed.";
  echo "Script '$TARGET_FOLDER/Install.sh' deleted."
  echo "'$1' installation completed."
}

# System update
SystemUpdate() {
  echo "System update started."
  apt-get clean -qq
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (clean).";
  apt-get autoclean -qq
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (autoclean).";
  apt-get check -qq
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (check).";
  apt-get update -qq
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (update).";
  apt-get full-upgrade -y -qq
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (full-upgrade).";
  apt-get autoremove -y -qq
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (autoremove).";
  echo "System update completed."
}

# System restart
SystemRestart() {
  if ! [ -f /etc/Devices.Configuration/Restart ]; then
    echo "System restart started."
    touch /etc/Devices.Configuration/Restart
    [ $? != 0 ] && DisplayErrorAndStop "System restart failed (touch).";
    shutdown -r now "System restart (Install.sh)"
    [ $? != 0 ] && DisplayErrorAndStop "System restart failed (shutdown).";
    sleep 300
  else
    echo "System restart resumed."
    rm /etc/Devices.Configuration/Restart
    [ $? != 0 ] && DisplayErrorAndStop "System restart failed (rm).";
    echo "System restart completed."
  fi
}

# Synchronize system clock
SynchronizeClock() {
  echo "System clock synchronization started."
  InstallPackages ntpdate
  /usr/sbin/ntpdate -u time.nist.gov
  echo "System clock synchronization completed."
}

# Setup scheduled jobs
SetupScheduledJobs() {
  echo "'$1' scheduled jobs setup started."
  case $1 in
    "Devices.Client")
      SetupScheduledJob $1 "Devices.Client.dll" "*/5 * * * * cd /root/Devices.Client && /root/.dotnet/dotnet Devices.Client.dll execute --tasks Monitoring,Configuration >> Devices.Client.log 2>&1" ;;
    "Devices.Client.Solutions")
      SetupScheduledJob $1 "Devices.Client.Solutions.dll" "*/5 * * * * cd /root/Devices.Client.Solutions && /root/.dotnet/dotnet Devices.Client.Solutions.dll garden >> Devices.Client.Solutions.log 2>&1" ;;
  esac
  echo "'$1' scheduled jobs setup completed."
}

# Setup scheduled job
SetupScheduledJob() {
  echo "'$1' scheduled job setup started."
  crontab -l > crontab.txt
  sed -i "/$2/d" crontab.txt
  echo "$3" >> crontab.txt
  [ $? != 0 ] && DisplayErrorAndStop "'$1' scheduled job setup failed."
  crontab crontab.txt
  [ $? != 0 ] && DisplayErrorAndStop "'$1' scheduled job setup failed."
  rm crontab.txt
  [ $? != 0 ] && DisplayErrorAndStop "'$1' scheduled job setup failed."
  echo "'$1' scheduled job setup completed."
}

# Execute command
ExecuteCommand() {
  echo "Command execution started."
  eval "$1"
  [ $? != 0 ] && DisplayErrorAndStop "Command execution failed.";
  echo "Command execution completed."
}

# Upload device logs
UploadDeviceLogs() {
  echo "Upload device logs started."
  InstallPackages zip
  LOG_FILES="DeviceLogs.$(hostname).zip"
  if [ -f $LOG_FILES ]; then
    rm $LOG_FILES
    [ $? != 0 ] && DisplayErrorAndStop "Upload device logs failed (rm $LOG_FILES).";
  fi
  local folders=("/root/Devices.Client/*.log" "/root/Devices.Client/Logs/*" "/root/Devices.Client.Solutions/*.log" "/root/Devices.Client.Solutions/Logs/*")
  for folder in "${folders[@]}"; do
    zip -qj $LOG_FILES $folder
    [ $? != 0 ] && DisplayErrorAndStop "Upload device logs failed (zip -qj $LOG_FILES $folder).";
  done
  HOST_URL=$(cat /root/Devices.Client/appsettings.Production.json | grep -oP '(?<="Host": ")[^"]*')
  DEVICE_TOKEN=$(cat /etc/Devices.Configuration/Devices.Common.DeviceToken)
  curl -H "deviceToken: $DEVICE_TOKEN" -F filename=$LOG_FILES -F upload=@$LOG_FILES -fks "$HOST_URL/Service/Monitoring/UploadDeviceLogs"
  [ $? != 0 ] && DisplayErrorAndStop "Upload device logs failed (curl).";
  rm $LOG_FILES
  [ $? != 0 ] && DisplayErrorAndStop "Upload device logs failed (rm $LOG_FILES).";
  echo "Upload device logs completed."
}

# Get specified operation
if [ -z $1 ]; then
  DisplayErrorAndStop "No operation specified.";
elif [ -n $1 ]; then
  OPERATION=$1;
fi

# Execute oeration
case $OPERATION in
  InstallClient) InstallClient "$2" ;;
  SystemUpdate) SystemUpdate ;;
  SystemRestart) SystemRestart ;;
  SynchronizeClock) SynchronizeClock ;;
  SetupScheduledJobs) SetupScheduledJobs "$2" ;;
  ExecuteCommand) ExecuteCommand "$2" ;;
  UploadDeviceLogs) UploadDeviceLogs ;;
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac