// RadarDisplay.cs
using UnityEngine;
using UnityEngine.UI;

public class RadarDisplay : MonoBehaviour
{
    [Header("References")]
    public Transform radarStation;     // world location of radar
    public Transform plane;            // the plane to show
    public RectTransform blip;         // UI image for the blip (child of radarScreen)
    public RectTransform radarScreen;  // the circular UI image

    [Header("Settings")]
    public float radarRange = 10000f;    // meters
    public bool hideWhenOutOfRange = true;
    public bool clampToEdge = true;    // if plane is beyond range, show blip on edge (if not hidden)

    float screenRadius;

    void Start()
    {
        if (radarScreen != null)
            screenRadius = radarScreen.rect.width * 0.5f; // works if radarScreen has explicit size
        else
            screenRadius = 100f;
    }

    void Update()
    {
         if (plane == null || radarStation == null || blip == null) return;

    // world offset (ignore Y)
    Vector3 offset3D = plane.position - radarStation.position;
    Vector2 offset = new Vector2(offset3D.x, offset3D.z);
    float distance = offset.magnitude;

    // --- AUTO SCALE RADAR RANGE ---
    // radar always covers plane's distance plus a margin (20%)
    radarRange = Mathf.Max(1000f, distance * 1.2f);

    // --- UPDATE BLIP POSITION ---
    blip.gameObject.SetActive(true);
    Vector2 scaled = (offset / radarRange) * screenRadius;

    if (clampToEdge && scaled.magnitude > screenRadius)
        scaled = scaled.normalized * screenRadius;

    blip.anchoredPosition = scaled;
    }
}
