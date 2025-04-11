using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform player;
    public float followDistance = 5f;
    public float heightOffset = 2.5f;
    public float lookAtHeightOffset = 1.7f;
    public float smoothSpeed = 0.125f;

    public float mouseSensitivity = 3f;
    public float verticalRotationLimit = 30f;

    private float currentXRotation = 0f;
    private float currentYRotation = 0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentYRotation = angles.y;
        currentXRotation = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (player == null)
            return;

        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Apply rotations
        currentYRotation += mouseX;
        currentXRotation -= mouseY;
        currentXRotation = Mathf.Clamp(currentXRotation, -verticalRotationLimit, verticalRotationLimit);

        // Apply rotation to camera
        Quaternion rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -followDistance);
        Vector3 desiredPosition = player.position + new Vector3(0, heightOffset, 0) + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Camera looks at player
        Vector3 lookTarget = player.position + Vector3.up * lookAtHeightOffset;
        transform.LookAt(lookTarget);

        // Sync player’s horizontal facing direction with camera
        Vector3 flatForward = rotation * Vector3.forward;
        flatForward.y = 0f;

        if (flatForward != Vector3.zero)
        {
            player.forward = Vector3.Lerp(player.forward, flatForward, Time.deltaTime * 10f); // smooth rotation
        }
    }
}
