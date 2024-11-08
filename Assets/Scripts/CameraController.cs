using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Player transform reference.
    [Space(10)]
    public Transform player;

    // Camera settings.
    [Space(10)]
    public float rotationSpeed = 2f;
    public float distance = 5;
    public float minVerticalAngle = -10f;
    public float maxVerticalAngle = 45f;
    public Vector2 framingOffset;

    // Inversion flags.
    [Space(10)]
    public bool invertX;
    public bool invertY;

    // Rotation variables.
    private float rotationX;
    private float rotationY;

    // // Inversion values.
    private float invertXValue;
    private float invertYValue;

    // Initialize cursor settings.
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame.
    private void Update()
    {
        // Set inversion values.
        invertXValue = (invertX) ? -1 : 1;
        invertYValue = (invertY) ? -1 : 1;

        // Handle vertical rotation.
        rotationX += Input.GetAxis("Camera Y") * invertYValue * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        // Handle horizontal rotation.
        rotationY += Input.GetAxis("Camera X") * invertXValue * rotationSpeed;

        // Calculate target rotation and camera position.
        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        var focusPosition = player.position + new Vector3(framingOffset.x, framingOffset.y);

        // Apply the position and rotation to the camera.
        transform.SetPositionAndRotation(focusPosition - targetRotation * new Vector3(0, 0, distance), targetRotation);
    }

    // Returns the rotation around the Y-axis (ignores vertical rotation).
    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);
}
