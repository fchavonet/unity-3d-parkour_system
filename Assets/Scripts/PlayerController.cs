using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Camera controller reference.
    [Space(10)]
    public CameraController cameraController;
    public Animator animator;

    // Movement and rotation settings.
    [Space(10)]
    public float moveSpeed = 5f;
    public float rotationSpeed = 500f;

    // Target rotation for smoothing.
    private Quaternion targetRotation;

    // Update is called once per frame.
    private void Update()
    {
        // Get horizontal and vertical input.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement magnitude.
        float moveAmount = Mathf.Clamp01(Math.Abs(horizontal) + Math.Abs(vertical));

        // Create normalized movement vector.
        var moveInput = (new Vector3(horizontal, 0, vertical)).normalized;

        // Apply camera's planar rotation to movement.
        var moveDirection = cameraController.PlanarRotation * moveInput;

        // If there is movement input, move the player.
        if (moveAmount > 0)
        {
            // Move the player.
            transform.position += moveSpeed * Time.deltaTime * moveDirection;
            // Rotate the player to face movement direction.
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        // Smooth rotation to target.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Update animator's moveAmount for blending.
        animator.SetFloat("moveAmount", moveAmount, 0.2f, Time.deltaTime);
    }
}
