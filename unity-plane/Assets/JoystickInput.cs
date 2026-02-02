using UnityEngine;
using NativeWebSocket;
using System;
using System.Text;

[Serializable]
public class JoystickData
{
    public float dx;
    public float dy;
}

public class JoystickInput : MonoBehaviour
{
    [Header("Plane Movement Settings")]
    public float moveSpeed = 50f;      // Forward/back speed
    public float turnSpeed = 60f;      // Rotation speed (yaw)
    public float smoothing = 8f;       // Smooth interpolation

    [Header("Joystick Settings")]
    public float deadzone = 0.1f;      // Minimal movement to detect

    private WebSocket websocket;
    private float joystickX = 0f;
    private float joystickY = 0f;
    private float currentX = 0f;
    private float currentY = 0f;

    private float lastReceivedTime = 0f;
    private float timeout = 0.2f;      // Reset input if no data for 200ms

    async void Start()
    {
        // Connect to your WebSocket server
        websocket = new WebSocket("Server IP");

        websocket.OnOpen += () => Debug.Log("WebSocket connected!");
        websocket.OnError += (e) => Debug.LogError("WebSocket Error: " + e);
        websocket.OnClose += (e) => Debug.Log("WebSocket closed!");

        websocket.OnMessage += (bytes) =>
        {
            string message = Encoding.UTF8.GetString(bytes);

            // 🔹 Debug raw message
            Debug.Log("Raw WebSocket message: " + message);

            try
            {
                JoystickData data = JsonUtility.FromJson<JoystickData>(message);

                // 🔹 Debug parsed values
                Debug.Log($"Parsed dx: {data.dx}, dy: {data.dy}");

                float x = Mathf.Clamp(data.dx / 100f, -1f, 1f); // normalize to -1..1
                float y = Mathf.Clamp(-data.dy / 100f, -1f, 1f); // invert Y

                joystickX = Mathf.Abs(x) < deadzone ? 0f : x;
                joystickY = Mathf.Abs(y) < deadzone ? 0f : y;

                lastReceivedTime = Time.time;
            }
            catch (Exception ex)
            {
                Debug.Log("Error parsing joystick JSON: " + ex.Message);
            }
        };

        await websocket.Connect();
    }

    void Update()
    {
        // Dispatch WebSocket messages
        websocket?.DispatchMessageQueue();

        // Reset if no new data received
        if (Time.time - lastReceivedTime > timeout)
        {
            joystickX = 0f;
            joystickY = 0f;
        }

        // Smooth input for less jitter
        currentX = Mathf.Lerp(currentX, joystickX, smoothing * Time.deltaTime);
        currentY = Mathf.Lerp(currentY, joystickY, smoothing * Time.deltaTime);

        // Apply movement and rotation to plane
        transform.Rotate(Vector3.up, currentX * turnSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * currentY * moveSpeed * Time.deltaTime, Space.Self);
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
            await websocket.Close();
    }
}
