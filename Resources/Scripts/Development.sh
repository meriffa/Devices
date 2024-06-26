#!/bin/bash

SOLUTION_FOLDER=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/../.." &> /dev/null && pwd)

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}";
  exit 1;
}

# Create solution & projects
CreateSolution() {
  # Create solution
  dotnet new sln --name Devices --output ./Sources
  # Create projects
  dotnet new classlib --language "C#" --output ./Sources/Devices.Common
  dotnet new classlib --language "C#" --output ./Sources/Devices.Service
  dotnet new razorclasslib --language "C#" --output ./Sources/Devices.Web --support-pages-and-views
  dotnet new console --language "C#" --output ./Sources/Devices.Client
  # Add solution projects
  dotnet sln ./Sources/Devices.sln add ./Sources/Devices.Common/Devices.Common.csproj
  dotnet sln ./Sources/Devices.sln add ./Sources/Devices.Service/Devices.Service.csproj
  dotnet sln ./Sources/Devices.sln add ./Sources/Devices.Web/Devices.Web.csproj
  dotnet sln ./Sources/Devices.sln add ./Sources/Devices.Client/Devices.Client.csproj
  # Add project references
  dotnet add ./Sources/Devices.Common/Devices.Common.csproj package Microsoft.Extensions.Configuration
  dotnet add ./Sources/Devices.Common/Devices.Common.csproj package Microsoft.Extensions.Hosting
  dotnet add ./Sources/Devices.Common/Devices.Common.csproj package Microsoft.Extensions.Logging
  dotnet add ./Sources/Devices.Common/Devices.Common.csproj package Microsoft.Extensions.Options.ConfigurationExtensions
  dotnet add ./Sources/Devices.Service/Devices.Service.csproj reference ./Sources/Devices.Common/Devices.Common.csproj
  dotnet add ./Sources/Devices.Service/Devices.Service.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
  dotnet add ./Sources/Devices.Service/Devices.Service.csproj package Npgsql
  dotnet add ./Sources/Devices.Web/Devices.Web.csproj reference ./Sources/Devices.Service/Devices.Service.csproj
  dotnet add ./Sources/Devices.Client/Devices.Client.csproj reference ./Sources/Devices.Common/Devices.Common.csproj
  dotnet add ./Sources/Devices.Client/Devices.Client.csproj package CommandLineParser
  dotnet add ./Sources/Devices.Client/Devices.Client.csproj package Microsoft.Extensions.Configuration
  dotnet add ./Sources/Devices.Client/Devices.Client.csproj package Microsoft.Extensions.Configuration.CommandLine
  dotnet add ./Sources/Devices.Client/Devices.Client.csproj package Microsoft.Extensions.Configuration.EnvironmentVariables
  dotnet add ./Sources/Devices.Client/Devices.Client.csproj package Microsoft.Extensions.Configuration.Json
  dotnet add ./Sources/Devices.Client/Devices.Client.csproj package Microsoft.Extensions.Hosting
  dotnet add ./Sources/Devices.Client/Devices.Client.csproj package Serilog.Extensions.Hosting
  dotnet add ./Sources/Devices.Client/Devices.Client.csproj package Serilog.Formatting.Compact
  dotnet add ./Sources/Devices.Client/Devices.Client.csproj package Serilog.Settings.Configuration
  dotnet add ./Sources/Devices.Client/Devices.Client.csproj package Serilog.Sinks.File
  # Setup development certificate
  dotnet dev-certs clean
  dotnet dev-certs https --trust
}

# Archive solution
ArchiveSolution() {
  SOLUTION_NAME="$(basename $SOLUTION_FOLDER).zip"
  TARGET_FOLDER="/media/sf_Downloads"
  # Cleanup solution
  folders=("bin" "obj" "build" "Publish" "TestResults" "Migrations")
  for i in "${folders[@]}"
  do
    rm -rf $(find $SOLUTION_FOLDER -name "$i")
  done
  # Create archive
  pushd $SOLUTION_FOLDER > /dev/null
  if [ -e ../$SOLUTION_NAME ]; then
    rm ../$SOLUTION_NAME
  fi
  zip -r -q ../$SOLUTION_NAME .
  popd > /dev/null
  # Move archive
  if [ -e $TARGET_FOLDER/$SOLUTION_NAME ]; then
    rm $TARGET_FOLDER/$SOLUTION_NAME
  fi
  mv "$(dirname "$SOLUTION_FOLDER")/$SOLUTION_NAME" $TARGET_FOLDER
  echo "Solution archive '$SOLUTION_NAME' created."
}

# Install .NET SDK 8.0 (Package Manager)
InstallDotNetSDK() {
  wget -q -O packages-microsoft-prod.deb https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb
  sudo dpkg -i packages-microsoft-prod.deb
  rm packages-microsoft-prod.deb
  sudo apt-get update -qq
  DEBIAN_FRONTEND=noninteractive sudo apt-get install dotnet-sdk-8.0 -y -qq
  echo "export DOTNET_CLI_TELEMETRY_OPTOUT=1" >> ~/.bashrc
  dotnet --version | grep "8.0."
  echo ".NET SDK install completed.";
}

# Install Visual Studio Code
InstallVisualStudioCode() {
  wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > packages.microsoft.gpg
  sudo install -D -o root -g root -m 644 packages.microsoft.gpg /etc/apt/keyrings/packages.microsoft.gpg
  sudo sh -c 'echo "deb [arch=amd64,arm64,armhf signed-by=/etc/apt/keyrings/packages.microsoft.gpg] https://packages.microsoft.com/repos/code stable main" > /etc/apt/sources.list.d/vscode.list'
  rm -f packages.microsoft.gpg
  DEBIAN_FRONTEND=noninteractive sudo apt-get install apt-transport-https -y -qq
  sudo apt-get update -qq
  DEBIAN_FRONTEND=noninteractive sudo apt-get install code -y -qq
  code --version | grep "x64"
  extensions=("alefragnani.Bookmarks" "ms-dotnettools.csharp" "streetsidesoftware.code-spell-checker" "hediet.vscode-drawio")
  for extension in "${extensions[@]}"
  do
    code --install-extension $extension
  done
  echo "Visual Studio Code install completed.";
}

# Install Visual Studio Debugger
InstallVisualStudioDebugger() {
  curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l ~/.vsdbg
  echo "Visual Studio Debugger install completed.";
}

# Install RSync
InstallRSync() {
  DEBIAN_FRONTEND=noninteractive sudo apt-get install rsync -y -qq
  echo "RSync install completed.";
}

# Deploy client project
DeployClient() {
  echo "'$1' deployment started."
  pushd $SOLUTION_FOLDER/Sources/$1 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "'$1' deployment failed."
  dotnet build --configuration Debug --nologo --verbosity quiet 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "'$1' deployment failed."
  rsync -rlptzq --no-implied-dirs --progress --delete --mkpath ./bin/Debug/net8.0/ HOST_SBC:~/$1.Debug/
  [ $? != 0 ] && DisplayErrorAndStop "'$1' deployment failed."
  popd 1> /dev/null
  echo "'$1' deployment completed."
}

# Get specified operation
if [ -z $1 ]; then
  DisplayErrorAndStop "No operation specified.";
elif [ -n $1 ]; then
  OPERATION=$1;
fi

# Execute oeration
case $OPERATION in
  CreateSolution) CreateSolution ;;
  ArchiveSolution) ArchiveSolution ;;
  InstallDotNetSDK) InstallDotNetSDK ;;
  InstallVisualStudioCode) InstallVisualStudioCode ;;
  InstallVisualStudioDebugger) InstallRInstallVisualStudioDebugger ;;
  InstallRSync) InstallRSync ;;
  DeployClient) DeployClient "$2" ;;
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac