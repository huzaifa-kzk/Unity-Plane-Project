

https://github.com/user-attachments/assets/4c96124c-ae02-46c9-905c-cf71d28e83d7


# Unity Remote Controller App

A learning project that connects a mobile **React Native** app with a **Unity plane simulator**, enabling real-time plane control using WebSocket communication.

## Features

- **Real-time control:** Mobile app sends joystick commands to a Unity simulator over WebSocket.
- **React Native app:** Cross-platform mobile controller built with **Expo**.
- **Unity integration:** Plane simulator receives controller input from the WebSocket relay.
- **Containerized backend:** Node.js WebSocket server can run locally or in Docker.

## Project Structure

- `plane-controller-app/` - Expo React Native controller app.
- `unity-websocket-server/` - Node.js WebSocket relay server.
- `unity-plane/` - Unity simulator project.

## Configure the Controller App

The controller does not hard-code a public server IP. Copy the example environment file and set your WebSocket URL:

```bash
cd plane-controller-app
cp .env.example .env
```

Then edit `.env`:

```bash
EXPO_PUBLIC_WEBSOCKET_URL=ws://YOUR_SERVER_IP:8080
```

For local testing, keep:

```bash
EXPO_PUBLIC_WEBSOCKET_URL=ws://localhost:8080
```

## Run the WebSocket Server

```bash
cd unity-websocket-server
npm install
npm start
```

Or run it with Docker:

```bash
cd unity-websocket-server
docker build -t unity-plane-websocket-server .
docker run -p 8080:8080 unity-plane-websocket-server
```

## Unity Assets

The Unity assets are required for the simulator. The public Google Drive asset folder is tracked in `unityassets`.

Download the asset pack from that link and place the extracted files in:

```bash
unity-plane/Assets/PrivateAssets/
```

Keep source scripts, scenes, prefabs, and `.meta` files in git so the Unity project is reproducible.

For private or large assets, use one of these:

- A private git repository added as a submodule under `unity-plane/Assets/PrivateAssets/`.
- Git LFS for large binary assets such as `.fbx`, `.png`, `.jpg`, `.wav`, and `.unitypackage`.
- A private release artifact or package registry if the assets should not live in the main repo.
- An untracked local `unityassets.local` file for private download/repo notes.

Example private submodule setup:

```bash
git submodule add <PRIVATE_ASSET_REPO_URL> unity-plane/Assets/PrivateAssets
```

## Connect Unity

Set the same WebSocket URL in the Unity networking script:

```bash
ws://YOUR_SERVER_IP:8080
```

You can run the WebSocket server locally, on a VM, or in any container host.
