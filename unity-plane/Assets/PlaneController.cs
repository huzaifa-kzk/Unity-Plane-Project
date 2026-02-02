using UnityEngine;
using UnityEngine.InputSystem; // New Input System support

public class PlaneController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 50f;      // forward/back speed
    public float turnSpeed = 60f;      // rotation speed

    void Update()
    {
        // --- Keyboard input using new system ---
        float moveInput = 0f;
        float turnInput = 0f;

        if (Keyboard.current.wKey.isPressed) moveInput += 1f;
        if (Keyboard.current.sKey.isPressed) moveInput -= 1f;
        if (Keyboard.current.dKey.isPressed) turnInput += 1f;
        if (Keyboard.current.aKey.isPressed) turnInput -= 1f;

        // --- Rotate plane (yaw) ---
        transform.Rotate(Vector3.up, turnInput * turnSpeed * Time.deltaTime);

        // --- Move plane forward/backward ---
        transform.Translate(Vector3.forward * moveInput * moveSpeed * Time.deltaTime);
    }
}
