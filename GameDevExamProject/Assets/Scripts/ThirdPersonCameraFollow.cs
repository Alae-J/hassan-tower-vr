﻿using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;           // Player transform
    public float followDistance = 5f;  // Distance from player
    public float heightOffset = 2.5f;  // Default height offset
    public float lookAtHeightOffset = 1.7f; // Target height
    public float smoothSpeed = 0.125f; // Movement smoothing

    // Mouse look settings
    public float mouseSensitivity = 3f;
    public float verticalRotationLimit = 30f; // Limit in degrees (up and down)

    private float currentXRotation = 0f;
    private float currentYRotation = 0f;

    void Start()
    {
        // Initialize rotation values
        Vector3 angles = transform.eulerAngles;
        currentYRotation = angles.y;
        currentXRotation = angles.x;

        // Optionally lock cursor for game
        // Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (player == null)
            return;

        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Apply horizontal rotation (no limits)
        currentYRotation += mouseX;

        // Apply vertical rotation (with limits)
        currentXRotation -= mouseY; // Inverted for natural feel
        currentXRotation = Mathf.Clamp(currentXRotation, -verticalRotationLimit, verticalRotationLimit);

        // Calculate rotation - keep camera oriented properly
        Quaternion rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);

        // Calculate position - use rotation to position camera at fixed distance
        Vector3 negativeDistance = new Vector3(0, 0, -followDistance);
        Vector3 position = player.position + new Vector3(0, heightOffset, 0) + rotation * negativeDistance;

        // Apply position with smoothing
        transform.position = Vector3.Lerp(transform.position, position, smoothSpeed);

        // Look at point above player
        Vector3 lookTarget = player.position + Vector3.up * lookAtHeightOffset;
        transform.LookAt(lookTarget);
    }
}