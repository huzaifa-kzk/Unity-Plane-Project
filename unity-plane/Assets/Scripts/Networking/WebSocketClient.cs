using UnityEngine;
using NativeWebSocket;
using System.Text;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket websocket;

    async void Start()
    {
        // Replace localhost with your public IP
        websocket = new WebSocket("Server IP");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connected to WebSocket server");
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = Encoding.UTF8.GetString(bytes);
            Debug.Log("Received: " + message);

            // Parse JSON message using the existing JoystickData class
            JoystickData data = JsonUtility.FromJson<JoystickData>(message);

            Debug.Log($"dx: {data.dx}, dy: {data.dy}");
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError("WebSocket error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket closed");
        };

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    async void OnApplicationQuit()
    {
        if (websocket != null)
            await websocket.Close();
    }
}
