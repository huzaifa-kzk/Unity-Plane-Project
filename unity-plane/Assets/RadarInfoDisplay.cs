using UnityEngine;
using TMPro;
using System.IO;
using System.Text;

public class RadarInfoDisplay : MonoBehaviour
{
    [Header("UI Text References")]
    public TMP_Text freqText;
    public TMP_Text rangeText;
    public TMP_Text gainText;
    public TMP_Text bandwidthText;
    public TMP_Text resText;
    public TMP_Text pdText;
    public TMP_Text dopplerText;

    [Header("Radar References")]
    public Transform radarStation;
    public Transform plane;

    [Header("Radar Parameters")]
    public float frequencyGHz = 10.0f;        // Radar frequency in GHz
    public float maxRangeKm = 10f;
    public float antennaGainDb = 30f;
    public float bandwidthMHz = 10f;
    public float noiseFigureDb = 3f;
    public float baseSnrDb = 40f;            // SNR at 1 km (clear weather)
    public float detectionThresholdDb = 13f; // Minimum SNR for detection

    [Header("Weather Effects")]
    [Range(0f, 50f)] public float rainRateMmPerHr = 0f; // 0 = clear, 50 = very heavy
    private float specificAttenuationDbPerKm; // Computed from rain rate

    private Vector3 prevPlanePos;

    // --- Data for Logging ---
    private StringBuilder csvBuilder;
    private float logInterval = 0.5f; // seconds
    private float nextLogTime = 0f;
    private string csvPath;

    void Start()
    {
        UpdateStaticInfo();

        if (plane != null)
            prevPlanePos = plane.position;

        // --- CSV Setup ---
        csvPath = Path.Combine(Application.dataPath, "RadarData.csv");
        csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("Time_s,Range_km,SNR_dB,Pd,Doppler_Hz,Rain_mm_per_hr");
    }

    void Update()
    {
        // --- Safety checks ---
        if (radarStation == null)
        {
            Debug.LogWarning("Radar station not assigned!");
            return;
        }

        if (plane == null)
        {
            Debug.LogWarning("Plane not assigned!");
            return;
        }

        // --- Distance ---
        float distanceKm = Vector3.Distance(radarStation.position, plane.position) / 100f;
        distanceKm = Mathf.Max(distanceKm, 0.1f);

        // --- Rain attenuation ---
        specificAttenuationDbPerKm = GetRainAttenuation(frequencyGHz, rainRateMmPerHr);
        float totalRainLossDb = specificAttenuationDbPerKm * distanceKm;

        // --- Radar SNR ---
        float snrAtRange = baseSnrDb - 40f * Mathf.Log10(distanceKm) - totalRainLossDb;
        snrAtRange -= noiseFigureDb;

        // --- Detection Probability ---
        float Pd = CalculateDetectionProbability(snrAtRange);

        // --- Doppler calculation ---
        Vector3 relativeVelocity = Vector3.zero;
        Rigidbody planeRb = plane.GetComponent<Rigidbody>();
        if (planeRb != null)
            relativeVelocity = planeRb.linearVelocity;
        else
            relativeVelocity = (plane.position - prevPlanePos) / Time.deltaTime;

        relativeVelocity *= 10f; // Unity units -> m/s
        float radialVelocity = Vector3.Dot(relativeVelocity, (radarStation.position - plane.position).normalized);
        prevPlanePos = plane.position;

        float transmittedFreqHz = frequencyGHz * 1e9f;
        float wavelength = 3e8f / transmittedFreqHz;
        float dopplerShiftHz = (2f * radialVelocity) / wavelength;
        float dopplerFreqHz = transmittedFreqHz + dopplerShiftHz;

        // --- Update UI safely ---
        if (rangeText != null) rangeText.text = $"Current Range: {distanceKm:F2} km";
        if (pdText != null) 
        {
            pdText.text = $"Pd (Detection Prob.): {Pd * 100f:F1}%";
            pdText.color = Pd > 0.8f ? Color.green : Pd > 0.5f ? Color.yellow : Color.red;
        }
        if (dopplerText != null) 
        {
            dopplerText.text = $"Doppler Frequency: {dopplerFreqHz / 1e9f:F6} GHz";
            if (Mathf.Abs(dopplerShiftHz) < 100f)
                dopplerText.color = Color.white;
            else if (dopplerShiftHz > 0f)
                dopplerText.color = Color.red;   // Target approaching
            else
                dopplerText.color = Color.blue;  // Target receding
        }

        // --- CSV Logging ---
        if (Time.time >= nextLogTime)
        {
            if (csvBuilder != null)
                csvBuilder.AppendLine($"{Time.time:F2},{distanceKm:F3},{snrAtRange:F2},{Pd:F3},{dopplerShiftHz:F1},{rainRateMmPerHr:F1}");
            nextLogTime = Time.time + logInterval;
        }
    }

    void OnApplicationQuit()
    {
        if (csvBuilder != null)
        {
            File.WriteAllText(csvPath, csvBuilder.ToString());
            Debug.Log($"Radar data saved to: {csvPath}");
        }
    }

    void UpdateStaticInfo()
    {
        if (freqText != null) freqText.text = $"Transmit Frequency: {frequencyGHz} GHz";
        if (rangeText != null) rangeText.text = $"Max Range: {maxRangeKm} km";
        if (gainText != null) gainText.text = $"Antenna Gain: {antennaGainDb} dB";
        if (bandwidthText != null) bandwidthText.text = $"Bandwidth: {bandwidthMHz} MHz";
        if (resText != null)
        {
            float rangeRes = 3e8f / (2f * bandwidthMHz * 1e6f);
            resText.text = $"Range Resolution: {rangeRes:F1} m";
        }
    }

    float CalculateDetectionProbability(float snrDb)
    {
        float snrLinear = Mathf.Pow(10f, snrDb / 10f);
        float threshold = Mathf.Pow(10f, detectionThresholdDb / 10f);
        float Pd = 1f / (1f + Mathf.Exp(-(snrLinear - threshold) / threshold * 3f));
        return Mathf.Clamp01(Pd);
    }

    float GetRainAttenuation(float freqGHz, float rainRate)
    {
        float a = 0.0108f * Mathf.Pow(freqGHz, 1.2f);
        float b = 1.276f;
        float gammaR = a * Mathf.Pow(rainRate, b); // dB/km
        return gammaR;
    }
}
