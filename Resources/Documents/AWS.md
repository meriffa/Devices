## AWS Configuration

### Management
- [AWS Console](https://console.aws.amazon.com) -> Sign In

### Setup Networking (VPC)
- VPC -> Your VPCs -> Create VPC: Resources To Create = VPC only, Name Tag = 'Devices.Host VPC', IPv4 CIDR block = IPv4 CIDR manual input, IPv4 CIDR = 192.168.0.0/24, IPv6 CIDR block = No IPv6 CIDR block, Tenancy = Default -> Create VPC
- VPC -> Subnets -> Create Subnet: VPC ID = 'Devices.Host VPC', Subnet name = 'Devices.Host Subnet', Availability Zone = us-east-1a, IPv4 VPC CIDR block = 192.168.0.0/24, IPv4 subnet CIDR block = 192.168.0.0/24 -> Create Subnet
- VPC -> Internet Gateways -> Create Internet Gateway: Name = 'Devices.Host Internet Gateway' -> Create Internet Gateway -> Attach To VPC: VPC = 'Devices.Host VPC' -> Attach To VPC -> Devices.Host VPC
- VPC -> Route Tables: Name = 'Devices.Host Route Table', Routes -> Edit Routes: Destination = 0.0.0.0/0, Target: 'Devices.Host Internet Gateway' -> Save Changes
- VPC -> DHCP Option Sets: Name = 'Devices.Host DHCP Option Set'
- VPC -> Elastic IP Addresses -> Allocate Elastic IP Address: Network border group = us-east-1, Public IPv4 address pool = Amazon, Name = 'Devices.Host Elastic IP Address' -> Allocate
- VPC -> Network ACLs: Name = 'Devices.Host Network ACL'
- VPC -> Security Groups: Name = 'Devices.Host Security Group', Inbound Rule Name = 'Devices.Host Inbound Rule', Outbound Rule Name = 'Devices.Host Outbound Rule'
- VPC -> Security Groups -> 'Devices.Host Security Group' -> Inbound Rules -> 'Devices.Host Inbound Rule' -> Edit Inbound Rules -> Add Rule: Type = SSH, Source = My IP, Name = 'Devices.Host Inbound Rule SSH' -> Save Rules
- VPC -> Security Groups -> 'Devices.Host Security Group' -> Inbound Rules -> 'Devices.Host Inbound Rule' -> Edit Inbound Rules -> Add Rule: Type = PostgreSQL, Source = My IP, Name = 'Devices.Host Inbound Rule PostgreSQL' -> Save Rules
- VPC -> Security Groups -> 'Devices.Host Security Group' -> Inbound Rules -> 'Devices.Host Inbound Rule' -> Edit Inbound Rules -> Add Rule: Type = HTTP, Source = 0.0.0.0/0, Name = 'Devices.Host Inbound Rule HTTP' -> Save Rules
- VPC -> Security Groups -> 'Devices.Host Security Group' -> Inbound Rules -> 'Devices.Host Inbound Rule' -> Edit Inbound Rules -> Add Rule: Type = HTTPS, Source = 0.0.0.0/0, Name = 'Devices.Host Inbound Rule HTTPS' -> Save Rules

### Setup Public Domain
- Route 53 -> Hosted Zones -> Create Hosted Zone: Domain Name = \<public.domain>, Type = Public hosted zone, Name = 'Devices.Host Hosted Zone' -> Create hosted zone
- Route 53 -> Hosted Zones -> \<public.domain> -> Create Record -> Record name = \<blank>, Value = \<Devices.Host Elastic IP Address Value> -> Create records
- Route 53 -> Hosted Zones -> \<public.domain> -> Create Record -> Record name = www, Value = \<Devices.Host Elastic IP Address Value> -> Create records
- Route 53 -> Health Checks -> Create Health Check: Name = 'Devices.Host Health Check', What to monitor = Endpoint, Specify endpoint by = Domain, Protocol = HTTPS, Domain = www.\<public.domain>, Port = 443, Path = \<blank> -> Next -> Create alarm = Yes, Send notifications to = Existing SNS topic (NotifyMe) -> Create health check

### Purchase Reserved Instance
- EC2 -> Reserved Instances -> Purchase Reserved Instances: Only show offerings that reserve capacity = On, Platform = Linux/UNIX, Availability Zone = us-east-1a, Tenancy = Default, Offering Class = Standard, Instance Type = t3.xlarge, Term = 12 months to 36 months, Payment Option = All upfront -> Search -> Add To Cart -> Order All

### Create Virtual Machine (EC2 Instance)
- EC2 -> Key Pairs -> Create Key Pair: Name = 'Devices.Host Key Pair', Key pair type = RSA, Private key file format = .pem -> Create Key Pair
- EC2 -> Instances -> Launch Instances: Name = 'Devices.Host Instance', AMI = Debian 12 (HVM), SSD Volume Type, 64-bit (x86), Instance Type = t3.xlarge, Key pair = 'Devices.Host Key Pair', VPC = 'Devices.Host VPC', Subnet = 'Devices.Host Subnet', Auto-assign public IP = Off, Select existing group = On, Common Security Groups = 'Devices.Host Security Group', Storage = 1x 50 GB gp3, Number of instances = 1 -> Launch Instance
- EC2 -> Volumes -> Name = 'Devices.Host Volume'
- EC2 -> Network Interfaces -> Name = 'Devices.Host NIC'
- VPC -> Elastic IP Addresses -> 'Devices.Host Elastic IP Address' -> Associate Elastic IP address -> Network Interface = 'Devices.Host NIC' -> Associate

### Configure Virtual Machine (EC2 Instance)
- Configure Shell: `./AWS.sh ConfigureShell`
- Update System: `./AWS.sh SystemUpdate`
- Deploy PostgreSQL: `./AWS.sh DeployPostgreSQL`
- Deploy Database: `./AWS.sh DeployDatabase`
- Deploy Nginx: `./AWS.sh DeployNginx`
- Deploy ASP.NET Core: `./AWS.sh DeployASPNETCore`
- Deploy Devices.Host: `./AWS.sh DeployDevicesHost`
- Package & Upload Devices.Host Packages: `./AWS.sh DeployDevicesHostPackages`
- Upload Devices.Host Packages: `./AWS.sh UploadDevicesHostPackages`