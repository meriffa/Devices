#!/bin/bash

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}";
  exit 1;
}

# Install packages
InstallPackages() {
  local packages=("$@")
  for package in "${packages[@]}"; do
    apt-cache policy $package | grep -q "Installed:";
    [ $? != 0 ] && DisplayErrorAndStop "Package '$package' not found."
    if apt-cache policy $package | grep -q "Installed: (none)"; then
      echo "Package '$package' installation started."
      DEBIAN_FRONTEND=noninteractive apt-get install $package -y -qq
      [ $? != 0 ] && DisplayErrorAndStop "Package '$package' installation failed."
      echo "Package '$package' installation completed."
    fi
  done
}

# Install Python packages
InstallPythonPackages() {
  local packages=("$@")
  for package in "${packages[@]}"; do
    local package_name=$(echo $package | cut -d ',' -f 1)
    local package_install=$(echo $package | cut -d ',' -f 2)
    python3 -c "import $package_name" &> /dev/null
    if [ $? != 0 ]; then
      InstallPackages "python3-$package_install"
    fi
  done
}

# Install .NET client project
InstallClient() {
  echo "'$1' installation started."
  TARGET_FOLDER=$HOME/$1
  rm -rf $TARGET_FOLDER/
  [ $? != 0 ] && DisplayErrorAndStop "'$1' installation failed (1).";
  mkdir -p $TARGET_FOLDER/
  [ $? != 0 ] && DisplayErrorAndStop "'$1' installation failed (2).";
  cp -r * $TARGET_FOLDER/
  [ $? != 0 ] && DisplayErrorAndStop "'$1' installation failed (3).";
  rm $TARGET_FOLDER/Install.sh
  [ $? != 0 ] && DisplayErrorAndStop "'$1' installation failed (4).";
  echo "'$1' installation completed."
}

# Setup scheduled jobs
SetupScheduledJobs() {
  echo "'$1' scheduled jobs setup started."
  case $1 in
    "Devices.Client")
      SetupScheduledJob $1 "Devices.Client.dll" "*/5 * * * * cd /root/Devices.Client && /usr/bin/dotnet Devices.Client.dll execute --tasks Monitoring,Configuration >> /var/log/Devices.Client.log 2>&1" ;;
    "Devices.Client.Solutions")
      SetupScheduledJob $1 "Devices.Client.Solutions.dll" "*/5 * * * * cd /root/Devices.Client.Solutions && /usr/bin/dotnet Devices.Client.Solutions.dll garden >> /var/log/Devices.Client.Solutions.log 2>&1" ;;
  esac
  echo "'$1' scheduled jobs setup completed."
}

# Setup scheduled job
SetupScheduledJob() {
  echo "'$1' scheduled job setup started."
  crontab -l > crontab.txt
  sed -i "/$2/d" crontab.txt
  echo "$3" >> crontab.txt
  [ $? != 0 ] && DisplayErrorAndStop "'$1' scheduled job setup failed (1)."
  crontab crontab.txt
  [ $? != 0 ] && DisplayErrorAndStop "'$1' scheduled job setup failed (2)."
  rm crontab.txt
  [ $? != 0 ] && DisplayErrorAndStop "'$1' scheduled job setup failed (3)."
  echo "'$1' scheduled job setup completed."
}

# Install Python client project
InstallClientPython() {
  InstallPackages python3
  InstallPythonPackages "pip,pip" "venv,venv" "numpy,numpy" "cv2,opencv" "picamera2,picamera2"
  CreateClientPythonService "devices-client-python"
  StopService "devices-client-python"
  InstallClient "$1"
  StartService "devices-client-python"
}

# Start service
StartService() {
  sudo systemctl start $1 &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Service '$1' start failed failed (1)."
  sudo systemctl status $1 --no-pager --no-legend --lines 0 | grep -i "Active: active (running)" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Service '$1' start failed failed (2)."
  echo "Service '$1' started."
}

# Stop service
StopService() {
  sudo systemctl stop $1 &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Service '$1' stop failed failed."
  echo "Service '$1' stopped."
}

