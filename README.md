# Unity Remote Controller App

A learning project that connects a mobile **React Native** app with a **Unity plane simulator**, enabling real-time plane control using WebSocket communication.  

## Features

- **Real-time control:** Mobile app sends commands to a Unity simulator over WebSocket.  
- **React Native app:** Cross-platform mobile app built with **Expo**.  
- **Unity integration:** Plane simulator built in **Unity** receives inputs from the mobile controller.  
- **Customizable UI:** Mobile app displays controller interface and real-time feedback.  

## DevOps & Cloud Aspects

- **Infrastructure as Code (IaC):** AWS resources provisioned using **Terraform** for reproducible and version-controlled infrastructure.  
- **Containerized Backend:** Node.js WebSocket server runs in a **Docker container** for consistent deployment.  
- **Cloud Deployment:** Backend hosted on **AWS EC2**, managing real-time connections from mobile clients.  

# Unity Plane Controller - EC2 WebSocket Server Setup

This guide will help a new developer spin up the EC2 WebSocket server for the Unity Plane Controller project.

---

## Spin Up the EC2 Instance

You can launch an EC2 instance either:

- **Manually via AWS Console**  
- **Automatically via Terraform** in the `awsinfra` folder:

```bash
terraform init
terraform apply
```
## SSH into the EC2
```bash
ssh -i <your-private-key>.pem ec2-user@<EC2_PUBLIC_IP>
```
## Install Docker (if not pre-installed)
```bash
sudo amazon-linux-extras install docker -y
sudo service docker start
```
## Pull the Docker Image
```bash
docker pull huzaifakzk/unity-plane-project-joystick-server:latest
```
## Run the WebSocket Server
```bash
docker run -d -p 8080:8080 huzaifakzk/unity-plane-project-joystick-server:latest
```
## Connect the Unity Client
**6.1 Update WebSocket URL in Unity**

*unity-plane/Scripts/Networking/WebSocketClient.cs*
```bash
websocket = new WebSocket("ws://<PUBLIC_IP>:8080");
```
Replace <PUBLIC_IP> with your EC2 public IP.

*unity-plane/JoystickInput.cs*
```bash
websocket = new WebSocket("ws://<PUBLIC_IP>:8080");
```
Replace <PUBLIC_IP> with your EC2 public IP.

Our EC2 instance is now ready to serve the Unity Plane Controller via WebSocket.
