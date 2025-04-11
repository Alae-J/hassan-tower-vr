using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 3f;
    public float minY = -35f;
    public float maxY = 60f;

    private float verticalRotation = 0f;

    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minY, maxY);

        // Apply vertical rotation (pitch)
        transform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
    }
}
