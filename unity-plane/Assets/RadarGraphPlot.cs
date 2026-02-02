using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RadarGraphPlot : MonoBehaviour
{
    public float snrMin = 0f;
    public float snrMax = 20f;
    public int points = 100;
    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = points;
        line.useWorldSpace = false;
        line.startWidth = 2f;
        line.endWidth = 2f;

        // Set color
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f) }
        );
        line.colorGradient = gradient;

        PlotGraph();
    }

    float ProbabilityOfDetection(float snr)
    {
        return Mathf.Clamp01(1f - Mathf.Exp(-0.25f * snr));
    }

    void PlotGraph()
    {
        float width = 400f;  // pixels
        float height = 200f; // pixels
        float xOffset = 0f;
        float yOffset = 0f;

        for (int i = 0; i < points; i++)
        {
            float snr = Mathf.Lerp(snrMin, snrMax, i / (float)(points - 1));
            float pd = ProbabilityOfDetection(snr);  

            float x = xOffset + (snr - snrMin) / (snrMax - snrMin) * width;
            float y = yOffset + pd * height;

            line.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}
