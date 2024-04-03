#!/bin/bash

SOLUTION_FOLDER=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/../.." &> /dev/null && pwd)

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}"
  exit 1
}

# Install packages
InstallPackages() {
  local packages=("$@")
  for package in "${packages[@]}"; do
    ssh HOST_SBC "sudo apt-cache policy $package | grep -q \"Installed:\""
    [ $? != 0 ] && DisplayErrorAndStop "Package '$package' not found."
    if ssh HOST_SBC "sudo apt-cache policy $package | grep -q \"Installed: (none)\""; then
      echo "Package '$package' installation started."
      ssh HOST_SBC -t "DEBIAN_FRONTEND=noninteractive sudo apt-get install $package -y -qq"
      [ $? != 0 ] && DisplayErrorAndStop "Package '$package' installation failed."
      echo "Package '$package' installation completed."
    fi
  done
}

# Download image
DownloadImage() {
  echo "Image download started."
  wget -q -O dietpi.img.xz https://dietpi.com/downloads/images/$1
  [ $? != 0 ] && DisplayErrorAndStop "Image download failed."
  sha256sum dietpi.img.xz | grep -iq $2
  [ $? != 0 ] && DisplayErrorAndStop "Image download failed."
  echo "Image download completed."
}

# Write image
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

# Setup device
SetupDevice() {
  echo "Device setup started."
  ssh HOST_SBC "if [[ \$(grep dietpi /etc/passwd) ]]; then userdel dietpi; fi"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "if [[ -d \"/home/dietpi\" ]]; then rm -rf /home/dietpi; fi"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "sed -i \"s/# alias ls='ls \\\$LS_OPTIONS'\$/alias ls='ls -al --color=auto --group-directories-first'/\" ~/.bashrc"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "if [[ \$(grep -L \"unset HISTFILE\" ~/.bashrc) ]]; then echo \"unset HISTFILE\" >> ~/.bashrc; fi"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "/boot/dietpi/func/change_hostname \"$1\"" 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  echo "Device setup completed."
}

# System update
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

# Setup firewall
SetupFirewall() {
  echo "Firewall setup started."
  InstallPackages "ufw"
  ssh HOST_SBC "ufw allow OpenSSH"
  [ $? != 0 ] && DisplayErrorAndStop "Firewall setup failed."
  ssh HOST_SBC "ufw --force enable"
  [ $? != 0 ] && DisplayErrorAndStop "Firewall setup failed."
  echo "Firewall setup completed."
}

# Install .NET Runtime
InstallNETRuntime() {
  echo ".NET Runtime installation started."
  InstallPackages "wget" "libicu-dev"
  ssh HOST_SBC "if ! [ -f /opt/dotnet/dotnet ]; then curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0 --runtime dotnet --install-dir /opt/dotnet --no-path; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (dotnet-install)."
  ssh HOST_SBC "if [[ \$(grep -L \"DOTNET_ROOT\" ~/.bashrc) ]]; then echo \"export DOTNET_ROOT=/opt/dotnet\" >> ~/.bashrc; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (DOTNET_ROOT)."
  ssh HOST_SBC "if [[ \$(grep -L \"DOTNET_CLI_TELEMETRY_OPTOUT\" ~/.bashrc) ]]; then echo \"export DOTNET_CLI_TELEMETRY_OPTOUT=1\" >> ~/.bashrc; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (DOTNET_CLI_TELEMETRY_OPTOUT)."
  ssh HOST_SBC "if ! [ -f /usr/bin/dotnet ]; then sudo ln -s /opt/dotnet/dotnet /usr/bin/dotnet; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (ln)."
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

# Configuration
Configuration() {
  # # Enable Camera
  # dietpi-config 1 -> RPi Camera = On -> Reboot
  # ls /dev/video*
  # # Enable I2C
  # dietpi-config 3 -> I2C State = On, I2C Frequency = 100kHz -> Reboot
  # i2cdetect -y 1
  # # Enable SPI
  # dietpi-config 3 -> SPI State = On -> Reboot
  # ls /sys/dev/spi*
  # # Enable 1-Wire
  # echo "dtoverlay=w1-gpio,gpiopin=4" >> /boot/config.txt
  # reboot now
  # ls /sys/bus/w1/devices/
}

# Get specified operation
if [ -z $1 ]; then
  DisplayErrorAndStop "No operation specified."
elif [ -n $1 ]; then
  OPERATION=$1
fi

# Execute oeration
case $OPERATION in
  DownloadImageArm32) DownloadImage "DietPi_RPi-ARMv6-Bookworm.img.xz" "FD30B65EEDC9FD50886C41ABA4B72F0BE0F39943344981A243F0462D2296D866" ;;
  DownloadImageArm64) DownloadImage "DietPi_RPi-ARMv8-Bookworm.img.xz" "B94792CE957B50C452C825223D545ACE8E0664B445FD4330D309181887C1B491" ;;
  DownloadImageArm64Pi5) DownloadImage "testing/DietPi_RPi5-ARMv8-Bookworm.img.xz" "E5B11A42AD74A384A8BD0B8F2006F6AE8810A04D6FD6C3350D3BEE3B6FA61F2E" ;;
  WriteImage) WriteImage ;;
  SetupDevice) SetupDevice "$2" ;;
  SystemUpdate) SystemUpdate ;;
  SetupFirewall) SetupFirewall ;;
  InstallNETRuntime) InstallNETRuntime ;;
  DownloadClient) DownloadClient "$2" $3 ;;
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac