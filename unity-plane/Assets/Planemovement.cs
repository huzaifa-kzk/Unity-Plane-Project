// PlaneMovement.cs
using UnityEngine;

public class PlaneMovement : MonoBehaviour
{
    public float speed = 105f;      // forward speed (m/s)
    public float turnSpeed = 55f;  // degrees per second yaw

    void Update()
    {
        // Move forward in the plane's local forward direction
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        // Slow yaw to create a gentle turning/circling
        transform.Rotate(0f, turnSpeed * Time.deltaTime, 0f, Space.Self);
    }
}
