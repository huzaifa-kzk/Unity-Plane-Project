using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class RadarAWSClient : MonoBehaviour
{
    [Header("AWS API")]
    [SerializeField]
    private string radarApiUrl =
        "https://g3ilkqz4o6.execute-api.us-east-1.amazonaws.com/radar";

    [Header("Send Interval (seconds)")]
    public float sendInterval = 1.0f;

    private void Start()
    {
        InvokeRepeating(nameof(SendRadarFrame), 2f, sendInterval);
    }

    void SendRadarFrame()
    {
        RadarPayload payload = new RadarPayload
        {
            timestamp = DateTime.UtcNow.ToString("o"),
            radar_id = "CW-RADAR-001",
            frequency_ghz = 10.0f,

            target = new TargetData
            {
                range_m = UnityEngine.Random.Range(500f, 2000f),
                velocity_mps = UnityEngine.Random.Range(-50f, 50f),
                rcs_sqm = 3.0f
            },

            environment = new EnvironmentData
            {
                rain_rate_mmph = UnityEngine.Random.Range(0f, 10f),
                attenuation_db = UnityEngine.Random.Range(0f, 3f)
            },

            signal = new SignalData
            {
                doppler_hz = UnityEngine.Random.Range(500f, 4000f),
                received_power_dbm = -80f,
                snr_db = UnityEngine.Random.Range(5f, 20f)
            }
        };

        string json = JsonUtility.ToJson(payload, true);
        StartCoroutine(PostRadarData(json));
    }

    IEnumerator PostRadarData(string json)
    {
        using UnityWebRequest request =
            new UnityWebRequest(radarApiUrl, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("AWS ERROR: " + request.error);
            Debug.LogError(request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Radar frame sent to AWS");
        }
    }
}
