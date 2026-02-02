provider "aws" {
  region = "us-east-1"  # Free tier friendly
}

# 1. Key Pair for SSH (use your public key directly)
resource "aws_key_pair" "dev_key" {
  key_name   = "Key-Name"
  public_key = file("SSH-Key-Path")
}

# 2. VPC
resource "aws_vpc" "plane_vpc" {
  cidr_block = "10.0.0.0/16"
  tags = { Name = "plane-app-vpc" }
}

# 3. Public Subnet
resource "aws_subnet" "plane_subnet" {
  vpc_id                  = aws_vpc.plane_vpc.id
  cidr_block              = "10.0.1.0/24"
  map_public_ip_on_launch = true
  tags = { Name = "plane-app-subnet" }
}

# 4. Internet Gateway
resource "aws_internet_gateway" "plane_igw" {
  vpc_id = aws_vpc.plane_vpc.id
  tags   = { Name = "plane-app-igw" }
}

# 5. Route Table
resource "aws_route_table" "plane_route" {
  vpc_id = aws_vpc.plane_vpc.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.plane_igw.id
  }

  tags = { Name = "plane-app-route" }
}

# Associate route table with subnet
resource "aws_route_table_association" "plane_rta" {
  subnet_id      = aws_subnet.plane_subnet.id
  route_table_id = aws_route_table.plane_route.id
}

# 6. Security Group
resource "aws_security_group" "plane_sg" {
  name        = "plane-app-sg"
  description = "Allow SSH and WebSocket"
  vpc_id      = aws_vpc.plane_vpc.id

  ingress {
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = [""YOUR_IP/32""]
  }

  ingress {
    from_port   = 8080
    to_port     = 8080
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = { Name = "plane-app-sg" }
}

# 7. EC2 Instance
resource "aws_instance" "plane_server" {
  ami                   = "ami-0c02fb55956c7d316"
  instance_type          = "t2.micro"
  subnet_id              = aws_subnet.plane_subnet.id
  key_name               = aws_key_pair.dev_key.key_name  # uses new key
  vpc_security_group_ids = [aws_security_group.plane_sg.id]

  tags = { Name = "plane-websocket-server" }
}

# 8. Output the public IP of EC2
output "ec2_public_ip" {
  value = aws_instance.plane_server.public_ip
}
