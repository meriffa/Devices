#!/bin/bash

SOLUTION_FOLDER=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/../.." &> /dev/null && pwd)

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}"
  exit 1
}

# Install Packages
InstallPackages() {
  local packages=("$@")
  for package in "${packages[@]}"; do
    if ssh HOST_SBC "sudo apt-cache policy $package | grep -q \"Installed: (none)\"" ; then
      echo "Package '$package' installation started."
      ssh HOST_SBC -t "sudo apt-get install $package -y -qq"
      [ $? != 0 ] && DisplayErrorAndStop "Package '$package' installation failed."
      echo "Package '$package' installation completed."
    else
      echo "Package '$package' installation skipped."
    fi
  done
}

# Download Image
DownloadImage() {
  echo "Image download started."
  wget -q -O raspios.img.xz https://downloads.raspberrypi.com/raspios_lite_arm64/images/raspios_lite_arm64-2024-03-15/2024-03-15-raspios-bookworm-arm64-lite.img.xz
  [ $? != 0 ] && DisplayErrorAndStop "Image download failed (wget)."
  sha256sum raspios.img.xz | grep -iq "58A3EC57402C86332E67789A6B8F149AEEB4E7BB0A16C9388A66EA6E07012E45"
  [ $? != 0 ] && DisplayErrorAndStop "Image download failed (grep)."
  echo "Image download completed."
}

# Write Image
WriteImage() {
  echo "Image write started."
  sudo umount /dev/sda2 && sudo umount /dev/sda1
  xzcat raspios.img.xz | sudo dd of=/dev/sda bs=64k oflag=dsync status=progress && sync
  sudo mkdir /media/$USER/bootfs && sudo mount /dev/sda1 /media/$USER/bootfs
  sudo touch /media/$USER/bootfs/ssh
  echo "$USER:$(echo '<UserPassword>' | openssl passwd -6 -stdin)" | sudo tee /media/$USER/bootfs/userconf.txt
  sudo umount /dev/sda1 && sudo eject /dev/sda && sudo rmdir /media/$USER/bootfs
  echo "Image write completed."
}

# Setup SSH
SetupSSH() {
  echo "SSH setup started."
  ssh-copy-id -f -i <PublicKey> HOST_SBC
  [ $? != 0 ] && DisplayErrorAndStop "SSH setup failed."
  ssh HOST_SBC "sudo sed -i 's/^#PasswordAuthentication yes$/PasswordAuthentication no/' /etc/ssh/sshd_config"
  [ $? != 0 ] && DisplayErrorAndStop "SSH setup failed."
  ssh HOST_SBC "sudo sed -i 's/^#PubkeyAuthentication yes$/PubkeyAuthentication yes/' /etc/ssh/sshd_config"
  [ $? != 0 ] && DisplayErrorAndStop "SSH setup failed."
  ssh HOST_SBC "sudo sed -i 's/^#PermitRootLogin prohibit-password$/PermitRootLogin no/' /etc/ssh/sshd_config"
  [ $? != 0 ] && DisplayErrorAndStop "SSH setup failed."
  ssh HOST_SBC "if [[ \$(grep -L \"AllowUsers \" /etc/ssh/sshd_config) ]]; then echo \"AllowUsers $USER\" | sudo tee -a /etc/ssh/sshd_config 1> /dev/null; fi"
  [ $? != 0 ] && DisplayErrorAndStop "SSH setup failed."
  ssh HOST_SBC "echo -n | sudo tee /etc/motd"
  [ $? != 0 ] && DisplayErrorAndStop "SSH setup failed."
  ssh HOST_SBC "sudo systemctl restart sshd"
  [ $? != 0 ] && DisplayErrorAndStop "SSH setup failed."
  echo "SSH setup completed."
}

# Setup Device
SetupDevice() {
  echo "Device setup started."
  ssh HOST_SBC "sed -i \"s/alias ls='ls --color=auto'\$/alias ls='ls -al --color=auto --group-directories-first'/\" ~/.bashrc"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "if [[ \$(grep -L \"alias sudo\" ~/.bashrc) ]]; then echo \"alias sudo='sudo '\" >> ~/.bashrc; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".Device setup failed."
  ssh HOST_SBC "if [[ \$(grep -L \"unset HISTFILE\" ~/.bashrc) ]]; then echo \"unset HISTFILE\" >> ~/.bashrc; fi"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "sudo raspi-config nonint do_hostname \"$1\""
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "sudo raspi-config nonint do_boot_behaviour B1"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "sudo raspi-config nonint do_change_locale en_US.UTF-8 UTF-8"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "sudo raspi-config nonint do_change_timezone US/Eastern"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "sudo raspi-config nonint do_configure_keyboard us"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "sudo raspi-config nonint do_wifi_country US"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "sudo raspi-config nonint do_expand_rootfs"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  echo "Device setup completed."
}

# System Update
SystemUpdate() {
  echo "System update started."
  ssh HOST_SBC "sudo apt-get clean -qq"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_SBC "sudo apt-get autoclean -qq"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_SBC "sudo apt-get check -qq"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_SBC -t "sudo apt-get update"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_SBC -t "sudo apt-get full-upgrade -y"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_SBC "sudo apt-get autoremove -y -qq"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  echo "System update completed."
}

# Setup Firewall
SetupFirewall() {
  echo "Firewall setup started."
  InstallPackages "ufw"
  ssh HOST_SBC "sudo ufw allow OpenSSH"
  [ $? != 0 ] && DisplayErrorAndStop "Firewall setup failed."
  ssh HOST_SBC "sudo ufw --force enable"
  [ $? != 0 ] && DisplayErrorAndStop "Firewall setup failed."
  echo "Firewall setup completed."
}

# Install .NET Runtime
InstallNETRuntime() {
  echo ".NET Runtime installation started."
  InstallPackages "wget" "libicu-dev"
  ssh HOST_SBC "if ! [ -f \$HOME/.dotnet/dotnet ]; then curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0 --runtime dotnet --install-dir \$HOME/.dotnet --no-path; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (dotnet-install)."
  ssh HOST_SBC "if [[ \$(grep -L \"DOTNET_ROOT\" ~/.bashrc) ]]; then echo \"export DOTNET_ROOT=\$HOME/.dotnet\" >> ~/.bashrc; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (DOTNET_ROOT)."
  ssh HOST_SBC "if [[ \$(grep -L \"DOTNET_CLI_TELEMETRY_OPTOUT\" ~/.bashrc) ]]; then echo \"export DOTNET_CLI_TELEMETRY_OPTOUT=1\" >> ~/.bashrc; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (DOTNET_CLI_TELEMETRY_OPTOUT)."
  ssh HOST_SBC "if ! [ -f /usr/bin/dotnet ]; then sudo ln -s \$HOME/.dotnet/dotnet /usr/bin/dotnet; fi"
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

# Get specified operation
if [ -z $1 ]; then
  DisplayErrorAndStop "No operation specified."
elif [ -n $1 ]; then
  OPERATION=$1
fi

# Execute oeration
case $OPERATION in
  DownloadImage) DownloadImage ;;
  WriteImage) WriteImage ;;
  SetupSSH) SetupSSH ;;
  SetupDevice) SetupDevice "$2" ;;
  SystemUpdate) SystemUpdate ;;
  SetupFirewall) SetupFirewall ;;
  InstallNETRuntime) InstallNETRuntime ;;
  DownloadClient) DownloadClient "$2" $3 ;;
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac