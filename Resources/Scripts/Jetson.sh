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
    ssh HOST_JET "sudo apt-cache policy $package | grep -q \"Installed:\""
    [ $? != 0 ] && DisplayErrorAndStop "Package '$package' not found."
    if ssh HOST_JET "sudo apt-cache policy $package | grep -q \"Installed: (none)\""; then
      echo "Package '$package' installation started."
      ssh HOST_JET -t "DEBIAN_FRONTEND=noninteractive sudo apt-get install $package -y -qq"
      [ $? != 0 ] && DisplayErrorAndStop "Package '$package' installation failed."
      echo "Package '$package' installation completed."
    fi
  done
}

# System update
SystemUpdate() {
  echo "System update started."
  ssh HOST_JET "sudo apt-get clean -qq"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_JET "sudo apt-get autoclean -qq"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_JET "sudo apt-get check -qq"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_JET -t "sudo apt-get update"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_JET -t "sudo apt-get full-upgrade -y"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  ssh HOST_JET "sudo apt-get autoremove -y -qq"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed."
  echo "System update completed."
}

# Setup firewall
SetupFirewall() {
  echo "Firewall setup started."
  InstallPackages "ufw"
  ssh HOST_JET "sudo ufw allow OpenSSH"
  [ $? != 0 ] && DisplayErrorAndStop "Firewall setup failed."
  ssh HOST_JET "sudo ufw --force enable"
  [ $? != 0 ] && DisplayErrorAndStop "Firewall setup failed."
  echo "Firewall setup completed."
}

# Install .NET Runtime
InstallNETRuntime() {
  echo ".NET Runtime installation started."
  InstallPackages "wget" "curl" "libicu-dev"
  ssh HOST_JET "if ! [ -f /opt/dotnet/dotnet ]; then curl -sSL https://dot.net/v1/dotnet-install.sh | sudo bash /dev/stdin --channel 8.0 --runtime dotnet --install-dir /opt/dotnet --no-path; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (dotnet-install)."
  ssh HOST_JET "if [[ \$(grep -L \"DOTNET_ROOT\" ~/.bashrc) ]]; then echo \"export DOTNET_ROOT=/opt/dotnet\" >> ~/.bashrc; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (DOTNET_ROOT)."
  ssh HOST_JET "if [[ \$(grep -L \"DOTNET_CLI_TELEMETRY_OPTOUT\" ~/.bashrc) ]]; then echo \"export DOTNET_CLI_TELEMETRY_OPTOUT=1\" >> ~/.bashrc; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (DOTNET_CLI_TELEMETRY_OPTOUT)."
  ssh HOST_JET "if [[ \$(sudo grep -L \"DOTNET_ROOT\" /root/.bashrc) ]]; then echo \"export DOTNET_ROOT=/opt/dotnet\" | sudo tee -a /root/.bashrc 1> /dev/null; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (DOTNET_ROOT)."
  ssh HOST_JET "if [[ \$(sudo grep -L \"DOTNET_CLI_TELEMETRY_OPTOUT\" /root/.bashrc) ]]; then echo \"export DOTNET_CLI_TELEMETRY_OPTOUT=1\" | sudo tee -a /root/.bashrc 1> /dev/null; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (DOTNET_CLI_TELEMETRY_OPTOUT)."
  ssh HOST_JET "if ! [ -f /usr/bin/dotnet ]; then sudo ln -s /opt/dotnet/dotnet /usr/bin/dotnet; fi"
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime installation failed (ln)."
  ssh HOST_JET "dotnet --list-runtimes | grep -i \"Microsoft.NETCore.App 8.0\"" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop ".NET Runtime verification failed."
  echo ".NET Runtime installation completed."
}

# Download Devices.Client
DownloadClient() {
  echo "'Devices.Client' download started."
  local TOKEN_REQUEST="[{\"type\": 2, \"value\": \"Ethernet:$1\"}]"
  local BEARER_TOKEN=$(ssh HOST_JET "curl -H \"Content-Type: application/json\" -X POST -d '$TOKEN_REQUEST' -fks https://<HostPlaceholder>/Service/Identity/GetDeviceBearerToken")
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Client' download failed."
  ssh HOST_JET "curl -o ~/Devices.Client.zip -H \"Authorization: Bearer $BEARER_TOKEN\" -fks https://<HostPlaceholder>/Service/Configuration/GetReleasePackage?releaseId=$2"
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Client' download failed."
  ssh HOST_JET "sudo unzip -qq ~/Devices.Client.zip -d /root/Devices.Client/"
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Client' extract failed."
  ssh HOST_JET "rm ~/Devices.Client.zip"
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
  SystemUpdate) SystemUpdate ;;
  SetupFirewall) SetupFirewall ;;
  InstallNETRuntime) InstallNETRuntime ;;
  DownloadClient) DownloadClient "$2" $3 ;;
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac