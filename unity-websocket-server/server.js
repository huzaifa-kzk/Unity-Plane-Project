const WebSocket = require('ws');
const fs = require('fs');
const path = require('path');

// ===== Minimal additions =====
const PORT = process.env.PORT || 8080;
// =============================

// Log file path
const logFile = path.join(__dirname, 'joystick.log');

// Function to append messages to the log file
function logMessage(message) {
    const timestamp = new Date().toISOString();
    const logLine = `[${timestamp}] ${message}\n`;

    // Write to file (your original behavior)
    fs.appendFile(logFile, logLine, (err) => {
        if (err) console.error('Failed to write log:', err);
    });

    // ALSO log to console (Docker-friendly)
    console.log(logLine.trim());
}

// Use configurable port
const wss = new WebSocket.Server({ port: PORT, host: '0.0.0.0' });

console.log(`WebSocket server running on ws://0.0.0.0:${PORT}`);

wss.on('connection', (ws) => {
    console.log('New client connected');

    ws.on('message', (message) => {
        logMessage(`Received: ${message.toString()}`);

        // Broadcast to all connected clients
        wss.clients.forEach((client) => {
            if (client.readyState === WebSocket.OPEN) {
                client.send(message.toString());
                logMessage(`Sent: ${message.toString()}`);
            }
        });
    });

    ws.on('close', () => {
        console.log('Client disconnected');
    });
});