# Create Python client service
CreateClientPythonService() {
  if ! [ -f /etc/systemd/system/$1.service ]; then
    echo "Python client service setup started."
    sudo tee /etc/systemd/system/$1.service 1> /dev/null << END
[Unit]
Description=Devices.Client.Solutions.Python Application
After=network.target

[Service]
WorkingDirectory=/root/Devices.Client.Solutions.Python
ExecStart=/usr/bin/python3 /root/Devices.Client.Solutions.Python/Program.py -w 1280 -h 720 -f 30 -p 8888 -v /root/Videos
Restart=always
RestartSec=10
TimeoutStopSec=90
KillSignal=SIGINT
SyslogIdentifier=$1
User=root

[Install]
WantedBy=multi-user.target
END
    [ $? != 0 ] && DisplayErrorAndStop "Python client service setup failed (1).";
    sudo chown -R root:root /etc/systemd/system/$1.service
    [ $? != 0 ] && DisplayErrorAndStop "Python client service setup failed (2).";
    sudo systemctl enable $1 &> /dev/null
    [ $? != 0 ] && DisplayErrorAndStop "Python client service setup failed (3).";
    sudo systemctl status $1 --no-pager --no-legend --lines 0 | grep -i "Loaded: loaded (/etc/systemd/system/$1.service; enabled;" &> /dev/null
    [ $? != 0 ] && DisplayErrorAndStop "Python client service setup failed (4).";
    echo "Python client service setup completed."
  fi
}

# System update
SystemUpdate() {
  echo "System update started."
  apt-get clean -qq
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (1).";
  apt-get autoclean -qq
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (2).";
  apt-get check -qq
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (3).";
  apt-get update -qq
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (4).";
  apt-get full-upgrade -y -qq
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (5).";
  apt-get autoremove -y -qq
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (6).";
  echo "System update completed."
}

# System restart
SystemRestart() {
  if ! [ -f /etc/Devices.Configuration/Restart ]; then
    echo "System restart started."
    touch /etc/Devices.Configuration/Restart
    [ $? != 0 ] && DisplayErrorAndStop "System restart failed (1).";
    /usr/sbin/shutdown -r now "System restart (Install.sh)"
    #systemctl reboot --force --force
    [ $? != 0 ] && DisplayErrorAndStop "System restart failed (2).";
    sleep 300
  else
    echo "System restart resumed."
    rm /etc/Devices.Configuration/Restart
    [ $? != 0 ] && DisplayErrorAndStop "System restart failed (3).";
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
  LOG_FILE="DeviceLogs.$(hostname).zip"
  if [ -f $LOG_FILE ]; then
    rm $LOG_FILE
    [ $? != 0 ] && DisplayErrorAndStop "Upload device logs failed (1).";
  fi
  local folders=("/var/log/Devices.*")
  for folder in "${folders[@]}"; do
    zip -qj $LOG_FILE $folder
    [ $? != 0 ] && DisplayErrorAndStop "Upload device logs failed (2).";
  done
  HOST_URL=$(cat /root/Devices.Client/appsettings.Production.json | grep -oP '(?<="Host": ")[^"]*')
  DEVICE_TOKEN=$(dotnet /root/Devices.Client/Devices.Client.dll execute --tasks Identity)
  curl -H "deviceToken: $DEVICE_TOKEN" -F filename=$LOG_FILE -F upload=@$LOG_FILE -fks "$HOST_URL/Service/Monitoring/UploadDeviceLogs"
  [ $? != 0 ] && DisplayErrorAndStop "Upload device logs failed (3).";
  rm $LOG_FILE
  [ $? != 0 ] && DisplayErrorAndStop "Upload device logs failed (4).";
  echo "Upload device logs completed (File = '$LOG_FILE')."
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
  SetupScheduledJobs) SetupScheduledJobs "$2" ;;
  InstallClientPython) InstallClientPython "$2" ;;
  SystemUpdate) SystemUpdate ;;
  SystemRestart) SystemRestart ;;
  SynchronizeClock) SynchronizeClock ;;
  ExecuteCommand) ExecuteCommand "$2" ;;
  UploadDeviceLogs) UploadDeviceLogs ;;
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac