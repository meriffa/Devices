#!/bin/bash

SOLUTION_FOLDER=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/../.." &> /dev/null && pwd)

# Display error and stop
DisplayErrorAndStop() {
  echo -e "${RED}$1${NC}"
  exit 1
}

# Configure shell
ConfigureShell() {
  echo "Shell configuration started."
  ssh HOST_AWS "sed -i \"s/alias ls='ls --color=auto'$/alias ls='ls -al --color=auto --group-directories-first'/\" ~/.bashrc"
  [ $? != 0 ] && DisplayErrorAndStop "Shell configuration failed (alias)."
  ssh HOST_AWS "sudo sed -i \"s/# alias ls='ls \\\$LS_OPTIONS'$/alias ls='ls -al --color=auto --group-directories-first'/\" /root/.bashrc"
  [ $? != 0 ] && DisplayErrorAndStop "Shell configuration failed (alias)."
  ssh HOST_AWS "echo \"unset HISTFILE\" >> ~/.bashrc"
  [ $? != 0 ] && DisplayErrorAndStop "Shell configuration failed (HISTFILE)."
  ssh HOST_AWS "echo -n | sudo tee /etc/motd" 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Shell configuration failed (motd)."
  echo "Shell configuration completed."
}

# System update
SystemUpdate() {
  echo "System update started."
  ssh HOST_AWS "sudo apt-get clean"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (clean)."
  ssh HOST_AWS "sudo apt-get autoclean"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (autoclean)."
  ssh HOST_AWS "sudo apt-get check"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (check)."
  ssh HOST_AWS -t "sudo apt-get update"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (update)."
  ssh HOST_AWS -t "sudo apt-get full-upgrade -y"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (full-upgrade)."
  ssh HOST_AWS "sudo apt-get autoremove -y"
  [ $? != 0 ] && DisplayErrorAndStop "System update failed (autoremove)."
  echo "System update completed."
}

# Install packages
InstallPackages() {
  local packages=("$@")
  for package in "${packages[@]}"; do
    ssh HOST_AWS "sudo apt-cache policy $package | grep -q \"Installed:\""
    [ $? != 0 ] && DisplayErrorAndStop "Package '$package' not found."
    if ssh HOST_AWS "sudo apt-cache policy $package | grep -q \"Installed: (none)\""; then
      echo "Package '$package' installation started."
      ssh HOST_AWS -t "DEBIAN_FRONTEND=noninteractive sudo apt-get install $package -y -qq"
      [ $? != 0 ] && DisplayErrorAndStop "Package '$package' installation failed."
      echo "Package '$package' installation completed."
    fi
  done
}

# Deploy PostgreSQL
DeployPostgreSQL() {
  InstallPackages "wget" "gnupg"
  InstallPostgreSQL
  ConfigurePostgreSQL
}

# Install PostgreSQL
InstallPostgreSQL() {
  echo "PostgreSQL installation started."
  ssh HOST_AWS "sudo sh -c 'echo \"deb https://apt.postgresql.org/pub/repos/apt \$(lsb_release -cs)-pgdg main\" > /etc/apt/sources.list.d/pgdg.list'"
  [ $? != 0 ] && DisplayErrorAndStop "PostgreSQL installation failed."
  ssh HOST_AWS "wget -q -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | sudo apt-key add -"
  [ $? != 0 ] && DisplayErrorAndStop "PostgreSQL installation failed."
  ssh HOST_AWS "sudo apt-get update"
  [ $? != 0 ] && DisplayErrorAndStop "PostgreSQL installation failed."
  ssh HOST_AWS -t "DEBIAN_FRONTEND=noninteractive sudo apt-get install postgresql-16 -y -qq"
  [ $? != 0 ] && DisplayErrorAndStop "PostgreSQL installation failed."
  ssh HOST_AWS "sudo systemctl status postgresql --no-pager --no-legend --lines 0 | grep -i \"Active: active (exited)\""
  [ $? != 0 ] && DisplayErrorAndStop "PostgreSQL installation failed."
  echo "PostgreSQL installation completed."
}

# Configure PostgreSQL
ConfigurePostgreSQL() {
  echo "PostgreSQL configuration started."
  VM_RAM=16384
  let "VM_RAM_GB=$VM_RAM / 1024"
  let "VM_PSQL_SHARED_BUFFERS=$VM_RAM_GB / 4"
  let "VM_PSQL_WORK_MEM=$VM_RAM / (4 * 100)"
  let "VM_PSQL_MAINTENANCE_WORK_MEM=$VM_RAM / 20"
  let "VM_PSQL_EFFECTIVE_CACHE_SIZE=$VM_RAM_GB / 2"
  VM_PSQL_TEMP_BUFFERS="64"
  ssh HOST_AWS "sudo sed -i 's/^shared_buffers = 128MB/shared_buffers = ${VM_PSQL_SHARED_BUFFERS}GB/g' /etc/postgresql/16/main/postgresql.conf"
  [ $? != 0 ] && DisplayErrorAndStop "PostgreSQL configuration failed."
  ssh HOST_AWS "sudo sed -i 's/^#work_mem = 4MB/work_mem = ${VM_PSQL_WORK_MEM}MB/g' /etc/postgresql/16/main/postgresql.conf"
  [ $? != 0 ] && DisplayErrorAndStop "PostgreSQL configuration failed."
  ssh HOST_AWS "sudo sed -i 's/^#maintenance_work_mem = 64MB/maintenance_work_mem = ${VM_PSQL_MAINTENANCE_WORK_MEM}MB/g' /etc/postgresql/16/main/postgresql.conf"
  [ $? != 0 ] && DisplayErrorAndStop "PostgreSQL configuration failed."
  ssh HOST_AWS "sudo sed -i 's/^#effective_cache_size = 4GB/effective_cache_size = ${VM_PSQL_EFFECTIVE_CACHE_SIZE}GB/g' /etc/postgresql/16/main/postgresql.conf"
  [ $? != 0 ] && DisplayErrorAndStop "PostgreSQL configuration failed."
  ssh HOST_AWS "sudo sed -i 's/^#temp_buffers = 8MB/temp_buffers = ${VM_PSQL_TEMP_BUFFERS}MB/g' /etc/postgresql/16/main/postgresql.conf"
  [ $? != 0 ] && DisplayErrorAndStop "PostgreSQL configuration failed."
  ssh HOST_AWS "sudo sed -i 's/^#listen_addresses = \x27localhost\x27/listen_addresses = \x27*\x27/g' /etc/postgresql/16/main/postgresql.conf"
  [ $? != 0 ] && DisplayErrorAndStop "PostgreSQL configuration failed."
  ssh HOST_AWS "sudo sed -i 's/^# TYPE  DATABASE        USER            ADDRESS                 METHOD$/# TYPE  DATABASE        USER            ADDRESS                 METHOD\nhost    all             all             0.0.0.0\/0               md5/g' /etc/postgresql/16/main/pg_hba.conf"
  [ $? != 0 ] && DisplayErrorAndStop "PostgreSQL configuration failed."
  ssh HOST_AWS "sudo systemctl restart postgresql"
  [ $? != 0 ] && DisplayErrorAndStop "PostgreSQL configuration failed."
  echo "PostgreSQL configuration completed."
}

# Deploy database
DeployDatabase() {
  CreateDatabase
  ConfigureDatabase
}

# Create database
CreateDatabase() {
  echo "Database 'Devices.Data' creation started."
  ssh HOST_AWS "sudo su - postgres -c \"psql -c 'CREATE ROLE \\\"DevicesUser\\\" WITH LOGIN SUPERUSER ENCRYPTED PASSWORD \\\$\\\$<PasswordPlaceholder>\\\$\\\$;' -q\""
  [ $? != 0 ] && DisplayErrorAndStop "Database creation failed (CREATE ROLE)."
  ssh HOST_AWS "sudo su - postgres -c \"psql -c 'CREATE DATABASE \\\"Devices.Data\\\";' -q\""
  [ $? != 0 ] && DisplayErrorAndStop "Database creation failed (CREATE DATABASE)."
  ssh HOST_AWS "sudo su - postgres -c \"psql -c 'ALTER DATABASE \\\"Devices.Data\\\" OWNER TO \\\"DevicesUser\\\";' -q\""
  [ $? != 0 ] && DisplayErrorAndStop "Database creation failed (ALTER DATABASE)."
  echo "Database 'Devices.Data' creation completed."
}

# Configure database
ConfigureDatabase() {
  echo "Database 'Devices.Data' configuration started."
  scp -q $SOLUTION_FOLDER/Resources/Scripts/Database.sql HOST_AWS:~
  [ $? != 0 ] && DisplayErrorAndStop "Database configuration failed (scp)."
  ssh HOST_AWS "sudo mv ~/Database.sql / && sudo chmod a+r /Database.sql && sudo su - postgres -c \"psql -d \\\"Devices.Data\\\" -f /Database.sql -q\" && sudo rm /Database.sql"
  [ $? != 0 ] && DisplayErrorAndStop "Database configuration failed (script)."
  echo "Database 'Devices.Data' configuration completed."
}

# Backup database
BackupDatabase() {
  echo "Database 'Devices.Data' backup started."
  ssh HOST_AWS "sudo su - postgres -c \"pg_dump -Fc \\\"Devices.Data\\\" -f Devices.Data.bak\""
  [ $? != 0 ] && DisplayErrorAndStop "Database 'Devices.Data' backup failed."
  scp -q HOST_AWS:/var/lib/postgresql/Devices.Data.bak $HOME/Transfer/Devices.Data.bak
  [ $? != 0 ] && DisplayErrorAndStop "Database 'Devices.Data' backup failed."
  ssh HOST_AWS "sudo rm /var/lib/postgresql/Devices.Data.bak"
  [ $? != 0 ] && DisplayErrorAndStop "Database 'Devices.Data' backup failed."
  echo "Database 'Devices.Data' backup completed."
}

# Restore database
RestoreDatabase() {
  echo "Database 'Devices.Data' restore started."
  ssh HOST_AWS "sudo su - postgres -c \"psql -c 'DROP DATABASE IF EXISTS \\\"Devices.Data\\\" WITH (FORCE);' -q\""
  [ $? != 0 ] && DisplayErrorAndStop "Database creation failed (DROP DATABASE)."
  ssh HOST_AWS "sudo su - postgres -c \"psql -c 'DROP ROLE IF EXISTS \\\"DevicesUser\\\";' -q\""
  [ $? != 0 ] && DisplayErrorAndStop "Database creation failed (DROP ROLE)."
  CreateDatabase
  scp -q $HOME/Transfer/Devices.Data.bak HOST_AWS:~
  [ $? != 0 ] && DisplayErrorAndStop "Database 'Devices.Data' restore failed."
  ssh HOST_AWS "sudo mv ~/Devices.Data.bak /var/lib/postgresql/"
  [ $? != 0 ] && DisplayErrorAndStop "Database 'Devices.Data' restore failed."
  ssh HOST_AWS "sudo chown postgres:postgres /var/lib/postgresql/Devices.Data.bak"
  [ $? != 0 ] && DisplayErrorAndStop "Database 'Devices.Data' restore failed."
  ssh HOST_AWS "sudo su - postgres -c \"pg_restore --dbname \\\"Devices.Data\\\" Devices.Data.bak\""
  [ $? != 0 ] && DisplayErrorAndStop "Database 'Devices.Data' restore failed."
  ssh HOST_AWS "sudo rm /var/lib/postgresql/Devices.Data.bak"
  [ $? != 0 ] && DisplayErrorAndStop "Database 'Devices.Data' restore failed."
  echo "Database 'Devices.Data' restore completed."
}

