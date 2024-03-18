#!/bin/bash

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}";
  exit 1;
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

# Install Package
InstallPackage() {
  echo "Package '$1' installation started."
  apt-get install $1 -y -qq
  [ $? != 0 ] && DisplayErrorAndStop "Package '$1' installation failed."
  echo "Package '$1' installation completed."
}

# Synchronize system clock
SynchronizeClock() {
  echo "System clock synchronization started."
  if apt-cache policy ntpdate | grep -q "Installed: (none)" ; then
    InstallPackage ntpdate
  fi
  /usr/sbin/ntpdate -u time.nist.gov
  echo "System clock synchronization completed."
}

# Setup scheduled jobs
SetupScheduledJobs() {
  echo "'$1' scheduled jobs setup started."
  case $1 in
    "Devices.Client")
      SetupScheduledJob "Monitoring" "Devices.Client.dll Monitoring" "*/5 * * * * cd /root/Devices.Client && /root/.dotnet/dotnet Devices.Client.dll Monitoring >> Devices.Client.log 2>&1"
      [ $? != 0 ] && DisplayErrorAndStop "'$1' scheduled jobs setup failed."
      SetupScheduledJob "Configuration" "Devices.Client.dll Configuration" "*/5 * * * * cd /root/Devices.Client && /root/.dotnet/dotnet Devices.Client.dll Configuration >> Devices.Client.log 2>&1"
      [ $? != 0 ] && DisplayErrorAndStop "'$1' scheduled jobs setup failed." ;;
    "Devices.Client.Solutions")
      SetupScheduledJob "Garden" "Devices.Client.Solutions.dll Garden" "*/5 * * * * cd /root/Devices.Client.Solutions && /root/.dotnet/dotnet Devices.Client.Solutions.dll Garden >> Devices.Client.Solutions.log 2>&1"
      [ $? != 0 ] && DisplayErrorAndStop "'$1' scheduled jobs setup failed." ;;
  esac
  echo "'$1' scheduled jobs setup completed."
}

# Setup scheduled job
SetupScheduledJob() {
  echo "'$1' scheduled job setup started."
  crontab -l > crontab.txt
  sed -i "/$2/d" crontab.txt
  echo "$3" | tee -a crontab.txt 1> /dev/null;
  [ $? != 0 ] && DisplayErrorAndStop "'$1' scheduled job setup failed.";
  crontab crontab.txt
  [ $? != 0 ] && DisplayErrorAndStop "'$1' scheduled job setup failed.";
  rm crontab.txt
  [ $? != 0 ] && DisplayErrorAndStop "'$1' scheduled job setup failed.";
  echo "'$1' scheduled job setup completed."
}

# Execute command
ExecuteCommand() {
  echo "Command execution started."
  eval "$1"
  [ $? != 0 ] && DisplayErrorAndStop "Command execution failed.";
  echo "Command execution completed."
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
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac