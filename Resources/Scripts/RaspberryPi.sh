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
  # Raspberry Pi OS with desktop
  wget -q -O raspios.img.xz https://downloads.raspberrypi.com/raspios_arm64/images/raspios_arm64-2024-03-15/2024-03-15-raspios-bookworm-arm64.img.xz
  [ $? != 0 ] && DisplayErrorAndStop "Image download failed (wget)."
  sha256sum raspios.img.xz | grep -iq "7E53A46AAB92051D523D7283C080532BEBB52CE86758629BF1951BE9B4B0560F"
  [ $? != 0 ] && DisplayErrorAndStop "Image download failed (grep)."
  # # Raspberry Pi OS Lite
  # wget -q -O raspios.img.xz https://downloads.raspberrypi.com/raspios_lite_arm64/images/raspios_lite_arm64-2024-03-15/2024-03-15-raspios-bookworm-arm64-lite.img.xz
  # [ $? != 0 ] && DisplayErrorAndStop "Image download failed (wget)."
  # sha256sum raspios.img.xz | grep -iq "58A3EC57402C86332E67789A6B8F149AEEB4E7BB0A16C9388A66EA6E07012E45"
  # [ $? != 0 ] && DisplayErrorAndStop "Image download failed (grep)."
  echo "Image download completed."
}

# Write image
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

# Setup device
SetupDevice() {
  echo "Device setup started."
  ssh HOST_SBC "sed -i \"s/alias ls='ls --color=auto'\$/alias ls='ls -al --color=auto --group-directories-first'/\" ~/.bashrc"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "if [[ \$(grep -L \"alias sudo\" ~/.bashrc) ]]; then echo \"alias sudo='sudo '\" >> ~/.bashrc; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".Device setup failed."
  ssh HOST_SBC "if [[ \$(grep -L \"unset HISTFILE\" ~/.bashrc) ]]; then echo \"unset HISTFILE\" >> ~/.bashrc; fi"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "sudo sed -i \"s/# alias ls='ls \\\$LS_OPTIONS'\$/alias ls='ls -al --color=auto --group-directories-first'/\" /root/.bashrc"
  [ $? != 0 ] && DisplayErrorAndStop "Device setup failed."
  ssh HOST_SBC "if [[ \$(sudo grep -L \"unset HISTFILE\" /root/.bashrc) ]]; then echo \"unset HISTFILE\" | sudo tee -a /root/.bashrc 1> /dev/null; fi"
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

# System update
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

# Setup firewall
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
  ssh HOST_SBC "if ! [ -f /opt/dotnet/dotnet ]; then curl -sSL https://dot.net/v1/dotnet-install.sh | sudo bash /dev/stdin --channel 8.0 --runtime dotnet --install-dir /opt/dotnet --no-path; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (dotnet-install)."
  ssh HOST_SBC "if [[ \$(grep -L \"DOTNET_ROOT\" ~/.bashrc) ]]; then echo \"export DOTNET_ROOT=/opt/dotnet\" >> ~/.bashrc; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (DOTNET_ROOT)."
  ssh HOST_SBC "if [[ \$(grep -L \"DOTNET_CLI_TELEMETRY_OPTOUT\" ~/.bashrc) ]]; then echo \"export DOTNET_CLI_TELEMETRY_OPTOUT=1\" >> ~/.bashrc; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (DOTNET_CLI_TELEMETRY_OPTOUT)."
  ssh HOST_SBC "if [[ \$(sudo grep -L \"DOTNET_ROOT\" /root/.bashrc) ]]; then echo \"export DOTNET_ROOT=/opt/dotnet\" | sudo tee -a /root/.bashrc 1> /dev/null; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (DOTNET_ROOT)."
  ssh HOST_SBC "if [[ \$(sudo grep -L \"DOTNET_CLI_TELEMETRY_OPTOUT\" /root/.bashrc) ]]; then echo \"export DOTNET_CLI_TELEMETRY_OPTOUT=1\" | sudo tee -a /root/.bashrc 1> /dev/null; fi"
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
  local TOKEN_REQUEST="[{\"type\": 2, \"value\": \"Ethernet:$1\"}]"
  local BEARER_TOKEN=$(ssh HOST_SBC "curl -H \"Content-Type: application/json\" -X POST -d '$TOKEN_REQUEST' -fks https://<HostPlaceholder>/Service/Identity/GetDeviceBearerToken")
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Client' download failed."
  ssh HOST_SBC "curl -o ~/Devices.Client.zip -H \"Authorization: Bearer $BEARER_TOKEN\" -fks https://<HostPlaceholder>/Service/Configuration/GetReleasePackage?releaseId=$2"
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Client' download failed."
  ssh HOST_SBC "sudo unzip -qq ~/Devices.Client.zip -d /root/Devices.Client/"
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Client' extract failed."
  ssh HOST_SBC "rm ~/Devices.Client.zip"
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Client' extract failed."
  echo "'Devices.Client' download completed."
}

# Deploy Nginx
DeployNginx() {
  InstallNginx
  ConfigureNginx
}

# Install Nginx
InstallNginx() {
  echo "Nginx installation started."
  ssh HOST_SBC -t "DEBIAN_FRONTEND=noninteractive sudo apt-get install nginx -y -qq"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx installation failed."
  echo "Nginx installation completed."
}

# Configure Nginx
ConfigureNginx() {
  echo "Nginx configuration started."
  # SSL Configuration
  ssh HOST_SBC "sudo mkdir -p /etc/nginx/ssl/Devices.Host/"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  scp -q "$SOLUTION_FOLDER/Resources/Certificates/Devices.Host.pem" HOST_SBC:~
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_SBC "sudo mv ~/Devices.Host.pem /etc/nginx/ssl/Devices.Host/"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  scp -q "$SOLUTION_FOLDER/Resources/Certificates/Devices.Host.key" HOST_SBC:~
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_SBC "sudo mv ~/Devices.Host.key /etc/nginx/ssl/Devices.Host/"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_SBC "sudo chown -R root:root /etc/nginx/ssl"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_SBC "sudo chmod 400 /etc/nginx/ssl/Devices.Host/Devices.Host.key"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  # Add Server Block
  ssh HOST_SBC "sudo tee /etc/nginx/sites-available/Devices.Host 1> /dev/null << END
server {
        listen 80;
        listen [::]:80;
        server_name Devices.Host.HTTP;
        return 301 https://\\\$host:8443\\\$request_uri;
}
server {
        listen 8443 ssl;
        listen [::]:8443 ssl;
        ssl_certificate /etc/nginx/ssl/Devices.Host/Devices.Host.pem;
        ssl_certificate_key /etc/nginx/ssl/Devices.Host/Devices.Host.key;
        server_name Devices.Host.HTTPS;
        location / {
                proxy_pass         http://localhost:5000;
                proxy_redirect     off;
                proxy_http_version 1.1;
                proxy_set_header   Connection keep-alive;
                proxy_set_header   Host \\\$host;
                proxy_set_header   X-Real-IP \\\$remote_addr;
                proxy_set_header   X-Forwarded-For \\\$proxy_add_x_forwarded_for;
                proxy_set_header   X-Forwarded-Proto \\\$scheme;
                proxy_set_header   X-Forwarded-Host \\\$host:\\\$server_port;
                proxy_set_header   X-Forwarded-Port \\\$server_port;
                proxy_cache_bypass \\\$http_upgrade;
                proxy_read_timeout 3600;
        }
}
END"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_SBC "sudo chown -R root:root /etc/nginx/sites-available"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_SBC "sudo ln -s /etc/nginx/sites-available/Devices.Host /etc/nginx/sites-enabled/"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  # Remove Server Block
  ssh HOST_SBC "sudo unlink /etc/nginx/sites-enabled/default"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_SBC "sudo rm -f /etc/nginx/sites-available/default"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_SBC "sudo rm -rf /var/www/html"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  # Server Configuration
  ssh HOST_SBC "sudo sed -i 's/# server_tokens off;/server_tokens off;/g' /etc/nginx/nginx.conf"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_SBC "sudo sed -i 's/# gzip_types text/gzip_types text/g' /etc/nginx/nginx.conf"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_SBC "sudo nginx -t" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_SBC "sudo systemctl restart nginx"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_SBC "sudo systemctl status nginx --no-pager --no-legend --lines 0 | grep -i \"Active: active (running)\"" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Nginx verification failed."
  echo "Nginx configuration completed."
}

# Configuration
Configuration() {
  # Detect I2C devices
  DEBIAN_FRONTEND=noninteractive sudo apt-get install i2c-tools -y -qq
  i2cdetect -y 1
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
  DeployNginx) DeployNginx ;;
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac