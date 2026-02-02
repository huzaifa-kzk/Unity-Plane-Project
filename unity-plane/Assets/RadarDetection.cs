// RadarDetection.cs
using UnityEngine;

public class RadarDetection : MonoBehaviour
{
    public Transform plane;
    public float detectionRange = 10000f;

    void Update()
    {
        if (plane == null) return;
        float d = Vector3.Distance(transform.position, plane.position);
        if (d <= detectionRange)
        {
            // You can raise an event here or play a sound
            Debug.Log($"Plane detected! Distance: {d:F1} m");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