# Change database user password
ChangeDatabaseUserPassword() {
  echo "Database user 'DevicesUser' password change started."
  NEW_PASSWORD=$(pwgen -1 -s 32 1)
  ssh HOST_AWS "sudo su - postgres -c \"psql -c 'ALTER ROLE \\\"DevicesUser\\\" WITH LOGIN SUPERUSER ENCRYPTED PASSWORD \\\$\\\$$NEW_PASSWORD\\\$\\\$;' -q\""
  [ $? != 0 ] && DisplayErrorAndStop "Operation failed."
  echo "Database user 'DevicesUser' password change completed ($NEW_PASSWORD)."
}

# Deploy Nginx
DeployNginx() {
  InstallNginx
  ConfigureNginx
}

# Install Nginx
InstallNginx() {
  echo "Nginx installation started."
  ssh HOST_AWS -t "DEBIAN_FRONTEND=noninteractive sudo apt-get install nginx -y -qq"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx installation failed."
  echo "Nginx installation completed."
}

# Configure Nginx
ConfigureNginx() {
  echo "Nginx configuration started."
  # SSL Configuration
  ssh HOST_AWS "sudo mkdir -p /etc/nginx/ssl/Devices.Host/"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  scp -q "$SOLUTION_FOLDER/Resources/Certificates/Devices.Host.pem" HOST_AWS:~
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo mv ~/Devices.Host.pem /etc/nginx/ssl/Devices.Host/"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  scp -q "$SOLUTION_FOLDER/Resources/Certificates/Devices.Host.key" HOST_AWS:~
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo mv ~/Devices.Host.key /etc/nginx/ssl/Devices.Host/"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo chown -R root:root /etc/nginx/ssl"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo chmod 400 /etc/nginx/ssl/Devices.Host/Devices.Host.key"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  # Add Server Block
  ssh HOST_AWS "sudo mkdir -p /var/www/Devices.Host"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo chown -R www-data:www-data /var/www/Devices.Host"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo mkdir -p /var/www/Devices.Host.Keys"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo chown -R www-data:www-data /var/www/Devices.Host.Keys"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo mkdir -p /var/log/Devices.Host"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo chown -R www-data:www-data /var/log/Devices.Host"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo tee /etc/nginx/sites-available/Devices.Host 1> /dev/null << END
server {
        listen 80 default_server;
        listen [::]:80 default_server deferred;
        return 444;
}
server {
        listen 80;
        listen [::]:80;
        server_name <HostPlaceholder>;
        return 301 https://\\\$host\\\$request_uri;
}
server {
        listen 443 ssl;
        listen [::]:443 ssl;
        ssl_certificate /etc/nginx/ssl/Devices.Host/Devices.Host.pem;
        ssl_certificate_key /etc/nginx/ssl/Devices.Host/Devices.Host.key;
        server_name <HostPlaceholder>;
        location / {
                proxy_pass         http://localhost:5000;
                proxy_http_version 1.1;
                proxy_set_header   Upgrade \\\$http_upgrade;
                proxy_set_header   Connection keep-alive;
                proxy_set_header   Host \\\$host;
                proxy_cache_bypass \\\$http_upgrade;
                proxy_set_header   X-Forwarded-For \\\$proxy_add_x_forwarded_for;
                proxy_set_header   X-Forwarded-Proto \\\$scheme;
                proxy_read_timeout 3600;
        }
}
END"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo chown -R root:root /etc/nginx/sites-available"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo chmod -R 755 /var/www/Devices.Host"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo ln -s /etc/nginx/sites-available/Devices.Host /etc/nginx/sites-enabled/"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  # Remove Server Block
  ssh HOST_AWS "sudo unlink /etc/nginx/sites-enabled/default"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo rm -f /etc/nginx/sites-available/default"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo rm -rf /var/www/html"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  # Server Configuration
  ssh HOST_AWS "sudo sed -i 's/# server_tokens off;/server_tokens off;/g' /etc/nginx/nginx.conf"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo sed -i 's/# gzip_types text/gzip_types text/g' /etc/nginx/nginx.conf"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo nginx -t" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo systemctl restart nginx"
  [ $? != 0 ] && DisplayErrorAndStop "Nginx configuration failed."
  ssh HOST_AWS "sudo systemctl status nginx --no-pager --no-legend --lines 0 | grep -i \"Active: active (running)\"" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "Nginx verification failed."
  echo "Nginx configuration completed."
}

# Deploy ASP.NET Core
DeployASPNETCore() {
  InstallPackages "wget" "unzip"
  InstallASPNETCore
  ConfigureASPNETCore
}

# Install ASP.NET Core
InstallASPNETCore() {
  echo "ASP.NET Core installation started."
  ssh HOST_AWS "wget -q https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb"
  [ $? != 0 ] && DisplayErrorAndStop "ASP.NET Core installation failed."
  ssh HOST_AWS -t "sudo dpkg -i packages-microsoft-prod.deb"
  [ $? != 0 ] && DisplayErrorAndStop "ASP.NET Core installation failed."
  ssh HOST_AWS "rm packages-microsoft-prod.deb"
  [ $? != 0 ] && DisplayErrorAndStop "ASP.NET Core installation failed."
  ssh HOST_AWS "sudo apt-get update"
  [ $? != 0 ] && DisplayErrorAndStop "ASP.NET Core installation failed."
  ssh HOST_AWS -t "DEBIAN_FRONTEND=noninteractive sudo apt-get install aspnetcore-runtime-8.0 -y -qq"
  [ $? != 0 ] && DisplayErrorAndStop "ASP.NET Core installation failed."
  ssh HOST_AWS "dotnet --list-runtimes | grep -i \"Microsoft.AspNetCore.App 8.0\"" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "ASP.NET Core verification failed."
  echo "ASP.NET Core installation completed."
}

# Configure ASP.NET Core
ConfigureASPNETCore() {
  echo "ASP.NET Core configuration started."
  ssh HOST_AWS "sudo tee /etc/systemd/system/devices-host.service 1> /dev/null << END
[Unit]
Description=Devices.Host Application

[Service]
WorkingDirectory=/var/www/Devices.Host
ExecStart=/usr/bin/dotnet /var/www/Devices.Host/Devices.Host.dll
Restart=always
RestartSec=10
TimeoutStopSec=90
KillSignal=SIGINT
SyslogIdentifier=devices-host
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
END"
  [ $? != 0 ] && DisplayErrorAndStop "ASP.NET Core configuration failed."
  ssh HOST_AWS "sudo chown -R root:root /etc/systemd/system/devices-host.service"
  [ $? != 0 ] && DisplayErrorAndStop "ASP.NET Core configuration failed."
  ssh HOST_AWS "sudo systemctl enable devices-host" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "ASP.NET Core configuration failed."
  ssh HOST_AWS "sudo systemctl status devices-host --no-pager --no-legend --lines 0 | grep -i \"Loaded: loaded (/etc/systemd/system/devices-host.service; enabled;\"" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "ASP.NET Core verification failed."
  echo "ASP.NET Core configuration completed."
}

# Deploy Devices.Host project
DeployDevicesHost() {
  PackageDevicesHost
  ExtractDevicesHost
}

# Package Devices.Host project
PackageDevicesHost() {
  echo "'Devices.Host' packaging started."
  pushd $SOLUTION_FOLDER/Sources/Devices.Host 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' packaging failed."
  rm -rf ./Publish
  dotnet publish --configuration Release --output ./Publish --nologo --verbosity quiet
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' packaging failed."
  pushd Publish 1> /dev/null
  zip -rq Devices.Host.zip .
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' packaging failed."
  scp Devices.Host.zip HOST_AWS:~
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' packaging failed."
  popd 1> /dev/null
  rm -rf ./Publish
  popd 1> /dev/null
  echo "'Devices.Host' packaging completed."
}

# Extract Devices.Host project
ExtractDevicesHost() {
  echo "'Devices.Host' extraction started."
  ssh HOST_AWS "sudo systemctl stop devices-host" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' extraction failed."
  ssh HOST_AWS "sudo rm -rf /var/www/Devices.Host/*"
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' extraction failed."
  ssh HOST_AWS "sudo unzip -qq ~/Devices.Host.zip -d /var/www/Devices.Host"
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' extraction failed."
  ssh HOST_AWS "sudo rm ~/Devices.Host.zip"
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' extraction failed."
  ssh HOST_AWS "sudo chown -R www-data:www-data /var/www/Devices.Host"
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' extraction failed."
  ssh HOST_AWS "sudo systemctl start devices-host" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' extraction failed."
  ssh HOST_AWS "sudo systemctl status devices-host --no-pager --no-legend --lines 0 | grep -i \"Active: active (running)\"" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' extraction failed."
  echo "'Devices.Host' extraction completed."
}

# Deploy Devices.Host packages
DeployDevicesHostPackages() {
  PackageClient "Devices.Client"
  PackageClient "Devices.Client.Solutions"
  PackageClientPython "Devices.Client.Solutions.Python"
  PackageInstall
  UploadDevicesHostPackages
}

# Upload Devices.Host packages
UploadDevicesHostPackages() {
  echo "'Devices.Host' packages upload started."
  ssh HOST_AWS "sudo mkdir -p /etc/Devices.Configuration/Packages/"
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' packages upload failed."
  scp $SOLUTION_FOLDER/../Devices.Configuration/Packages/*.zip HOST_AWS:~
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' packages upload failed."
  ssh HOST_AWS "sudo mv ~/*.zip /etc/Devices.Configuration/Packages/" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' packages upload failed."
  ssh HOST_AWS "sudo chown -R www-data:www-data /etc/Devices.Configuration" &> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "'Devices.Host' packages upload failed."
  echo "'Devices.Host' packages upload completed."
}

# Get Devices.Host logs
DownloadDevicesHostLogs() {
  echo "'Devices.Host' log download started."
  scp -q HOST_AWS:/var/log/Devices.Host/*.json .
  echo "'Devices.Host' log download completed."
}

# Get uploaded device logs
DownloadDeviceLogs() {
  echo "Device logs download started."
  scp -q HOST_AWS:/etc/Devices.Configuration/DeviceLogs/*.zip .
  echo "Device logs download completed."
}

# Package client project
PackageClient() {
  echo "'$1' packaging started."
  pushd $SOLUTION_FOLDER/Sources/$1 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "'$1' packaging failed."
  rm -rf ./Publish
  dotnet publish --configuration Release --output ./Publish --nologo --verbosity quiet
  [ $? != 0 ] && DisplayErrorAndStop "'$1' packaging failed."
  cp $SOLUTION_FOLDER/Resources/Scripts/Install.sh ./Publish/
  [ $? != 0 ] && DisplayErrorAndStop "'$1' packaging failed."
  pushd Publish 1> /dev/null
  zip -rq $1.zip .
  [ $? != 0 ] && DisplayErrorAndStop "'$1' packaging failed."
  SHA256_HASH=($(sha256sum $1.zip))
  echo "Hash = ${SHA256_HASH^^}"
  mkdir -p $SOLUTION_FOLDER/../Devices.Configuration/Packages/
  [ $? != 0 ] && DisplayErrorAndStop "'$1' packaging failed."
  mv $1.zip $SOLUTION_FOLDER/../Devices.Configuration/Packages/
  [ $? != 0 ] && DisplayErrorAndStop "'$1' packaging failed."
  popd 1> /dev/null
  rm -rf ./Publish
  popd 1> /dev/null
  echo "'$1' packaging completed."
}

# Package Python client project
PackageClientPython() {
  echo "'$1' packaging started."
  pushd $SOLUTION_FOLDER/Sources/$1 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "'$1' packaging failed."
  rm -rf $1.zip
  rm -rf Install.sh
  cp $SOLUTION_FOLDER/Resources/Scripts/Install.sh .
  [ $? != 0 ] && DisplayErrorAndStop "'$1' packaging failed."
  zip -rq $1.zip .
  [ $? != 0 ] && DisplayErrorAndStop "'$1' packaging failed."
  SHA256_HASH=($(sha256sum $1.zip))
  echo "Hash = ${SHA256_HASH^^}"
  mkdir -p $SOLUTION_FOLDER/../Devices.Configuration/Packages/
  [ $? != 0 ] && DisplayErrorAndStop "'$1' packaging failed."
  mv $1.zip $SOLUTION_FOLDER/../Devices.Configuration/Packages/
  [ $? != 0 ] && DisplayErrorAndStop "'$1' packaging failed."
  rm Install.sh
  popd 1> /dev/null
  echo "'$1' packaging completed."
}

# Package Install.sh
PackageInstall() {
  echo "'Install.sh' packaging started."
  pushd $SOLUTION_FOLDER/Resources/Scripts 1> /dev/null
  [ $? != 0 ] && DisplayErrorAndStop "'Install.sh' packaging failed."
  zip -rq Install.zip Install.sh
  SHA256_HASH=($(sha256sum Install.zip))
  echo "Hash = ${SHA256_HASH^^}"
  [ $? != 0 ] && DisplayErrorAndStop "'Install.sh' packaging failed."
  mkdir -p $SOLUTION_FOLDER/../Devices.Configuration/Packages/
  [ $? != 0 ] && DisplayErrorAndStop "'Install.sh' packaging failed."
  mv Install.zip $SOLUTION_FOLDER/../Devices.Configuration/Packages/
  [ $? != 0 ] && DisplayErrorAndStop "'Install.sh' packaging failed."
  popd 1> /dev/null
  echo "'Install.sh' packaging completed."
}

# Register device
RegisterDevice() {
  echo "Device registration started."
  ssh HOST_AWS "echo \"INSERT INTO \\\"Device\\\" VALUES ($1, '$3', '$2', '$5', TRUE);\" | sudo su - postgres -c \"psql -d \\\"Devices.Data\\\" -q\""
  [ $? != 0 ] && DisplayErrorAndStop "Device registration failed (Device)."
  ssh HOST_AWS "echo \"INSERT INTO \\\"DeviceFingerprint\\\" VALUES (2, 'Ethernet:$4', $1);\" | sudo su - postgres -c \"psql -d \\\"Devices.Data\\\" -q\""
  [ $? != 0 ] && DisplayErrorAndStop "Device registration failed (DeviceFingerprint)."
  ssh HOST_AWS "echo \"INSERT INTO \\\"DeviceApplication\\\" VALUES ($1, 1, TRUE), ($1, 2, TRUE), ($1, 3, TRUE), ($1, 4, TRUE);\" | sudo su - postgres -c \"psql -d \\\"Devices.Data\\\" -q\""
  [ $? != 0 ] && DisplayErrorAndStop "Device registration failed (DeviceFingerprint)."
  echo "Device registration completed."
}

# Get specified operation
if [ -z $1 ]; then
  DisplayErrorAndStop "No operation specified."
elif [ -n $1 ]; then
  OPERATION=$1
fi

# Execute oeration
case $OPERATION in
  ConfigureShell) ConfigureShell ;;
  SystemUpdate) SystemUpdate ;;
  DeployPostgreSQL) DeployPostgreSQL ;;
  DeployDatabase) DeployDatabase ;;
  BackupDatabase) BackupDatabase ;;
  RestoreDatabase) RestoreDatabase ;;
  ChangeDatabaseUserPassword) ChangeDatabaseUserPassword ;;
  DeployNginx) DeployNginx ;;
  DeployASPNETCore) DeployASPNETCore ;;
  DeployDevicesHost) DeployDevicesHost ;;
  DeployDevicesHostPackages) DeployDevicesHostPackages ;;
  UploadDevicesHostPackages) UploadDevicesHostPackages ;;
  DownloadDevicesHostLogs) DownloadDevicesHostLogs ;;
  DownloadDeviceLogs) DownloadDeviceLogs ;;
  PackageClient) PackageClient "$2" ;;
  PackageClientPython) PackageClientPython "$2" ;;
  PackageInstall) PackageInstall ;;
  RegisterDevice) RegisterDevice $2 "$3" "$4" "$5" "$6" ;;
  *) DisplayErrorAndStop "Invalid operation '$OPERATION' specified." ;;
esac