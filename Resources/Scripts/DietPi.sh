#!/bin/bash

SOLUTION_FOLDER=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/../.." &> /dev/null && pwd)

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}"
  exit 1
}

# Install Package
InstallPackage() {
  echo "Package '$1' installation started."
  ssh HOST_SBC -t "apt-get install $1 -y -qq"
  [ $? != 0 ] && DisplayErrorAndStop "Package '$1' installation failed."
  echo "Package '$1' installation completed."
}

# Download Image
DownloadImage() {
  echo "Image download started."
  wget -q -O dietpi.img.xz https://dietpi.com/downloads/images/DietPi_RPi-$1-Bookworm.img.xz
  [ $? != 0 ] && DisplayErrorAndStop "Image download failed."
  sha256sum dietpi.img.xz | grep -q $2
  [ $? != 0 ] && DisplayErrorAndStop "Image download failed."
  echo "Image download completed."
}

# Write Image
WriteImage() {
  echo "Image write started."
  sudo umount /dev/sda2 && sudo umount /dev/sda1
  xzcat dietpi.img.xz | sudo dd of=/dev/sda bs=64k oflag=dsync status=progress && sync
  sudo mkdir /media/$USER/bootfs && sudo mount /dev/sda1 /media/$USER/bootfs
  sudo sed -i "s/^AUTO_SETUP_LOCALE=C.UTF-8$/AUTO_SETUP_LOCALE=en_US.UTF-8/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^AUTO_SETUP_KEYBOARD_LAYOUT=gb$/AUTO_SETUP_KEYBOARD_LAYOUT=us/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^AUTO_SETUP_TIMEZONE=Europe\/London$/AUTO_SETUP_TIMEZONE=US\/Eastern/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^AUTO_SETUP_NET_WIFI_COUNTRY_CODE=GB$/AUTO_SETUP_NET_WIFI_COUNTRY_CODE=US/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^AUTO_SETUP_NET_HOSTNAME=DietPi$/AUTO_SETUP_NET_HOSTNAME=HOST_SBC/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^AUTO_SETUP_HEADLESS=0$/AUTO_SETUP_HEADLESS=1/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^AUTO_SETUP_SSH_SERVER_INDEX=-1$/AUTO_SETUP_SSH_SERVER_INDEX=-2/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^#AUTO_SETUP_SSH_PUBKEY=ssh-ed25519 AAAAAAAA111111111111BBBBBBBBBBBB222222222222cccccccccccc333333333333 mySSHkey$/AUTO_SETUP_SSH_PUBKEY=<SSHPublicKey>/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^AUTO_SETUP_WEB_SERVER_INDEX=0$/AUTO_SETUP_WEB_SERVER_INDEX=-1/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^AUTO_SETUP_BROWSER_INDEX=-1$/AUTO_SETUP_BROWSER_INDEX=0/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^AUTO_SETUP_AUTOMATED=0$/AUTO_SETUP_AUTOMATED=1/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^AUTO_SETUP_GLOBAL_PASSWORD=dietpi$/AUTO_SETUP_GLOBAL_PASSWORD=<GlobalPassword>/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^SURVEY_OPTED_IN=-1$/SURVEY_OPTED_IN=0/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^CONFIG_CHECK_DIETPI_UPDATES=1$/CONFIG_CHECK_DIETPI_UPDATES=0/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^CONFIG_CHECK_APT_UPDATES=1$/CONFIG_CHECK_APT_UPDATES=0/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^CONFIG_SERIAL_CONSOLE_ENABLE=1$/CONFIG_SERIAL_CONSOLE_ENABLE=0/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^CONFIG_ENABLE_IPV6=1$/CONFIG_ENABLE_IPV6=0/g" /media/$USER/bootfs/dietpi.txt
  sudo sed -i "s/^SOFTWARE_DISABLE_SSH_PASSWORD_LOGINS=0$/SOFTWARE_DISABLE_SSH_PASSWORD_LOGINS=1/g" /media/$USER/bootfs/dietpi.txt
  # WiFi Only
  # sudo sed -i "s/^AUTO_SETUP_NET_ETHERNET_ENABLED=1$/AUTO_SETUP_NET_ETHERNET_ENABLED=0/g" /media/$USER/bootfs/dietpi.txt
  # sudo sed -i "s/^AUTO_SETUP_NET_WIFI_ENABLED=0$/AUTO_SETUP_NET_WIFI_ENABLED=1/g" /media/$USER/bootfs/dietpi.txt
  # sudo sed -i "s/^aWIFI_SSID\[0\]=''$/aWIFI_SSID\[0\]='SSID_Name'/g" /media/$USER/bootfs/dietpi-wifi.txt
  # sudo sed -i "s/^aWIFI_KEY\[0\]=''$/aWIFI_KEY\[0\]='SSID_Password'/g" /media/$USER/bootfs/dietpi-wifi.txt
  sudo umount /dev/sda1 && sudo eject /dev/sda && sudo rmdir /media/$USER/bootfs
  echo "Image write completed."
}

