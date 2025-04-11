using UnityEngine;

public class PlayerMouseLook : MonoBehaviour
{
    public float mouseSensitivity = 3f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        // Rotate player horizontally (Y-axis)
        transform.Rotate(0f, mouseX, 0f);
    }
}