# Setup Device
SetupDevice() {
  echo "Device setup started."
  ssh HOST_SBC "userdel dietpi"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "rm -rf /home/dietpi"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "echo \"alias ls='ls -al --color=auto --group-directories-first'\" >> ~/.bashrc"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "echo 'unset HISTFILE' >> ~/.bashrc"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "/boot/dietpi/func/change_hostname \"$1\"" 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  echo "Device setup completed."
}

# System Update
SystemUpdate() {
  echo "System update started."
  ssh HOST_SBC "apt-get clean -qq"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_SBC "apt-get autoclean -qq"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_SBC "apt-get check -qq"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_SBC -t "apt-get update"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_SBC -t "apt-get full-upgrade -y"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_SBC "apt-get autoremove -y -qq"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  echo "System update completed."
}

# Setup Firewall
SetupFirewall() {
  echo "Firewall setup started."
  InstallPackage "ufw"
  ssh HOST_SBC "ufw allow OpenSSH"
  [ $? != 0 ] && DisplayErrorAndStop "Firewall setup failed."
  ssh HOST_SBC "ufw --force enable"
  [ $? != 0 ] && DisplayErrorAndStop "Firewall setup failed."
  echo "Firewall setup completed."
}

# Install .NET Runtime
InstallNETRuntimePackage() {
  echo ".NET Runtime installation started."
  InstallPackage "wget"
  ssh HOST_SBC "wget -q https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed."
  ssh HOST_SBC -t "dpkg -i packages-microsoft-prod.deb"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed."
  ssh HOST_SBC "rm packages-microsoft-prod.deb"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed."
  ssh HOST_SBC "apt-get update"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed."
  ssh HOST_SBC -t "apt-get install dotnet-runtime-8.0 -y"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed."
  ssh HOST_SBC "dotnet --list-runtimes | grep -i \"Microsoft.NETCore.App 8.0\"" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime verification failed."
  echo ".NET Runtime installation completed."
}

# Install .NET Runtime
InstallNETRuntimeManual() {
  echo ".NET Runtime installation started."
  InstallPackage "wget"
  InstallPackage "libicu-dev"
  ssh HOST_SBC "wget -q -O dotnet-runtime.tar.gz $1"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed."
  ssh HOST_SBC "mkdir -p \$HOME/.dotnet"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed."
  ssh HOST_SBC "tar zxf dotnet-runtime.tar.gz -C \$HOME/.dotnet"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed."
  ssh HOST_SBC "rm dotnet-runtime.tar.gz"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed."
  ssh HOST_SBC "echo 'export DOTNET_ROOT=\$HOME/.dotnet' >> ~/.bashrc"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed."
  ssh HOST_SBC "echo 'export DOTNET_CLI_TELEMETRY_OPTOUT=1' >> ~/.bashrc"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed."
  ssh HOST_SBC "ln -s /root/.dotnet/dotnet /usr/bin/dotnet"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed."
  ssh HOST_SBC "dotnet --list-runtimes | grep -i \"Microsoft.NETCore.App 8.0\"" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime verification failed."
  echo ".NET Runtime installation completed."
}

# Download Devices.Client
DownloadClient() {
  echo "'Devices.Client' download started."
  ssh HOST_SBC "curl -o ~/Devices.Client.zip -H \"deviceToken: $1\" -fks https://<HostPlaceholder>/Service/Configuration/GetReleasePackage?releaseId=$2"
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Client' download failed."
  ssh HOST_SBC "unzip -qq ~/Devices.Client.zip -d ~/Devices.Client/"
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Client' extract failed."
  ssh HOST_SBC "rm ~/Devices.Client.zip"
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Client' extract failed."
  echo "'Devices.Client' download completed."
}

# Get specified operation
if [ -z $1 ]; then
  DisplayErrorAndStop "No operation specified."
elif [ -n $1 ]; then
  OPERATION=$1
fi

# Execute oeration
case $OPERATION in
  DownloadImagePi0) DownloadImage "ARMv6" "fd30b65eedc9fd50886c41aba4b72f0be0f39943344981a243f0462d2296d866" ;;
  DownloadImagePi4) DownloadImage "ARMv8" "b94792ce957b50c452c825223d545ace8e0664b445fd4330d309181887c1b491" ;;
  WriteImage) WriteImage ;;
  SetupDevice) SetupDevice "$2" ;;
  SystemUpdate) SystemUpdate ;;
  SetupFirewall) SetupFirewall ;;
  InstallNETRuntimeManualArm32) InstallNETRuntimeManual "https://download.visualstudio.microsoft.com/download/pr/a3caf5aa-a29a-41a2-b3db-7d68b606dc1a/478f27b65c19dafd3c3120fbdeb99295/dotnet-runtime-8.0.3-linux-arm.tar.gz" ;;
  InstallNETRuntimeManualArm64) InstallNETRuntimeManual "https://download.visualstudio.microsoft.com/download/pr/988a1d6e-6bfb-406c-90ba-682f5c11a7fc/28208806b0a6151c4e5d9e1441b01a6f/dotnet-runtime-8.0.3-linux-arm64.tar.gz" ;;
  DownloadClient) DownloadClient "$2" $3 ;;
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac